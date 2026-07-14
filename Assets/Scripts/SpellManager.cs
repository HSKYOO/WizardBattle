using UnityEngine;

public class SpellManager : MonoBehaviour
{
    public BCIManager bci;
    public CharacterStat playerStats;
    public CharacterStat targetStats; // 허수아비
    public SceneController sceneController;

    public int attackDamage = 30;
    public int healAmount = 40; // 고정 회복량
    public float healDuration = 2.0f; // 2초간 지속 회복

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
        playerStats.StartHealOverTime(healAmount, healDuration);
        bci.StartCooldown(BCIManager.SpellType.Heal);
        sceneController.OnSpellUsed("Heal");
    }
}