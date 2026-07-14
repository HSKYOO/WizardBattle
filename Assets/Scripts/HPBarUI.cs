using UnityEngine;
using UnityEngine.UI;

public class HPBarUI : MonoBehaviour
{
    public Slider slider;

    public void UpdateHPBar(int current, int max)
    {
        slider.maxValue = max;
        slider.value = current;
    }
}