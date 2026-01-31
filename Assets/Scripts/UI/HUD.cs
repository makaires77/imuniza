using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] private Text healthyDisplay = null;
    [SerializeField] private Text moneyHealthyDisplay = null;
    [Space]
    [SerializeField] private Text sickDisplay = null;
    [SerializeField] private Text deadDisplay = null;
    [Space]
    [SerializeField] private CharactersManager characters = null;
    [SerializeField] private CameraBehaviour cameraBehavior = null;
    [SerializeField] private GameOver gameOver = null;

    private ActionPanel actionpanel = null;

    private int indexHealthyCharacter = -1;
    private int indexSickCharacter = -1;
    private int indexDeadCharacter = -1;

    private void Awake()
    {
        actionpanel = GetComponent<ActionPanel>();
    }

    public void UpdateHealthy()
    {
        int countCharacters = characters.CharactersHealthy.Length;
        UpdateNumberDisplay(healthyDisplay, countCharacters);

        UpdateMoneyAgainWithHealthy(countCharacters);
        gameOver.CheckWin();
    }

    public void UpdateSick()
    {
        UpdateNumberDisplay(sickDisplay, characters.CharactersSick.Length);
    }

    public void UpdateDead()
    {
        UpdateNumberDisplay(deadDisplay, characters.CharactersDead.Length);
        gameOver.CheckLose();
    }

    public void DisplayGainMoneyHealthy()
    {

    }

    public void FindHealthyButton()
    {
        CharacterStatus[] characterHealthy = characters.CharactersHealthy;
        if (characterHealthy != null && characterHealthy.Length > 0)
        {
            ++indexHealthyCharacter;
            if (indexHealthyCharacter >= characterHealthy.Length)
                indexHealthyCharacter = 0;

            cameraBehavior.SetTarget(characterHealthy[indexHealthyCharacter].transform);
            actionpanel.Show(characterHealthy[indexHealthyCharacter].transform, false);
        }
    }

    public void FindSickButton()
    {
        CharacterStatus[] characterSick = characters.CharactersSick;
        if (characterSick != null && characterSick.Length > 0)
        {
            ++indexSickCharacter;
            if (indexSickCharacter >= characterSick.Length)
                indexSickCharacter = 0;

            cameraBehavior.SetTarget(characterSick[indexSickCharacter].transform);
            actionpanel.Show(characterSick[indexSickCharacter].transform, false);
        }
    }

    public void  FindDeadButton()
    {
        CharacterStatus[] characterDead = characters.CharactersDead;
        if (characterDead != null && characterDead.Length > 0)
        {
            ++indexDeadCharacter;
            if (indexDeadCharacter >= characterDead.Length)
                indexDeadCharacter = 0;

            cameraBehavior.SetTarget(characterDead[indexDeadCharacter].transform);
            actionpanel.Show(characterDead[indexDeadCharacter].transform, false);
        }
    }

    private void UpdateNumberDisplay(Text displayText, int count)
    {
        displayText.text = count.ToString();
    }

    private void UpdateMoneyAgainWithHealthy(int numberHealthyCharacters)
    {
        moneyHealthyDisplay.text = "+ $" + MoneyManager.UpdateAgainWithHealthy(numberHealthyCharacters).ToString();
    }
}
