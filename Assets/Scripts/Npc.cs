using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Npc : MonoBehaviour
{
    public static bool bossDefeated;
    public Animator _animator;
    public bool playerNear;
    public bool normalState = false;
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        if (bossDefeated && playerNear && !normalState) {
            _animator.SetBool("Fear", true);
            _animator.applyRootMotion = true;
            StartCoroutine(BackToNormal());
        }  

        if (normalState) {
            _animator.applyRootMotion = false;
            _animator.SetBool("Fear", false);
            _animator.SetBool("Normal", true);
        }
    }

    private void OnTriggerStay(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            playerNear = true;
        } else {
            playerNear = false;
        }
    }

    IEnumerator BackToNormal() {
        yield return new WaitForSeconds(5f);
            normalState = true;
            StartCoroutine(WinScene());
    }

    IEnumerator WinScene()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("GameOver");
    }
}
