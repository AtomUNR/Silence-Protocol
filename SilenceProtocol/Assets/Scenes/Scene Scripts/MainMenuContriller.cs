
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuContriller : MonoBehaviour
{
    public void OnPlayHostButtonPressed()
    {
        Debug.Log("Start HOST Was Selected. \n");
        SceneManager.LoadScene("Host Join Room");
    }
    public void OnPlayPlayerButtonPressed()
    {
        Debug.Log("Start PLAYER Was Selected. \n");
        SceneManager.LoadScene("Player Join Room");
    }
    public void OnExitButtonPressed()
    {
        Debug.Log("Exiting Game... \n");
        Application.Quit();
    }
}
