using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ParticleSpawn : NetworkBehaviour
{
    public GameObject particle;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            SpawnParticleServerRpc();
        }
    }

    [ServerRpc]
    void SpawnParticleServerRpc() {
        SpawnParticleClientRpc();
    }

    [ClientRpc]
    void SpawnParticleClientRpc() {
        GameObject.Instantiate(particle, transform.position, Quaternion.identity);
    }
}
