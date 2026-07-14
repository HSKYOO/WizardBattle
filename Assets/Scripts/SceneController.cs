using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public Button nextStageBtn;              // GameObject → Button 으로 타입 변경
    public Image nextStageBtnImage;           // 추가: 버튼 색 전환용
    public Color activeColor = Color.white;
    public Color inactiveColor = new Color(0.4f, 0.4f, 0.4f, 1f);

    private HashSet<string> usedSpells = new HashSet<string>();

    void Start()
    {
        SetButtonState(false); // 추가: 시작할 때 비활성 + 회색
    }

    public void OnSpellUsed(string spellName)
    {
        usedSpells.Add(spellName);
        if (usedSpells.Count >= 3)
            SetButtonState(true); // SetActive(true) 대신 이걸로 교체
    }

    void SetButtonState(bool isActive) // 추가된 함수
    {
        nextStageBtn.interactable = isActive;
        nextStageBtnImage.color = isActive ? activeColor : inactiveColor;
    }

    public void GoToBattleScene()
    {
        SceneManager.LoadScene("BattleScene");
    }
}