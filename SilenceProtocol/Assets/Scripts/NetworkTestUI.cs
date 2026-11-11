using Unity.Netcode;
using UnityEngine;

public class NetworkTestUI : MonoBehaviour
{
    void OnGUI()
    {
        var nm = NetworkManager.Singleton;
        if (nm == null || nm.IsListening) return;

        GUILayout.BeginArea(new Rect(20, 20, 200, 120));
        if (GUILayout.Button("Start Host", GUILayout.Height(40))) nm.StartHost();
        if (GUILayout.Button("Start Client", GUILayout.Height(40))) nm.StartClient();
        GUILayout.EndArea();
    }
}