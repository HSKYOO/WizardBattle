using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public BCIManager bci;
    public SpellManager spellManager;
    public CharacterStats stats;

    [Header("Debug / Testing")]
    [Tooltip("체크하면 BCI Flag가 꺼져 있어도 키보드(Q,W,E)만으로 마법을 쓸 수 있습니다! (개발/테스트용)")]
    public bool ignoreBCIFlags = false;

    private void Awake()
    {
        // Inspector에서 연결을 깜빡했더라도 자기 자신에게 있는 컴포넌트라면 자동으로 찾아옵니다.
        if (spellManager == null) spellManager = GetComponent<SpellManager>();
        if (stats == null) stats = GetComponent<CharacterStats>();
    }

    void Update()
    {
        // 1. 필수 컴포넌트가 없거나, 플레이어가 이미 사망한 상태라면 마법 사용 불가!
        if (spellManager == null || bci == null) return;
        if (stats != null && stats.CurrentHP <= 0) return;

        // 2. 디버그 모드가 켜져있다면 Flag를 무시하고 true로 취급, 아니라면 BCI Flag 상태 확인
        bool canAttack = ignoreBCIFlags || bci.attackFlag;
        bool canDefense = ignoreBCIFlags || bci.defenseFlag;
        bool canHeal = ignoreBCIFlags || bci.healFlag;

        // 3. 공격 마법 (Q 키)
        if (Input.GetKeyDown(KeyCode.Q) && canAttack && !bci.IsOnCooldown(BCIManager.SpellType.Attack))
        {
            spellManager.CastAttack();
        }

        // 4. 방어 마법 (W 키)
        if (Input.GetKeyDown(KeyCode.W) && canDefense && !bci.IsOnCooldown(BCIManager.SpellType.Defense))
        {
            spellManager.CastDefense();
        }

        // 5. 회복 마법 (E 키)
        if (Input.GetKeyDown(KeyCode.E) && canHeal && !bci.IsOnCooldown(BCIManager.SpellType.Heal))
        {
            spellManager.CastHeal();
        }
    }
}