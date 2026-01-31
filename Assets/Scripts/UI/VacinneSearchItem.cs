using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VacinneSearchItem : MonoBehaviour
{
    [SerializeField] private Image icon = null;
    [SerializeField] private Image iconMake = null;
    [SerializeField] private Sprite imgVaccineIcon = null;
    [SerializeField] private Text nameSickness = null;
    [SerializeField] private Text money = null;
    [SerializeField] private Image loadImg = null;
    [Space]
    [SerializeField] private GameObject panelNumVaccine = null;
    [SerializeField] private TextMeshProUGUI numVaccinesBuy = null;
    [Space]
    [SerializeField] private LaboratorioPanel laboratory = null;
    [SerializeField] private PopUp popUp = null;

    private Button button;
    private TimeEvent timeEvent;
    private ClockBehaviour clock;
    public DataVaccine vaccineData;

    private int cost = 50;
    private int numVaccines = 0;

    private TimeEvent.ActionPerSecond updateSearchDelegate;

    private void Awake()
    {
        button = GetComponentInChildren<Button>();
        timeEvent = FindObjectOfType<TimeEvent>();
        clock = FindObjectOfType<ClockBehaviour>();
    }

    private void Start()
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(SpawnPopUpBuySearch);

        updateSearchDelegate += () => LoadAsyc();
    }

    public int GetNumVaccinesAvailable()
    {
        return numVaccines;
    }

    public void SetData(DataVaccine vaccine)
    {
        vaccineData = vaccine;

        icon.sprite = vaccineData.icon;
        nameSickness.text = vaccineData.name;
    }

    int totalLoad;
    float curSec;
    private void SpawnPopUpBuySearch()
    {
        popUp.ShowQuestionYesNo("Deseja desenvolver a vacina de " + vaccineData.name + " por $" + cost + "? Tempo: 2 dias", () => 
        {
            if (cost > MoneyManager.CurrentMoney)
                return;

            MoneyManager.CurrentMoney -= cost;
            int daySearch = 2;
            totalLoad = clock.TotalDaySec * daySearch;
            curSec = totalLoad;

            timeEvent.AddActionInSpecificSecAndDay(FinishSearchVaccine, clock.CurrentSec, clock.CurrentDay + daySearch);
            timeEvent.AddActionPerSec(ref updateSearchDelegate);

            loadImg.gameObject.SetActive(true);
            money.gameObject.SetActive(false);

            laboratory.DisableAllVaccinesForSearch();
        });
    }

    private void SpawnPopUpBuyVaccine()
    {
        popUp.ShowQuestionYesNo("Deseja adicionar mais uma vacina de " + vaccineData.name + " ao estoque por $" + cost + "? Tempo: meio dia", () =>
        {
            if (cost > MoneyManager.CurrentMoney)
                return;

            MoneyManager.CurrentMoney -= cost;
            int daySearch = 2;

            int dayMakeVaccine = clock.CurrentDay;
            int secMakeVaccine = clock.CurrentSec + (clock.TotalDaySec / 2);
            if(secMakeVaccine > clock.TotalDaySec)
            {
                dayMakeVaccine++;
                secMakeVaccine = secMakeVaccine - clock.TotalDaySec;
            }

            totalLoad = clock.TotalDaySec * daySearch;
            curSec = totalLoad;

            timeEvent.AddActionInSpecificSecAndDay(FinishMakeVaccine, secMakeVaccine, dayMakeVaccine);
            timeEvent.AddActionPerSec(ref updateSearchDelegate);

            loadImg.gameObject.SetActive(true);
            money.gameObject.SetActive(false);

            laboratory.DisableAllVaccinesForSearch();
        });
    }

    private bool FinishSearchVaccine()
    {
        Debug.Log("FINISH SEARCH VACCINE");
        iconMake.sprite = imgVaccineIcon;

        cost = 15;
        money.text = "$" + cost;

        loadImg.gameObject.SetActive(false);
        money.gameObject.SetActive(true);

        numVaccines = 1;
        numVaccinesBuy.text = numVaccines.ToString();

        panelNumVaccine.gameObject.SetActive(true);

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(SpawnPopUpBuyVaccine);

        laboratory.EnableAllVaccinesForSearch();

        return true;
    }

    private bool FinishMakeVaccine()
    {
        Debug.Log("==FINISH MAKE VACCINE==");

        loadImg.gameObject.SetActive(false);
        money.gameObject.SetActive(true);

        ++numVaccines;
        numVaccinesBuy.text = numVaccines.ToString();

        laboratory.EnableAllVaccinesForSearch();

        return true;
    }

    private void LoadAsyc()
    {
        if (curSec > 0)
        {
            loadImg.fillAmount = curSec / totalLoad;

            --curSec;
        }
        else
            timeEvent.RemoveActionPerSec(ref updateSearchDelegate);
    }
}
