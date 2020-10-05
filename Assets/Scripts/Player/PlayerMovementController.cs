using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{

    Vector3 defaultPostition;
    private CharacterController2D controller2D;

    public void Awake()
    {
        controller2D = gameObject.GetComponent<CharacterController2D>();
    }

    public void Init()
    {
        defaultPostition = transform.position;
    }

    public void Reset()
    {
        transform.position = defaultPostition;
        transform.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
    }

    public void Update(){
        float horizontalMovement = Input.GetAxis("Horizontal");
        bool jump = Input.GetButtonDown("Jump");
        controller2D.Move(horizontalMovement,jump);
    }

}
