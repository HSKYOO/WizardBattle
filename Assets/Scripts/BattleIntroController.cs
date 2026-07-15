using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BattleIntroController : MonoBehaviour
{
    [Header("Fade")]
    public Image fadeOverlay;
    public float fadeDuration = 1f;

    [Header("Player")]
    public Transform player;
    public Vector3 playerStartPos;
    public Vector3 playerBattlePos;
    public float playerMoveDuration = 0.8f;

    [Header("Boss")]
    public Transform boss;
    public Vector3 bossStartPos;
    public Vector3 bossBattlePos;
    public float bossMoveDuration = 0.4f;

    [Header("Fight Text")]
    public GameObject fightTextPanel;
    public float fightTextDuration = 1f;

    [Header("Battle Systems")]
    public MonoBehaviour[] battleScripts;

    void Start()
    {
        player.position = playerStartPos;
        boss.position = bossStartPos;
        fightTextPanel.SetActive(false);
        SetBattleScriptsEnabled(false);

        StartCoroutine(PlayIntroSequence());
    }

    IEnumerator PlayIntroSequence()
    {
        yield return StartCoroutine(FadeOut());
        yield return StartCoroutine(MoveTo(player, playerStartPos, playerBattlePos, playerMoveDuration));
        yield return StartCoroutine(MoveTo(boss, bossStartPos, bossBattlePos, bossMoveDuration));

        fightTextPanel.SetActive(true);
        yield return new WaitForSeconds(fightTextDuration);
        fightTextPanel.SetActive(false);

        SetBattleScriptsEnabled(true);
    }

    IEnumerator FadeOut()
    {
        float timer = 0f;
        Color c = fadeOverlay.color;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            fadeOverlay.color = new Color(c.r, c.g, c.b, alpha);
            yield return null;
        }
        fadeOverlay.gameObject.SetActive(false);
    }

    IEnumerator MoveTo(Transform target, Vector3 from, Vector3 to, float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            t = 1 - (1 - t) * (1 - t);
            target.position = Vector3.Lerp(from, to, t);
            yield return null;
        }
        target.position = to;
    }

    void SetBattleScriptsEnabled(bool state)
    {
        foreach (var script in battleScripts)
        {
            script.enabled = state;
        }
    }
}