using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(UIDocument))]
public class VictoryUI : MonoBehaviour
{
    UIDocument uiDoc;
    VisualElement root;
    Button mainMenuButton;
    Button exitGameButton;

    void Start()
    {
        uiDoc = GetComponent<UIDocument>();
        root = uiDoc.rootVisualElement;
        mainMenuButton = root.Q<Button>("mainMenuButton");
        exitGameButton = root.Q<Button>("exitGameButton");

        mainMenuButton.clicked += HandleMenuButtonClick;
        exitGameButton.clicked += HandleExitButtonClick;
    }

    void HandleMenuButtonClick()
    {
        SceneManager.LoadScene("Menu");
    }

    void HandleExitButtonClick()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
