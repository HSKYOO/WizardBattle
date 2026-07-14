using UnityEngine;
using System.Collections;

public class BCIManager : MonoBehaviour
{
    public bool attackFlag;
    public bool defenseFlag;
    public bool healFlag;

    public float flagDuration = 2f;   // Flag 유지 시간
    public float cooldownDuration = 3f; // 쿨타임

    public SpellIconUI attackIcon;
    public SpellIconUI defenseIcon;
    public SpellIconUI healIcon;

    private bool attackOnCooldown;
    private bool defenseOnCooldown;
    private bool healOnCooldown;

    void Update()
    {
        // TODO: 실제 EEG 값으로 교체. 지금은 테스트용 키 입력
        if (Input.GetKeyDown(KeyCode.Alpha1) && !attackOnCooldown) StartCoroutine(RaiseFlag(SpellType.Attack));
        if (Input.GetKeyDown(KeyCode.Alpha2) && !defenseOnCooldown) StartCoroutine(RaiseFlag(SpellType.Defense));
        if (Input.GetKeyDown(KeyCode.Alpha3) && !healOnCooldown) StartCoroutine(RaiseFlag(SpellType.Heal));
    }

    public enum SpellType { Attack, Defense, Heal }

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

    // 스킬 사용 후 SpellManager가 호출해줄 쿨타임 시작 함수
    public void StartCooldown(SpellType type)
    {
        StartCoroutine(CooldownRoutine(type));
    }

    IEnumerator CooldownRoutine(SpellType type)
    {
        SetCooldown(type, true);
        yield return new WaitForSeconds(cooldownDuration);
        SetCooldown(type, false);
    }

    void SetCooldown(SpellType type, bool value)
    {
        switch (type)
        {
            case SpellType.Attack: attackOnCooldown = value; break;
            case SpellType.Defense: defenseOnCooldown = value; break;
            case SpellType.Heal: healOnCooldown = value; break;
        }
    }

    public bool IsOnCooldown(SpellType type)
    {
        switch (type)
        {
            case SpellType.Attack: return attackOnCooldown;
            case SpellType.Defense: return defenseOnCooldown;
            case SpellType.Heal: return healOnCooldown;
        }
        return false;
    }
}