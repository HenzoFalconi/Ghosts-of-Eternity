using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public Entity entity;

    // ================================
    // REGEN SYSTEM
    // ================================
    [Header("Player Regen System")]
    public bool regenHPEnabled = true;
    public float regenHPTime = 5f;
    public int regenHPValue = 5;

    public bool regenMPEnabled = true;
    public float regenMPTime = 10f;
    public int regenMPValue = 5;

    // ================================
    // FIRE WEAPON
    // ================================
    [Header("Arma de fogo")]
    public int maxAmmo = 10;
    public int currentAmmo = 10;
    public int reserveAmmo = 20;
    public float bulletRange = 10f;

    // ================================
    // GAME MANAGER
    // ================================
    [Header("Game Manager")]
    public GameManager manager;

    // ================================
    #region UI CONFIG
    [Header("Player Shortcuts")]
    public KeyCode attributesKey = KeyCode.C;

    [Header("Player UI Panels")]
    public GameObject attributesPanel;

    [Header("Player UI")]
    public Slider health;
    public Slider mana;
    public Slider stamina;
    public Slider exp;
    public Text expText;
    public Text levelText;

    public Text strTxt;
    public Text resTxt;
    public Text intTxt;
    public Text wilTxt;

    public Button strPositiveBtn;
    public Button resPositiveBtn;
    public Button intPositiveBtn;
    public Button wilPositiveBtn;

    public Button strNegativeBtn;
    public Button resNegativeBtn;
    public Button intNegativeBtn;
    public Button wilNegativeBtn;

    public Text pointsTxt;
    #endregion

    // XP / LEVEL
    [Header("Exp")]
    public int currentExp = 0;
    public int expBase = 50;
    public int expLeft = 50;
    public float expMod = 1.2f;
    public GameObject levelUpFX;
    public AudioClip levelUpSound;
    public int givePoints = 5;

    // Respawn
    [Header("Respawn")]
    public float respawnTime = 5;
    public GameObject prefab;

    // ================================
    // MAIN START
    // ================================
    void Start()
    {
        string scene = SceneManager.GetActiveScene().name;

        // Carregar dados AO ENTRAR EM QUALQUER FASE
        LoadPlayerData();

        if (manager == null)
        {
            Debug.LogError("O Player precisa do GameManager ligado!");
            return;
        }

        // Recalcular stats
        entity.maxHealth = manager.CalculateHealth(entity);
        entity.maxMana = manager.CalculateMana(entity);
        entity.maxStamina = manager.CalculateStamina(entity);

        entity.currentHealth = entity.maxHealth;
        entity.currentMana = entity.maxMana;
        entity.currentStamina = entity.maxStamina;

        // Inicializar UI
        health.maxValue = entity.maxHealth;
        health.value = entity.currentHealth;

        mana.maxValue = entity.maxMana;
        mana.value = entity.currentMana;

        stamina.maxValue = entity.maxStamina;
        stamina.value = entity.currentStamina;

        exp.maxValue = expLeft;
        exp.value = currentExp;

        expText.text = $"Exp: {currentExp}/{expLeft}";
        levelText.text = entity.level.ToString();

        SetupUIButtons();
        UpdatePoints();

        StartCoroutine(RegenHealth());
        StartCoroutine(RegenMana());
    }

    // ================================
    void Update()
    {
        if (entity.dead) return;

        // Pausar animação quando o jogo está pausado
        Animator anim = GetComponent<Animator>();
        if (manager != null && manager.paused)
        {
            if (anim != null) anim.speed = 0f;
            GetComponent<PlayerController>().enabled = false;
            return;
        }
        else
        {
            if (anim != null) anim.speed = 1f;
            GetComponent<PlayerController>().enabled = true;
        }

        if (entity.currentHealth <= 0)
            Die();

        if (Input.GetKeyDown(attributesKey))
            attributesPanel.SetActive(!attributesPanel.activeSelf);

        // Atualizar UI em tempo real
        health.value = entity.currentHealth;
        mana.value = entity.currentMana;
        stamina.value = entity.currentStamina;
        exp.value = currentExp;

        expText.text = $"Exp: {currentExp}/{expLeft}";
        levelText.text = entity.level.ToString();
    }

    // ================================
    // REGENERATION
    IEnumerator RegenHealth()
    {
        while (true)
        {
            if (regenHPEnabled && entity.currentHealth < entity.maxHealth)
                entity.currentHealth = Mathf.Min(entity.currentHealth + regenHPValue, entity.maxHealth);

            yield return new WaitForSeconds(regenHPTime);
        }
    }

    IEnumerator RegenMana()
    {
        while (true)
        {
            if (regenMPEnabled && entity.currentMana < entity.maxMana)
                entity.currentMana = Mathf.Min(entity.currentMana + regenMPValue, entity.maxMana);

            yield return new WaitForSeconds(regenMPTime);
        }
    }

    // ================================
    // DEATH
    void Die()
    {
        entity.currentHealth = 0;
        entity.dead = true;
        StopAllCoroutines();
        GetComponent<PlayerController>().enabled = false;
        StartCoroutine(LoadGameOver());
    }

    IEnumerator LoadGameOver()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        SceneManager.LoadScene("Scenes/GameOver");
    }

    // ================================
    // EXPERIENCE
    public void GainExp(int amount)
    {
        currentExp += amount;
        SavePlayerData();

        if (currentExp >= expLeft)
            LevelUp();
    }

    public void LevelUp()
    {
        currentExp -= expLeft;
        entity.level++;
        entity.points += givePoints;
        UpdatePoints();

        entity.currentHealth = entity.maxHealth;

        expLeft = Mathf.FloorToInt(expBase * Mathf.Pow(expMod, entity.level));

        SavePlayerData();

        if (entity.entityAudio != null && levelUpSound != null)
            entity.entityAudio.PlayOneShot(levelUpSound);

        if (levelUpFX != null)
            Instantiate(levelUpFX, transform);
    }

    // ================================
    // STATUS POINTS
    public void SetupUIButtons()
    {
        strPositiveBtn.onClick.AddListener(() => AddPoints(1));
        resPositiveBtn.onClick.AddListener(() => AddPoints(2));
        intPositiveBtn.onClick.AddListener(() => AddPoints(3));
        wilPositiveBtn.onClick.AddListener(() => AddPoints(4));

        strNegativeBtn.onClick.AddListener(() => RemovePoints(1));
        resNegativeBtn.onClick.AddListener(() => RemovePoints(2));
        intNegativeBtn.onClick.AddListener(() => RemovePoints(3));
        wilNegativeBtn.onClick.AddListener(() => RemovePoints(4));
    }

    public void UpdatePoints()
    {
        strTxt.text = entity.strength.ToString();
        resTxt.text = entity.resistence.ToString();
        intTxt.text = entity.intelligence.ToString();
        wilTxt.text = entity.willpower.ToString();
        pointsTxt.text = entity.points.ToString();
    }

    public void AddPoints(int index)
    {
        if (entity.points <= 0) return;

        switch (index)
        {
            case 1: entity.strength++; PlayerPrefs.SetInt("STR", entity.strength); break;
            case 2: entity.resistence++; PlayerPrefs.SetInt("RES", entity.resistence); break;
            case 3: entity.intelligence++; PlayerPrefs.SetInt("INT", entity.intelligence); break;
            case 4: entity.willpower++; PlayerPrefs.SetInt("WIL", entity.willpower); break;
        }

        entity.points--;
        PlayerPrefs.SetInt("PlayerPoints", entity.points);
        PlayerPrefs.Save();

        UpdatePoints();
    }

    public void RemovePoints(int index)
    {
        switch (index)
        {
            case 1: if (entity.strength > 0) entity.strength--; PlayerPrefs.SetInt("STR", entity.strength); break;
            case 2: if (entity.resistence > 0) entity.resistence--; PlayerPrefs.SetInt("RES", entity.resistence); break;
            case 3: if (entity.intelligence > 0) entity.intelligence--; PlayerPrefs.SetInt("INT", entity.intelligence); break;
            case 4: if (entity.willpower > 0) entity.willpower--; PlayerPrefs.SetInt("WIL", entity.willpower); break;
        }

        entity.points++;
        PlayerPrefs.SetInt("PlayerPoints", entity.points);
        PlayerPrefs.Save();

        UpdatePoints();
    }

    // ================================
    // SAVE / LOAD
    void SavePlayerData()
    {
        PlayerPrefs.SetInt("PlayerLevel", entity.level);
        PlayerPrefs.SetInt("PlayerExp", currentExp);
        PlayerPrefs.SetInt("PlayerPoints", entity.points);

        PlayerPrefs.SetInt("STR", entity.strength);
        PlayerPrefs.SetInt("RES", entity.resistence);
        PlayerPrefs.SetInt("INT", entity.intelligence);
        PlayerPrefs.SetInt("WIL", entity.willpower);

        PlayerPrefs.Save();
    }

    void LoadPlayerData()
    {
        entity.level = PlayerPrefs.GetInt("PlayerLevel", 1);
        currentExp = PlayerPrefs.GetInt("PlayerExp", 0);
        entity.points = PlayerPrefs.GetInt("PlayerPoints", 0);

        entity.strength = PlayerPrefs.GetInt("STR", entity.strength);
        entity.resistence = PlayerPrefs.GetInt("RES", entity.resistence);
        entity.intelligence = PlayerPrefs.GetInt("INT", entity.intelligence);
        entity.willpower = PlayerPrefs.GetInt("WIL", entity.willpower);
    }
}
