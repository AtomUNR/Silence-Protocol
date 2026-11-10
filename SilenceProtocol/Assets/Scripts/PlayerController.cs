using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : NetworkBehaviour
{
    [SerializeField] Transform head;
    [SerializeField] Camera playerCamera;
    [SerializeField] AudioListener audioListener;

    [SerializeField] float walkSpeed = 3.5f;
    [SerializeField] float sprintSpeed = 5.5f;
    [SerializeField] float gravity = -20f;
    [SerializeField] float mouseSensitivity = 2f;

    readonly NetworkVariable<float> netPitch = new(
        0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    CharacterController controller;
    float pitch;
    float verticalVelocity;

    public override void OnNetworkSpawn()
    {
        controller = GetComponent<CharacterController>();
        
        Debug.Log("Spawned Object");

        playerCamera.enabled = IsOwner;
        audioListener.enabled = IsOwner;

        if (IsOwner)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            controller.enabled = false;
        }
    }

    void Update()
    {
        if (!IsOwner)
        {
            float p = Mathf.LerpAngle(head.localEulerAngles.x, netPitch.Value, 12f * Time.deltaTime);
            head.localEulerAngles = new Vector3(p, 0f, 0f);
            return;
        }

        // Escape frees the mouse so you can click other windows; click to recapture.
        if (Input.GetKeyDown(KeyCode.Escape)) { Cursor.lockState = CursorLockMode.None; Cursor.visible = true; }
        if (Input.GetMouseButtonDown(0)) { Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false; }

        // Look
        float mx = Input.GetAxis("Mouse X") * mouseSensitivity;
        float my = Input.GetAxis("Mouse Y") * mouseSensitivity;
        transform.Rotate(0f, mx, 0f);
        pitch = Mathf.Clamp(pitch - my, -85f, 85f);
        head.localEulerAngles = new Vector3(pitch, 0f, 0f);
        netPitch.Value = pitch;

        // Move
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        input = Vector3.ClampMagnitude(input, 1f);
        float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;
        Vector3 move = transform.TransformDirection(input) * speed;

        if (controller.isGrounded && verticalVelocity < 0f) verticalVelocity = -2f;
        verticalVelocity += gravity * Time.deltaTime;
        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);
    }
}