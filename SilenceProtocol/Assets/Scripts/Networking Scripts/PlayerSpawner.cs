using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] NetworkObject playerPrefab;
    [SerializeField] string gameSceneName = "temp";

    void Start()
    {
        var network = NetworkManager.Singleton;
        if(network.gameObject != gameObject)
        {
            return;
        }
        network.OnServerStarted += HandleServerStarted;
    }

    void HandleServerStarted()
    {
        //https://learn.microsoft.com/en-us/training/modules/create-manage-events/4-subscribe-unsubscribe-events-csharp
        //https://docs.unity3d.com/Packages/com.unity.netcode.gameobjects@1.7/api/Unity.Netcode.NetworkSceneManager.OnLoadEventCompleted.html
        var sceneManager = NetworkManager.Singleton.SceneManager;
        sceneManager.OnLoadEventCompleted -= HandleLoadEventCompleted;
        sceneManager.OnLoadEventCompleted += HandleLoadEventCompleted;
        //OnLoadEventCompleted is NOT a method, its an event.
    }

    void HandleLoadEventCompleted(string sceneName, LoadSceneMode mode,
                                  List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        if(sceneName != gameSceneName)
        {
            return;
        }

        for (int i = 0; i < clientsCompleted.Count; i++)
        {
            NetworkObject player = Instantiate(playerPrefab);
            player.SpawnAsPlayerObject(clientsCompleted[i], true);
        }
    }
}