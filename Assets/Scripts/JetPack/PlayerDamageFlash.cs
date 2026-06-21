using System.Collections;
using UnityEngine;

public class PlayerDamageFlash : MonoBehaviour
{
    [SerializeField] private Renderer[] renderers;
    [SerializeField] private Color flashColor = new Color(1f, 0.2f, 0.2f);
    [SerializeField] private float flashDuration = 0.12f;
    [SerializeField] private int flashAmount = 3;

    private Color[] originalColors;
    private Coroutine flashCoroutine;

    private void Awake()
    {
        originalColors = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            originalColors[i] = renderers[i].material.color;
        }
    }

    public void Flash()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        for (int i = 0; i < flashAmount; i++)
        {
            SetColor(flashColor);
            yield return new WaitForSeconds(flashDuration);

            SetOriginalColors();
            yield return new WaitForSeconds(flashDuration);
        }
    }

    private void SetColor(Color color)
    {
        foreach (Renderer renderer in renderers)
        {
            renderer.material.color = color;
        }
    }

    private void SetOriginalColors()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = originalColors[i];
        }
    }
}
