using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelProgression : MonoBehaviour
{
    [Header("Configuração")]
    public int currentLevel = 1;
    public string nextScene = "fase2";
    public float delayBeforeTransition = 3f;

    private int monstersRemaining = 0;
    private bool transitioning = false;

    void Start()
    {
        // Conta os monstros existentes na cena
        Monster[] initial = FindObjectsOfType<Monster>();
        monstersRemaining = initial.Length;

        Debug.Log("Monstros iniciais: " + monstersRemaining);

        // Eventos
        Monster.OnMonsterSpawned += OnMonsterSpawned;
        Monster.OnMonsterDied += OnMonsterDied;

        // Se não tiver nenhum, passa a fase
        if (monstersRemaining <= 0)
            TryCompleteLevel();
    }

    void OnDestroy()
    {
        Monster.OnMonsterSpawned -= OnMonsterSpawned;
        Monster.OnMonsterDied -= OnMonsterDied;
    }

    void OnMonsterSpawned(Monster m)
    {
        monstersRemaining++;
        Debug.Log("Monstro spawnado. Total: " + monstersRemaining);
    }

    void OnMonsterDied(Monster m)
    {
        monstersRemaining = Mathf.Max(0, monstersRemaining - 1);
        Debug.Log("Monstro morreu. Restam: " + monstersRemaining);
        TryCompleteLevel();
    }

    void TryCompleteLevel()
    {
        if (transitioning) return;

        if (monstersRemaining <= 0)
        {
            StartCoroutine(HandleLevelComplete());
        }
    }

    IEnumerator HandleLevelComplete()
    {
        transitioning = true;

        Debug.Log("Nível completo! Transição em " + delayBeforeTransition);

        yield return new WaitForSecondsRealtime(delayBeforeTransition);

        // Sistema de desbloqueio
        int unlocked = PlayerPrefs.GetInt("UnlockedLevel", 1);
        if (currentLevel >= unlocked)
        {
            PlayerPrefs.SetInt("UnlockedLevel", currentLevel + 1);
            PlayerPrefs.Save();
            Debug.Log($"Fase {currentLevel + 1} desbloqueada!");
        }

        SceneManager.LoadScene(nextScene);
    }
}
