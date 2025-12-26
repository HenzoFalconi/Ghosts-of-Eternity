using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 10;
    public float speed = 15f;
    public float lifeTime = 2f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        Boss boss = collision.GetComponent<Boss>();

        if (boss != null)
        {
        
            boss.TakeDamage(damage); 
            Debug.Log("Boss atingido! Dano: " + damage);
            Destroy(gameObject); 
            return; 
        }

        if (collision.CompareTag("Enemy"))
        {
            Monster monster = collision.GetComponent<Monster>();
            if (monster != null && !monster.entity.dead)
            {
                monster.entity.currentHealth -= damage; // Aplicação de dano direta
                monster.entity.target = GameObject.FindGameObjectWithTag("Player");
                Debug.Log("Inimigo atingido! Dano: " + damage);
            }
            Destroy(gameObject);
        }
        

        else if (collision.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}