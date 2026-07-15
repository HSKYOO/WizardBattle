using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HPBarUI : MonoBehaviour
{
    public Slider slider;

    private Coroutine hpCoroutine;

    public void UpdateHPBar(int current, int max)
    {
        slider.maxValue = max;
        if (hpCoroutine != null)
        {
            StopCoroutine(hpCoroutine);
        }

        hpCoroutine = StartCoroutine(SmoothHPChange(current));
    }

    private IEnumerator SmoothHPChange(float targetHP)
    {
        float startHP = slider.value;
        float elapsedTime = 0f;
        float duration = 0.2f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            slider.value = Mathf.Lerp(
                startHP,
                targetHP,
                elapsedTime / duration
            );
            yield return null;
        }
        slider.value = targetHP;
    }
}