using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class Sounds : MonoBehaviour
{

    public AudioSource audioSource;
    public AudioClip audioClip;
    public AudioClip shot;
    public float volumen = 1f;

    // public Transform triggerZone;
    // public float triggerDistance = 5.0f;

    void Update()
    {
        // float distance = Vector3.Distance(transform.position, triggerZone.position);
        // if (distance < triggerDistance)
        // {
        //     if (!audioSource.isPlaying)
        //     {
        //         audioSource.Play();
        //     }
        // }
        // else
        // {
        //     if (audioSource.isPlaying)
        //     {
        //         audioSource.Stop();
        //     }
        // }
    }

    public void OnTriggerEnter(Collider other) {
        AudioSource.PlayClipAtPoint(audioClip,gameObject.transform.position);
        Destroy(gameObject);
    }
    public void OnMouseDown() {
        audioSource.PlayOneShot(shot);
    }

}
