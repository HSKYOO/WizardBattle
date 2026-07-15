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

    // 중복 없이 사용한 마법 종류 기록
    private readonly HashSet<string> usedSpells = new HashSet<string>();

    [Header("Scene Names")]
    public string startSceneName = "StartScene";
    public string practiceSceneName = "PracticeScene";
    public string battleSceneName = "BattleScene";
    public string victoryEndSceneName = "End";
    public string defeatEndSceneName = "DefeatEnd";

    private void Start()
    {
        // PracticeScene에서만 버튼이 연결되어 있을 때 초기화
        if (nextStageBtn != null || nextStageBtnImage != null)
        {
            SetButtonState(false);
        }
    }

    // =========================================================
    // PracticeScene 튜토리얼 로직
    // =========================================================

    public void OnSpellUsed(string spellName)
    {
        if (string.IsNullOrWhiteSpace(spellName))
        {
            Debug.LogWarning("[SceneController] 비어 있는 마법 이름이 전달되었습니다.");
            return;
        }

        usedSpells.Add(spellName);

        Debug.Log(
            $"[SceneController] 튜토리얼 마법 시전: {spellName} " +
            $"(현재 체험한 마법: {usedSpells.Count}/3개)"
        );

        if (usedSpells.Count >= 3)
        {
            SetButtonState(true);
            Debug.Log(
                "[SceneController] 모든 마법 체험 완료! " +
                "다음 스테이지 버튼이 활성화되었습니다."
            );
        }
    }

    private void SetButtonState(bool isActive)
    {
        if (nextStageBtn != null)
        {
            nextStageBtn.interactable = isActive;
        }

        if (nextStageBtnImage != null)
        {
            nextStageBtnImage.color =
                isActive ? activeColor : inactiveColor;
        }
    }

    // =========================================================
    // StartScene 버튼
    // =========================================================

    // 시작 버튼: 바로 게임씬으로 이동
    public void StartGame()
    {
        LoadBattleScene();
    }

    // 튜토리얼 버튼: 테스트/연습씬으로 이동
    public void StartTutorial()
    {
        LoadPracticeScene();
    }

    // =========================================================
    // Scene 이동
    // =========================================================

    public void LoadBattleScene()
    {
        LoadSceneByName(battleSceneName);
    }

    public void LoadPracticeScene()
    {
        LoadSceneByName(practiceSceneName);
    }

    public void LoadStartScene()
    {
        LoadSceneByName(startSceneName);
    }

    public void LoadVictoryEndScene()
    {
        LoadSceneByName(victoryEndSceneName);
    }

    public void LoadDefeatEndScene()
    {
        LoadSceneByName(defeatEndSceneName);
    }

    // 기존 팀원 코드 호환용
    public void GoToBattleScene()
    {
        LoadBattleScene();
    }

    // 기존 코드에서 LoadEndScene을 호출하는 경우 승리 엔딩으로 이동
    public void LoadEndScene()
    {
        LoadVictoryEndScene();
    }

    // End / DefeatEnd의 AGAIN 버튼용
    public void RestartGame()
    {
        Debug.Log("[SceneController] 게임을 다시 시작합니다.");
        LoadBattleScene();
    }

    private void LoadSceneByName(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError(
                "[SceneController] 이동할 Scene 이름이 비어 있습니다."
            );
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError(
                $"[SceneController] '{sceneName}' Scene을 불러올 수 없습니다. " +
                "Scene 이름과 Build Profiles 등록 여부를 확인하세요."
            );
            return;
        }

        Debug.Log($"[SceneController] '{sceneName}' Scene으로 이동합니다.");
        SceneManager.LoadScene(sceneName);
    }

    // =========================================================
    // 게임 종료
    // =========================================================

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