using UnityEngine;
using UnityEngine.InputSystem;

public class BottleInteraction : MonoBehaviour
{
    public Transform playerCamera;
    public Transform holdPoint;
    public Rigidbody bottle;
    public float pickupRange = 3f;
    public float throwSpeed = 10f;

    private bool isHolding;
    private Collider bottleCollider;

    void Start()
    {
        bottleCollider = bottle.GetComponent<Collider>();
    }

    void Update()
    {
        if (Keyboard.current == null || Mouse.current == null)
            return;

        if (Cursor.lockState != CursorLockMode.Locked)
            return;

        if (!isHolding && Keyboard.current.eKey.wasPressedThisFrame)
        {
            TryPickup();
        }
        else if (isHolding &&
                 Mouse.current.leftButton.wasPressedThisFrame)
        {
            Throw();
        }
    }

    void TryPickup()
    {
        if (bottle == null)
            return;
        // Stop at the first surface in front of the camera.
        if (!Physics.Raycast(
                playerCamera.position,
                playerCamera.forward,
                out RaycastHit hit,
                pickupRange))
            return;

        if (hit.rigidbody != bottle)
            return;

        // Clear motion before switching off physics.
        bottle.linearVelocity = Vector3.zero;
        bottle.angularVelocity = Vector3.zero;
        bottle.isKinematic = true;
        bottleCollider.enabled = false;

        // Attach it without changing its size.
        bottle.transform.SetParent(holdPoint, true);
        bottle.transform.localPosition = Vector3.zero;
        bottle.transform.localRotation = Quaternion.identity;

        isHolding = true;
    }

    void Throw()
    {
        bottle.GetComponent<BottleBreak>().Arm();
        bottle.transform.SetParent(null, true);
        bottleCollider.enabled = true;
        bottle.isKinematic = false;

        // Launch in the direction the camera faces.
        bottle.linearVelocity = playerCamera.forward * throwSpeed;

        isHolding = false;
    }
}