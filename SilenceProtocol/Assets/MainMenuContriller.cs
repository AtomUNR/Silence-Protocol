
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuContriller : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

}

    public void StartGame(){ 
        //this just goes to next menu
        SceneManager.LoadScene("PlayMenu");
    }

    public void ExitGame(){
        Application.Quit();
    }
}
