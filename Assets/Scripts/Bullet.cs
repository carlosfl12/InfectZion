using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class Bullet : NetworkBehaviour
{
    public Rigidbody bulletRb;
    public Gun parent;
    public float flyTime = 3f;
    public float bulletSpeed = 1800f;
    public float damage = 2f;

    public GameObject Explosion;
    // Start is called before the first frame update

    void Awake() {
        bulletRb = GetComponent<Rigidbody>();
        bulletRb.AddForce(transform.forward * bulletSpeed, ForceMode.VelocityChange);
        // Invoke("DestroyBullet", flyTime);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    private void OnTriggerEnter(Collider other) {
        if (!IsOwner) {
            return;
        }
        if (other.CompareTag("Enemy")) {
            other.SendMessage("DamageTaken", damage);
        }
        if (other.gameObject.CompareTag("Player")) {
            return;
        }

        if (other.gameObject.tag == "barril")
        {
            StartCoroutine(other.gameObject.GetComponent<ExplosionItem>().ExplosionTime(0.5f));
            // Destroy(other.gameObject);
            Destroy(gameObject);
            //When it collides with an object, it instantiates the flare and disappears in 1 second.
            GameObject b = Instantiate(Explosion, transform.position, transform.rotation);
            Destroy(b, 1);

        }

        parent.DestroyBulletServerRpc();

    }
}
