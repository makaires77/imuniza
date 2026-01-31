using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataSickness", menuName = "Data/Sickness")]
public class DataSickness : ScriptableObject
{
    public new string name;
    public Sprite icon;
    public int transmissibility;
    public int lethality;

    public DataSymptoms[] symptoms;

    public bool Transmission(CharacterStatus victim, int currentDay)
    {

        int percentTransmission = transmissibility - victim.ImuneSystem;

        if (percentTransmission <= 0 || CharacterStatus.NumberAventsSicknessInday >= currentDay)
            return false;

        if (victim.VaccinesTaken != null)
        {
            foreach (var vaccine in victim.VaccinesTaken)
            {
                for (int i = 0; i < vaccine.prevents.Length; i++)
                {
                    if (vaccine.prevents[i] == this)
                        return false;
                }
            }
        }

        if (Random.Range(0, 100) <= percentTransmission)
        {
            victim.ChangeHealthConditionSick(this);
            CharacterStatus.NumberAventsSicknessInday++;

            return true;
        }

        return false;
    }
}