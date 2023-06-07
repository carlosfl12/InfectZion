using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
public class Gun : NetworkBehaviour
{
    public Transform aimTarget;
    public GameObject bulletPrefab;
    public GameObject cannon;
    public float shootCooldown = 0.25f;
    public float lastShotTime = 0;
    public LayerMask layerMask = new LayerMask();
    public bool reloading = false;
    public float timeToReload = 2f;

    public TMP_Text text;

    public int maxAmmo = 240;
    public int bullets = 30;

    public bool canShoot = true;
    public List<GameObject> spawnedBullets = new List<GameObject>();

    private void Start() {
        if (IsOwner && (IsClient || IsHost)) {
            //          bullets.Value.ToString() porque tiene variable de red maxAmmo.Value.ToString()
            text.text = bullets.ToString() + " / " + maxAmmo.ToString();
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer) {
            // reloading.Value = false;
            // canShoot.Value = false;
            // bullets.Value = 30;
            // maxAmmo.Value = 240; Cambio a 60
        }
    }


    public void Shoot() {
        if (reloading) {
            return;
        }

        if (Time.time - lastShotTime > shootCooldown && canShoot) {
            GameObject.Instantiate(bulletPrefab, transform.position, transform.rotation);
            lastShotTime = Time.time;
            bullets--; // Si repercute en el funcionamiento del programa (Animacion o cualquier otra cosa) variable de red
            if (bullets <= 0) {
                reloading = true;
                StartCoroutine(Reloading(timeToReload));
            }
            if (maxAmmo == 0 && bullets == 0) {
                canShoot = false; //Logica del programa = VARIABLE DE RED
            }
            if (IsOwner) {
                text.text = bullets.ToString() + " / " + maxAmmo.ToString();
            }
        }

        
    }

    private void Update() {
        if (!IsOwner) {
            return;
        }
        // if (IsOwner && (IsHost || IsClient)) {}
        Vector3 mouseWorldPosition = Vector3.zero;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f, layerMask)) {
            aimTarget.position = hit.point;
            mouseWorldPosition = hit.point;
        }
        cannon.transform.LookAt(aimTarget.position, Vector3.up); //VARIABLE DE RED
        // porque es un transform y se tiene que notificar

        Reload();
    }
    void Reload() {
        if (Input.GetKeyDown(KeyCode.R)) {
            //SERVER
            reloading = true;
            GetComponentInParent<Player>().anim.SetBool("IsReloading", true);
            StartCoroutine(Reloading(timeToReload));

        }
    }
    IEnumerator Reloading(float seconds) {
        int currentAmmo = bullets; // Se puede eliminar
        // if (IsServer) {
        if (currentAmmo != 30) { //bullets.Value 
            bullets = 30; // maxAmmo.Value -= (30 - bullets.Value);
            maxAmmo -= bullets - currentAmmo; // bullets.Value = 30;
            yield return new WaitForSeconds(seconds);
            // if (IsClient || IsHost) { 
                //GetComponentInParent<Player>().anim.SetBool("IsReloading", false);
                // if (IsOwner) {
                    // text.text = bullets.Value.ToString() + " / " + maxAmmo.Value.ToString();
                // }
            //}

            // if (IsServer) {
                // reloading.Value = false;
            // }
            GetComponentInParent<Player>().anim.SetBool("IsReloading", false);
            if (IsOwner) {
                text.text = bullets.ToString() + " / " + maxAmmo.ToString();
                // Hacer script de HUD aparte que lleve todo -> vida, municion, etc...
            }

        }
        reloading = false;
        
    }    

    [ServerRpc(RequireOwnership = false)]
    public void ShootServerRpc() {

        if (reloading) {
            return;
        }

        if (Time.time - lastShotTime > shootCooldown && canShoot) {
            GameObject bullet = Instantiate(bulletPrefab, cannon.transform.position, cannon.transform.rotation);
            spawnedBullets.Add(bullet);
            bullet.GetComponent<Bullet>().parent = this;
            bullet.GetComponent<NetworkObject>().Spawn();
            lastShotTime = Time.time; // Fuera del server
            bullets--;
            if (bullets <= 0) {
                reloading = true;
                StartCoroutine(Reloading(timeToReload));
            }
            if (maxAmmo == 0 && bullets == 0) {
                canShoot = false;
            }
            if (IsOwner) {
                text.text = bullets.ToString() + " / " + maxAmmo.ToString();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyBulletServerRpc() {
        if (spawnedBullets.Count == 0) {
            return;
        }
        GameObject toDestroy = spawnedBullets[0];
        toDestroy.GetComponent<NetworkObject>().Despawn();
        spawnedBullets.Remove(toDestroy);

        if (toDestroy != null) {
            Destroy(toDestroy);
        }
    }

    [ClientRpc] // Tenia que haber ido en el server
    void PlayerAimingClientRpc() {
        Vector3 mouseWorldPosition = Vector3.zero;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f, layerMask)) {
            aimTarget.position = hit.point;
            mouseWorldPosition = hit.point;
        }
        cannon.transform.LookAt(aimTarget.position, Vector3.up);
    }
}
