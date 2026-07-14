using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [Header("Tutorial UI (Practice Scene)")]
    [Tooltip("3가지 마법을 모두 사용하면 활성화될 다음 스테이지 이동 버튼")]
    public Button nextStageBtn;
    public Image nextStageBtnImage;
    public Color activeColor = Color.white;
    public Color inactiveColor = new Color(0.4f, 0.4f, 0.4f, 1f);

    // 팀원의 아이디어: 중복 없이 사용한 마법 종류를 기록하는 집합(HashSet)
    private HashSet<string> usedSpells = new HashSet<string>();

    [Header("Scene Names")]
    public string startSceneName = "StartScene";
    public string practiceSceneName = "PracticeScene";
    public string battleSceneName = "BattleScene";
    public string endSceneName = "EndScene";

    void Start()
    {
        SetButtonState(false); // 시작할 땐 비활성 + 회색
    }

    // =========================================================================
    // 1. 튜토리얼 (Practice Scene) 마법 사용 감지 로직 
    // =========================================================================
    public void OnSpellUsed(string spellName)
    {
        usedSpells.Add(spellName);
        Debug.Log($"[SceneController] 튜토리얼 마법 시전: {spellName} (현재 체험한 마법: {usedSpells.Count}/3개)");

        if (usedSpells.Count >= 3)
        {
            SetButtonState(true);
            Debug.Log("[SceneController] 모든 마법 체험 완료! 다음 스테이지 이동 버튼이 활성화되었습니다.");
        }
    }

    void SetButtonState(bool isActive)
    {
        if (nextStageBtn != null) nextStageBtn.interactable = isActive;
        if (nextStageBtnImage != null) nextStageBtnImage.color = isActive ? activeColor : inactiveColor;
    }

    // =========================================================================
    // 2. 씬 이동 메서드 (팀원 메서드 호환 & GameManager 에러 방지 통합)
    // =========================================================================

    public void GoToBattleScene()
    {
        LoadBattleScene();
    }

    public void LoadBattleScene()
    {
        Debug.Log($"[SceneController] '{battleSceneName}' 씬으로 이동합니다.");
        SceneManager.LoadScene(battleSceneName);
    }

    public void LoadEndScene()
    {
        Debug.Log($"[SceneController] '{endSceneName}' 씬으로 이동합니다.");
        SceneManager.LoadScene(endSceneName);
    }

    public void LoadPracticeScene()
    {
        Debug.Log($"[SceneController] '{practiceSceneName}' 씬으로 이동합니다.");
        SceneManager.LoadScene(practiceSceneName);
    }

    public void LoadStartScene()
    {
        Debug.Log($"[SceneController] '{startSceneName}' 씬으로 이동합니다.");
        SceneManager.LoadScene(startSceneName);
    }

    public void ExitGame()
    {
        Debug.Log("[SceneController] 게임을 종료합니다.");
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}