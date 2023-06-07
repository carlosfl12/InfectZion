using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Cinemachine;
using TMPro;
using UnityEngine.SceneManagement;
public class Player : NetworkBehaviour
{

    public enum PlayerState {
        Idle,
        Walking,
        Aiming,
        MeleeWeapon,
        DistanceWeapon,
        Death
    }
    //Game manager??
    public List<GameObject> enemies;
    public NetworkVariable<PlayerState> _currentState = new NetworkVariable<PlayerState>();
    public float speed = 5f;
    public bool running = false;
    public float rotationSpeed = 180f;
    public CinemachineVirtualCamera cam;
    public CharacterController character;
    public Animator anim;
    public float fallVelocity;
    public float gravity = 9.8f;
    public Vector3 movePlayer;
    public float jumpForce = 3f;
    public GameObject aimTarget;
    public LayerMask layer;
    public Gun gun;
    public bool isGrounded;
    public bool isDead;
    public bool isRunning;
    public bool isBackward;
    public Transform rightHand;
    public GameObject weaponHandler;
    public GameObject akPrefab;

    public GameObject axePrefab;
    public GameObject currentWeapon;

    public AudioSource audioSource;

    public AudioClip shot;

    //Net variables
    NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>();
    NetworkVariable<Vector3> networkRotation = new NetworkVariable<Vector3>();
    NetworkVariable<bool> hasWeapon = new NetworkVariable<bool>();

    private void Start() {
        character = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        GameManager.sharedInstance.AddPlayer(gameObject);
        
    }

    public override void OnNetworkSpawn()
    {
        cam = GameObject.FindGameObjectWithTag("VC").GetComponent<CinemachineVirtualCamera>();
        if (IsOwner && (IsHost || IsClient)) {
            SpawnGunServerRpc();
            Debug.Log(OwnerClientId + " ; " + currentWeapon);
        }
        UpdatePositionServerRpc();
    }
    

    private void Update() {
        if (isDead) {
            return;
        }
        if (IsOwner) {
            cam.Priority = 10;
            cam.Follow = transform;
            cam.LookAt = transform;
            // if (currentWeapon == null) {
            //     currentWeapon = transform.Find("WeaponHandler(Clone)").Find("AK(Clone)").gameObject;

            //     gun = currentWeapon.GetComponent<Gun>();
            //     gun.aimTarget = transform.Find("Aim").transform;
            //     gun.text = transform.Find("PlayerCanvas").GetComponentInChildren<TMP_Text>();
            // }

            

            if (Input.GetAxis("Vertical") > 0 || Input.GetAxis("Vertical") < 0) {
                ChangePlayerStateServerRpc(PlayerState.Walking);
            }
            
            if (Input.GetMouseButton(0) && _currentState.Value == PlayerState.DistanceWeapon) {
                gun.ShootServerRpc();
                if (audioSource.isPlaying) {
                    return;
                } else {
                    audioSource.PlayOneShot(shot);
                }
            }

            transform.Find("WeaponHandler(Clone)").transform.position = rightHand.transform.position;
            transform.Find("WeaponHandler(Clone)").transform.rotation = rightHand.transform.rotation;
            // Move(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")));

            if (Input.GetKeyDown(KeyCode.T)) {
                Debug.Log("Llamada desde: " + OwnerClientId);
            }

            switch (_currentState.Value) {
                case PlayerState.Idle:
                    IdleBehaviour();
                    break;
                case PlayerState.MeleeWeapon:
                    MeleeBehaviour();
                    break;
                case PlayerState.DistanceWeapon:
                    DistanceBehaviour();
                    break;
                case PlayerState.Walking:
                    Play();
                    break;
            }

            foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy")) {
                if (!enemies.Contains(enemy)) {
                    enemies.Add(enemy);
                    enemy.GetComponent<Enemy>().players = GameObject.FindGameObjectsWithTag("Player");
                }
            }

            } else {
                cam.Priority = 0;
                
            }

            if (IsServer) {
                foreach (GameObject player in GameManager.sharedInstance.players) {
                    if (player.GetComponent<Player>().gun == null) {
                        GetWeaponClientRpc();
                    }
                }
            }

        
    }

    void IdleBehaviour() {
        anim.SetBool("IsAttacking", false);
        anim.SetBool("MeleeWeapon", true);
        anim.SetBool("DistanceWeapon", false);
    }

    void MeleeBehaviour() {
        if (Input.GetMouseButton(0)) {
            anim.SetBool("IsAttacking", true);
        } else {
            anim.SetBool("IsAttacking",false);
        }
        anim.SetBool("MeleeWeapon", true);
        anim.SetBool("DistanceWeapon", false);

        gun.text.enabled = false;
    }

    void DistanceBehaviour() {
        gun.text.enabled = true;
        isRunning = false;
        isBackward = false;
        anim.SetBool("MeleeWeapon", false);
        anim.SetBool("IsRunning", isRunning);
        anim.SetBool("isBackward", isBackward);
        if (Input.GetMouseButton(1)) {
            anim.SetBool("DistanceWeapon", true);
            currentWeapon.transform.position = currentWeapon.transform.parent.TransformPoint(new Vector3(0, 0.149f, 0.012f));
            currentWeapon.transform.rotation = Quaternion.Euler(new Vector3(14f, 189f, 253f));

        }else {
            anim.SetBool("DistanceWeapon", false);
        }
    }

    void Attack() {
        GameObject[] enemies = GameManager.sharedInstance.zombies.ToArray();
        foreach (GameObject enemy in enemies) {
            if (Vector3.Distance(transform.position, enemy.transform.position) <= 2.5f) {
                enemy.SendMessage("DamageTaken", 2);
            }
        }
    }

    public void onDeadHandler() {
        isDead = true;
        anim.SetBool("IsDead", isDead);
        character.center = new Vector3(0, 1.75f, 0);
        character.enabled = false;
        StartCoroutine(LoadDeadScene());

    }
    IEnumerator LoadDeadScene(){
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("GameOver");
    }
    [ServerRpc]
    public void UpdateStateServerRpc(PlayerState newState) {
      //añadir Estados   
    }

    void Play() {
        Vector3 direction = Input.GetAxis("Vertical") * transform.forward;
        Vector3 rotation = new Vector3(0, Input.GetAxis("Horizontal"), 0);

        if (IsOwner && _currentState.Value != PlayerState.DistanceWeapon) {
            Move(direction);
            Rotate(rotation);
            SetGravity(direction);
            PlayerSkills();
        }
    }

    void SetGravity(Vector3 direction) {
        if (character.isGrounded) {
            fallVelocity = -gravity * Time.deltaTime;
            movePlayer.y = fallVelocity;
        } else {
            fallVelocity -= gravity * Time.deltaTime;
            movePlayer.y = fallVelocity;
        }
    }

    void PlayerSkills() {
        isGrounded = character.isGrounded;
        if (character.isGrounded && Input.GetButtonDown("Jump")) {
            fallVelocity = jumpForce;
            movePlayer.y = fallVelocity;
        }
        character.Move(movePlayer * Time.deltaTime);
    }

    void Move(Vector3 direction) {
        anim.SetBool("DistanceWeapon", false);
        anim.SetBool("IsRunning", isRunning);
        anim.SetBool("isBackward", isBackward);
        anim.SetBool("MeleeWeapon", false);
        if (direction == Vector3.zero || _currentState.Value == PlayerState.DistanceWeapon) {
            isRunning = false;
            isBackward = false;
            return;
        }
        if (Input.GetKey(KeyCode.LeftShift)) {
            direction *= 2;
        }
        if (Input.GetAxis("Vertical") > 0) {
            isRunning = true;
            isBackward = false;
        } else{
            direction *= 0.5f;
            isRunning = false;
            isBackward = true;
        }

        movePlayer = direction;
        SetGravity(movePlayer);
        PlayerSkills();
        character.Move(movePlayer * speed * Time.deltaTime);
    }

    void Rotate(Vector3 rotation) {
        character.transform.Rotate(rotation * rotationSpeed * Time.deltaTime);
    }

    [ServerRpc]
    public void ChangePlayerStateServerRpc(PlayerState state) {
        _currentState.Value = state;
    }

    [ServerRpc]
    void SpawnGunServerRpc() {

        GameObject weaponHandlerGO = Instantiate(weaponHandler, rightHand.position, rightHand.rotation);
        GameObject ak = Instantiate(akPrefab, weaponHandlerGO.transform.position, Quaternion.identity);
        GameObject axeGO = Instantiate(axePrefab, weaponHandlerGO.transform.position, Quaternion.identity);
        weaponHandlerGO.GetComponent<NetworkObject>().Spawn();
        ak.GetComponent<NetworkObject>().Spawn();
        axeGO.GetComponent<NetworkObject>().Spawn();
        hasWeapon.Value = true;
        // gun = ak.GetComponent<Gun>();
        // gun.aimTarget = transform.Find("Aim").transform;
        // gun.text = transform.Find("PlayerCanvas").GetComponentInChildren<TMP_Text>();
        
        weaponHandlerGO.transform.parent = transform;
        ak.transform.parent = transform.Find("WeaponHandler(Clone)");
        axeGO.transform.parent = transform.Find("WeaponHandler(Clone)");
        ak.GetComponent<Gun>().aimTarget = transform.Find("Aim");
        // 

        AssingWeaponToPlayerClientRpc();
    }


    [ClientRpc]
    void AssingWeaponToPlayerClientRpc() {
        currentWeapon = transform.Find("WeaponHandler(Clone)").Find("AK(Clone)").gameObject;
    }
    [ClientRpc]
    void GetWeaponClientRpc() {
        if (transform.Find("WeaponHandler(Clone)") == null) {
            return;
        }
        currentWeapon = transform.Find("WeaponHandler(Clone)").Find("AK(Clone)").gameObject;

        currentWeapon.transform.position = rightHand.transform.position;
        currentWeapon.transform.rotation = rightHand.transform.rotation;
        gun = currentWeapon.GetComponent<Gun>();
        gun.text = transform.Find("PlayerCanvas").GetComponentInChildren<TMP_Text>();
    }

    [ServerRpc(RequireOwnership = false)]
    void UpdatePositionServerRpc() {
        transform.position = new Vector3(Random.Range(-5f, 5f), -0.1f, Random.Range(-5f, 5f));
        transform.rotation = new Quaternion(0, 180, 0, 0);
    }
}
