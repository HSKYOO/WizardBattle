using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("HP Bar UI References (팀원이 만든 부품 연결)")]
    [Tooltip("플레이어의 HP 바에 붙어있는 HPBarUI 컴포넌트")]
    public HPBarUI playerHPBar;
    
    [Tooltip("허수아비(또는 적)의 HP 바에 붙어있는 HPBarUI 컴포넌트")]
    public HPBarUI targetHPBar;

    [Header("Character Stats References")]
    public CharacterStats playerStats;
    public CharacterStats targetStats;

    [Header("Spell Icons (기획서: Flag ON/OFF 컬러 전환용)")]
    [Tooltip("하단 공격 마법 UI 이미지 [Q]")]
    public Image attackIcon;
    [Tooltip("하단 방어 마법 UI 이미지 [W]")]
    public Image defenseIcon;
    [Tooltip("하단 회복 마법 UI 이미지 [E]")]
    public Image healIcon;

    [Header("Icon Color Settings")]
    [Tooltip("Flag가 ON일 때 아이콘 색상 (기본: 원래 색상 100% 밝기)")]
    public Color activeColor = Color.white;
    
    [Tooltip("Flag가 OFF일 때 아이콘 색상 (기본: 어둡고 칙칙한 흑백/그레이 느낌)")]
    public Color inactiveColor = new Color(0.3f, 0.3f, 0.3f, 1f);

    [Header("BCI System Reference")]
    public BCIManager bciManager;

    private void Start()
    {
        // 1. 플레이어와 타겟(적)의 체력 변경 이벤트에 팀원이 만든 HPBarUI의 업데이트 메서드를 연결!
        if (playerStats != null && playerHPBar != null)
        {
            playerStats.OnHPChanged.AddListener(playerHPBar.UpdateHPBar);
            // 시작할 때 현재 체력으로 HP 바 초기화
            playerHPBar.UpdateHPBar(playerStats.CurrentHP, playerStats.maxHP);
        }

        if (targetStats != null && targetHPBar != null)
        {
            targetStats.OnHPChanged.AddListener(targetHPBar.UpdateHPBar);
            targetHPBar.UpdateHPBar(targetStats.CurrentHP, targetStats.maxHP);
        }
    }

    private void Update()
    {
        // 2. 매 프레임마다 BCI Flag 상태를 확인하여 마법 아이콘 색상(컬러/흑백)을 실시간으로 변경 
        UpdateSpellIconsVisual();
    }

    /// <summary>
    /// BCI Flag에 따라 마법 아이콘을 컬러(ON) 또는 흑백/어둡게(OFF) 처리하는 메서드 
    /// </summary>
    private void UpdateSpellIconsVisual()
    {
        if (bciManager == null) return;

        // 공격 아이콘 컬러/흑백 전환 
        if (attackIcon != null)
        {
            attackIcon.color = bciManager.attackFlag ? activeColor : inactiveColor;
        }

        // 방어 아이콘 컬러/흑백 전환
        if (defenseIcon != null)
        {
            defenseIcon.color = bciManager.defenseFlag ? activeColor : inactiveColor;
        }

        // 회복 아이콘 컬러/흑백 전환
        if (healIcon != null)
        {
            healIcon.color = bciManager.healFlag ? activeColor : inactiveColor;
        }
    }
}