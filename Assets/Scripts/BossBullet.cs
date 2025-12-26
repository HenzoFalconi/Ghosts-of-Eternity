using UnityEngine;

public class BossBullet : MonoBehaviour
{
    public int damage = 10;
    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Rotaciona a bala para apontar na direção do movimento
        if (rb.velocity != Vector2.zero)
        {
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90); // ajuste de 90°
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            Player p = col.GetComponent<Player>();
            if (p != null)
            {
                p.entity.TakeDamage(damage);
                Destroy(gameObject);
            }
        }

        if (col.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
