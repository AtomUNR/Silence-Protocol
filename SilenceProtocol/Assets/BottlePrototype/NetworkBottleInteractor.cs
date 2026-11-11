using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkBottleInteractor : NetworkBehaviour
{
    public Transform playerCamera;
    public Transform holdPoint;
    public float pickupRange = 3f;

    public Vector3 ServerHoldPosition { get; private set; }
    public Quaternion ServerHoldRotation { get; private set; }

    private NetworkBottle serverBottle;
    private float nextPoseTime;

    void Update()
    {
        if (!IsSpawned || !IsOwner ||
            Keyboard.current == null || Mouse.current == null ||
            Cursor.lockState != CursorLockMode.Locked)
            return;

        if (Keyboard.current.eKey.wasPressedThisFrame &&
            Physics.Raycast(playerCamera.position,
                playerCamera.forward, out RaycastHit hit, pickupRange))
        {
            NetworkBottle bottle =
                hit.collider.GetComponentInParent<NetworkBottle>();

            if (bottle != null && bottle.IsSpawned)
            {
                PickupRpc(new NetworkObjectReference(bottle.NetworkObject),
                    holdPoint.position, holdPoint.rotation);
            }
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            ThrowRpc(playerCamera.forward,
                holdPoint.position, holdPoint.rotation);
        }

        // Send the camera's holding position 20 times per second.
        if (Time.unscaledTime >= nextPoseTime)
        {
            nextPoseTime = Time.unscaledTime + 0.05f;
            HoldPoseRpc(holdPoint.position, holdPoint.rotation);
        }
    }

    bool ValidPose(Vector3 position)
    {
        return Vector3.Distance(transform.position, position) <= 3f;
    }

    [Rpc(SendTo.Server, RequireOwnership = true)]
    void PickupRpc(NetworkObjectReference reference,
        Vector3 position, Quaternion rotation)
    {
        if (serverBottle != null || !ValidPose(position))
            return;

        if (!reference.TryGet(out NetworkObject obj))
            return;

        NetworkBottle bottle = obj.GetComponent<NetworkBottle>();
        if (bottle == null ||
            Vector3.Distance(transform.position, obj.transform.position)
                > pickupRange + 1f)
            return;

        ServerHoldPosition = position;
        ServerHoldRotation = rotation;

        if (bottle.TryHold(this))
            serverBottle = bottle;
    }

    [Rpc(SendTo.Server, RequireOwnership = true)]
    void HoldPoseRpc(Vector3 position, Quaternion rotation)
    {
        if (serverBottle == null || !ValidPose(position))
            return;

        ServerHoldPosition = position;
        ServerHoldRotation = rotation;
    }

    [Rpc(SendTo.Server, RequireOwnership = true)]
    void ThrowRpc(Vector3 direction,
        Vector3 position, Quaternion rotation)
    {
        if (serverBottle == null || !ValidPose(position) ||
            direction.sqrMagnitude < 0.01f)
            return;

        serverBottle.transform.SetPositionAndRotation(position, rotation);
        serverBottle.Launch(direction);
        serverBottle = null;
    }
}