using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]

public class PlayerControllerButtonMashRace : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private float playerSpeed = 10.0f;
    private float jumpHeight = 1.0f;
    private float gravityValue = -9.81f;

    private bool xPressed = false;
    private bool aPressed = false;
    private bool yPressed = false;
    private bool bPressed = false;
    private Vector2 movementInput = Vector2.zero;
    // Start is called before the first frame update
    void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();

    }
    public void OnMove(InputAction.CallbackContext context) {
        movementInput = context.ReadValue<Vector2>();
    }

    public void OnXPressed(InputAction.CallbackContext context) {
        xPressed = context.action.triggered;
    }

    public void OnAPressed(InputAction.CallbackContext context) {
        aPressed = context.action.triggered;
    }

    public void OnYPressed(InputAction.CallbackContext context) {
        yPressed = context.action.triggered;
    }

    public void OnBPressend(InputAction.CallbackContext context) {
        bPressed = context.action.triggered;
    }

    // Update is called once per frame
    void Update()
    {
        if (yPressed) {
            Debug.Log(gameObject.name + " apertou o botão Y");
        }

        if (xPressed) {
            Debug.Log(gameObject.name + " apertou o botão X");
        }

        if (bPressed) {
            Debug.Log(gameObject.name + " apertou o botão B");
        }

        if (aPressed) {
            Debug.Log(gameObject.name + " apertou o botão A");
        }

        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0) {
            playerVelocity.y = 0f;
        }

        Vector3 move = new Vector3(movementInput.x, 0, movementInput.y);
        controller.Move(move * Time.deltaTime * playerSpeed);

        // Makes the player jump
        if (xPressed && groundedPlayer) {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }
}
