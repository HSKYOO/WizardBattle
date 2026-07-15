using UnityEngine;
using System.Collections;

public class SpellManager : MonoBehaviour
{
    public BCIManager bci;
    public CharacterStats playerStats;
    public CharacterStats targetStats; // 허수아비
    public SceneController sceneController;

    public int attackDamage = 30;
    public int healAmount = 40; // 고정 회복량
    public float healDuration = 2.0f; // 2초간 지속 회복

    [Header("Attack Animation")]
    public GameObject fireball;
    public float fireballSpeed = 5f;

    public void CastAttack()
    {
        StartCoroutine(FireballAttack());

        bci.StartCooldown(BCIManager.SpellType.Attack);
        sceneController.OnSpellUsed("Attack");
    }

    private IEnumerator FireballAttack()
    {
        // Fireball을 플레이어 위치에서 시작
        fireball.transform.position = playerStats.transform.position;
        fireball.SetActive(true);

        // 적에게 이동
        while (Vector3.Distance(
                   fireball.transform.position,
                   targetStats.transform.position) > 0.1f)
        {
            fireball.transform.position = Vector3.MoveTowards(
                fireball.transform.position,
                targetStats.transform.position,
                fireballSpeed * Time.deltaTime
            );

            yield return null;
        }

        // 적에게 도착한 순간 데미지
        targetStats.TakeDamage(attackDamage);

        // Fireball 숨기기
        fireball.SetActive(false);
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