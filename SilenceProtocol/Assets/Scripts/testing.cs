using System.Threading.Tasks;

public interface ISignInService
{
    bool isSignedIn { get; }
    Task InitializeAsync();
    Task SignInAsync();
}

public class SignInFlow
{
    
    public bool Ready { get; private set; }
    public string Status { get; private set; }
    
}