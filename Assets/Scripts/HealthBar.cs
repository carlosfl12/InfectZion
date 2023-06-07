using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public Health health;
    // Start is called before the first frame update
    void Start()
    {
        health = GetComponentInParent<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateHealth() {
        float x = health.currentHealth / health.maxHealth;
        if (x <= 0) {
            x = 0;
        }
        transform.localScale = new Vector3(x, 1, 1);
    }
}
