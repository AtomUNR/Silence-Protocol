using UnityEngine;

public class BottleBreak : MonoBehaviour
{
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
        if (!isArmed || isBroken)
            return;

        isBroken = true;
        Debug.Log("Bottle broke!", this);
        Destroy(gameObject);
    }
}