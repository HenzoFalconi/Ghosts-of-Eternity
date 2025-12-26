using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    void Awake()
    {
        if (!PlayerPrefs.HasKey("HasInitialized"))
        {
            PlayerPrefs.SetInt("UnlockedLevel", 1);
            PlayerPrefs.SetInt("HasInitialized", 1);
            PlayerPrefs.Save();

            Debug.Log("Primeira inicialização — apenas fase 1 liberada.");
        }
    }
}
