using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class GameManager : MonoBehaviour
{
    [Header("Intro Start")]
    public bool paused = false;
    public GameObject introTextObject;
    public int blinkCount = 3;
    public float blinkOn = 0.5f;
    public float blinkOff = 0.3f;

    void Start()
    {
        StartCoroutine(IntroSequence());
    }

    IEnumerator IntroSequence()
    {
        paused = true;

        if (introTextObject != null)
        {
            introTextObject.SetActive(false);

            for (int i = 0; i < blinkCount; i++)
            {
                introTextObject.SetActive(true);
                yield return new WaitForSecondsRealtime(blinkOn);

                introTextObject.SetActive(false);
                yield return new WaitForSecondsRealtime(blinkOff);
            }
        }

        paused = false;
    }

    // ----- CALCULOS DE ATRIBUTOS -----

    public int CalculateHealth(Entity entity)
    {
        int result = (entity.resistence * 10) + (entity.level * 4) + 10;
        return result;
    }

    public int CalculateMana(Entity entity)
    {
        int result = (entity.intelligence * 10) + (entity.level * 4) + 5;
        return result;
    }

    public int CalculateStamina(Entity entity)
    {
        int result = (entity.resistence + entity.willpower) + (entity.level * 2) + 5;
        return result;
    }

    public int CalculateDamage(Entity entity, int weaponDamage)
    {
        System.Random rnd = new System.Random();
        int result = (entity.strength * 2) + (weaponDamage * 2) + (entity.level * 3) + rnd.Next(1, 20);
        return result;
    }

    public int CalculateDefense(Entity entity, int armorDefense)
    {
        int result = (entity.resistence * 2) + (entity.level * 3) + armorDefense;
        return result;
    }
}
