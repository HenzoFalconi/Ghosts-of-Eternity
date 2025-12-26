using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class Monster : MonoBehaviour
{
    [Header("Controller")]
    public Entity entity;
    // Eventos para spawn e morte
    public static System.Action<Monster> OnMonsterSpawned;
    public static System.Action<Monster> OnMonsterDied;

    public GameManager manager;
    public static System.Action<int> OnAliveMonstersChanged;
    public static int aliveMonsters = 0;

    [Header("Patrol")]
    public List<Transform> waypointList = new List<Transform>();
    public float arrivalDistance = 0.5f;
    public float waitTime = 3f;
    public int waypointID;

    private Transform targetWaypoint;
    private int currentWaypoint = 0;
    private float currentWaitTime = 0f;
    private float lastDistanceToTarget = 0f;

    [Header("Chase Settings")]
    public Transform player;
    public float chaseRange = 10f;
    public float stopDistance = 1.5f;
    public bool permanentChase = true;

    [Header("Attack Settings")]
    public int damage = 10;
    public float attackCooldown = 1.5f;
    private bool canAttack = true;

    [Header("Experience & Loot")]
    public int rewardExperience = 15;
    public int lootGoldMin = 0;
    public int lootGoldMax = 10;

    [Header("Respawn")]
    public GameObject prefab;
    public bool respawn = true;
    public float respawnTime = 10f;

    [Header("UI")]
    public Slider healthSlider;

    private Rigidbody2D rb2D;
    private Animator anim;
    private Vector2 movement;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        rb2D.isKinematic = false;
        anim = GetComponent<Animator>();

        manager = GameObject.FindObjectOfType<GameManager>();

        
        if (player == null)
            player = GameObject.FindWithTag("Player")?.transform;

      
        if (manager != null)
        {
            entity.maxHealth = manager.CalculateHealth(entity);
            entity.maxMana = manager.CalculateMana(entity);
            entity.maxStamina = manager.CalculateStamina(entity);
        }

        entity.currentHealth = entity.maxHealth;
        entity.currentMana = entity.maxMana;
        entity.currentStamina = entity.maxStamina;

        if (healthSlider != null)
        {
            healthSlider.maxValue = entity.maxHealth;
            healthSlider.value = entity.currentHealth;
        }

        
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Waypoint"))
        {
            WaypointID idComponent = obj.GetComponent<WaypointID>();
            if (idComponent != null && idComponent.ID == waypointID)
                waypointList.Add(obj.transform);
        }

        if (waypointList.Count > 0)
        {
            targetWaypoint = waypointList[currentWaypoint];
            lastDistanceToTarget = Vector2.Distance(transform.position, targetWaypoint.position);
        }

        currentWaitTime = waitTime;
    }

    void Update()
    {
        if (entity.dead) return;

        
        if (manager != null && manager.paused)
        {
            
            if (anim != null) anim.speed = 0f;
            return;
        }
        else
        {
            if (anim != null) anim.speed = 1f;
        }

        
        if (healthSlider != null)
            healthSlider.value = entity.currentHealth;


        if (entity.currentHealth <= 0)
        {
            entity.currentHealth = 0;
            Die();
            return;
        }

        float distanceToPlayer = player != null ? Vector2.Distance(transform.position, player.position) : Mathf.Infinity;


        if (distanceToPlayer <= chaseRange)
        {
            entity.inCombat = true;
            MoveTowardsTarget(player);
            // ➕ Adicione esta parte:
            if (distanceToPlayer <= stopDistance && canAttack)
            {
            StartCoroutine(Attack(player));
            }
        }
        else
        {
            entity.inCombat = false;
            if (waypointList.Count > 0)
                Patrol();
            else
                anim.SetBool("isWalking", false);
        }
    }

    private void FixedUpdate()
    {
        
        if (manager != null && manager.paused)
            return;

        
        rb2D.MovePosition(rb2D.position + movement * (entity.speed * Time.fixedDeltaTime));
    }

    void Patrol()
    {
        if (targetWaypoint == null) return;

        float distanceToTarget = Vector2.Distance(transform.position, targetWaypoint.position);

        if (distanceToTarget <= arrivalDistance || distanceToTarget >= lastDistanceToTarget)
        {
            anim.SetBool("isWalking", false);

            if (currentWaitTime <= 0)
            {
                currentWaypoint++;
                if (currentWaypoint >= waypointList.Count)
                    currentWaypoint = 0;

                targetWaypoint = waypointList[currentWaypoint];
                lastDistanceToTarget = Vector2.Distance(transform.position, targetWaypoint.position);
                currentWaitTime = waitTime;
            }
            else
            {
                currentWaitTime -= Time.deltaTime;
            }
            movement = Vector2.zero;
        }
        else
        {
            anim.SetBool("isWalking", true);
            lastDistanceToTarget = distanceToTarget;

            Vector2 direction = (targetWaypoint.position - transform.position).normalized;
            anim.SetFloat("input_x", direction.x);
            anim.SetFloat("input_y", direction.y);

            movement = direction;
        }
    }

    void MoveTowardsTarget(Transform target)
    {
        if (target == null) return;

        Vector2 direction = (target.position - transform.position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 1f, LayerMask.GetMask("Obstacles"));
        if (hit.collider != null)
        {
            // Tenta desviar um pouco
            Vector2 perpendicular = Vector2.Perpendicular(direction);
            Vector2 avoidDir = (direction + perpendicular * 0.5f).normalized;

            direction = avoidDir;
        }

        movement = direction;
        anim.SetBool("isWalking", true);
        anim.SetFloat("input_x", direction.x);
        anim.SetFloat("input_y", direction.y);

        rb2D.MovePosition(rb2D.position + movement * (entity.speed * Time.fixedDeltaTime));
    }

    IEnumerator Attack(Transform target)
    {
        canAttack = false;
        anim.SetTrigger("attack");

        yield return new WaitForSeconds(0.3f); 

        if (target != null)
        {
            Player playerComponent = target.GetComponent<Player>();
            if (playerComponent != null && !playerComponent.entity.dead)
            {
                int dmg = manager.CalculateDamage(entity, damage);
                int def = manager.CalculateDefense(playerComponent.entity, playerComponent.entity.defense);
                int dmgResult = Mathf.Max(dmg - def, 0);

                Debug.Log($"{entity.name} atacou {playerComponent.name} causando {dmgResult} de dano.");
                playerComponent.entity.TakeDamage(dmgResult);
            }
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    void Flip(float directionX)
    {
        Vector3 scale = transform.localScale;
        if (directionX > 0) scale.x = Mathf.Abs(scale.x);
        else if (directionX < 0) scale.x = Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    public void Die()
    {
        if (entity.dead) return;
        entity.dead = true;

        anim.SetTrigger("die");
        anim.SetBool("isWalking", false);
        movement = Vector2.zero;

        Player player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Player>();
        if (player != null)
                player.GainExp(entity.expReward);

        Debug.Log($"Inimigo {entity.name} morreu e deu {entity.expReward} de XP.");

        OnMonsterDied?.Invoke(this);

        StopAllCoroutines();
        if (respawn && !FindObjectOfType<LevelProgression>())
            StartCoroutine(Respawn());
        else
            Destroy(gameObject, 2f);
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnTime);

        GameObject newMonster = Instantiate(prefab, transform.position, transform.rotation);
        newMonster.name = prefab.name;
        Destroy(gameObject);
    }

    void OnEnable()
    {
        aliveMonsters++;
        OnAliveMonstersChanged?.Invoke(aliveMonsters);
        OnMonsterSpawned?.Invoke(this);
    }

    void OnDisable()
    {
        
        if (entity != null && !entity.dead)
        {
            aliveMonsters = Mathf.Max(0, aliveMonsters - 1);
            OnAliveMonstersChanged?.Invoke(aliveMonsters);
        }
    }

}
