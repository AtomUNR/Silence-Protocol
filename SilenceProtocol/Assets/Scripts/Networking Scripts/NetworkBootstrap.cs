using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Networking.Transport.Relay;
using UnityEngine;

public class NetworkBootstrap : MonoBehaviour
{ 
    public static NetworkBootstrap Instance { get; private set; } 
    //Accessing the instance of the NetworkBootstrap  (singleton)

    [SerializeField] int maxPlayers = 4;

    string hostCode = "";
    string typedCode = "";
    string status = "Signing in...";
    bool ready;
    bool busy;
    Task initTask;

    public string HostCode => hostCode;
    public string Status => status;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        initTask = InitAsync();
    }

    async Task InitAsync() 
    {
        /*
        the 'async' keyword tells the compiler that this method has an
        'await' keyword. without it we get complialtion errors.
        check this link for more info: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/task-asynchronous-programming-model
       
        Its important to note that these keywords are NOT unique to Unity, they exist within the C# realm.
        Same thing goes for the Task object of which, more info can be found here on it: https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task?view=net-10.0
        */
        try
        {
            //https://docs.unity.com/en-us/services/services-core-api
            //The documentation for Unity Services.
            await UnityServices.InitializeAsync();

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                //https://docs.unity.com/en-us/authentication/use-anon-sign-in
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            ready = true;
            status = "Ready";
        }catch (System.Exception Excep)
        {
            status = "Sign in Failed: " + Excep.Message;
            Debug.Log(status);
        }
    }
    
    
    
    // Called by buttons / room scenes
    public void Host()
    {
        if (!busy)
        {
            //This ignores results: https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/patterns/discards
            _ = HostAsync();
        }
    }
    
    public void SetJoinCode(string code)
    {
        typedCode = code;
    }
    
    public void Join()
    {
        if (!busy)
        {
            _ = JoinAsync();
        }
    }
    async Task HostAsync() //starts relay server
    {
        /*
        https://docs.unity.com/en-us/mps-sdk/tutorials/relay-and-ngo
        */
        //https://docs.unity.com/en-us/mps-sdk/networking/qos
        busy = true;
        try
        {
            await initTask; 
            if (!ready)
            {
                busy = false;
                return;
            }

            Allocation allocation;
            try
            {
                allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers - 1);
            }
            catch(System.Exception excep)
            {
                status = "Creation Allocation request failed: " + excep.Message;
                Debug.Log(status);
                busy = false;
                return;
            }

            try
            {
                hostCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            }
            catch (System.Exception excep)
            {
                status = "Create Join code request failed: " + excep.Message;
                Debug.Log(status);
                return;
            }

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(AllocationUtils.ToRelayServerData(allocation, "dtls"));

            NetworkManager.Singleton.StartHost();
            status = "Hosting";
        }
        catch (System.Exception excep)
        {
            status = "Host failed: " + excep.Message;
            Debug.Log(status);
        }
        busy = false;
    }

    async Task JoinAsync()
    {
        busy = true;
        try
        {
            await initTask;
            if(!ready)
            {
                busy = false;
                return;
            } else if (string.IsNullOrWhiteSpace(typedCode))
            {
                status = "Enter a join code";
                busy = false;
                return;
            }

            JoinAllocation join = await RelayService.Instance.JoinAllocationAsync(typedCode.Trim().ToUpper());

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(AllocationUtils.ToRelayServerData(join, "dtls"));

            NetworkManager.Singleton.StartClient();
            status = "Joining...";
        }
        catch (System.Exception e) { status = "Join failed: " + e.Message; }
        busy = false;
    }
    
    public void Leave()
    {
        var networkMan = NetworkManager.Singleton;
        if(networkMan != null && networkMan.IsListening)
        {
            networkMan.Shutdown();
        }
        hostCode = "";
        if (ready)
        {
            status = "Ready";
        }
    }
}