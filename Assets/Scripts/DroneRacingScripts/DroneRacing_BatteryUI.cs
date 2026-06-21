using UnityEngine;
using UnityEngine.UI;

public class DroneRacing_BatteryUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private Color fullColor = new Color(0.25f, 0.8f, 0.25f, 1f);
    [SerializeField] private Color mediumColor = new Color(1f, 0.65f, 0.2f, 1f);
    [SerializeField] private Color lowColor = new Color(1f, 0.2f, 0.2f, 1f);

    public void SetBattery(int currentCharges, int maxCharges)
    {
        if (fillImage == null) return;

        maxCharges = Mathf.Max(1, maxCharges);
        currentCharges = Mathf.Clamp(currentCharges, 0, maxCharges);

        fillImage.fillAmount = (float)currentCharges / maxCharges;

        if (currentCharges >= maxCharges)
        {
            fillImage.color = fullColor;
        }
        else if (currentCharges >= 2)
        {
            fillImage.color = mediumColor;
        }
        else if (currentCharges >= 1)
        {
            fillImage.color = lowColor;
        }
        else
        {
            fillImage.color = lowColor;
        }
    }
}
