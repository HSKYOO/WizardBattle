using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Character References")]
    [Tooltip("플레이어의 CharacterStats")]
    public CharacterStats playerStats;
    [Tooltip("적(AI)의 CharacterStats")]
    public CharacterStats enemyStats;
    [Tooltip("전투 종료 시 AI 동작을 멈추기 위한 EnemyController")]
    public EnemyController enemyController;

    [Header("Scene Controller Reference")]
    [Tooltip("결과 씬(End Scene)으로 이동시켜 줄 SceneController")]
    public SceneController sceneController;

    [Header("Game Settings")]
    [Tooltip("사망 후 결과 씬으로 넘어가기 전 대기 시간 (초)")]
    public float delayBeforeEndScene = 2.0f;

    // [핵심] End Scene에서 승리인지 패배인지 확인하기 위한 전역(Static) 변수
    // (씬이 바뀌어도 값이 유지되므로 다음 씬에서 이 변수를 읽으면 됩니다!)
    public static bool IsVictory = false;

    // 게임이 이미 종료되었는지 확인하는 플래그 (중복 실행 방지)
    private bool isGameOver = false;

    private void Start()
    {
        // 1. 캐릭터 사망 이벤트에 게임오버 메서드 연결
        if (playerStats != null)
        {
            playerStats.OnDeath.AddListener(OnPlayerDeath);
        }
        else
        {
            Debug.LogError("[GameManager] Player Stats가 연결되지 않았습니다!");
        }

        if (enemyStats != null)
        {
            enemyStats.OnDeath.AddListener(OnEnemyDeath);
        }
        else
        {
            Debug.LogError("[GameManager] Enemy Stats가 연결되지 않았습니다!");
        }
    }

    /// <summary>
    /// 플레이어 체력이 0이 되었을 때 호출 [패배] [cite: 10, 83]
    /// </summary>
    private void OnPlayerDeath()
    {
        if (isGameOver) return; // 이미 게임이 끝났다면 무시
        isGameOver = true;

        Debug.Log("💀 [GameManager] 플레이어 사망... 패배(Defeat)!");
        IsVictory = false; // 패배 기록 [cite: 83]

        // AI가 쓰러진 플레이어를 계속 공격하지 않도록 즉시 정지 
        if (enemyController != null)
        {
            enemyController.StopAI();
        }

        // 2초 뒤 End Scene으로 이동하는 코루틴 실행
        StartCoroutine(EndGameRoutine());
    }

    /// <summary>
    /// 적 체력이 0이 되었을 때 호출 [승리] [cite: 10, 76]
    /// </summary>
    private void OnEnemyDeath()
    {
        if (isGameOver) return;
        isGameOver = true;

        Debug.Log("🎉 [GameManager] 적 처치 완료! 승리(Victory)!");
        IsVictory = true; // 승리 기록 [cite: 76]

        // 적 AI 작동 정지 
        if (enemyController != null)
        {
            enemyController.StopAI();
        }

        // 2초 뒤 End Scene으로 이동하는 코루틴 실행
        StartCoroutine(EndGameRoutine());
    }

    /// <summary>
    /// 타격감 및 사망 애니메이션 연출을 위해 대기 후 씬을 전환하는 코루틴
    /// </summary>
    private IEnumerator EndGameRoutine()
    {
        Debug.Log($"[GameManager] {delayBeforeEndScene}초 후 결과 화면(End Scene)으로 이동합니다...");
        yield return new WaitForSeconds(delayBeforeEndScene);

        // SceneController가 연결되어 있다면 End Scene으로 이동
        if (sceneController != null)
        {
            // (주의: SceneController에 EndScene으로 이동하는 메서드 이름에 맞춰 호출하세요!)
            sceneController.LoadEndScene(); 
        }
        else
        {
            Debug.LogWarning("[GameManager] SceneController가 없어 씬 전환을 할 수 없습니다. Inspector 창을 확인해주세요!");
        }
    }

    private void OnDestroy()
    {
        // 씬이 변경되거나 오브젝트가 파괴될 때 이벤트 연결 해제 (메모리 누수 방지)
        if (playerStats != null) playerStats.OnDeath.RemoveListener(OnPlayerDeath);
        if (enemyStats != null) enemyStats.OnDeath.RemoveListener(OnEnemyDeath);
    }
}