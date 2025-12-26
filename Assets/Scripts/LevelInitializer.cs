using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelInitializer : MonoBehaviour
{
    public Entity playerEntity;

    void Start()
    {
        string scene = SceneManager.GetActiveScene().name;

        // Reset somente na Game1
        if (scene == "Game1")
        {
            playerEntity.ResetToBase();
            Debug.Log("Reset de atributos (somente Game1).");
        }
    }
}
