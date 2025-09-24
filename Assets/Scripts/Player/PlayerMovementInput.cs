using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementInput : MonoBehaviour
{
    public Animator animator;
    public Rigidbody2D rigidBody;
    private Vector2 movementInput;
    public Camera mainCamera;
    public delegate float DirectionDelegate();
    private bool inputActive = false;

    private float rotationLerpTime = 1f; 
    private float lerpStartTime;
    private Quaternion targetRotation;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator.SetBool("isWalking", false);
        mainCamera = Camera.main;

        PlayerInputActions playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Move.performed += Move_performed;
        playerInputActions.Player.Move.canceled  += Move_canceled;
    }

    private void Move_canceled(InputAction.CallbackContext context)
    {
        movementInput = Vector2.zero;
        rigidBody.velocity = Vector2.zero;
        animator.SetBool("isWalking", false);
        inputActive = false; // Clear the active input flag
    }

    private void Move_performed(InputAction.CallbackContext obj)
    {
        if (!inputActive) // Only process the input if no other input is active
        {
            movementInput = obj.ReadValue<Vector2>();
            animator.SetBool("isWalking", true);
            inputActive = true; // Set the active input flag
        }
    }

    private void Update()
    {
        // Rotate player based on mouse position
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 direction = mousePosition - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        targetRotation = Quaternion.Euler(new Vector3(0, 0, angle + 90));

        // Start or continue lerping towards the target rotation
        float timeSinceStarted = Time.time - lerpStartTime;
        float percentageComplete = timeSinceStarted / rotationLerpTime;

        // Update the rotation
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, percentageComplete);

        // Reset lerp start time if the target is reached or if just starting
        if (percentageComplete >= 1.0f)
        {
            lerpStartTime = Time.time;
        }
    }

    void FixedUpdate()
    {
        // Move the player based on WASD input relative to facing direction
        if (movementInput != Vector2.zero)
        {
            // The following if else statement gets the vector where the player travels.
            Vector3 movementDirection = Vector3.zero;
            if      (movementInput == new Vector2( 1, 0)) movementDirection = MovePlayer(Right);
            else if (movementInput == new Vector2(-1, 0)) movementDirection = MovePlayer(Left);
            else if (movementInput == new Vector2( 0, 1)) movementDirection = MovePlayer(Forward);
            else if (movementInput == new Vector2( 0,-1)) movementDirection = MovePlayer(Backward);

            Vector3 velocity = movementDirection * Globals.PLAYER_SPEED;            
            rigidBody.velocity = velocity;
            CustomEvents.UpdateGroundLayer(velocity);
        }
        else
        {
            rigidBody.velocity = Vector2.zero;
        }
    }

    private Vector3 MovePlayer(DirectionDelegate direction)
    {
        float facingAngleInRadians = transform.rotation.eulerAngles.z * Mathf.Deg2Rad + direction.Invoke();
        
        return new Vector3(Mathf.Cos(facingAngleInRadians), Mathf.Sin(facingAngleInRadians),0).normalized;
    }

    private void OnEnable()
    {
        CustomEvents.OnFireWeapon += Move_canceled;
    }

    private void OnDisable()
    {
        CustomEvents.OnFireWeapon -= Move_canceled;
    }


    float Forward()  => -MathF.PI / 2;
    float Backward() =>  MathF.PI / 2;
    float Left()     =>  0f;
    float Right()    =>  MathF.PI;

}
