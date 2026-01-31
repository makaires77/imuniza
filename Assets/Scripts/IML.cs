using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IML : MonoBehaviour
{
    [SerializeField] private NavMeshAgent ambulance = null;
    [SerializeField] private Transform IMLPoint = null;
    private StateAmbulance stateAmbulance;

    public List<CharacterStatus> Characters = new List<CharacterStatus>();
    public int CurrentIndexCharacter = -1;

    private void Start()
    {
        ambulance.isStopped = true;
        stateAmbulance = StateAmbulance.None;
    }


    public void CallCollect(Transform characterDead)
    {
        Characters.Add(characterDead.GetComponent<CharacterStatus>());

        if (stateAmbulance == StateAmbulance.None)
            DriveNextCharacter();

        characterDead.GetComponent<CharacterStatus>().IsCallIML = true;

        int numQueue = 0;
        for (int i = CurrentIndexCharacter; i < Characters.Count; i++)
        {
            numQueue++;
            Characters[i].GetComponentInChildren<StatusBehaviour>().SetIML(numQueue);
        }
    }

    private void Update()
    {
        if (!ambulance.isStopped && stateAmbulance != StateAmbulance.None && !ambulance.pathPending) {

            if (stateAmbulance == StateAmbulance.DriveCharacter && ambulance.remainingDistance < 3)
            {
                TakePatientIML();
            }
            else if (stateAmbulance == StateAmbulance.DriveIML && ambulance.remainingDistance < 1)
            {
                if(CurrentIndexCharacter < Characters.Count - 1)
                    DriveNextCharacter();
            }
        }
    }

    private void TakePatientIML()
    {
        IA character = Characters[CurrentIndexCharacter].GetComponentInParent<IA>();
        character.gameObject.SetActive(false);

        stateAmbulance = StateAmbulance.DriveIML;
        ambulance.SetDestination(IMLPoint.position);
    }

    private void DriveNextCharacter()
    {
        ++CurrentIndexCharacter;
        ambulance.isStopped = false;
        ambulance.SetDestination(Characters[CurrentIndexCharacter].transform.position);
        stateAmbulance = StateAmbulance.DriveCharacter;

        int numQueue = 0;
        for (int i = CurrentIndexCharacter; i < Characters.Count; i++)
        {
            numQueue++;
            Characters[i].GetComponentInChildren<StatusBehaviour>().SetIML(numQueue);
        }
    }

    public enum StateAmbulance
    {
        DriveIML,
        DriveCharacter,
        None
    }
}
