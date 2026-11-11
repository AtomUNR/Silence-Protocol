using UnityEngine;
using UnityEngine.SceneManagement;
public class BackButton : MonoBehaviour
{
    public void OnBackPressed()
    {
        string errMessage = "";
        if(NetworkBootstrap.Instance != null)
        {
            NetworkBootstrap.Instance.Leave();
            try
            {
            SceneManager.LoadScene("MainMenu");              
            }
            catch(System.Exception excep)
            {
                errMessage = "You probably changed the name of the MainMenu scene to something else..." + excep;
                Debug.Log(errMessage);
            }
        }
    }
}
