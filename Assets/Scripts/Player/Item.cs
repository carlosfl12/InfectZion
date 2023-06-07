using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            Destroy(gameObject);
            other.gameObject.GetComponent<Health>().currentHealth += 5;
            if (other.gameObject.GetComponent<Health>().currentHealth >= other.gameObject.GetComponent<Health>().maxHealth) {
                other.gameObject.GetComponent<Health>().currentHealth = other.gameObject.GetComponent<Health>().maxHealth;
                other.gameObject.GetComponentInChildren<HealthBar>().UpdateHealth();
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
