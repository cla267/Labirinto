using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class moovement : MonoBehaviourPunCallbacks
{
    public float speed = 3f;
    public int jumpSelected;
    public float jumpHeight = 3f;
    public float mass = 1;

    public LayerMask groundMask;
    public Transform groundCheck;
    public float groundDistance;

    public GameObject cam;

    public bool isGrounded;
    float movementSpeed;

    Vector3 velocity;

    public static bool canGravity = true;
    public static bool canJump = true;
    public static bool canMove = true;
    
    CharacterController controller;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        movementSpeed = speed;

        if(photonView.IsMine) cam.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
            
            if(canGravity) Gravity();
            if(canMove) Move();
            if(canJump) Jump();
        }
    }

    void Move()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector2 axis = new Vector2(x, z).normalized;
        Vector3 move = transform.right * axis.x + transform. forward * axis.y;

        controller.Move(move * speed * Time.deltaTime);
    }

    void Jump()
    {
        if(isGrounded && Input.GetKey(KeyCode.Space) && jumpSelected == 1)
        {
           velocity.y = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
        }else if(isGrounded && Input.GetKeyDown(KeyCode.Space) && jumpSelected == 2)
        {
           velocity.y = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
        }else if(isGrounded && Input.GetKeyUp(KeyCode.Space) && jumpSelected == 3)
        {
           velocity.y = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
        }
    }

    void Gravity()
    {
        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -1.5f;
        }

        velocity.y += Physics.gravity.y * mass * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void OnDrawGizmos() {
        Gizmos.DrawSphere(groundCheck.position, groundDistance);
    }
}
