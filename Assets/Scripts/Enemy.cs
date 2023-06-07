using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]private enum ZombieState {
        Walking,
        Ragdoll,
        Running,
        Eating,
        Attacking
    }

    public AudioSource biting;
    public Rigidbody[] rigidbodies;
    [SerializeField]private ZombieState _currentState = ZombieState.Walking;
    public Vector3 targetPosition;
    public Vector3 towardsTarget;
    public float wanderRadius = 5f;
    public Animator _animator;
    public CharacterController _characterController;
    public GameObject[] players;
    public GameObject target;
    public int zombieDamage = 2;
    public float nearestDistance = 25f;
    public bool canAttack = false;
    public bool hasTarget = false;
    public bool canRun = false;
    public bool playerDead = false;

    // Start is called before the first frame update
    private void Awake() {
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        _animator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();
        DisableRagdoll();
    }
    void Start()
    {
        RecalculatePosition();
        GameManager.sharedInstance.AddEnemy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        switch (_currentState) {
            case ZombieState.Walking:
                WalkingBehaviour();
                break;
            case ZombieState.Ragdoll:
                RagdollBehaviour();
                break;
            case ZombieState.Attacking:
                AttackingBehaviour();
                break;
            case ZombieState.Running:
                RunningBehaviour();
                break;
            case ZombieState.Eating:
                EatingBehaviour();
                break;
            
        }
    }

    public void onDeadHandler() {
        EnableRagdoll();
        _currentState = ZombieState.Ragdoll;
        Destroy(gameObject, 2f);
    }

    public GameObject GetNearestTarget() {

        //Comprobar distancia para que coja target
        if (players.Length == 0) {
            return null;
        }

        foreach (GameObject player in players) {
            float distance = Vector3.Distance(transform.position, player.transform.position);

            if (distance < nearestDistance) {
                target = player;
                nearestDistance = distance;

            }
        }
        return target;
    }

    void DisableRagdoll() {
        foreach (Rigidbody _rigidbody in rigidbodies) {
            _rigidbody.isKinematic = true;
        }

        _animator.enabled = true;
        _characterController.enabled = true;
    }

    void EnableRagdoll() {
        foreach (Rigidbody _rigidbody in rigidbodies) {
            _rigidbody.isKinematic = false;
        }
        _animator.enabled = false;
        _characterController.enabled = false;
    }
    public void WalkingBehaviour() {
        GetNearestTarget();
        if (target == null) {
            towardsTarget = targetPosition - transform.position;
            if (towardsTarget.magnitude < 1f) {
                RecalculatePosition();
            }
            MoveTowards(towardsTarget.normalized);
            Debug.DrawLine(transform.position, targetPosition, Color.green);
        } else {
            if (Vector3.Distance(transform.position, GetNearestTarget().transform.position) < 10f && GetNearestTarget().GetComponent<Health>().currentHealth > 0) {
                Quaternion toRotation = Quaternion.LookRotation(target.transform.position, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 20 * Time.deltaTime);
                transform.LookAt(target.transform.position);
            } else {
                if (GetNearestTarget().GetComponent<Health>().currentHealth <= 0) {
                    target = null;
                }
                towardsTarget = targetPosition - transform.position;
                if (towardsTarget.magnitude < 0.30f) {
                    RecalculatePosition();
                }
                MoveTowards(towardsTarget.normalized);
                Debug.DrawLine(transform.position, targetPosition, Color.green);
            }
        }

        canAttack = false;
        hasTarget = false;
        canRun = false;
        _animator.SetBool("CanAttack", canAttack);
        _animator.SetBool("HasTarget", hasTarget);
        _animator.SetBool("CanRun", canRun);

        if (target != null && target.GetComponent<Health>().currentHealth > 0) {
            hasTarget = true;
            _currentState = ZombieState.Running;
        }
    }
    public void RunningBehaviour() {
        canRun = true;
        canAttack = false;
        _animator.SetBool("HasTarget", hasTarget);
        _animator.SetBool("CanRun", canRun);
        _animator.SetBool("CanAttack", canAttack);

        Quaternion toRotation = Quaternion.LookRotation(GetNearestTarget().transform.position, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 20 * Time.deltaTime);
        transform.LookAt(target.transform.position);

        if (Vector3.Distance(transform.position, target.transform.position) <= 2.5f && target.GetComponent<Health>().currentHealth > 0) {
            _currentState = ZombieState.Attacking;
        }

        if (target.GetComponent<Health>().currentHealth <= 0) {
            hasTarget = false;
            _animator.SetBool("HasTarget", hasTarget);
            if (Vector3.Distance(transform.position, target.transform.position) <= 2.5f) {
                _currentState = ZombieState.Eating;
            } else {
                _currentState = ZombieState.Walking;
            }
        }

    }

    public void RagdollBehaviour() {
        playerDead = false;
        canAttack = false;
        canRun = false;
        hasTarget = false;
        _animator.SetBool("PlayerDead", playerDead);
        _animator.SetBool("CanAttack", canAttack);
        _animator.SetBool("CanRun", canRun);
        _animator.SetBool("HasTarget", hasTarget);
    }

    public void AttackingBehaviour() {
        float distance = Vector3.Distance(transform.position, GetNearestTarget().transform.position);
        canAttack = true;
        canRun = false;
        _animator.SetBool("CanAttack", canAttack);
        _animator.SetBool("CanRun", canRun);

        if (target.GetComponent<Health>().currentHealth <= 0 && distance <= 2f) {
            _currentState = ZombieState.Eating;
        }
        if (distance > 2.5f) {
            _currentState = ZombieState.Walking;
        }
    }

    public void EatingBehaviour() {
        playerDead = true;
        canAttack = false;
        canRun = false;
        hasTarget = false;
        _animator.SetBool("PlayerDead", playerDead);
        _animator.SetBool("CanAttack", canAttack);
        _animator.SetBool("CanRun", canRun);
        _animator.SetBool("HasTarget", hasTarget);
        biting.Play();
    }

    public void Attack() {
        if (Vector3.Distance(transform.position, GetNearestTarget().transform.position) <= 2.5f) {
            GetNearestTarget().SendMessage("DamageTaken", zombieDamage);
        }
    }

    public void RecalculatePosition() {
        targetPosition = transform.position + Random.insideUnitSphere * wanderRadius;
        targetPosition.y = transform.position.y;
    }

    public void MoveTowards(Vector3 direction) {
        transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, 3f * Time.deltaTime);
        
        Quaternion towardsTargetRotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, towardsTargetRotation, 180f * Time.deltaTime);
    }
}
