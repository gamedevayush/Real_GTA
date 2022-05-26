using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    private CharacterController controller;
    private Animator animator;
    public Transform playerCamera;
    public float speed = 6f;
    public float runSpeed = 4f;
    public float turnSmoothTime = 0.1f;
    public float turnSmootjVelocity;
    public float playerSpeed;
    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (controller == null) {
            return;
        }
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal,0f,vertical);
        
        if (direction.magnitude >= 0.1f)
        {

            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + playerCamera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmootjVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDirection.normalized * playerSpeed * Time.deltaTime);
        }

        if (direction != Vector3.zero && !Input.GetKey(KeyCode.LeftShift))
        {
            Walk();
            playerSpeed = speed;
        } else if (direction != Vector3.zero && Input.GetKey(KeyCode.LeftShift)) {
            Run();
            playerSpeed = runSpeed;
        }else if (Vector3.zero == direction) {
            Idle();
        }
    }

    private void Walk() {
        animator.SetFloat("Movement", 0.5f);

    }

    private void Idle() {
        animator.SetFloat("Movement", 0);
    }

    private void Run() {
        animator.SetFloat("Movement",1);
    }
}
