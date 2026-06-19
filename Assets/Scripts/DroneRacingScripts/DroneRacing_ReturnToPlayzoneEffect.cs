using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DroneRacing_ReturnToPlayzoneEffect : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI text;

    [SerializeField] private float flickerSpeed = 4f;
    [SerializeField] private float minAlpha = 0.2f;
    [SerializeField] private float maxAlpha = 1f;

    void Update()
    {
        float alpha = Mathf.Lerp(
            minAlpha,
            maxAlpha,
            Mathf.PingPong(Time.unscaledTime * flickerSpeed, 1f)
        );

        SetAlpha(image, alpha);
        SetAlpha(text, alpha);
    }

    private void SetAlpha(Graphic graphic, float alpha)
    {
        Color color = graphic.color;
        color.a = alpha;
        graphic.color = color;
    }
}