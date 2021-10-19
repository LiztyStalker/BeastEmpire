using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class BattleFieldOutpost
{

    private static BattleFieldOutpost _current = null;

    public static BattleFieldOutpost Current
    {
        get
        {
            if (_current == null)
                Debug.LogError("InitializeBattleFieldOutpost�� ȣ���ؾ� �մϴ�");
            return _current;
        }
    }

    public RegionMockGameActor regionL = new RegionMockGameActor();
    public RegionMockGameActor regionR = new RegionMockGameActor();

    public BattleFieldData battleFieldData;


    private bool _isChallenge = false;
    private int _challengeLevel = 0;

    public bool IsChallenge() => _isChallenge;
    public void SetChallenge(bool isChallenge) => _isChallenge = isChallenge;
    public int GetChallengeLevel() => _challengeLevel;
    public void AddChallengeLevel() => _challengeLevel++;
    public void ClearChallengeLevel() => _challengeLevel = 0;

    public bool IsChallengeEnd() => _challengeLevel >= 4;

    public static void InitializeBattleFieldOutpost()
    {
        if (_current == null)
        {
            _current = new BattleFieldOutpost();
        }
    }

    public static void Dispose()
    {
        _current = null;
    }

    /// <summary>
    /// ���ְ� ī�� ����
    /// </summary>
    /// <param name="commanderCard"></param>
    /// <param name="typeTeam"></param>
    public void SetCommanderCard(CommanderCard commanderCard, TYPE_TEAM typeTeam)
    {
        if (typeTeam == TYPE_TEAM.Left)
            regionL.SetCommanderCard(commanderCard);
        else
            regionR.SetCommanderCard(commanderCard);
    }

    /// <summary>
    /// ������ ��� ����
    /// </summary>
    /// <param name="uCard"></param>
    /// <param name="typeTeam"></param>
    /// <returns></returns>
    public bool IsEnoughLeadership(UnitCard uCard, TYPE_TEAM typeTeam)
    {
        if (typeTeam == TYPE_TEAM.Left)
            return regionL.IsEnoughLeadership(uCard);
        else
            return regionR.IsEnoughLeadership(uCard);
    }

    /// <summary>
    /// ����� ��� ����
    /// </summary>
    /// <param name="uCard"></param>
    /// <param name="typeTeam"></param>
    /// <returns></returns>
    public bool IsEnoughEmployCost(UnitCard uCard, TYPE_TEAM typeTeam)
    {
        if (typeTeam == TYPE_TEAM.Left)
            return regionL.IsEnoughEmployCost(uCard);
        else
            return regionR.IsEnoughEmployCost(uCard);
    }

    /// <summary>
    /// ����ī�� �߰�
    /// </summary>
    /// <param name="uCard"></param>
    /// <param name="typeTeam"></param>
    public void AddCard(UnitCard uCard, TYPE_TEAM typeTeam)
    {

        if (typeTeam == TYPE_TEAM.Left)
            regionL.AddCard(uCard);
        else
            regionR.AddCard(uCard);
        _refreshEvent?.Invoke();

    }

    /// <summary>
    /// ����ī�� ����
    /// </summary>
    /// <param name="uCard"></param>
    /// <param name="typeTeam"></param>
    public void RemoveCard(UnitCard uCard, TYPE_TEAM typeTeam)
    {

        if (typeTeam == TYPE_TEAM.Left)
            regionL.RemoveCard(uCard);
        else
            regionR.RemoveCard(uCard);
        _refreshEvent?.Invoke();
    }

    /// <summary>
    /// ��� ���� �� ���� ȸ��
    /// </summary>
    public void AllRecovery()
    {
        regionL.AllRecovery();
        regionR.AllRecovery();
    }

    //public int GetCostValue(TYPE_TEAM typeTeam)
    //{
    //    return (typeTeam == TYPE_TEAM.Left) ? regionL.costValue : regionR.costValue;
    //}

    //public int GetIronValue(TYPE_TEAM typeTeam)
    //{
    //    return (typeTeam == TYPE_TEAM.Left) ? regionL.ironValue : regionR.ironValue;
    //}

    //public string GetLeadershipText(TYPE_TEAM typeTeam)
    //{
    //    return (typeTeam == TYPE_TEAM.Left) ? $"{regionL.nowLeadershipValue}/{regionL.maxLeadershipValue}" : $"{regionR.nowLeadershipValue}/{regionR.maxLeadershipValue}";
    //}

    public bool IsEmptyUnitDataArray(TYPE_TEAM typeTeam)
    {
        return (typeTeam == TYPE_TEAM.Left) ? regionL.IsEmptyUnitDataArray() : regionR.IsEmptyUnitDataArray();
    }

    #region ##### Listener #####

    private System.Action _refreshEvent;
    public void AddOnRefreshCommanderData(System.Action act) => _refreshEvent += act;
    public void RemoveOnRefreshCommanderData(System.Action act) => _refreshEvent -= act;
    #endregion


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
        totalUnits_L.Clear();
        totalUnits_R.Clear();

        var dataArrayL = DataStorage.Instance.GetRandomDatasOrZero<UnitData>(100);
        var dataArrayR = DataStorage.Instance.GetRandomDatasOrZero<UnitData>(100);

        dataArrayL = dataArrayL.Where(data => data.SkeletonDataAsset != null && data.Icon != null && data.Tier == 3 && data.TypeUnitClass != TYPE_UNIT_CLASS.Wizard).ToArray();
        dataArrayR = dataArrayR.Where(data => data.SkeletonDataAsset != null && data.Icon != null && data.Tier == 3 && data.TypeUnitClass != TYPE_UNIT_CLASS.Wizard).ToArray();


        var uCardsL = UnitCard.Create(dataArrayL);
        var uCardsR = UnitCard.Create(dataArrayR);

        totalUnits_L.AddRange(uCardsL);
        totalUnits_R.AddRange(uCardsR);
    }

    public void AddCard(TYPE_TEAM typeTeam, UnitCard uCard)
    {
        if(typeTeam == TYPE_TEAM.Left)
            totalUnits_L.Add(uCard);
        else
            totalUnits_R.Add(uCard);
    }

    public void RemoveCard(TYPE_TEAM typeTeam, UnitCard uCard)
    {
        if (typeTeam == TYPE_TEAM.Left)
            totalUnits_L.Remove(uCard);
        else
            totalUnits_R.Remove(uCard);
    }

    public UnitCard[] GetChallangeDataArray(int level)
    {
        var dataSpear = DataStorage.Instance.GetDataOrNull<UnitData>("SpearShield");
        var dataPike = DataStorage.Instance.GetDataOrNull<UnitData>("Pike");
        var dataSword = DataStorage.Instance.GetDataOrNull<UnitData>("SwordShield");
        var dataTwoHanded = DataStorage.Instance.GetDataOrNull<UnitData>("TwoHandedSword");
        var dataMace = DataStorage.Instance.GetDataOrNull<UnitData>("Mace");
        var dataAxe = DataStorage.Instance.GetDataOrNull<UnitData>("AxeShield");

        var dataArcher = DataStorage.Instance.GetDataOrNull<UnitData>("Archer");
        var dataCorssbow = DataStorage.Instance.GetDataOrNull<UnitData>("Crossbow");
        var dataBlowgun = DataStorage.Instance.GetDataOrNull<UnitData>("Blowgun");
        var dataSkirmisher = DataStorage.Instance.GetDataOrNull<UnitData>("Skirmisher");

        var dataAssaulter = DataStorage.Instance.GetDataOrNull<UnitData>("Assaulter");
        var dataClaw = DataStorage.Instance.GetDataOrNull<UnitData>("Claw");
        var dataShield = DataStorage.Instance.GetDataOrNull<UnitData>("Shield");
        var dataArmor = DataStorage.Instance.GetDataOrNull<UnitData>("Armor");

        List<UnitCard> list = new List<UnitCard>();

        switch (level)
        {
            case 0:
                list.Add(UnitCard.Create(dataSpear));
                list.Add(UnitCard.Create(dataSpear));
                list.Add(UnitCard.Create(dataSpear));
                list.Add(UnitCard.Create(dataSpear));
                list.Add(UnitCard.Create(dataSpear));
                list.Add(UnitCard.Create(dataArcher));
                list.Add(UnitCard.Create(dataArcher));
                list.Add(UnitCard.Create(dataArcher));
                break;
            case 1:
                list.Add(UnitCard.Create(dataAxe));
                list.Add(UnitCard.Create(dataAxe));
                list.Add(UnitCard.Create(dataAxe));
                list.Add(UnitCard.Create(dataAxe));
                list.Add(UnitCard.Create(dataAxe));
                list.Add(UnitCard.Create(dataBlowgun));
                list.Add(UnitCard.Create(dataBlowgun));
                list.Add(UnitCard.Create(dataBlowgun));
                break;
            case 2:
                list.Add(UnitCard.Create(dataSpear));
                list.Add(UnitCard.Create(dataSpear));
                list.Add(UnitCard.Create(dataSpear));
                list.Add(UnitCard.Create(dataSpear));
                list.Add(UnitCard.Create(dataArcher));
                list.Add(UnitCard.Create(dataArcher));
                list.Add(UnitCard.Create(dataArcher));
                list.Add(UnitCard.Create(dataAssaulter));
                list.Add(UnitCard.Create(dataAssaulter));

                break;
            case 3:
                list.Add(UnitCard.Create(dataAxe));
                list.Add(UnitCard.Create(dataAxe));
                list.Add(UnitCard.Create(dataAxe));
                list.Add(UnitCard.Create(dataAxe));
                list.Add(UnitCard.Create(dataBlowgun));
                list.Add(UnitCard.Create(dataBlowgun));
                list.Add(UnitCard.Create(dataBlowgun));
                list.Add(UnitCard.Create(dataShield));
                list.Add(UnitCard.Create(dataShield));
                break;
            case 4:
                list.Add(UnitCard.Create(dataSpear));
                list.Add(UnitCard.Create(dataSpear));
                list.Add(UnitCard.Create(dataSpear));
                list.Add(UnitCard.Create(dataAxe));
                list.Add(UnitCard.Create(dataAxe));
                list.Add(UnitCard.Create(dataAxe));
                list.Add(UnitCard.Create(dataArcher));
                list.Add(UnitCard.Create(dataArcher));
                list.Add(UnitCard.Create(dataBlowgun));
                list.Add(UnitCard.Create(dataBlowgun));
                list.Add(UnitCard.Create(dataAssaulter));
                list.Add(UnitCard.Create(dataAssaulter));
                list.Add(UnitCard.Create(dataClaw));
                list.Add(UnitCard.Create(dataClaw));
                list.Add(UnitCard.Create(dataShield));
                list.Add(UnitCard.Create(dataShield));
                list.Add(UnitCard.Create(dataArmor));
                list.Add(UnitCard.Create(dataArmor));
                break;
        }
        return list.ToArray();
    }
}



public class UIMockGame : MonoBehaviour
{
    [SerializeField]
    private Transform _dragPanel;

    [SerializeField]
    private UIMockBattleField _uiBattleField;

    [SerializeField]
    private UIOutpost _lOutpost;

    [SerializeField]
    private UIOutpost _rOutpost;

    [SerializeField]
    private UIUnitOutpostBarrack _uiBarrack;

    [SerializeField]
    private UnityEngine.UI.Button _startGameBtn;

    [SerializeField]
    private UnityEngine.UI.Button _backBtn;

    public Transform dragPanel => _dragPanel;

    private void Start()
    {
        MockGameData.instance.InitializeUnits();

        BattleFieldOutpost.InitializeBattleFieldOutpost();


        var data = DataStorage.Instance.GetFirstDataOrNull<CommanderData>();
        BattleFieldOutpost.Current.SetCommanderCard(CommanderCard.Create(data), TYPE_TEAM.Left);
        BattleFieldOutpost.Current.SetCommanderCard(CommanderCard.Create(data), TYPE_TEAM.Right);



        _uiBattleField.Initialize();
        _uiBattleField.SetOnBattleFieldListener(SetBattleFieldEvent);
        _uiBattleField.SetBattleField();

        _uiBarrack.Initialize();
        _uiBarrack.SetOnUnitInformationListener(ShowUnitInformation);
        _uiBarrack.SetOnUnitChangeListener(UnitOutpostToBarracksEvent);
        _uiBarrack.AddOnRefreshListener(UnitRefreshEvent);
        _uiBarrack.AddOnRefreshListener(CommanderRefreshEvent);




        _lOutpost.Initialize();
        _lOutpost.SetOnUnitListener(() => 
            {
                if (_uiBarrack.isActiveAndEnabled)
                    _uiBarrack.Hide();
                else
                {
                    _uiBarrack.SetData(MockGameData.instance.totalUnits_L);
                    _uiBarrack.Show(TYPE_TEAM.Left);
                }
            }
        );
        _lOutpost.SetOnUnitInformationListener(ShowUnitInformation);
        _lOutpost.SetOnSkillInformationListener(ShowSkillInformation);
        _lOutpost.SetOnUnitChangeListener(UnitBarracksToOutpostEvent);
        _lOutpost.AddOnRefreshListener(UnitRefreshEvent);
        _lOutpost.AddOnRefreshListener(CommanderRefreshEvent);
        _lOutpost.SetOnCommanderDataListener(BattleFieldOutpost.Current.SetCommanderCard);
        _lOutpost.SetOnEnoughListener(IsUnitEnough);




        _rOutpost.Initialize();

        var isChallange = BattleFieldOutpost.Current.IsChallenge();
                
        _rOutpost.SetChallenge(isChallange);

        if (isChallange)
        {
            BattleFieldOutpost.Current.regionR.AddCards(MockGameData.instance.GetChallangeDataArray(BattleFieldOutpost.Current.GetChallengeLevel()));
        }

        _rOutpost.SetOnUnitListener(() =>
            {
                if (_uiBarrack.isActiveAndEnabled)
                    _uiBarrack.Hide();
                else
                {
                    _uiBarrack.SetData(MockGameData.instance.totalUnits_R);
                    _uiBarrack.Show(TYPE_TEAM.Right);
                }
            }
        );

        _rOutpost.SetOnUnitInformationListener(ShowUnitInformation);
        _rOutpost.SetOnSkillInformationListener(ShowSkillInformation);
        _rOutpost.SetOnUnitChangeListener(UnitBarracksToOutpostEvent);
        _rOutpost.AddOnRefreshListener(UnitRefreshEvent);
        _rOutpost.AddOnRefreshListener(CommanderRefreshEvent);
        _rOutpost.SetOnCommanderDataListener(BattleFieldOutpost.Current.SetCommanderCard);
        _rOutpost.SetOnEnoughListener(IsUnitEnough);


        UnitRefreshEvent(TYPE_TEAM.Left);
        UnitRefreshEvent(TYPE_TEAM.Right);
        CommanderRefreshEvent(TYPE_TEAM.Left);
        CommanderRefreshEvent(TYPE_TEAM.Right);


        _startGameBtn.onClick.AddListener(StartGame);
        _backBtn.onClick.AddListener(OnBackClicked);
    }

    private void OnDestroy()
    {
        _uiBarrack.CleanUp();

        _lOutpost.RemoveOnRefreshListener(UnitRefreshEvent);
        _lOutpost.RemoveOnRefreshListener(CommanderRefreshEvent);

        _rOutpost.RemoveOnRefreshListener(UnitRefreshEvent);
        _rOutpost.RemoveOnRefreshListener(CommanderRefreshEvent);

        _uiBarrack.RemoveOnRefreshListener(UnitRefreshEvent);
        _uiBarrack.RemoveOnRefreshListener(CommanderRefreshEvent);

        _uiBattleField.CleanUp();

    }

    private bool IsUnitEnough(TYPE_TEAM typeTeam, UnitCard uCard)
    {
        var isEnough = BattleFieldOutpost.Current.IsEnoughLeadership(uCard, typeTeam) && BattleFieldOutpost.Current.IsEnoughEmployCost(uCard, typeTeam);

        if (!isEnough)
        {
            ShowNotEnoughPopup();
        }

        return isEnough;
    }

    private void CommanderRefreshEvent(TYPE_TEAM typeTeam)
    {
        if (typeTeam == TYPE_TEAM.Left)
        {
            _lOutpost.RefreshCommanderCard(BattleFieldOutpost.Current.regionL);
        }
        else
        {
            _rOutpost.RefreshCommanderCard(BattleFieldOutpost.Current.regionR);
        }
    }

    private void UnitRefreshEvent(TYPE_TEAM typeTeam)
    {
        if (typeTeam == TYPE_TEAM.Left)
        {
            _lOutpost.RefreshUnits(BattleFieldOutpost.Current.regionL.GetUnitCards());
            _uiBarrack.SetData(MockGameData.instance.totalUnits_L);
        }
        else
        {
            _rOutpost.RefreshUnits(BattleFieldOutpost.Current.regionR.GetUnitCards());
            _uiBarrack.SetData(MockGameData.instance.totalUnits_R);
        }
        _uiBarrack.Refresh();
    }

    private void UnitOutpostToBarracksEvent(TYPE_TEAM typeTeam, UnitCard uCard)
    {
        MockGameData.instance.AddCard(typeTeam, uCard);
        BattleFieldOutpost.Current.RemoveCard(uCard, typeTeam);

    }

    private void UnitBarracksToOutpostEvent(TYPE_TEAM typeTeam, UnitCard uCard)
    {
        MockGameData.instance.RemoveCard(typeTeam, uCard);
        BattleFieldOutpost.Current.AddCard(uCard, typeTeam);
    }

    private void ShowNotEnoughPopup()
    {
        var ui = UICommon.Current.GetUICommon<UIPopup>();
        ui.ShowApplyPopup("�ڿ� �Ǵ� �α��� �����մϴ�.", "Ȯ��", null);
    }
    private void ShowUnitInformation(UnitCard uCard)
    {
        var uiUnitInfor = UICommon.Current.GetUICommon<UIUnitInformation>();
        uiUnitInfor.Show(uCard, Input.mousePosition);
    }

    private void ShowSkillInformation(SkillData skillData, Vector2 screenPosition)
    {
        var ui = UICommon.Current.GetUICommon<UISkillInformation>();
        ui.Show(skillData, screenPosition);
    }


    private void SetBattleFieldEvent(BattleFieldData battlefieldData)
    {
        BattleFieldOutpost.Current.battleFieldData = battlefieldData;
        _uiBattleField.RefreshBattleField(battlefieldData);
    }

    public void StartGame()
    {
        if (BattleFieldOutpost.Current.IsEmptyUnitDataArray(TYPE_TEAM.Left))
        {
            Debug.Log("CommanderL Empty");
            return;
        }
        if (BattleFieldOutpost.Current.IsEmptyUnitDataArray(TYPE_TEAM.Right))
        {
            Debug.Log("CommanderR Empty");
            return;
        }

        LoadManager.SetNextSceneName("Test_BattleField");
        UnityEngine.SceneManagement.SceneManager.LoadScene(LoadManager.LoadSceneName);
    }

    private void OnBackClicked()
    {
        LoadManager.SetNextSceneName("Test_MainTitle");
        UnityEngine.SceneManagement.SceneManager.LoadScene(LoadManager.LoadSceneName);
    }

}