using TMPro;
using UnityEngine;

public class HostRoomController : MonoBehaviour
{
    [SerializeField] TMP_Text joinCodeText;

    void Start()
    {
        if (NetworkBootstrap.Instance == null)
        {
            Debug.LogError("No NetworkBootstrap found. Press Play from the MainMenu scene.");
            enabled = false;
            return;
        }
        NetworkBootstrap.Instance.Host();
    }

    void Update()
    {
        var temp = NetworkBootstrap.Instance;
        joinCodeText.text = string.IsNullOrEmpty(temp.HostCode) ? temp.Status : "Join Code: " + temp.HostCode;
    }
}