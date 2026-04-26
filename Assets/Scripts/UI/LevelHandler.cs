using UnityEngine;

public class LevelHandler : MonoBehaviour
{
    public GameObject gameOverOverlay;

    void Start()
    {
        if (gameOverOverlay != null)
            gameOverOverlay.SetActive(false);
    }

    public void GameOver()
    {
        Debug.Log("[LevelHandler] Game Over.");
        if (gameOverOverlay != null)
            gameOverOverlay.SetActive(true);
    }
}
