using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovements : MonoBehaviour
{
    Vector3 moveDirection;
    public float runSpeed = 4;
    public float jogSpeed = 2.5f;
    public float jumpSpeed = 8;
    public float currentSpeed;
    public float gravity = 20;
    public bool hasLanded;
    public PlayerState currentState;


    public enum PlayerState
    {
        Idling,
        Crouching,
        Walking,
        Jumping,
        Aiming,
        Jogging,
    }

    void Update()
    {
        DoInput();

        switch (currentState)
        {
            case PlayerState.Idling:
                currentSpeed = 1;
                break;

            case PlayerState.Crouching:
                currentSpeed = jogSpeed / 2;
                break;

            case PlayerState.Walking:
                currentSpeed = runSpeed;
                break;

            case PlayerState.Jumping:
                break;

            case PlayerState.Aiming:
                currentSpeed = runSpeed;
                break;

            case PlayerState.Jogging:
                currentSpeed = jogSpeed;
                break;
        }

        if (currentState == PlayerState.Crouching)
        {
            Player.Instance.cc.center = Vector3.up * 0.75f;
            Player.Instance.cc.height = 1.5f;
        }
        else
        {
            Player.Instance.cc.center = Vector3.up;
            Player.Instance.cc.height = 2;
        }


        if (Player.Instance.cc.isGrounded)
        {
            moveDirection = Quaternion.Euler(Vector3.up * Player.Instance.cam.rotor.transform.eulerAngles.y) * new Vector3(Player.Instance.inputs.GetAxis("Horizontal"), 0, Player.Instance.inputs.GetAxis("Vertical")) * currentSpeed;

            if (Player.Instance.inputs.GetButtonDown("Jump") && !Player.Instance.inputs.GetButton("Crouch"))
            {
                moveDirection.y = jumpSpeed;
            }
        }

        moveDirection.y -= gravity * Time.deltaTime;

        Player.Instance.cc.Move(moveDirection * Time.deltaTime);

        void DoInput()
        {
            Vector3 input = new Vector2(Player.Instance.inputs.GetAxis("Horizontal"), Player.Instance.inputs.GetAxis("Vertical"));

            if (input.magnitude == 0 && Player.Instance.cc.isGrounded && !Player.Instance.inputs.GetButton("Crouch"))
                currentState = PlayerState.Idling;

            if (input.magnitude > 0 && Player.Instance.inputs.GetButton("Walk") && Player.Instance.cc.isGrounded && !Player.Instance.inputs.GetButton("Crouch"))
                currentState = PlayerState.Walking;

            if (input.magnitude > 0 && Player.Instance.cc.isGrounded && !Player.Instance.inputs.GetButton("Crouch") && !Player.Instance.inputs.GetButton("Walk"))
                currentState = PlayerState.Jogging;

            if (Player.Instance.inputs.GetButtonDown("Jump") && Player.Instance.cc.isGrounded && !Player.Instance.inputs.GetButton("Crouch"))
                currentState = PlayerState.Jumping;

            if (Player.Instance.cc.isGrounded && Player.Instance.inputs.GetButton("Crouch"))
                currentState = PlayerState.Crouching;
        }
    }
}

