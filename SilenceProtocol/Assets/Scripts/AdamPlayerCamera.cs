using Unity.Netcode;
using UnityEngine;

public class AdamPlayerCamera : NetworkBehaviour
{
    [SerializeField] Camera playerCamera;
    [SerializeField] AudioListener listener;

    public override void OnNetworkSpawn()
    {
        // Only the player who owns this capsule gets a working camera.
        playerCamera.enabled = IsOwner;
        if (listener != null) listener.enabled = IsOwner;
    }
}