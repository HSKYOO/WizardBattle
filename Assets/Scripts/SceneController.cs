using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [Header("Tutorial UI (Practice Scene)")]
    [Tooltip("3가지 마법을 모두 사용하면 활성화될 다음 스테이지 이동 버튼")]
    public GameObject nextStageBtn;
    
    // 팀원의 아이디어: 중복 없이 사용한 마법 종류를 기록하는 집합(HashSet)
    private HashSet<string> usedSpells = new HashSet<string>();

    [Header("Scene Names")]
    public string startSceneName = "StartScene";
    public string practiceSceneName = "PracticeScene";
    public string battleSceneName = "BattleScene";
    public string endSceneName = "EndScene";

    // =========================================================================
    // 1. 튜토리얼 (Practice Scene) 마법 사용 감지 로직 
    // =========================================================================
    public void OnSpellUsed(string spellName)
    {
        // 사용한 마법 이름을 기록 (HashSet이라 같은 마법을 여러 번 써도 1개로 간주)
        usedSpells.Add(spellName);
        Debug.Log($"[SceneController] 튜토리얼 마법 시전: {spellName} (현재 체험한 마법: {usedSpells.Count}/3개)");

        // 3가지 마법(Attack, Defense, Heal)을 모두 사용해보았을 때 버튼 활성
        if (usedSpells.Count >= 3 && nextStageBtn != null)
        {
            nextStageBtn.SetActive(true);
            Debug.Log("[SceneController] 모든 마법 체험 완료! 다음 스테이지 이동 버튼이 활성화되었습니다.");
        }
    }

    // =========================================================================
    // 2. 씬 이동 메서드 (팀원 메서드 호환 & GameManager 에러 방지 통합)
    // =========================================================================

    /// <summary>
    /// [팀원이 연결한 버튼 호환용] 배틀 씬으로 이동
    /// </summary>
    public void GoToBattleScene()
    {
        LoadBattleScene();
    }

    /// <summary>
    /// [Practice 씬 -> 다음으로 버튼] 또는 [End 씬 -> Again? 버튼] 클릭 시 실전 대결(Battle)로 이동
    /// </summary>
    public void LoadBattleScene()
    {
        Debug.Log($"[SceneController] '{battleSceneName}' 씬으로 이동합니다.");
        SceneManager.LoadScene(battleSceneName);
    }

    /// <summary>
    /// [GameManager 필수 호출 메서드] 전투에서 승패 결정 후 결과 화면(End)으로 이동
    /// </summary>
    public void LoadEndScene()
    {
        Debug.Log($"[SceneController] '{endSceneName}' 씬으로 이동합니다.");
        SceneManager.LoadScene(endSceneName);
    }

    /// <summary>
    /// [Start 씬 -> Start 버튼] 클릭 시 연습장(Practice)으로 이동
    /// </summary>
    public void LoadPracticeScene()
    {
        Debug.Log($"[SceneController] '{practiceSceneName}' 씬으로 이동합니다.");
        SceneManager.LoadScene(practiceSceneName);
    }

    /// <summary>
    /// 다시 시작 화면(Start)으로 돌아가고 싶을 때 호출
    /// </summary>
    public void LoadStartScene()
    {
        Debug.Log($"[SceneController] '{startSceneName}' 씬으로 이동합니다.");
        SceneManager.LoadScene(startSceneName);
    }

    /// <summary>
    /// [Start 씬, End 씬 -> Exit 버튼] 클릭 시 게임 종료
    /// </summary>
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