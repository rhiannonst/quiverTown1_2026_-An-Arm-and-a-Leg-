using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    UIDocument uiDoc;
    VisualElement root;
    VisualElement escapeMenuContainer;
    Button mainMenuButton;
    Button exitGameButton;

    void Start()
    {
        uiDoc = GetComponent<UIDocument>();
        root = uiDoc.rootVisualElement;
        escapeMenuContainer = root.Q<VisualElement>("escapeMenuContainer");
        mainMenuButton = root.Q<Button>("mainMenuButton");
        exitGameButton = root.Q<Button>("exitGameButton");

        mainMenuButton.clicked += HandleMenuButtonClick;
        exitGameButton.clicked += HandleExitButtonClick;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (escapeMenuContainer.style.display == DisplayStyle.Flex)
                escapeMenuContainer.style.display = DisplayStyle.None;
            else
                escapeMenuContainer.style.display = DisplayStyle.Flex;
        }
    }

    public void HandleMenuButtonClick()
    {
        HandleLoadScene("Menu");
    }

    public void HandleLoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void HandleExitButtonClick()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
