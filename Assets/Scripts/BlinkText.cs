using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BlinkText : MonoBehaviour
{
    public Text uiText;
    public float blinkInterval = 1f;
    private bool isVisible = true;

    void Start()
    {
        if (uiText == null)
            uiText = GetComponent<Text>();

        uiText.enabled = true; // garante que começa visível
        StartCoroutine(Blink());
    }

    IEnumerator Blink()
    {
        while (true)
        {
            yield return new WaitForSeconds(blinkInterval);
            isVisible = !isVisible;
            uiText.enabled = isVisible;
        }
    }
}
