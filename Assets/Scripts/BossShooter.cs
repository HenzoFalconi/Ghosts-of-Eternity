using UnityEngine;
using System.Collections;

public class BossShooter : MonoBehaviour
{
    [Header("Bullet Settings")]
    public GameObject bulletPrefab;      // Prefab da bala do Player (use o mesmo)
    public float bulletSpeed = 10f;      // Velocidade das balas do Boss
    public float shootInterval = 1.5f;   // Tempo entre rajadas

    [Header("Boss Reference")]
    public Monster monster; // Referência ao script Monster (para pegar dano, vida, etc.)

    private bool canShoot = true;

    void Start()
    {
        if (monster == null)
            monster = GetComponent<Monster>();

        // Impede o boss de andar
        monster.permanentChase = false;
        monster.waypointList.Clear();
        monster.chaseRange = 0f;
        monster.stopDistance = 0f;
    }

    void Update()
    {
        if (monster == null || monster.entity.dead) return;

        if (canShoot)
            StartCoroutine(ShootCircle());
    }

    IEnumerator ShootCircle()
    {
        canShoot = false;

        // Atira em todas as direções (24 tiros com 15° entre eles)
        for (int angle = 0; angle < 360; angle += 15)
        {
            ShootAtAngle(angle);
        }

        yield return new WaitForSeconds(shootInterval);
        canShoot = true;
    }

    void ShootAtAngle(float angle)
    {
        // Calcula a direção
        float rad = angle * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

        // Instancia a bala
        GameObject b = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        // Ajusta direção do bullet
        Rigidbody2D rb = b.GetComponent<Rigidbody2D>();
        rb.velocity = direction.normalized * bulletSpeed;

        // Faz o bullet causar dano no Player
        BossBullet bossBullet = b.AddComponent<BossBullet>();
        bossBullet.damage = monster.damage;
    }
}
