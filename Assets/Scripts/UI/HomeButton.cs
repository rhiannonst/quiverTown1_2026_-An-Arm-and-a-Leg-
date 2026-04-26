using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeButton : MonoBehaviour
{
    public void OnPressed()
    {
        SceneManager.LoadScene(0);
    }
}
