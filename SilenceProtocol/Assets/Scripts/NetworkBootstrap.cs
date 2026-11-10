using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class NetworkBootstrap : MonoBehaviour
{
    [SerializeField] int maxPlayers = 4;

    string hostCode = "";
    string typedCode = "";
    string status = "Signing in...";
    bool ready;
    bool busy;

    async void Start()
    {
        try
        {
            await UnityServices.InitializeAsync();
            if (!AuthenticationService.Instance.IsSignedIn)
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            ready = true;
            status = "Ready";
        }
        catch (System.Exception e)
        {
            status = "Sign-in failed: " + e.Message;
        }
    }

    async Task HostAsync()
    {
        busy = true;
        try
        {
            Allocation alloc = await RelayService.Instance.CreateAllocationAsync(maxPlayers - 1);
            hostCode = await RelayService.Instance.GetJoinCodeAsync(alloc.AllocationId);

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(AllocationUtils.ToRelayServerData(alloc, "dtls"));

            NetworkManager.Singleton.StartHost();
            status = "Hosting";
        }
        catch (System.Exception e) { status = "Host failed: " + e.Message; }
        busy = false;
    }

    async Task JoinAsync()
    {
        busy = true;
        try
        {
            JoinAllocation join = await RelayService.Instance.JoinAllocationAsync(typedCode.Trim().ToUpper());

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(AllocationUtils.ToRelayServerData(join, "dtls"));

            NetworkManager.Singleton.StartClient();
            status = "Joining...";
        }
        catch (System.Exception e) { status = "Join failed: " + e.Message; }
        busy = false;
    }

    void OnGUI()
    {
        var nm = NetworkManager.Singleton;
        if (nm == null) return;

        GUILayout.BeginArea(new Rect(20, 20, 300, 260));
        GUILayout.Label(status);

        if (nm.IsListening)
        {
            if (nm.IsHost)
            {
                GUILayout.Label("Join code (share this):");
                GUILayout.TextField(hostCode);
            }
        }
        else
        {
            GUI.enabled = ready && !busy;
            if (GUILayout.Button("Host", GUILayout.Height(40))) _ = HostAsync();

            typedCode = GUILayout.TextField(typedCode, 6);
            if (GUILayout.Button("Join", GUILayout.Height(40))) _ = JoinAsync();
            GUI.enabled = true;
        }
        GUILayout.EndArea();
    }
}