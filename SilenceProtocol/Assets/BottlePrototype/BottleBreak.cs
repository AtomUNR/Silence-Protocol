using UnityEngine;

public class BottleBreak : MonoBehaviour
{
    public AudioClip impactSound;
    public AudioClip shatterSound;

    [Range(0f, 1f)]
    public float volume = 1f;

    public float minimumImpactSpeed = 0.5f;

    private bool isArmed;
    private bool isBroken;

    public void Arm()
    {
        isArmed = true;
    }

    public void Disarm()
    {
        isArmed = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isBroken)
            return;

        if (isArmed)
        {
            isBroken = true;

            if (shatterSound != null)
                AudioSource.PlayClipAtPoint(
                    shatterSound, transform.position, volume);

            Debug.Log("Bottle broke!", this);
            Destroy(gameObject);
        }
        else if (collision.relativeVelocity.magnitude >= minimumImpactSpeed)
        {
            if (impactSound != null)
                AudioSource.PlayClipAtPoint(
                    impactSound, transform.position, volume);
        }
    }
}