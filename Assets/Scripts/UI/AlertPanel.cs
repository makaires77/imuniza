using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertPanel : MonoBehaviour
{
    [SerializeField] private AlertAffectedButton prefFocusInfectionAlertButton = null;
    [SerializeField] private AlertAffectedButton prefInterpersonalInfectionAlertButton = null;
    [SerializeField] private AlertAffectedButton prefDeadInfectionAlertButton = null;

    [SerializeField] private AlertGoodButton prefDiagnosisAlertButton = null;
    [SerializeField] private AlertGoodButton prefHospitalAlertButton = null;
    [SerializeField] private AlertGoodButton prefHelthCenterAlertButton = null;

    public void SpawnAlertFocus(CharacterStatus affectedTarget)
    {

        AlertAffectedButton alert = Instantiate(prefFocusInfectionAlertButton, transform);
        alert.AffectedTarget = affectedTarget;
        alert.UpdateInfo();

        alert.gameObject.SetActive(true);
    }

    public void SpawnAlertInterpersonalInfection(CharacterStatus affectedTarget, CharacterStatus transmitterCharacter)
    {
        AlertAffectedButton alert = Instantiate(prefInterpersonalInfectionAlertButton, transform);
        alert.AffectedTarget = affectedTarget;
        alert.TransmitterCharacter = transmitterCharacter;

        alert.gameObject.SetActive(true);
    }

    public void SpawnAlertDead(CharacterStatus affectedTarget)
    {
        AlertAffectedButton alert = Instantiate(prefDeadInfectionAlertButton, transform);
        alert.AffectedTarget = affectedTarget;

        alert.gameObject.SetActive(true);
    }

    public void SpawnAlertDiagnosis(Transform target)
    {
        AlertGoodButton alert = Instantiate(prefDiagnosisAlertButton, transform);
        alert.AffectedTarget = target.GetComponent<CharacterStatus>();

        alert.gameObject.SetActive(true);
    }

    public void SpawnAlertHospital(Transform target)
    {
        AlertGoodButton alert = Instantiate(prefHospitalAlertButton, transform);
        alert.AffectedTarget = target.GetComponent<CharacterStatus>();

        alert.gameObject.SetActive(true);
    }
    public void SpawnAlertHelthCenter(Transform target, DataVaccine vaccine)
    {
        AlertGoodButton alert = Instantiate(prefHelthCenterAlertButton, transform);
        alert.AffectedTarget = target.GetComponent<CharacterStatus>();
        alert.VaccineTake = vaccine;

        alert.gameObject.SetActive(true);
    }
}
