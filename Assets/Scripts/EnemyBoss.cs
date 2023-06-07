using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss : Enemy
{
    public float speed = 2f;
    public float ballForce = 70f;
    public enum BossStates {
        Walk,
        Running,
        Dead,
        Attack,
        DistanceAttack
    }

    public BossStates bossState;
    public bool canDistanceAttack = true;
    public Transform rightHand;
    public GameObject ballPrefab;
    private void Update() {
        if (Npc.bossDefeated) {
            return;
        }
        switch(bossState) {
            case BossStates.Walk:
                WalkingBehaviour();
                break;
            case BossStates.Attack:
                AttackingBehaviour();
                break;
            case BossStates.Running:
                RunningBehaviour();
                break;
            case BossStates.DistanceAttack:
                DistanceBehaviour();
                break;
            case BossStates.Dead:
                DeadBehaviour();
                break;
        }

        if (target != null) {
            bossState = BossStates.Running;
            if (bossState == BossStates.Running) {
                transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
            }

            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance <= 2.5f) {
                bossState = BossStates.Attack;
            } else {
                if (canDistanceAttack) {
                    bossState = BossStates.DistanceAttack;
                }
            }
        }
    }

    public void onBossDeadHandler() {
        Npc.bossDefeated = true;
        _animator.SetBool("IsDead", true);
        _animator.SetBool("IsDistance", false);
        _animator.SetBool("CanRun", false);
        _animator.SetBool("CanAttack", false);
        bossState = BossStates.Dead;
    }


    public void DistanceBehaviour() {
        _animator.SetBool("IsDistance", true);
        _animator.SetBool("CanAttack", false);
        _animator.SetBool("CanRun", false);

        transform.LookAt(target.transform.position);

    }

    public void Throw() {
        StartCoroutine(CantDistanceAttackForSeconds(10f));
        _animator.SetBool("IsDistance", false);
        bossState = BossStates.Running;
        _animator.SetBool("CanRun", true);
        GameObject ballRb = Instantiate(ballPrefab, rightHand.position, rightHand.rotation);
        
        ballRb.GetComponent<Rigidbody>().AddForce(transform.forward * ballForce, ForceMode.Impulse);
    }

    public void DeadBehaviour() {
        return;
    }

    IEnumerator CantDistanceAttackForSeconds(float time) {
        canDistanceAttack = false;
        yield return new WaitForSeconds(time);
        canDistanceAttack = true;
    }
}
