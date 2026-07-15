using UnityEngine;

public class SpellIconUI : MonoBehaviour
{
    public SpriteRenderer iconRenderer;
    public Color activeColor = Color.white;
    public Color inactiveColor = new Color(0.3f, 0.3f, 0.3f, 1f);

    void Start()
    {
        SetActive(false);
    }

    public void SetActive(bool isActive)
    {
        iconRenderer.color = isActive ? activeColor : inactiveColor;
    }
}