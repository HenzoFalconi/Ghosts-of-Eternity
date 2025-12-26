using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Entity
{
    // ---------------------------------------------------------
    // IDENTIDADE
    // ---------------------------------------------------------
    [Header("Identity")]
    public string name;

    // VALOR ATUAL
    public int level = 1;

    // VALOR BASE
    public int baseLevel = 1;

    // ---------------------------------------------------------
    // ATRIBUTOS BASE + ATUAIS
    // ---------------------------------------------------------
    [Header("Stats")]
    public int strength = 1;
    public int resistence = 1;
    public int intelligence = 1;
    public int willpower = 1;
    public float speed = 2f;
    public int points = 0;

    // VALORES BASE (não mudam nunca)
    public int baseStrength = 1;
    public int baseResistence = 1;
    public int baseIntelligence = 1;
    public int baseWillpower = 1;
    public float baseSpeed = 2f;

    // ---------------------------------------------------------
    // HEALTH / MANA / STAMINA
    // ---------------------------------------------------------
    [Header("Health")]
    public int currentHealth;
    public int maxHealth;

    [Header("Mana")]
    public int currentMana;
    public int maxMana;

    [Header("Stamina")]
    public int currentStamina;
    public int maxStamina;

    // ---------------------------------------------------------
    // COMBATE
    // ---------------------------------------------------------
    [Header("Combat")]
    public int damage = 1;
    public int defense = 1;
    public float attackDistance = 0.5f;
    public float attackTimer = 1;
    public float cooldown = 2;
    public bool inCombat = false;
    public GameObject target;
    public bool combatCoroutine = false;
    public bool dead = false;

    // ---------------------------------------------------------
    // ARMA DE LONGA DISTÂNCIA
    // ---------------------------------------------------------
    [Header("Ranged Weapon")]
    public int maxAmmo = 10;
    public int currentAmmo = 10;
    public int reserveAmmo = 0;
    public float bulletRange = 10f;

    // ---------------------------------------------------------
    // UI / ÁUDIO
    // ---------------------------------------------------------
    [Header("UI / Audio")]
    public AudioSource entityAudio;
    public Slider healthBar;

    // ---------------------------------------------------------
    // RECOMPENSAS
    // ---------------------------------------------------------
    [Header("Rewards")]
    public int expReward = 0;
    public int goldReward = 0;

    // ---------------------------------------------------------
    // MÉTODOS
    // ---------------------------------------------------------

    public void TakeDamage(int damageAmount)
    {
        if (dead) return;

        currentHealth -= damageAmount;
        if (currentHealth < 0)
            currentHealth = 0;

        if (healthBar != null)
            healthBar.value = currentHealth;
    }

    public void Heal(int healAmount)
    {
        if (dead) return;

        currentHealth += healAmount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        if (healthBar != null)
            healthBar.value = currentHealth;
    }

    // ---------------------------------------------------------
    // RESET PARA VALORES BASE (usado ao entrar na fase 1)
    // ---------------------------------------------------------
    public void ResetToBase()
    {
        level = baseLevel;

        strength = baseStrength;
        resistence = baseResistence;
        intelligence = baseIntelligence;
        willpower = baseWillpower;
        speed = baseSpeed;

        // Atualiza pontos
        points = 0;

        // Reset vida/mana/stamina
        currentHealth = maxHealth;
        currentMana = maxMana;
        currentStamina = maxStamina;

        Debug.Log("Entity resetada para valores base.");
    }
}
