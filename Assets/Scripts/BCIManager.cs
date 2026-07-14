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

    [Header("Spell Icons")]
    public SpellIconUI attackIcon;
    public SpellIconUI defenseIcon;
    public SpellIconUI healIcon;

    // UIManager의 게이지와 남은 시간 출력을 위한 float 타이머
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
        float eeg8to13 = GetOpenVibeAlphaPower();
        float eeg13to30 = GetOpenVibeBetaPower();

        if (eeg8to13 >= 1600f && !IsOnCooldown(SpellType.Attack)) StartCoroutine(RaiseFlag(SpellType.Attack));
        if (eeg13to30 >= 800f && eeg13to30 <= 1500f && !IsOnCooldown(SpellType.Defense)) StartCoroutine(RaiseFlag(SpellType.Defense));
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
            case SpellType.Attack: 
                attackFlag = value; 
                attackIcon.SetActive(value); 
                break;
            case SpellType.Defense: 
                defenseFlag = value; 
                defenseIcon.SetActive(value); 
                break;
            case SpellType.Heal: 
                healFlag = value; 
                healIcon.SetActive(value); 
                break;
        }
    }

    public void StartCooldown(SpellType type)
    {
        SetFlag(type, false); // 마법 사용 시 Flag 즉시 강제 종료 (중복 사용 방지)

        switch (type)
        {
            case SpellType.Attack: attackCooldownTimer = cooldownDuration; break;
            case SpellType.Defense: defenseCooldownTimer = cooldownDuration; break;
            case SpellType.Heal: healCooldownTimer = cooldownDuration; break;
        }
    }

    public bool IsOnCooldown(SpellType type)
    {
        return GetRemainingCooldown(type) > 0f;
    }

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

    public float GetCooldownRatio(SpellType type)
    {
        return GetRemainingCooldown(type) / cooldownDuration;
    }
}