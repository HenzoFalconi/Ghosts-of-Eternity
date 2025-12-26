using UnityEngine;

public class AmmoPickUp : MonoBehaviour
{
    public int ammoAmount = 5;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.entity.currentAmmo += 10;
                Debug.Log("Pegou " + player.entity.currentAmmo + " balas!");
                Destroy(gameObject);
            }
        }
    }
}
