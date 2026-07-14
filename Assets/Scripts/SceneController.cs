using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public GameObject nextStageBtn;
    private HashSet<string> usedSpells = new HashSet<string>();

    public void OnSpellUsed(string spellName)
    {
        usedSpells.Add(spellName);
        if (usedSpells.Count >= 3)
            nextStageBtn.SetActive(true);
    }

    public void GoToBattleScene()
    {
        SceneManager.LoadScene("BattleScene");
    }
}