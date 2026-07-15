using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro 사용을 위해 필수!

public class UIManager : MonoBehaviour
{
    [Header("HP Bar UI References")]
    public HPBarUI playerHPBar;
    public HPBarUI targetHPBar;
    public CharacterStats playerStats;
    public CharacterStats targetStats;

    [Header("Spell Icons (기본 아이콘)")]
    public Image attackIcon;
    public Image defenseIcon;
    public Image healIcon;

    [Header("Cooldown Overlays (시계방향 회전하는 검은색 반투명 Image)")]
    public Image attackOverlay;
    public Image defenseOverlay;
    public Image healOverlay;

    [Header("Cooldown Texts (남은 시간 표시용 TextMeshPro)")]
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI defenseText;
    public TextMeshProUGUI healText;

    [Header("Icon Color Settings")]
    public Color activeColor = Color.white;
    public Color inactiveColor = new Color(0.3f, 0.3f, 0.3f, 1f); // BCI Flag OFF일 때 흑백 느낌

    [Header("BCI System Reference")]
    public BCIManager bciManager;

    private void Start()
    {
        if (playerStats != null && playerHPBar != null)
        {
            playerStats.OnHPChanged.AddListener(playerHPBar.UpdateHPBar);
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
        if (bciManager == null) return;

        // 3개 마법 UI를 실시간으로 갱신
        UpdateSingleSpellUI(BCIManager.SpellType.Attack, attackIcon, attackOverlay, attackText, bciManager.attackFlag);
        UpdateSingleSpellUI(BCIManager.SpellType.Defense, defenseIcon, defenseOverlay, defenseText, bciManager.defenseFlag);
        UpdateSingleSpellUI(BCIManager.SpellType.Heal, healIcon, healOverlay, healText, bciManager.healFlag);
    }

    /// <summary>
    /// 개별 마법 아이콘의 쿨타임 이펙트와 BCI Flag 상태를 통합 계산하는 함수
    /// </summary>
    private void UpdateSingleSpellUI(BCIManager.SpellType spellType, Image icon, Image overlay, TextMeshProUGUI text, bool isFlagOn)
    {
        float remainingTime = bciManager.GetRemainingCooldown(spellType);

        // 1. [쿨타임 진행 중] 일 때
        if (remainingTime > 0f)
        {
            // 오버레이와 텍스트를 활성화
            if (overlay != null)
            {
                overlay.enabled = true;
                // 남은 시간 비율(1.0 -> 0.0)에 따라 시계방향으로 검은색 영역이 줄어듦
                overlay.fillAmount = bciManager.GetCooldownRatio(spellType);
            }

            if (text != null)
            {
                text.enabled = true;
                // 소수점 첫째 자리까지 표시 (예: "2.5", "0.8")
                text.text = remainingTime.ToString("F1");
            }

            // 쿨타임 중일 때는 아이콘 바탕색을 어둡게(흑백) 유지
            if (icon != null) icon.color = inactiveColor;
        }
        // 2. [쿨타임이 끝났을 때]
        else
        {
            // 오버레이와 텍스트를 숨김
            if (overlay != null) overlay.enabled = false;
            if (text != null) text.enabled = false;

            // BCI Flag 상태에 따라 컬러(사용 가능) 또는 흑백(조건 미달)으로 변경
            if (icon != null)
            {
                icon.color = isFlagOn ? activeColor : inactiveColor;
            }
        }
    }
}