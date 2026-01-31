using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    [SerializeField] private CharactersManager charactersManager;
    [SerializeField] private DataVaccineManager vaccineManager;

    [Space]
    [SerializeField] private WinPanel winPanel;
    [SerializeField] private LosePanel losePanel;

    public bool EnableWinLose { get; set; } = false;

    public void CheckWin()
    {
        if (!EnableWinLose || charactersManager.Characters.Length < charactersManager.MaxCharactersinGame())
            return;

        bool isWin = true;
        foreach (var character in charactersManager.Characters)
        {
            if (character.VaccinesTaken != null)
            {
                if (character.VaccinesTaken.Count < vaccineManager.vaccines.Length || character.Health != HealthCondition.Healthy)
                {
                    isWin = false;
                    break;
                }
            }
        }

        if (isWin)
            winPanel.Show();
    }

    public void CheckLose()
    {
        bool isLose = true;

        foreach (var character in charactersManager.Characters)
        {
            if (character.Health != HealthCondition.Dead)
            {
                isLose = false;
                break;
            }
        }

        if (isLose)
            losePanel.Show();
    }
}
