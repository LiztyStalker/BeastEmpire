using System.Collections.Generic;
using UnityEngine;

public class MockGameCommander
{
    public CommanderActor commanderActor = CommanderActor.Create();
    public int costValue = 500;
    public int ironValue = 10;
    public void SetCommanderCard(CommanderCard commanderCard) => commanderActor.SetCommanderCard(commanderCard);
    public void AddCard(UnitCard uCard)
    {
        costValue -= uCard.AppearCostValue;
        commanderActor.AddCard(uCard);
    }
    public void RemoveCard(UnitCard uCard)
    {
        costValue += uCard.AppearCostValue;
        commanderActor.RemoveCard(uCard);
    }
    public bool IsEnoughLeadership(UnitCard uCard)
    {
        return commanderActor.IsEnoughLeadership(uCard);
    }

    public bool IsEnoughCost(UnitCard uCard)
    {
        return (costValue - uCard.AppearCostValue >= 0);
    }

    public UnitCard[] GetUnitCards() => commanderActor.unitDataArray;

    public bool IsEmptyUnitDataArray()
    {
        return commanderActor.IsEmptyUnitDataArray();
    }
    
    public int nowLeadershipValue => commanderActor.nowLeadershipValue;
    public int maxLeadershipValue => commanderActor.maxLeadershipValue;
}

public class MockGameOutpost
{
    public static MockGameOutpost instance = null;

    private System.Action _refreshEvent;

    public void SetOnRefreshCommanderData(System.Action act) => _refreshEvent = act;

    public MockGameCommander commander_L = new MockGameCommander();
    public MockGameCommander commander_R = new MockGameCommander();

    public BattleFieldData battleFieldData;

    public static void InitializeMockGameOutpost()
    {
        instance = new MockGameOutpost();
    }

    public static void Dispose()
    {
        instance = null;
    }

    public void SetCommanderCard(CommanderCard commanderCard, TYPE_TEAM typeTeam)
    {

        if (typeTeam == TYPE_TEAM.Left)
            commander_L.SetCommanderCard(commanderCard);
        else
            commander_R.SetCommanderCard(commanderCard);
    }
    public bool IsEnoughLeadership(UnitCard uCard, TYPE_TEAM typeTeam)
    {
        if (typeTeam == TYPE_TEAM.Left)
            return commander_L.IsEnoughLeadership(uCard);
        else
            return commander_R.IsEnoughLeadership(uCard);
    }

    public bool IsEnoughCost(UnitCard uCard, TYPE_TEAM typeTeam)
    {
        if (typeTeam == TYPE_TEAM.Left)
            return commander_L.IsEnoughCost(uCard);
        else
            return commander_R.IsEnoughCost(uCard);
    }


    public void AddCard(UnitCard uCard, TYPE_TEAM typeTeam)
    {

        if (typeTeam == TYPE_TEAM.Left)
            commander_L.AddCard(uCard);
        else
            commander_R.AddCard(uCard);
        _refreshEvent?.Invoke();

    }

    public void RemoveCard(UnitCard uCard, TYPE_TEAM typeTeam)
    {

        if (typeTeam == TYPE_TEAM.Left)
            commander_L.RemoveCard(uCard);
        else
            commander_R.RemoveCard(uCard);
        _refreshEvent?.Invoke();
    }

    public int GetCostValue(TYPE_TEAM typeTeam)
    {
        return (typeTeam == TYPE_TEAM.Left) ? commander_L.costValue : commander_R.costValue;
    }

    public int GetIronValue(TYPE_TEAM typeTeam)
    {
        return (typeTeam == TYPE_TEAM.Left) ? commander_L.ironValue : commander_R.ironValue;
    }

    public string GetLeadershipText(TYPE_TEAM typeTeam)
    {
        return (typeTeam == TYPE_TEAM.Left) ? $"{commander_L.nowLeadershipValue}/{commander_L.maxLeadershipValue}" : $"{commander_R.nowLeadershipValue}/{commander_R.maxLeadershipValue}";
    }

    public bool IsEmptyUnitDataArray(TYPE_TEAM typeTeam)
    {
        return (typeTeam == TYPE_TEAM.Left) ? commander_L.IsEmptyUnitDataArray() : commander_R.IsEmptyUnitDataArray();
    }
}

public class MockGameData
{

    private static MockGameData _instance;
    public static MockGameData instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new MockGameData();
            }
            return _instance;
        }
    }

    public List<UnitCard> totalUnits_L = new List<UnitCard>();
    public List<UnitCard> totalUnits_R = new List<UnitCard>();

    public void InitializeUnits()
    {
        totalUnits_L.AddRange(UnitCard.Create(DataStorage.Instance.GetRandomDatasOrZero<UnitData>(100)));// GetRandomUnitCards(100));
        totalUnits_R.AddRange(UnitCard.Create(DataStorage.Instance.GetRandomDatasOrZero<UnitData>(100)));
    }

}



public class MockGameManager : MonoBehaviour
{
    [SerializeField]
    Transform _dragPanel;

    [SerializeField]
    UIBattleField uiBattleField;


    [SerializeField]
    UIOutpost _lOutpost;

    [SerializeField]
    UIOutpost _rOutpost;

    [SerializeField]
    UIUnitOutpostBarrack uiBarrack;

    [SerializeField]
    UIUnitInformation uiUnitInformation;

    [SerializeField]
    UnityEngine.UI.Button _startGameBtn;

    //[SerializeField]
    //UIOutpost _rOutpost;

    public Transform dragPanel => _dragPanel;

    void Start()
    {
        MockGameOutpost.InitializeMockGameOutpost();
        


        _lOutpost.Initialize();
        _lOutpost.SetOnUnitListener(() => 
            {
            if (uiBarrack.isActiveAndEnabled)
                uiBarrack.Hide();
            else
                uiBarrack.Show(TYPE_TEAM.Left);
            }
        );

        _lOutpost.SetOnUnitInformationListener(unitData => {
            uiUnitInformation.ShowData(unitData, Input.mousePosition);
            uiUnitInformation.SetPosition(Input.mousePosition);
            //uiUnitInformation.transform.position = Input.mousePosition;
        });



        _rOutpost.Initialize();
        _rOutpost.SetOnUnitListener(() =>
        {
            if (uiBarrack.isActiveAndEnabled)
                uiBarrack.Hide();
            else
                uiBarrack.Show(TYPE_TEAM.Right);
        }
        );

        _rOutpost.SetOnUnitInformationListener(uCard => {
            uiUnitInformation.ShowData(uCard, Input.mousePosition);
            uiUnitInformation.SetPosition(Input.mousePosition);
            //uiUnitInformation.transform.position = Input.mousePosition;
        });





        uiBarrack.SetOnUnitInformationListener(uCard => {
            uiUnitInformation.ShowData(uCard, Input.mousePosition);
            uiUnitInformation.SetPosition(Input.mousePosition);
//            uiUnitInformation.transform.position = Input.mousePosition;
        });

        uiBarrack.SetOnUnitInformationCloseListener(uiUnitInformation.Hide);



        uiBattleField.Initialize();


        MockGameData.instance.InitializeUnits();

        uiUnitInformation.Initialize();

        uiBarrack.Hide();

        _startGameBtn.onClick.AddListener(StartGame);
    }



    public void StartGame()
    {
        if (MockGameOutpost.instance.IsEmptyUnitDataArray(TYPE_TEAM.Left))
        {
            Debug.Log("CommanderL Empty");
            return;
        }
        if (MockGameOutpost.instance.IsEmptyUnitDataArray(TYPE_TEAM.Right))
        {
            Debug.Log("CommanderR Empty");
            return;
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene("Test_BattleField");
    }

}
