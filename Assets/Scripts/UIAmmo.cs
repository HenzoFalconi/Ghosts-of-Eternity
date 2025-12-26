using UnityEngine;
using UnityEngine.UI;

public class UIAmmo : MonoBehaviour
{
    public Player player;
    public Text ammoText;

    void Update()
    {
        ammoText.text = "Ammo: " + player.entity.currentAmmo;
    }
}
