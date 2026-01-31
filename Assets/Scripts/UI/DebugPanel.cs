using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DebugPanel : MonoBehaviour
{
    [SerializeField] private Button buttonOpenPanel = null;
    [Space]
    [SerializeField] private InputField fieldTimingDays = null;
    [SerializeField] private Text numberEventInDay = null;
    [Space]
    [SerializeField] private Toggle linePathIMLOnOff = null;
    [SerializeField] private NavigationDebugger navigationDebugger = null;
    [Space]
    [SerializeField] private SettingData setting = null;

    private Animator anim;
    private bool isOpen = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        fieldTimingDays.text = setting.totalSecundsInDay.ToString();

        buttonOpenPanel.onClick.AddListener(() =>
        {
            if (isOpen)
            {
                anim.SetTrigger("Hide");
                isOpen = false;
            }
            else
            {
                anim.SetTrigger("Show");
                isOpen = true;
            }
        });

        fieldTimingDays.onEndEdit.AddListener(UpdateNumberSecInDay);

        linePathIMLOnOff.onValueChanged.AddListener(UpdateOnOffDebugLineIML);
    }

    private void UpdateNumberSecInDay(string sec)
    {
        setting.totalSecundsInDay = int.Parse(sec);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void UpdateNumberEventInDay(int countEvents)
    {
        numberEventInDay.text = countEvents.ToString();
    }

    private void UpdateOnOffDebugLineIML(bool input)
    {
        navigationDebugger.ActiveLineDebug = input;
    }
}
