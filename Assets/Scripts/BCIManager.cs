using UnityEngine;
using System.Collections;

public class BCIManager : MonoBehaviour
{
    [Header("Current Flag Status (2초 유지)")]
    public bool attackFlag;
    public bool defenseFlag;
    public bool healFlag;

    [Header("Duration Settings")]
    public float flagDuration = 2f;   // 기획서: Flag 유지 시간 2초 
    public float cooldownDuration = 3f; // 기획서 보완: 모든 마법 쿨타임 3초

    // [NEW] UIManager의 게이지와 남은 시간 출력을 위한 float 타이머
    private float attackCooldownTimer = 0f;
    private float defenseCooldownTimer = 0f;
    private float healCooldownTimer = 0f;

    public enum SpellType { Attack, Defense, Heal }

    void Update()
    {
        // 1. 매 프레임마다 남은 쿨타임 시간을 감소시킴 (0 이하로 내려가지 않게 고정)
        if (attackCooldownTimer > 0) attackCooldownTimer -= Time.deltaTime;
        if (defenseCooldownTimer > 0) defenseCooldownTimer -= Time.deltaTime;
        if (healCooldownTimer > 0) healCooldownTimer -= Time.deltaTime;

        // -------------------------------------------------------------------------
        // [테스트용 키보드 입력] : OpenVibe 연동 전까지 1, 2, 3 키로 Flag를 켭니다!
        // -------------------------------------------------------------------------
        if (Input.GetKeyDown(KeyCode.Alpha1) && !IsOnCooldown(SpellType.Attack)) 
            StartCoroutine(RaiseFlag(SpellType.Attack));
        if (Input.GetKeyDown(KeyCode.Alpha2) && !IsOnCooldown(SpellType.Defense)) 
            StartCoroutine(RaiseFlag(SpellType.Defense));
        if (Input.GetKeyDown(KeyCode.Alpha3) && !IsOnCooldown(SpellType.Heal)) 
            StartCoroutine(RaiseFlag(SpellType.Heal));

        // -------------------------------------------------------------------------
        // TODO: [OpenVibe 뇌파 데이터 연동 영역] 
        // 나중에 LSL이나 UDP 통신으로 실시간 EEG Power 변수를 받아오면 아래 주석을 풀고 구현합니다.
        // -------------------------------------------------------------------------
        /*
        float eeg8to13 = GetOpenVibeAlphaPower(); // 8~13Hz 대역 파워 [cite: 32, 54]
        float eeg13to30 = GetOpenVibeBetaPower(); // 13~30Hz 대역 파워 [cite: 44]

        // 공격 마법 (8~13Hz, Power >= 1600) 
        if (eeg8to13 >= 1600f && !IsOnCooldown(SpellType.Attack)) StartCoroutine(RaiseFlag(SpellType.Attack));
        // 방어 마법 (13~30Hz, 800 <= Power <= 1500) 
        if (eeg13to30 >= 800f && eeg13to30 <= 1500f && !IsOnCooldown(SpellType.Defense)) StartCoroutine(RaiseFlag(SpellType.Defense));
        // 회복 마법 (8~13Hz, Power <= 200) 
        if (eeg8to13 <= 200f && !IsOnCooldown(SpellType.Heal)) StartCoroutine(RaiseFlag(SpellType.Heal));
        */
    }

    IEnumerator RaiseFlag(SpellType type)
    {
        SetFlag(type, true);
        yield return new WaitForSeconds(flagDuration);
        SetFlag(type, false);
    }

    void SetFlag(SpellType type, bool value)
    {
        switch (type)
        {
            case SpellType.Attack: attackFlag = value; break;
            case SpellType.Defense: defenseFlag = value; break;
            case SpellType.Heal: healFlag = value; break;
        }
    }

    /// <summary>
    /// 마법 사용 시 SpellManager가 호출해 줄 쿨타임 시작 함수
    /// </summary>
    public void StartCooldown(SpellType type)
    {
        // 1. 마법을 사용했으니 켜져있던 Flag는 즉시 강제로 꺼줍니다 (중복 사용 방지!)
        SetFlag(type, false);

        // 2. float 타이머에 쿨타임 시간(3초)을 채워 넣습니다.
        switch (type)
        {
            case SpellType.Attack: attackCooldownTimer = cooldownDuration; break;
            case SpellType.Defense: defenseCooldownTimer = cooldownDuration; break;
            case SpellType.Heal: healCooldownTimer = cooldownDuration; break;
        }
    }

    /// <summary>
    /// 현재 쿨타임 중인지 확인하는 함수 (타이머가 0보다 크면 쿨타임 중)
    /// </summary>
    public bool IsOnCooldown(SpellType type)
    {
        return GetRemainingCooldown(type) > 0f;
    }

    /// <summary>
    /// [UIManager 연동 필수 함수 1] 정확한 잔여 쿨타임 초(float)를 반환
    /// </summary>
    public float GetRemainingCooldown(SpellType type)
    {
        switch (type)
        {
            case SpellType.Attack: return Mathf.Max(0f, attackCooldownTimer);
            case SpellType.Defense: return Mathf.Max(0f, defenseCooldownTimer);
            case SpellType.Heal: return Mathf.Max(0f, healCooldownTimer);
            default: return 0f;
        }
    }

    /// <summary>
    /// [UIManager 연동 필수 함수 2] 오버레이 회전 게이지(fillAmount)를 위한 비율(0.0 ~ 1.0) 반환
    /// </summary>
    public float GetCooldownRatio(SpellType type)
    {
        return GetRemainingCooldown(type) / cooldownDuration;
    }
}