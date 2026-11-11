using Unity.Netcode;
using UnityEngine;

public class NetworkBottle : NetworkBehaviour
{
    public AudioClip impactSound;
    public AudioClip shatterSound;
    public float volume = 1f;

    private Rigidbody body;
    private Collider bottleCollider;
    private NetworkBottleInteractor holder;
    private bool armed;
    private bool broken;

    private NetworkVariable<bool> held = new(false);

    void Awake()
    {
        body = GetComponent<Rigidbody>();
        bottleCollider = GetComponent<Collider>();
    }

    public override void OnNetworkSpawn()
    {
        held.OnValueChanged += OnHeldChanged;
        bottleCollider.enabled = !held.Value;
    }

    public override void OnNetworkDespawn()
    {
        held.OnValueChanged -= OnHeldChanged;
    }

    void OnHeldChanged(bool previous, bool current)
    {
        bottleCollider.enabled = !current;
    }

    // Only the server grants pickup, preventing simultaneous grabs.
    public bool TryHold(NetworkBottleInteractor player)
    {
        if (!IsServer || held.Value || armed || broken)
            return false;

        holder = player;
        body.linearVelocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        body.isKinematic = true;
        held.Value = true;
        bottleCollider.enabled = false;
        return true;
    }

    void LateUpdate()
    {
        if (!IsServer || !held.Value)
            return;

        if (holder == null || !holder.IsSpawned)
        {
            held.Value = false;
            bottleCollider.enabled = true;
            body.isKinematic = false;
            return;
        }

        transform.SetPositionAndRotation(
            holder.ServerHoldPosition, holder.ServerHoldRotation);
    }

    public void Launch(Vector3 direction)
    {
        if (!IsServer || !held.Value)
            return;

        holder = null;
        armed = true;
        held.Value = false;
        bottleCollider.enabled = true;
        body.isKinematic = false;
        body.linearVelocity = direction.normalized * 10f;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!IsSpawned || !IsServer || held.Value || broken)
            return;

        if (armed)
        {
            broken = true;
            PlaySoundRpc(true, transform.position);
            NetworkObject.Despawn();
        }
        else if (collision.relativeVelocity.magnitude > 0.5f)
        {
            PlaySoundRpc(false, transform.position);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    void PlaySoundRpc(bool shatter, Vector3 position)
    {
        AudioClip clip = shatter ? shatterSound : impactSound;
        if (clip != null)
            AudioSource.PlayClipAtPoint(clip, position, volume);
    }
}