using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Vector3 playerInput;
    public CharacterController player; 
    public float speed = 5f;
    public float jumpForce = 5f;
    public float fallVelocity;
    public float gravity = 9.81f;
    public bool isRunning = false;
    
    

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        playerInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (Input.GetKey(KeyCode.LeftShift)) {
            isRunning = true;
        } else {
            isRunning = false;
        }

        if (isRunning) {
            playerInput *= 2;
        }

        Move();
    }

    void Move() {

        if (Input.GetKeyDown(KeyCode.Space) && player.isGrounded) {
            fallVelocity = jumpForce;
        }

        fallVelocity -= gravity * Time.deltaTime;
        if (fallVelocity <= -3f) {
            fallVelocity = -3f; 
        }
        playerInput.y = fallVelocity;
        player.Move((playerInput * speed) * Time.deltaTime);
    }
}
