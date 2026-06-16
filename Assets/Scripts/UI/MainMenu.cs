using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject credits;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void PlayGame()
    {
        SceneManager.LoadScene("GridTest");
    }
    public void GoTutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }
    public void GoMenu()
    {
        SceneManager.LoadScene("Menu");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void CreditsOpen()
    {
        credits.SetActive(true);
    }
    public void CreditsClose()
    {
      credits.SetActive(false);
    }
}
