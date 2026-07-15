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

    [Header("Defense Animation")]
    public GameObject defenseEffect;

    [Header("Heal Animation")]
    public GameObject healEffect;

    public void CastAttack()
    {
        StartCoroutine(FireballAttack());

        bci.StartCooldown(BCIManager.SpellType.Attack);
        sceneController.OnSpellUsed("Attack");
    }

    private IEnumerator FireballAttack()
    {
        // Fireball 시작 위치: 플레이어의 오른쪽 위
        Vector3 startPosition = playerStats.transform.position
            + new Vector3(0.4f, 1.5f, 0f);

        // Fireball 도착 위치: 적의 조금 아래쪽
        Vector3 targetPosition = targetStats.transform.position
            + new Vector3(0f, -0.5f, 0f);

        fireball.transform.position = startPosition;
        fireball.SetActive(true);

        float elapsedTime = 0f;
        float duration = 0.3f; // 작을수록 빠르게 날아감
        float arcHeight = 1.2f; // 곡선 높이

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float t = elapsedTime / duration;

            // 시작 위치에서 목표 위치로 이동
            Vector3 position = Vector3.Lerp(
                startPosition,
                targetPosition,
                t
            );

            // 포물선 형태로 휘어지는 효과
            position.y += Mathf.Sin(t * Mathf.PI) * arcHeight;

            fireball.transform.position = position;

            yield return null;
        }

        // 정확한 도착 위치 설정
        fireball.transform.position = targetPosition;

        // Fireball 도착 시 적에게 데미지
        targetStats.TakeDamage(attackDamage);

        // Fireball 숨기기
        fireball.SetActive(false);
    }

    public void CastDefense()
    {
        // 플레이어 무적 상태 시작
        playerStats.SetInvulnerable(true);

        // 방어 애니메이션 활성화
        if (defenseEffect != null)
        {
            defenseEffect.SetActive(true);
        }

        Invoke(nameof(EndDefense), 1f); // 1초 무적

        bci.StartCooldown(BCIManager.SpellType.Defense);
        sceneController.OnSpellUsed("Defense");
    }

    void EndDefense()
    {
        // 플레이어 무적 상태 종료
        playerStats.SetInvulnerable(false);

        // 방어 애니메이션 비활성화
        if (defenseEffect != null)
        {
            defenseEffect.SetActive(false);
        }
    }

    public void CastHeal()
    {
        playerStats.StartHealOverTime(healAmount, healDuration);

        if (healEffect != null)
        {
            healEffect.SetActive(true);
        }

        Invoke(nameof(EndHealEffect), healDuration);

        bci.StartCooldown(BCIManager.SpellType.Heal);
        sceneController.OnSpellUsed("Heal");
    }

    void EndHealEffect()
    {
        if (healEffect != null)
        {
            healEffect.SetActive(false);
        }
    }
}