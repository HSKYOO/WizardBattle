using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("적의 마법을 실행할 SpellManager")]
    public SpellManager spellManager;
    [Tooltip("적의 체력 상태를 확인할 CharacterStats")]
    public CharacterStats stats;

    [Header("AI Pattern Settings (기획서 명시 패턴)")]
    [Tooltip("적 AI가 반복 실행할 마법 순서")]
    public SpellType[] actionPattern = new SpellType[]
    {
        SpellType.Attack,  // 1. 공격
        SpellType.Attack,  // 2. 공격
        SpellType.Defense, // 3. 방어
        SpellType.Heal,    // 4. 회복
        SpellType.Attack,  // 5. 공격
        SpellType.Defense  // 6. 방어
    };

    [Header("AI Timers")]
    [Tooltip("마법과 마법 사이의 행동 대기 시간 (초) - 유저의 난이도 체감에 맞게 조절")]
    public float actionInterval = 3.5f;
    
    [Tooltip("전투 시작 직후 첫 마법을 쓰기까지 대기하는 준비 시간 (초)")]
    public float initialDelay = 2.0f;

    private int currentPatternIndex = 0;
    private Coroutine aiCoroutine;

    public enum SpellType { Attack, Defense, Heal }

    private void Awake()
    {
        // 필수 컴포넌트 자동 연결
        if (spellManager == null) spellManager = GetComponent<SpellManager>();
        if (stats == null) stats = GetComponent<CharacterStats>();
    }

    private void Start()
    {
        // 전투 시작과 함께 AI 패턴 루프 코루틴 실행
        aiCoroutine = StartCoroutine(AILoopRoutine());
    }

    /// <summary>
    /// 적이 사망하거나 게임이 끝날 때까지 정해진 패턴을 무한 반복하는 코루틴 
    /// </summary>
    private IEnumerator AILoopRoutine()
    {
        // 1. 전투 시작 직후 플레이어가 상황을 파악하고 마법을 준비할 수 있도록 잠시 대기
        Debug.Log($"[{gameObject.name}] AI 패턴 전투 준비 중... ({initialDelay}초 후 시작)");
        yield return new WaitForSeconds(initialDelay);

        // 2. 무한 루프 시작 
        while (true)
        {
            // 적이 이미 사망했거나 컴포넌트가 없다면 AI 행동 즉시 중단
            if (stats != null && stats.CurrentHP <= 0)
            {
                Debug.Log($"[{gameObject.name}] 적이 사망하여 AI 패턴 루프를 정지합니다.");
                yield break;
            }

            // 현재 인덱스에 해당하는 마법 가져오기
            SpellType currentSpell = actionPattern[currentPatternIndex];
            
            // 마법 실행
            ExecuteSpell(currentSpell);

            // 다음 패턴 인덱스로 이동 (배열 끝에 도달하면 % 연산자로 인해 다시 0으로 돌아가 무한 반복) 
            currentPatternIndex = (currentPatternIndex + 1) % actionPattern.Length;

            // 다음 행동까지 설정된 시간만큼 대기
            yield return new WaitForSeconds(actionInterval);
        }
    }

    /// <summary>
    /// 지정된 마법 타입에 따라 SpellManager를 실행하는 함수
    /// </summary>
    private void ExecuteSpell(SpellType spellType)
    {
        if (spellManager == null) return;

        Debug.Log($"[{gameObject.name}] AI 패턴 발동 [{currentPatternIndex + 1}/{actionPattern.Length}] -> [{spellType}]");

        switch (spellType)
        {
            case SpellType.Attack:
                spellManager.CastAttack();
                break;
            case SpellType.Defense:
                spellManager.CastDefense();
                break;
            case SpellType.Heal:
                spellManager.CastHeal();
                break;
        }
    }

    /// <summary>
    /// 외부(GameManager 등)에서 전투가 끝나거나 적이 사망했을 때 AI를 강제로 멈출 수 있는 메서드
    /// </summary>
    public void StopAI()
    {
        if (aiCoroutine != null)
        {
            StopCoroutine(aiCoroutine);
            aiCoroutine = null;
            Debug.Log($"[{gameObject.name}] AI 작동 강제 중지 완료.");
        }
    }
}