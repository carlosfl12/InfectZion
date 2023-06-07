using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public float currentHealth = 5f;
    public float maxHealth = 5f;
    public UnityEvent onDamageTaken;
    public UnityEvent onDead;
    
    void DamageTaken(float amount) {
        currentHealth -= amount;
        onDamageTaken.Invoke();

        if (currentHealth <= 0) {
            onDead.Invoke();
            if (gameObject.CompareTag("Enemy")) {
                GameManager.sharedInstance.RemoveEnemy(gameObject);
            }
        }
    }


}
