using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    private static Animator[] animationsInScene;
    private static List<float> prevSpeedAnim;
    [SerializeField] private CharactersManager characterManager;
    [SerializeField] private SpeedManager speedManager;

    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (!instance)
                instance = FindObjectOfType<GameManager>();

            return instance;
        }
    }

    private void Start()
    {
        animationsInScene = FindObjectsOfType<Animator>();
        prevSpeedAnim = new List<float>();

        for (int i = 0; i < animationsInScene.Length; i++)
        {
            prevSpeedAnim.Add(animationsInScene[i].speed);
        }
    }

    private float previewSpeedIAs = 1f;

    private bool pause = false;
    public bool Pause
    {
        get { return pause; }
        set
        {
            if (value != pause)
            {
                pause = value;

                if (pause)
                {
                    for (int i = 0; i < animationsInScene.Length; i++)
                        animationsInScene[i].speed = 0;

                    for (int i = 0; i < characterManager.Characters.Length; i++)
                        characterManager.Characters[i].GetComponent<NavMeshAgent>().isStopped = true;

                    previewSpeedIAs = speedManager.SpeedGame;
                    speedManager.SpeedGame = 0;
                }
                else
                {
                    for (int i = 0; i < animationsInScene.Length; i++)
                        animationsInScene[i].speed = prevSpeedAnim[i];

                    for (int i = 0; i < characterManager.Characters.Length; i++)
                    {
                        if (characterManager.Characters[i].Health == HealthCondition.Healthy || characterManager.Characters[i].Health == HealthCondition.Sick)
                            characterManager.Characters[i].GetComponent<NavMeshAgent>().isStopped = false;
                    }

                    speedManager.SpeedGame = previewSpeedIAs;
                }
            }

        }
    }

    public static void PauseAllAnimations()
    {     
	for (int i = 0; i < animationsInScene.Length; i++)
		animationsInScene[i].speed = 0;
    }

    public static void PlayAllAnimations()
    {
	for (int i = 0; i < animationsInScene.Length; i++)
		animationsInScene[i].speed = prevSpeedAnim[i];
    }
}
