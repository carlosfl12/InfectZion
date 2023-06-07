using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<GameObject> zombies = new List<GameObject>();
    public static GameManager sharedInstance;
    public List<GameObject> players = new List<GameObject>();
    // Start is called before the first frame update

    private void Awake() {
        if (sharedInstance == null) {
            sharedInstance = this;
        }
    }
    void Start()
    {
        // foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy")) {
        //     if (!zombies.Contains(enemy)) {
        //         zombies.Add(enemy);
        //     }
        // }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddEnemy(GameObject enemy) {
        zombies.Add(enemy);
    }

    public void RemoveEnemy(GameObject enemy) {
        zombies.Remove(enemy);
    }

    public void AddPlayer(GameObject player) {
        players.Add(player);
    }
    public void RemovePlayer(GameObject player) {
        players.Remove(player);
    }
}
