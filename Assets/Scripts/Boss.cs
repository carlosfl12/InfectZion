using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public enum BossStates {
        Dead,
        DistanceAttack,
        NormalAttack,
        Running,
        Walking,
        Dancing
    }

    public BossStates currentState;
    public Vector3 targetPosition;
    public Vector3 towardsPosition;
    public float wanderRadius = 2f;
    public Animator animator;
    public GameObject target;
    public Health health;
    public float distance;
    public bool canDistanceAttack = true;
    public GameObject ballPrefab;
    public float ballForce = 15f;
    public Transform rightHand;
    

    void Start() {
        RecalculatePosition();
        towardsPosition = targetPosition - transform.position;
        MoveTowards(towardsPosition.normalized);

        animator = GetComponent<Animator>();
        currentState = BossStates.Dancing;

        health = GetComponentInChildren<Health>();
    }

    void Update() {
        switch(currentState) {
            case BossStates.Dead:
                DeadBehaviour();
                break;
            case BossStates.DistanceAttack:
                DistanceBehaviour();
                break;
            case BossStates.NormalAttack:
                AttackingBehaviour();
                break;
            case BossStates.Walking:
                //WalkingBehaviour();
                break;
            case BossStates.Running:
                RunningBehaviour();
                break;
            case BossStates.Dancing:
                DancingBehaviour();
                break;
        }

        if (GameObject.FindGameObjectWithTag("Player") != null) {
            target = GameObject.FindGameObjectWithTag("Player");       
        }
    }


    void DancingBehaviour() {
        animator.SetBool("IsDancing", true);
        if (target == null) {
            return;
        }
        distance = Vector3.Distance(transform.position, target.transform.position);
        if (health.currentHealth < health.maxHealth || distance < 10f) {
            currentState = BossStates.Running;
        }
    }

    public void OnBossDead() {
        currentState = BossStates.Dead;
        animator.SetBool("IsDancing", false);
        animator.SetBool("CanRun", false);
        animator.SetBool("CanAttack", false);
        animator.SetBool("IsDistance", false);
        animator.SetBool("IsDead", true);


    }

    void RunningBehaviour() {
        distance = Vector3.Distance(transform.position, target.transform.position);
        animator.SetBool("IsDancing", false);
        animator.SetBool("CanRun", true);
        animator.SetBool("CanAttack", false);

        if (distance <= 1.5f) {
            currentState = BossStates.NormalAttack;
        }
        else if (distance >= 5 && distance <= 10f && canDistanceAttack) {
            currentState = BossStates.DistanceAttack;
        }
        transform.LookAt(target.transform.position);
        Debug.DrawLine(transform.position, target.transform.position, Color.magenta);
        // MoveTowards(-target.transform.position);
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, 3f * Time.deltaTime);
    }

    void DeadBehaviour() {
        animator.SetBool("IsDancing", false);
        animator.SetBool("IsDead", true);
        Npc.bossDefeated = true;
    }

    void AttackingBehaviour() {
        distance = Vector3.Distance(transform.position, target.transform.position);
        animator.SetBool("CanRun", false);
        animator.SetBool("CanAttack", true);

        if (distance >= 2.5f) {
            currentState = BossStates.Running;
        }
        transform.LookAt(target.transform.position);
    }

    void Attack() {
        if (distance <= 2.5f) {
            target.SendMessage("DamageTaken", 5f);
        }
    }

    public void DistanceBehaviour() {
        animator.SetBool("IsDistance", true);
        animator.SetBool("CanAttack", false);
        animator.SetBool("CanRun", false);

        transform.LookAt(target.transform.position);

    }

    public void Throw() {
        StartCoroutine(CantDistanceAttackForSeconds(10f));
        animator.SetBool("IsDistance", false);
        animator.SetBool("CanRun", true);
        GameObject ballRb = Instantiate(ballPrefab, rightHand.position, rightHand.rotation);
        
        ballRb.GetComponent<Rigidbody>().AddForce(transform.forward * ballForce, ForceMode.Impulse);
        transform.LookAt(target.transform.position);
    }

    IEnumerator CantDistanceAttackForSeconds(float time) {
        yield return new WaitForSeconds(2f);
        currentState = BossStates.Running;
        canDistanceAttack = false;
        yield return new WaitForSeconds(time);
        canDistanceAttack = true;
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
