using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static TimeEvent;

[RequireComponent(typeof(TimeEvent))]
public class ClockBehaviour : MonoBehaviour
{
    [SerializeField] private float numberCallClock = 0.1f;
    [SerializeField] private Light lightWorld = null;
    [SerializeField] private RectTransform pointer = null;
    [SerializeField] private TextMeshProUGUI textDay = null;
    [SerializeField] private SettingData setting = null;
    [SerializeField] private SpeedManager speedManager = null;
    [SerializeField] private GameManager gameManager = null;

    public static bool PauseEvent = false;

    private TimeEvent timeEvent = null;

    private int totalDayTime = 5000;
    private float degreesPerCall = 0;
    private Vector3 clockRotationEuler = Vector3.zero;
    private Vector3 lightRotationEuler = Vector3.zero;

    private int currentDay;
    public int CurrentDay
	{
        get
        {
            return currentDay;
        }
        private set
        {
            currentDay = value;
            textDay.text = currentDay.ToString();
        }
    }

    public int TotalDaySec { get { return totalDayTime; } }
    public int CurrentSec { get; private set; }


    private void Awake()
    {
        timeEvent = GetComponent<TimeEvent>();

        lightRotationEuler = lightWorld.transform.rotation.eulerAngles;
        totalDayTime = setting.totalSecundsInDay;
    }

    private void Start()
    {
        CurrentDay = 1;
        CurrentSec = totalDayTime / 2;
        clockRotationEuler = pointer.localEulerAngles;

        float numbCall360 = totalDayTime / numberCallClock;
        degreesPerCall = 360f / numbCall360;
		
		StartCoroutine(DayWork());
		StartCoroutine(CountTime());
    }
	
	private IEnumerator DayWork()
	{
		while(true)
		{
			// Aguardar até que o jogo não esteja pausado (evita divisão por zero)
			while (gameManager.Pause || PauseEvent || speedManager.SpeedGame <= 0f)
			{
				yield return null;
			}

			clockRotationEuler -= Vector3.forward * degreesPerCall;
			pointer.localRotation = Quaternion.Euler(clockRotationEuler);

			lightRotationEuler += Vector3.right * (degreesPerCall);
			lightWorld.transform.rotation = Quaternion.Euler(lightRotationEuler);

			float timeCall = numberCallClock / speedManager.SpeedGame;
			yield return new WaitForSeconds(timeCall);
		}
	}
	
	private IEnumerator CountTime()
	{
		while(true)
		{
			// Aguardar até que o jogo não esteja pausado (evita divisão por zero)
			while (gameManager.Pause || PauseEvent || speedManager.SpeedGame <= 0f)
			{
				yield return null;
			}

			CurrentSec++;

			if(timeEvent.actionPerSecond != null)
				timeEvent.actionPerSecond();

			if (CurrentSec >= totalDayTime)
			{
				CurrentSec = 0;
				CurrentDay++;

				CharacterStatus.NumberAventsSicknessInday = 0;

				if(timeEvent.actionEndDay != null)
					timeEvent.actionEndDay();

				// Limpar eventos de dias passados para evitar memory leak
				timeEvent.CleanupPastDays(CurrentDay);
			}

			ActionClockDelegate action;
			if (timeEvent.actionInSecEveryDay.TryGetValue(CurrentSec, out action) && action != null)
				action();

			Dictionary<int, ActionClockDelegate> secDay;
			if (timeEvent.actionInDaySecundSpecific.TryGetValue(CurrentDay, out secDay) && secDay.TryGetValue(CurrentSec, out action))
			{
				action();
				// Limpar ação executada
				timeEvent.ClearDaySecondActions(CurrentDay, CurrentSec);
			}

			float timeCall = 1f / speedManager.SpeedGame;
			yield return new WaitForSeconds(timeCall);
		}
	}
}
