using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Movement : NetworkBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] float gravity = -20f;

    CharacterController controller;
    float verticalVelocity;

    public override void OnNetworkSpawn()
    {
        controller = GetComponent<CharacterController>();

        // Remote copies are moved by NetworkTransform, so the controller must not fight it.
        if (!IsOwner) controller.enabled = false;
    }

    void Update()
    {
        if (!IsOwner) return;   // only the player who owns this capsule reads input

        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        Vector3 move = Vector3.ClampMagnitude(input, 1f) * speed;

        if (controller.isGrounded && verticalVelocity < 0f) verticalVelocity = -2f;
        verticalVelocity += gravity * Time.deltaTime;
        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);
    }
}