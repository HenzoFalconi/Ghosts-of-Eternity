using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelButton : MonoBehaviour
{
    public int levelIndex; 
    public Button button;

    void Start()
    {
        if (button == null)
            button = GetComponent<Button>();

        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        if (levelIndex <= unlockedLevel)
            button.interactable = true;
        else
            button.interactable = false;

        button.onClick.AddListener(OnClickLevel);
    }

    void OnClickLevel()
    {
        string sceneName = "Game" + levelIndex;
        SceneManager.LoadScene(sceneName);
    }
}
