using UnityEngine;
using UnityEngine.UI;

public class SpeedManager : MonoBehaviour
{
	[SerializeField] private Button pauseButton = null;
	[SerializeField] private Button playButton = null;
	[SerializeField] private Button twoTimesButton = null;
	[SerializeField] private Button halfButton = null;
	[Space]
	[SerializeField] private CharactersManager characterManager = null;
    [SerializeField] private CarManager carManager = null;
    [SerializeField] private GameManager gameManager = null;

	private static float speedGame = 1f;
	public float SpeedGame {
		get
		{
			return speedGame;
		}
		set
		{
			speedGame = value;
			
            if(characterManager.IAs != null)
			    for(int i = 0; i < characterManager.IAs.Length; i++)
				    characterManager.IAs[i].SpeedUpdate(speedGame);
		}
	}
	
	private void Awake()
	{
		characterManager = FindObjectOfType<CharactersManager>();
	}
	
	private void Start()
	{
		pauseButton.onClick.AddListener(() => SpeedTimesPauseButton());
		playButton.onClick.AddListener(() => SpeedTimesPlayButton());
		twoTimesButton.onClick.AddListener(() => SpeedTimesTwoButton());
		halfButton.onClick.AddListener(() => SpeedTimesHalfButton());

		SpeedTimesPlayButton();
	}
	
	public void SpeedTimesTwoButton()
	{
		GameManager.PlayAllAnimations();
		ClockBehaviour.PauseEvent = false;
		gameManager.Pause = false;

		SpeedGame = 2f;

		twoTimesButton.interactable = false;
		halfButton.interactable = true;
		playButton.interactable = true;
		pauseButton.interactable = true;

		// REMOVIDO: Time.timeScale = 1; (conflita com coroutines que usam WaitForSeconds customizado)
	}

	public void SpeedTimesPauseButton()
	{
		GameManager.PauseAllAnimations();
		ClockBehaviour.PauseEvent = true;
		gameManager.Pause = true;
		SpeedGame = 0f;

		halfButton.interactable = true;
		twoTimesButton.interactable = true;
		playButton.interactable = true;

		pauseButton.interactable = false;

		// REMOVIDO: Time.timeScale = 0; (conflita com coroutines que usam WaitForSeconds customizado)
	}

	public void SpeedTimesPlayButton()
	{
		GameManager.PlayAllAnimations();
		ClockBehaviour.PauseEvent = false;
		gameManager.Pause = false;
		SpeedGame = 1f;

		halfButton.interactable = true;
		twoTimesButton.interactable = true;
		pauseButton.interactable = true;

		playButton.interactable = false;

		// REMOVIDO: Time.timeScale = 1; (conflita com coroutines que usam WaitForSeconds customizado)
	}

	public void SpeedTimesHalfButton()
	{
		GameManager.PlayAllAnimations();
		ClockBehaviour.PauseEvent = false;
		gameManager.Pause = false;
		SpeedGame = 0.5f;

		halfButton.interactable = false;
		twoTimesButton.interactable = true;
		playButton.interactable = true;
		pauseButton.interactable = true;

		// REMOVIDO: Time.timeScale = 1; (conflita com coroutines que usam WaitForSeconds customizado)
	}
}
