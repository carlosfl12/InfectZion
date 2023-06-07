using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionItem : MonoBehaviour
{
    public static ExplosionItem sharedInstance;
    public static bool inDamageRange = false;
    public List<GameObject> targets = new List<GameObject>();

    public void Start() {
        if (sharedInstance == null) {
            sharedInstance = this;
        }
        if (gameObject.CompareTag("barril")) {
            return;
        } else {
            StartCoroutine(ExplosionTime(3f));
        }
    }

    void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.CompareTag("Player") || collider.gameObject.CompareTag("Enemy")) {
            inDamageRange = true;
            if (!targets.Contains(collider.gameObject)) {
                targets.Add(collider.gameObject);
            }
        }
    }

    void OnTriggerExit(Collider collider) {
        if (collider.gameObject.CompareTag("Player") || collider.gameObject.CompareTag("Enemy")) {
            inDamageRange = false;
        }
        
        if (targets.Contains(collider.gameObject)) {
            targets.Remove(collider.gameObject);
        }
    }

    public void DoDamage() {
        foreach (GameObject target in targets) {
            target.GetComponent<Health>().currentHealth -= 5f;
            target.GetComponentInChildren<HealthBar>().UpdateHealth();
        }
        Destroy(gameObject);
    }

    public IEnumerator ExplosionTime(float time) {
        yield return new WaitForSeconds(time);
        DoDamage();
    }

}
