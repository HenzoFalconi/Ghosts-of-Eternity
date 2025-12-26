using UnityEngine;

public class CrossHair : MonoBehaviour
{
    public RectTransform crosshair;

    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        crosshair.position = mousePos; // Mira segue o mouse
    }
}
