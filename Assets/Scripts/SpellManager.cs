using UnityEngine;

public class SpellManager : MonoBehaviour
{
    public BCIManager bci;
    public CharacterStat playerStats;
    public CharacterStat targetStats; // 허수아비
    public SceneController sceneController;

    public int attackDamage = 30;
    public int healAmount = 30; // 고정 회복량

    public void CastAttack()
    {
        targetStats.TakeDamage(attackDamage);
        bci.StartCooldown(BCIManager.SpellType.Attack);
        sceneController.OnSpellUsed("Attack");
    }

    public void CastDefense()
    {
        playerStats.SetInvulnerable(true);
        Invoke(nameof(EndDefense), 1f); // 1초 무적
        bci.StartCooldown(BCIManager.SpellType.Defense);
        sceneController.OnSpellUsed("Defense");
    }

    void EndDefense()
    {
        playerStats.SetInvulnerable(false);
    }

    public void CastHeal()
    {
        playerStats.Heal(healAmount); // 즉시 30 회복 (지속 회복 원하면 StartHealOverTime(30, 2f) 사용)
        bci.StartCooldown(BCIManager.SpellType.Heal);
        sceneController.OnSpellUsed("Heal");
    }
}