using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class BottleTestMovement : MonoBehaviour
{
    public Transform playerCamera;
    public float moveSpeed = 4f;
    public float mouseSensitivity = 0.1f;

    private CharacterController controller;
    private float verticalSpeed;
    private float cameraPitch;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Keyboard.current == null || Mouse.current == null)
            return;

        //Escape releases the mouse; clicking captures it again.
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (Cursor.lockState != CursorLockMode.Locked)
            return;

        //Rotate the body sideways and the camera up/down.
        Vector2 look = Mouse.current.delta.ReadValue();
        transform.Rotate(0f, look.x * mouseSensitivity, 0f);

        cameraPitch -= look.y * mouseSensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, -80f, 80f);
        playerCamera.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);

        // Normalize so diagonal movement is not faster.
        Vector2 input = Vector2.zero;
        if (Keyboard.current.wKey.isPressed) input.y += 1;
        if (Keyboard.current.sKey.isPressed) input.y -= 1;
        if (Keyboard.current.dKey.isPressed) input.x += 1;
        if (Keyboard.current.aKey.isPressed) input.x -= 1;
        input = Vector2.ClampMagnitude(input, 1f);

        Vector3 movement =
            transform.right * input.x + transform.forward * input.y;

        // CharacterController.Move needs us to apply gravity.
        if (controller.isGrounded && verticalSpeed < 0f)
            verticalSpeed = -2f;

        verticalSpeed += Physics.gravity.y * Time.deltaTime;

        controller.Move(
            (movement * moveSpeed + Vector3.up * verticalSpeed)
            * Time.deltaTime);
    }

    void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}