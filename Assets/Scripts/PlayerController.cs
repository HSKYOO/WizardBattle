using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public BCIManager bci;
    public SpellManager spellManager;
    public CharacterStat stats;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && bci.attackFlag && !bci.IsOnCooldown(BCIManager.SpellType.Attack))
            spellManager.CastAttack();

        if (Input.GetKeyDown(KeyCode.W) && bci.defenseFlag && !bci.IsOnCooldown(BCIManager.SpellType.Defense))
            spellManager.CastDefense();

        if (Input.GetKeyDown(KeyCode.E) && bci.healFlag && !bci.IsOnCooldown(BCIManager.SpellType.Heal))
            spellManager.CastHeal();
    }
}