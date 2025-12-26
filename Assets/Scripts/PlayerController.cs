using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Player))]
public class PlayerController : MonoBehaviour
{
    [HideInInspector] public Player player;
    public Animator playerAnimator;

    [Header("Movimentação")]
    float input_x = 0;
    float input_y = 0;
    bool isWalking = false;
    Rigidbody2D rb2D;
    Vector2 movement = Vector2.zero;

    [Header("Interact / Teleporte")]
    public KeyCode interactKey = KeyCode.E;
    bool canTeleport = false;
    Region tmpRegion;

    [Header("Arma de fogo")]
    public Transform firePoint;     // ponto de saída da bala
    public GameObject bulletPrefab; // prefab da bala

    void Start()
    {
        player = GetComponent<Player>();
        rb2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // --- MOVIMENTO ---
        input_x = Input.GetAxisRaw("Horizontal");
        input_y = Input.GetAxisRaw("Vertical");
        isWalking = (Mathf.Abs(input_x) + Mathf.Abs(input_y)) > 0;

        if (playerAnimator != null)
        {
            playerAnimator.SetBool("isWalking", isWalking);
            playerAnimator.SetFloat("input_x", input_x);
            playerAnimator.SetFloat("input_y", input_y);
        }

        // --- ATAQUE MELEE ---
        if (player.entity.attackTimer < 0)
            player.entity.attackTimer = 0;
        else
            player.entity.attackTimer -= Time.deltaTime;

        if (player.entity.attackTimer == 0 && !isWalking)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                if (playerAnimator != null)
                    playerAnimator.SetTrigger("attack");

                player.entity.attackTimer = player.entity.cooldown;
                Attack();
            }
        }

        // --- TIRO ---
        if (Input.GetMouseButtonDown(1))
        {
            Shoot();
        }

        // --- RECARREGAR ---
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }

        // --- TELEPORT ---
        if (canTeleport && tmpRegion != null && Input.GetKeyDown(interactKey))
        {
            transform.position = tmpRegion.warpLocation.position;
        }
    }

    private void FixedUpdate()
    {
        movement = new Vector2(input_x, input_y).normalized;
        rb2D.MovePosition(rb2D.position + movement * player.entity.speed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Enemy"))
        {
            player.entity.target = collider.gameObject;
        }

        if (collider.CompareTag("Teleport"))
        {
            tmpRegion = collider.GetComponent<Teleport>().region;
            canTeleport = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Enemy"))
        {
            player.entity.target = null;
        }

        if (collider.CompareTag("Teleport"))
        {
            tmpRegion = null;
            canTeleport = false;
        }
    }

    // ======================================================
    // ================ ATAQUE CORPO A CORPO =================
    // ======================================================
    void Attack()
    {
        if (player.entity.target == null) return;

        Monster monster = player.entity.target.GetComponent<Monster>();
        if (monster == null || monster.entity.dead)
        {
            player.entity.target = null;
            return;
        }

        float distance = Vector2.Distance(transform.position, player.entity.target.transform.position);

        if (distance <= player.entity.attackDistance)
        {
            int dmg = player.manager.CalculateDamage(player.entity, player.entity.damage);
            int enemyDef = player.manager.CalculateDefense(monster.entity, monster.entity.defense);
            int result = Mathf.Max(dmg - enemyDef, 0);

            monster.entity.currentHealth -= result;
            monster.entity.target = this.gameObject;

            Debug.Log($"Ataque melee causou {result} de dano.");

            if (monster.entity.currentHealth <= 0 && !monster.entity.dead)
                monster.Die();
        }
    }

    // ======================================================
    // ======================= TIRO =========================
    // ======================================================
    void Shoot()
    {
        if (player.entity.currentAmmo <= 0)
        {
            Debug.Log("Sem munição!");
            return;
        }

        player.entity.currentAmmo--; // usa da ENTITY, não do Player
        Debug.Log($"Atirou! Restam {player.entity.currentAmmo} balas.");

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (bulletPrefab != null && firePoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.Euler(0, 0, angle));
            Bullet b = bullet.GetComponent<Bullet>();
            if (b != null)
                b.damage = player.entity.damage + 5;
        }

        // Raycast para dano instantâneo
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, direction, player.entity.bulletRange);
        if (hit.collider != null && hit.collider.CompareTag("Enemy"))
        {
            Monster monster = hit.collider.GetComponent<Monster>();
            if (monster != null && !monster.entity.dead)
            {
                int dmg = player.manager.CalculateDamage(player.entity, player.entity.damage + 5);
                int enemyDef = player.manager.CalculateDefense(monster.entity, monster.entity.defense);
                int result = Mathf.Max(dmg - enemyDef, 0);

                monster.entity.currentHealth -= result;
                monster.entity.target = this.gameObject;

                Debug.Log($"Tiro causou {result} de dano.");

                if (monster.entity.currentHealth <= 0 && !monster.entity.dead)
                    monster.Die();
            }
        }
    }


    // ======================================================
    // ====================== RELOAD ========================
    // ======================================================
    void Reload()
    {
        if (player.entity.currentAmmo < player.entity.maxAmmo && player.entity.reserveAmmo > 0)
        {
            int needed = player.entity.maxAmmo - player.entity.currentAmmo;
            int toLoad = Mathf.Min(needed, player.entity.reserveAmmo);

            player.entity.currentAmmo += toLoad;
            player.entity.reserveAmmo -= toLoad;

            Debug.Log($"Recarregou: +{toLoad} balas ({player.entity.currentAmmo}/{player.entity.reserveAmmo})");
        }
        else
        {
             Debug.Log("Munição já cheia ou sem reserva!");
        }
    }

}
