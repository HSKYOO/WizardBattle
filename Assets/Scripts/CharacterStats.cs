using UnityEngine;
using UnityEngine.Events;
using System.Collections;


public class CharacterStat : MonoBehaviour
{
    [Header("Health Settings")]
    [Tooltip("최대 체력 (기본값: 100)")]
    public int maxHP = 100;
    
    // 현재 체력 (외부에서 읽을 수는 있지만 수정은 메서드를 통해서만 가능)
    public int CurrentHP { get; private set; }

    [Header("Status Flags")]
    [Tooltip("방어 마법 사용 시 true로 설정되어 피해를 무시합니다.")]
    public bool isInvulnerable = false;

    [Header("Events (UI 및 게임 관리용)")]
    [Tooltip("체력이 변경될 때 호출됩니다. (현재 체력, 최대 체력)을 전달합니다.")]
    public UnityEvent<int, int> OnHPChanged;
    
    [Tooltip("체력이 0 이하가 되어 사망했을 때 호출됩니다.")]
    public UnityEvent OnDeath;

    private void Awake()
    {
        // 게임 시작 시 현재 체력을 최대 체력으로 초기화 
        CurrentHP = maxHP;
    }

    private void Start()
    {
        // 시작과 동시에 UI의 HP 바를 최대치로 반영하기 위해 이벤트 호출
        OnHPChanged?.Invoke(CurrentHP, maxHP);
    }

    /// <summary>
    /// 피해를 입었을 때 호출하는 메서드 (공격 마법에 적중당했을 때)
    /// </summary>
    /// <param name="damage">입을 피해량 (기본 공격 데미지: 30)</param>
    public void TakeDamage(int damage)
    {
        // 방어 마법 지속 시간 중이라면 피해량을 0으로 처리하고 무시 
        if (isInvulnerable)
        {
            Debug.Log($"[{gameObject.name}] 방어 마법 활성화 중! 공격을 막아냈습니다. (피해량 0) [cite: 51]");
            return;
        }

        // 피해를 입고 체력 감소
        CurrentHP -= damage;
        Debug.Log($"[{gameObject.name}] {damage}의 피해를 입었습니다. 남은 체력: {CurrentHP}");

        // 체력이 0 미만으로 내려가지 않도록 고정
        if (CurrentHP < 0)
        {
            CurrentHP = 0;
        }

        // HP 바 갱신 이벤트 호출
        OnHPChanged?.Invoke(CurrentHP, maxHP);

        // 체력이 0 이하라면 사망 처리 
        if (CurrentHP == 0)
        {
            Die();
        }
    }

    /// <summary>
    /// 즉시 체력을 회복할 때 호출하는 메서드
    /// </summary>
    public void Heal(int amount)
    {
        if (CurrentHP <= 0) return; // 이미 사망한 상태면 회복 불가

        CurrentHP += amount;
        if (CurrentHP > maxHP)
        {
            CurrentHP = maxHP; // 최대 체력을 넘지 않도록 고정
        }

        Debug.Log($"[{gameObject.name}] {amount} 회복! 현재 체력: {CurrentHP}");
        OnHPChanged?.Invoke(CurrentHP, maxHP);
    }

    /// <summary>
    /// 2초 동안 40의 체력을 서서히 회복하는 회복 마법용 메서드 
    /// SpellManager에서 이 메서드를 호출하면 됩니다.
    /// </summary>
    public void StartHealOverTime(int totalHeal = 40, float duration = 2.0f)
    {
        StartCoroutine(HealOverTimeCoroutine(totalHeal, duration));
    }

    private IEnumerator HealOverTimeCoroutine(int totalHeal, float duration)
    {
        Debug.Log($"[{gameObject.name}] 지속 회복 마법 시작! ({duration}초 동안 총 {totalHeal} 회복) ");
        
        // 0.2초마다 나눠서 회복시키기 위한 시간 간격
        float tickInterval = 0.2f; 
        int ticks = Mathf.RoundToInt(duration / tickInterval);
        int healPerTick = Mathf.RoundToInt((float)totalHeal / ticks);

        for (int i = 0; i < ticks; i++)
        {
            if (CurrentHP <= 0) yield break; // 회복 도중 사망하면 중단 

            Heal(healPerTick);
            yield return new WaitForSeconds(tickInterval);
        }
    }

    /// <summary>
    /// 방어 마법(W) 사용 시 무적 상태를 설정하는 메서드
    /// </summary>
    public void SetInvulnerable(bool state)
    {
        isInvulnerable = state;
        Debug.Log($"[{gameObject.name}] 무적 상태 변경: {isInvulnerable}");
    }

    /// <summary>
    /// 사망 처리 메서드
    /// </summary>
    private void Die()
    {
        Debug.Log($"[{gameObject.name}] 체력이 0이 되어 쓰러졌습니다! ");
        OnDeath?.Invoke(); // GameManager나 SceneController에서 이 이벤트를 감지하여 승리/패배 씬으로 이동합니다.
    }
}
