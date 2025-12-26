using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GameManagerMusic : MonoBehaviour
{
    // Variável estática para manter a referência à única instância
    private static GameManagerMusic instance;

    // --- Configurações do Inspector (Mantidas) ---
    [Header("Configuração da Música")]
    public AudioClip musicClip;
    [Range(0f, 1f)] 
    public float volume = 1f;

    private AudioSource audioSource;

    void Awake()
    {

        if (instance == null)
        {

            instance = this;
            

            DontDestroyOnLoad(this.gameObject);
        }
        else
        {

            Destroy(this.gameObject);
            return; 
        }

      
        audioSource = GetComponent<AudioSource>();

        audioSource.clip = musicClip;
        audioSource.loop = true;
        audioSource.volume = volume;

       
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    void Update()
    {
        
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
    }
}