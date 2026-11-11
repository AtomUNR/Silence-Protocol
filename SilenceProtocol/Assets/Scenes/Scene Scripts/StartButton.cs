using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
public class StartButton : MonoBehaviour
{
    [SerializeField] string gameSceneName = "temp";

    public void OnStartPressed()
    {
        var network = NetworkManager.Singleton;
        if(network == null || !network.IsHost)
        {
            return;
        }
        network.SceneManager.LoadScene(gameSceneName, LoadSceneMode.Single);
    }
}
