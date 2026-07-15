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
    private void ProcessBCIData()
    {
        // LSL Inlet이 연결되지 않았거나, OpenVibe에서 전송되는 데이터가 없다면 작동하지 않음
        if (eegInlet == null || !eegInlet.IsConnected) return;

        // OpenVibe에서 전송된 가장 최근의 뇌파 파워 배열 데이터를 가져옵니다.
        // (보통 채널 0번을 Alpha 대역(8~13Hz), 채널 1번을 Beta 대역(13~30Hz)으로 설정합니다)
        float[] sample = eegInlet.GetLastSample();

        if (sample != null && sample.Length >= 2)
        {
            float alphaPower = sample[0]; // 8~13 Hz 대역 파워 (눈 깜빡임 / 가만히 있기)
            float betaPower = sample[1];  // 13~30 Hz 대역 파워 (혀 깨물기)

            // 1. 공격 마법 (8~13Hz, Power >= 1600) -> 눈 깜빡임 감지
            if (alphaPower >= 1600f && !IsOnCooldown(SpellType.Attack)) 
            {
                Debug.Log($"[BCI 감지] 눈 깜빡임 파워({alphaPower}) 도달 -> 공격 Flag ON!");
                StartCoroutine(RaiseFlag(SpellType.Attack));
            }

            // 2. 방어 마법 (13~30Hz, 800 <= Power <= 1500) -> 혀 깨물기 감지
            if (betaPower >= 800f && betaPower <= 1500f && !IsOnCooldown(SpellType.Defense)) 
            {
                Debug.Log($"[BCI 감지] 혀 깨물기 파워({betaPower}) 도달 -> 방어 Flag ON!");
                StartCoroutine(RaiseFlag(SpellType.Defense));
            }

            // 3. 회복 마법 (8~13Hz, Power <= 200) -> 가만히 명상 상태 감지
            if (alphaPower <= 200f && !IsOnCooldown(SpellType.Heal)) 
            {
                Debug.Log($"[BCI 감지] 명상 파워({alphaPower}) 도달 -> 회복 Flag ON!");
                StartCoroutine(RaiseFlag(SpellType.Heal));
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