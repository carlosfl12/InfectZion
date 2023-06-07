using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class EnemyGenerator : NetworkBehaviour
{
    public GameObject[] enemies;
    public Transform spawnPosition;
    public int enemiesToSpawn;
    public int enemyCount;
    public int enemyIndex;

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            // StartCoroutine(SpawnEnemy(enemyIndex));
            GetComponent<Collider>().enabled = false;
            GenerateEnemiesServerRpc();
        }
    }

    IEnumerator SpawnEnemy() {
        while (enemyCount < enemiesToSpawn) {
            Vector3 randomPos = new Vector3(Random.Range(-2f, 2f), 0.3f, Random.Range(-2f, 2f));
            GameObject.Instantiate(enemies[enemyIndex], spawnPosition.position + randomPos, Quaternion.identity);
            enemyIndex++; 
            enemyCount++;
            if (enemyIndex > enemies.Length -1) {
                enemyIndex = 0;
            }
            yield return new WaitForSeconds(2f);
        }
    }

    [ServerRpc]
    void GenerateEnemiesServerRpc() {
        GenerateEnemiesClientRpc();
    }

    [ClientRpc]
    void GenerateEnemiesClientRpc() {
        GetComponent<Collider>().enabled = false;

        StartCoroutine(SpawnEnemy());
    }
}
