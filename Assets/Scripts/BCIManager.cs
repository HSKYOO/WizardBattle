using UnityEngine;
using System.Collections;
using LSL;             // [NEW] LSL 통신 코어 라이브러리
using LSL4Unity.Utils; // [NEW] 유니티용 LSL 유틸리티 및 데이터 변환 기능
// (주의: 사용하는 LSL4Unity 버전에 따라 아래 줄이 필요할 수 있습니다. 에러가 나면 지워주세요!)

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

    [Header("LSL BCI Hardware Connection (⭐ 핵심)")]
    [Tooltip("OpenVibe에서 데이터를 받아오는 LSL4Unity의 FloatInlet 스크립트를 연결하세요!")]
    public FloatInlet eegInlet; 

    [Header("LSL 3-Stream Connections")]
    [Tooltip("OpenVibe의 'LSL Export [8-13] attack' 상자와 연결된 FloatInlet")]
    public FloatInlet attackInlet;
    
    [Tooltip("OpenVibe의 'LSL Export [13-30]' (방어) 상자와 연결된 FloatInlet")]
    public FloatInlet defenseInlet;
    
    [Tooltip("OpenVibe의 'LSL Export [8-13] heal' 상자와 연결된 FloatInlet")]
    public FloatInlet healInlet;

    // UIManager의 게이지와 남은 시간 출력을 위한 float 타이머
    private float attackCooldownTimer = 0f;
    private float defenseCooldownTimer = 0f;
    private float healCooldownTimer = 0f;

    public enum SpellType { Attack, Defense, Heal }

    void Update()
    {
        // 1. 매 프레임마다 남은 쿨타임 시간을 감소시킴
        if (attackCooldownTimer > 0) attackCooldownTimer -= Time.deltaTime;
        if (defenseCooldownTimer > 0) defenseCooldownTimer -= Time.deltaTime;
        if (healCooldownTimer > 0) healCooldownTimer -= Time.deltaTime;

        // -------------------------------------------------------------------------
        // [테스트용 키보드 입력] : 장비 없이도 1, 2, 3 키로 Flag를 켜는 백업 기능
        // -------------------------------------------------------------------------
        if (Input.GetKeyDown(KeyCode.Alpha1) && !IsOnCooldown(SpellType.Attack)) 
            StartCoroutine(RaiseFlag(SpellType.Attack));
        if (Input.GetKeyDown(KeyCode.Alpha2) && !IsOnCooldown(SpellType.Defense)) 
            StartCoroutine(RaiseFlag(SpellType.Defense));
        if (Input.GetKeyDown(KeyCode.Alpha3) && !IsOnCooldown(SpellType.Heal)) 
            StartCoroutine(RaiseFlag(SpellType.Heal));

        // -------------------------------------------------------------------------
        // [OpenVibe 실시간 LSL 뇌파 데이터 연동] 
        // -------------------------------------------------------------------------
        ProcessBCIData();
    }

    /// <summary>
    /// LSL 통신을 통해 OpenVibe 데이터 스트림을 실시간으로 읽고 Flag를 작동시킵니다.
    /// </summary>
    /// <summary>
    /// 3개의 파이프(Inlet)에서 각각 독립적으로 뇌파 데이터를 읽어와 마법을 트리거합니다.
    /// </summary>
    private void ProcessBCIData()
    {
        // 1. 공격 마법 파이프 검사 (AttackStream)
        if (attackInlet != null && attackInlet.IsConnected)
        {
            float[] attackSample = attackInlet.GetLastSample();
            if (attackSample != null && attackSample.Length > 0)
            {
                // 👇 [확인용 로그] 유니티 콘솔에서 내 뇌파의 진짜 숫자를 확인하세요!
                Debug.Log($"👀 [실시간 공격 스트림 수치] : {attackSample[0]:F1}");

                // 쿨타임이 아니고, 임계값(현재 1600f -> 내 뇌파 수치에 맞게 수정!) 이상이면 발동
                if (!IsOnCooldown(SpellType.Attack) && attackSample[0] >= 1600f)
                {
                    Debug.Log($"🔥 [BCI 공격 감지] 파워: {attackSample[0]} -> 공격 Flag ON!");
                    StartCoroutine(RaiseFlag(SpellType.Attack));
                }
            }
        }

        // 2. 방어 마법 파이프 검사 (DefenseStream)
        if (defenseInlet != null && defenseInlet.IsConnected)
        {
            float[] defenseSample = defenseInlet.GetLastSample();
            if (defenseSample != null && defenseSample.Length > 0)
            {
                if (!IsOnCooldown(SpellType.Defense) && defenseSample[0] >= 800f && defenseSample[0] <= 1500f)
                {
                    Debug.Log($"🛡️ [BCI 방어 감지] 파워: {defenseSample[0]} -> 방어 Flag ON!");
                    StartCoroutine(RaiseFlag(SpellType.Defense));
                }
            }
        }

        // 3. 회복 마법 파이프 검사 (HealStream)
        if (healInlet != null && healInlet.IsConnected)
        {
            float[] healSample = healInlet.GetLastSample();
            if (healSample != null && healSample.Length > 0)
            {
                if (!IsOnCooldown(SpellType.Heal) && healSample[0] <= 200f)
                {
                    Debug.Log($"💚 [BCI 회복 감지] 파워: {healSample[0]} -> 회복 Flag ON!");
                    StartCoroutine(RaiseFlag(SpellType.Heal));
                }
            }
        }
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
                if (attackIcon != null) attackIcon.SetActive(value); 
                break;
            case SpellType.Defense: 
                defenseFlag = value; 
                if (defenseIcon != null) defenseIcon.SetActive(value); 
                break;
            case SpellType.Heal: 
                healFlag = value; 
                if (healIcon != null) healIcon.SetActive(value); 
                break;
        }
    }

    public void StartCooldown(SpellType type)
    {
        SetFlag(type, false); 

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