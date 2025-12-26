using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class Boss : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 10f;
    public float minShootInterval = 1.0f;
    public float maxShootInterval = 2.5f;
    private bool canShoot = true;
    private float startAngleOffset = 0f;

    [Header("Entity")]
    public Entity entity;

    [Header("Intro / Cutscene")]
    public float introDelay = 3f;
    private bool introFinished = false;

    [Header("UI")]
    public Slider healthSlider;

    private Animator anim;
    private Rigidbody2D rb;
    private Transform player;

    private bool phase2 = false;
    private bool rageMode = false;

    [Header("Movement")]
    public float movementInterval = 6f;  
    public float moveDuration = 2f;     
    public float moveSpeed = 3f;
    private bool canMove = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        if (healthSlider != null)
        {
            healthSlider.maxValue = entity.maxHealth;
            healthSlider.value = entity.currentHealth;
        }

        rb.isKinematic = false;

        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        entity.currentHealth = entity.maxHealth;

        anim.speed = 0f;
        canShoot = false;
        canMove = false;

        StartCoroutine(StartIntro());
    }

    void Update()
    {
        if (entity.dead) return;

        if (healthSlider != null)
            healthSlider.value = entity.currentHealth;

        if (entity.currentHealth <= 0)
        {
            Die();
            return;
        }


        if (!phase2 && entity.currentHealth <= entity.maxHealth * 0.5f)
            ActivatePhase2();


        if (!rageMode && entity.currentHealth <= entity.maxHealth * 0.1f)
            ActivateRageMode();

  
        if (introFinished && canShoot)
            StartCoroutine(ShootCircle());

   
        if (introFinished && canMove)
            StartCoroutine(RandomMovement());
    }


    IEnumerator RandomMovement()
    {
        canMove = false;

        yield return new WaitForSeconds(movementInterval);

        if (entity.dead) yield break;


        Vector2 direction = Random.insideUnitCircle.normalized;

        float time = 0f;

        while (time < moveDuration && !entity.dead)
        {
            time += Time.deltaTime;

   
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 0.6f, LayerMask.GetMask("Wall"));

            if (hit.collider != null)
            {
            
                direction = Random.insideUnitCircle.normalized;
            }

            rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);

            yield return new WaitForFixedUpdate();
        }

        canMove = true;
    }



    IEnumerator ShootCircle()
    {
        canShoot = false;

        startAngleOffset = Random.Range(0f, 360f / 15f) * 15f;

        for (int angle = 0; angle < 360; angle += 15)
        {
            ShootAtAngle(angle + startAngleOffset);
        }

        float randomInterval = Random.Range(minShootInterval, maxShootInterval);

        yield return new WaitForSeconds(randomInterval);
        canShoot = true;
    }

    void ShootAtAngle(float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

        GameObject b = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        Rigidbody2D rb2 = b.GetComponent<Rigidbody2D>();
        rb2.velocity = direction.normalized * bulletSpeed;

        BossBullet bossBullet = b.AddComponent<BossBullet>();
        bossBullet.damage = entity.damage;
    }



    void ActivatePhase2()
    {
        phase2 = true;

        bulletSpeed += 2f;
        minShootInterval = 1f;
        maxShootInterval = 1.6f;

        Debug.Log("Boss entrou na Fase 2!");
    }

    void ActivateRageMode()
    {
        rageMode = true;

        bulletSpeed *= 1.5f;
        minShootInterval = 0.8f;
        maxShootInterval = 1f;

        moveSpeed += 1f; 

        Debug.Log("Boss entrou no MODO FÚRIA!");
    }



    IEnumerator StartIntro()
    {
        yield return new WaitForSeconds(introDelay);

        introFinished = true;

        anim.speed = 1f;
        canShoot = true;
        canMove = true;
    }


    public void TakeDamage(int damageAmount)
    {
        if (entity.dead) return;

        entity.currentHealth -= damageAmount;

        if (healthSlider != null)
            healthSlider.value = entity.currentHealth;

        Debug.Log($"Boss took {damageAmount} damage. Health remaining: {entity.currentHealth}");
    }

    public void Die()
    {
        if (entity.dead) return;

        entity.dead = true;
        anim.SetTrigger("die");

        Player p = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Player>();
        if (p != null)
        {
            p.GainExp(entity.expReward);
        }

        Destroy(gameObject, 2f);
        StartCoroutine(HandleDeathAndWin());
    }

    IEnumerator HandleDeathAndWin()
    {
        Player p = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Player>();
        if (p != null)
            p.GainExp(entity.expReward);

        yield return new WaitForSeconds(1f);

        Destroy(gameObject);

        SceneManager.LoadScene("Win");
    }
}
