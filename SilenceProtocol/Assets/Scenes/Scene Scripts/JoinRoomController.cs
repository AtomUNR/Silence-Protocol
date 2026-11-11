using TMPro;
using UnityEngine;

public class JoinRoomController : MonoBehaviour
{
    //https://docs.unity3d.com/Packages/com.unity.textmeshpro@4.0/api/TMPro.TMP_InputField.html
    
    [SerializeField] TMP_InputField codeInput;
    [SerializeField] TMP_Text statusText;

    public void OnJoinPressed()
    {
        NetworkBootstrap.Instance.SetJoinCode(codeInput.text);
        NetworkBootstrap.Instance.Join();
    }

    void Update()
    {
        if (NetworkBootstrap.Instance != null)
            statusText.text = NetworkBootstrap.Instance.Status;
    }
}