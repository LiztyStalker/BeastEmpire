using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum TYPE_BATTLE_TEAM { None = -1, Left, Right }
public enum TYPE_BATTLE_ROUND { Morning, Evening, Night}
public enum TYPE_BATTLE_STEP { Setting, Order, Battle}

/// <summary>
/// 게임 셋팅, 게임제어 및 게임 명령을 적용합니다
/// </summary>
public class BattleFieldManager : MonoBehaviour
{
    private readonly string CANVAS_BATTLEFIELD_KEY = "Canvas@BattleField";

    private UIBattleField _uiGame;

    private FieldGenerator _fieldGenerator;

    private UnitManager _unitManager;

    private static CommanderCamp _commanderCamp = null;

    private TYPE_BATTLE_STEP _typeBattleField = TYPE_BATTLE_STEP.Setting;

    public TYPE_BATTLE_TEAM _firstTypeTeam = TYPE_BATTLE_TEAM.Right;

    private TYPE_BATTLE_TEAM _dropTeam = TYPE_BATTLE_TEAM.Left;

    private TYPE_BATTLE_ROUND _typeBattleRound = TYPE_BATTLE_ROUND.Morning;  

    private Coroutine _battleCoroutine = null;

    private bool _isReady = false;

    private bool _isAutoBattle = false;

    /// <summary>
    /// 현재 전투 턴 카운트
    /// </summary>
    private int _nowBattleTurnCount = BattleFieldSettings.BATTLE_TURN_COUNTER;

    public bool isRunning => _battleCoroutine != null;

    private static BattleFieldManager _current = null;

    public static BattleFieldManager Current
    {
        get
        {
            if (_current == null)
            {
                _current = FindObjectOfType<BattleFieldManager>();
            }
            return _current;
        }
    }


    #region ##### Initialize & CleanUp #####

    private void Awake()
    {
        //v 싱크 비적용 및 최소 프레임 적용
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        InitializeFieldManager();
        InitializeUnitManager();
        InitializeCommanderCamp();
        InitializeUI(_commanderCamp);

        //_uiGame?.SetUnitData(_commanderCamp.GetUnitDataArray(TYPE_TEAM.Left));
        //_uiGame?.SetSupply(_commanderCamp.NowSupplyValue(TYPE_TEAM.Left), _commanderCamp.GetSupplyRate(TYPE_TEAM.Left));
        //_uiGame?.ActivateUnitCardPanel();
    }

    private void OnDestroy()
    {
        CleanUpCommanderCamp();
        CleanUpUI();
        CleanUpFieldManager();
    }

    private void InitializeUI(CommanderCamp commanderCamp)
    {

        ///전투 캔바스 가져오기
        _uiGame = FindObjectOfType<UIBattleField>();
        if (_uiGame == null)
        {
            var obj = DataStorage.Instance.GetDataOrNull<GameObject>(CANVAS_BATTLEFIELD_KEY, null, null);
            var instance = Instantiate(obj);
            _uiGame = instance.GetComponent<UIBattleField>();
        }


        //Canvas Event 등록
        _uiGame?.Initialize();
        _uiGame?.SetOnNextTurnListener(NextTurn);
        _uiGame?.SetOnDragUnitListener(DragUnitCard);
        _uiGame?.SetOnDropUnitListener(DropUnitCard);
        _uiGame?.AddBattleTurnListener(SetTypeBattleTurns);
        _uiGame?.AddCancelUnitListener(CancelModifiedUnitActor);
        _uiGame?.AddModifiedUnitListener(ModifiedUnitActor);
        _uiGame?.AddReturnUnitListener(ReturnUnitActor);

        ///이벤트로 보낼 필요 있음
        _uiGame?.SetUnitData(commanderCamp.GetUnitDataArray(TYPE_BATTLE_TEAM.Left));
        _uiGame?.SetSupply(commanderCamp.NowSupplyValue(TYPE_BATTLE_TEAM.Left), commanderCamp.GetSupplyRate(TYPE_BATTLE_TEAM.Left));

        //UI 갱신
        commanderCamp.DecreaseHealth(TYPE_BATTLE_TEAM.Left, 0);
        commanderCamp.DecreaseHealth(TYPE_BATTLE_TEAM.Right, 0);
        commanderCamp.Supply(TYPE_BATTLE_TEAM.Left);

    }

    private void CleanUpUI()
    {
        //_uiGame?.SetOnNextTurnListener(NextTurn);
        //_uiGame?.SetOnDragUnitListener(DragUnit);
        //_uiGame?.SetOnDropUnitListener(DropUnit);
        _uiGame?.RemoveBattleTurnListener(SetTypeBattleTurns);
        _uiGame?.RemoveCancelUnitListener(CancelModifiedUnitActor);
        _uiGame?.RemoveModifiedUnitListener(ModifiedUnitActor);
        _uiGame?.RemoveReturnUnitListener(ReturnUnitActor);
    }

    private void InitializeFieldManager()
    {
        if (_fieldGenerator == null) _fieldGenerator = GetComponentInChildren<FieldGenerator>();
        _fieldGenerator.Initialize();
    }

    private void CleanUpFieldManager()
    {
        _fieldGenerator.CleanUp();
    }

    private void InitializeCommanderCamp()
    {

#if UNITY_EDITOR
        if (BattleFieldOutpost.Current == null)
        {
            _commanderCamp = InitializeTestGame();
        }
        else
        {
            _commanderCamp = InitializeMockGame();
        }
#else
        _commanderCamp = InitializeMockGame();
#endif

        _commanderCamp.AddAllHealthListener(CastleHealthEvent);
        _commanderCamp.AddSupplyListener(TYPE_BATTLE_TEAM.Left, SupplyEvent);

    }

    private void CleanUpCommanderCamp()
    {
        _commanderCamp.RemoveAllHealthListener(CastleHealthEvent);
        _commanderCamp.RemoveSupplyListener(TYPE_BATTLE_TEAM.Left, SupplyEvent);
    }

    private void InitializeUnitManager()
    {
        if (_unitManager == null) _unitManager = GetComponentInChildren<UnitManager>();
        _unitManager.CreateCastleUnit(TYPE_BATTLE_TEAM.Left);
        _unitManager.CreateCastleUnit(TYPE_BATTLE_TEAM.Right);
    }

    /// <summary>
    /// 막사에서 가져오기 (일반)
    /// </summary>
    private CommanderCamp InitializeMockGame()
    {
        var leftCommandActor = BattleFieldOutpost.Current.regionL.commanderActor;
        leftCommandActor.SetTeam(TYPE_BATTLE_TEAM.Left);

        var rightCommandActor = BattleFieldOutpost.Current.regionR.commanderActor;
        rightCommandActor.SetTeam(TYPE_BATTLE_TEAM.Right);

        var commanderCamp = CommanderCamp.Create(leftCommandActor, rightCommandActor);

        return commanderCamp;
    }


    #endregion

    private void Start()
    {
        _typeBattleRound = TYPE_BATTLE_ROUND.Morning;
        ActivateFormation();

    }

    void Update()
    {

        /// <summary>
        /// 입력 일체화 - 게임 시스템
        /// 입력 시스템 필요
        /// EventSystems와 관계는?
        /// </summary>

        var screenPosition = Input.mousePosition;

        if (Input.GetMouseButtonDown(0))
        {
            if (_unitManager.IsDrag())
            {
                _unitManager.ModifiedAction(screenPosition);
            }
            else {
                if (IsOrder())
                {
                    _uiGame.ClickAction(screenPosition);
                }
            }          
        }

        if (_unitManager.IsDrag())
        {
            _unitManager.ClickAction(screenPosition);
        }


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _uiGame.EscapeAction();
        }
    }

  

    /// <summary>
    /// 전투 명령 적용
    /// </summary>
    /// <param name="typeTeam"></param>
    /// <param name="typeBattleTurns"></param>
    public void SetTypeBattleTurns(TYPE_BATTLE_TEAM typeTeam, TYPE_BATTLE_TURN[] typeBattleTurns)
    {
        _commanderCamp.SetTypeBattleTurns(typeTeam, typeBattleTurns);
    }


    //배치 AI 적용 필요
    private void CreateEnemyUnits()
    {
        //기본
        //보병이 많으면 보병우선
        //사격이 많으면 사격우선


        //돌격 기갑 앞에서 시작 (전방의 적이 누군지 판단하고 그에 따라서 배치)
        //보병 앞에서 시작 (전방의 적이 누군지 판단하고 그에 따라서 배치)
        //사격 뒤에서 시작 (전방의 적이 누군지 판단하고 그에 따라서 배치)
        //셀을 순서대로 기록하면서 진행
        //턴제 전략 알고리즘 진행
        //사격 우선 - 사격병 먼저 배치
        //보병 우선 - 보병 먼저 배치
        //돌격 우선 - 돌격 및 기갑 먼저 배치
        //복합 - 각 병종을 일정 량에 맞춰 배치

        //전투 - 공격적 수비적
        //공격적 - 전진 및 돌격
        //수비적 - 방어 및 후퇴


        var rightCommanderActor = _commanderCamp.GetCommanderActor(TYPE_BATTLE_TEAM.Right);
        CreateUnitActorForAI(rightCommanderActor);
        SkillData.CastSkills(rightCommanderActor, TYPE_SKILL_CAST.DeployCast);
    }

   
    /// <summary>
    /// 다음턴 진행
    /// </summary>
    public void NextTurn()
    {
        switch (_typeBattleField)
        {
            case TYPE_BATTLE_STEP.Setting:
                if (_unitManager.IsLivedUnitCount(TYPE_BATTLE_TEAM.Left) == 0)
                {
                    _uiGame.ShowUnitSettingIsZeroPopup();
                }
                else
                {
                    _uiGame.ActivateBattleRoundPanel();
                    if (_firstTypeTeam == TYPE_BATTLE_TEAM.Left)
                        CreateEnemyUnits();

                    _typeBattleField++;
                }

                break;
            case TYPE_BATTLE_STEP.Order:

                var arr = _commanderCamp.GetTypeBattleTurns(TYPE_BATTLE_TEAM.Left);
                if (arr.Length == BattleFieldSettings.BATTLE_TURN_COUNTER)
                {
                    if (_battleCoroutine == null)
                    {
                        AudioManager.ActivateAudio("Warhorn", AudioManager.TYPE_AUDIO.SFX);
                        _battleCoroutine = StartCoroutine(TurnCoroutine(arr, _commanderCamp.GetTypeBattleTurns(TYPE_BATTLE_TEAM.Right)));
                        _typeBattleField++;
                    }
                }
                else
                {
                    _uiGame.ActivateBattleRoundPanel();
                    _uiGame.ShowIsNotEnoughCommandPopup();
                }
                break;
        }
    }


    //BattleFieldController 필요
    //전장 제어 코루틴 필요


    /// <summary>
    /// 전투 종료
    /// </summary>
    /// <returns></returns>
    private bool IsBattleEnd()
    {
        if (_nowBattleTurnCount == 0) return true;

        if (IsGameEnd())
        {
            _isReady = true;
            return true;
        }

        //return false;
        if (_unitManager.IsLiveUnitsEmpty()) 
        {
            _firstTypeTeam = (_unitManager.IsLivedUnitCount(TYPE_BATTLE_TEAM.Left) == 0) ? TYPE_BATTLE_TEAM.Right : TYPE_BATTLE_TEAM.Left;
            _isReady = true;
            return true;
        }
        return false;
    }





    /// <summary>
    /// 턴 코루틴
    /// </summary>
    /// <param name="battleTurnsLeft"></param>
    /// <param name="battleTurnsRight"></param>
    /// <returns></returns>
    IEnumerator TurnCoroutine(TYPE_BATTLE_TURN[] battleTurnsLeft, TYPE_BATTLE_TURN[] battleTurnsRight)
    {
        _uiGame.ActivateBattlePanel();
        
        _isReady = false;
        _isAutoBattle = false;

        _nowBattleTurnCount = battleTurnsLeft.Length;

        if (!_isReady)
        {
            yield return _unitManager.SetPreActiveActionUnits(_commanderCamp.GetCommanderActor(TYPE_BATTLE_TEAM.Left), _commanderCamp.GetCommanderActor(TYPE_BATTLE_TEAM.Right));

            while (!IsBattleEnd())
            {
                var leftOrder = battleTurnsLeft[battleTurnsLeft.Length - _nowBattleTurnCount];
                var rightOrder = battleTurnsRight[battleTurnsRight.Length - _nowBattleTurnCount];

                StartCoroutine(_unitManager.ActionUnits(TYPE_BATTLE_TEAM.Left, leftOrder));
                StartCoroutine(_unitManager.ActionUnits(TYPE_BATTLE_TEAM.Right, rightOrder));

                _uiGame.SetBattleTurnOrder(TYPE_BATTLE_TEAM.Left, leftOrder);
                _uiGame.SetBattleTurnOrder(TYPE_BATTLE_TEAM.Right, rightOrder);

                while (_unitManager.isRunning)
                {
                    yield return null;
                }
                _nowBattleTurnCount--;
            }

            yield return _unitManager.ReleasePreActiveActionUnits();

            //적군 아군이 남아있을때
            if (!_isReady)
            {
                TurnUnitActor();
                _typeBattleField = TYPE_BATTLE_STEP.Order;
                _uiGame.ActivateBattleRoundPanel();
                _battleCoroutine = null;
                yield break;
            }
        }

        TurnUnitActor();

        if (IsGameEnd())
        {
            _uiGame.GameEnd(GameResult());
            yield break;
        }


        if (_firstTypeTeam == TYPE_BATTLE_TEAM.Right)
        {
            while (_isReady)
            {
                yield return null;
                _isReady = false;
                _isAutoBattle = true;
            }
        }
        else
        {
            while (_isReady)
            {
                _uiGame.ShowAutoBattlePopup(SetAutoBattle);
                yield return null;
            }
        }


        while (_isReady)
        {
            yield return null;
        }

        if (_isAutoBattle)
        {
            int cnt = 3;
            while (!IsAutoBattleEnd(_firstTypeTeam))
            {
                StartCoroutine(_unitManager.ActionUnits(TYPE_BATTLE_TEAM.Left, TYPE_BATTLE_TURN.Forward));
                StartCoroutine(_unitManager.ActionUnits(TYPE_BATTLE_TEAM.Right, TYPE_BATTLE_TURN.Forward));

                while (_unitManager.isRunning)
                {
                    yield return null;
                }

                cnt--;
                if(cnt == 0)
                {
                    TurnUnitActor();
                    cnt = 3;
                }
            }
        }

        if (IsGameEnd())
        {
            _uiGame.GameEnd(GameResult());
            yield break;
        }


        NextRound();
        _battleCoroutine = null;

        yield return null;
    }

    /// <summary>
    /// 자동전투 적용
    /// </summary>
    /// <param name="isAutoBattle"></param>
    private void SetAutoBattle(bool isAutoBattle)
    {
        _isAutoBattle = isAutoBattle;
        _isReady = false;

        if (!isAutoBattle)
        {
            //보급반납
            _unitManager.ReturnUnitSupply(_commanderCamp.GetCommanderActor(TYPE_BATTLE_TEAM.Left));
        }
    }

    /// <summary>
    /// 자동 전투 종료
    /// </summary>
    /// <param name="typeTeam"></param>
    /// <returns></returns>
    private bool IsAutoBattleEnd(TYPE_BATTLE_TEAM typeTeam)
    {
        if (IsGameEnd())
        {
            return true;
        }

        return (_unitManager.IsLivedUnitCount(typeTeam) == 0);
    }


   
    /// <summary>
    /// 유닛 턴
    /// </summary>
    private void TurnUnitActor()
    {
        var blocks = FieldManager.GetAllBlocks();
        for (int i = 0; i < blocks.Length; i++)
        {
            if (!blocks[i].IsHasUnitActor())
            {
                blocks[i].Turn();
            }
        }
    }

    /// <summary>
    /// 다음 전투
    /// </summary>
    private void NextRound()
    {
        if(_typeBattleRound == TYPE_BATTLE_ROUND.Night)
        {
            _uiGame.GameEnd(GameResult());
            return;
        }

        _unitManager.ClearUnitCards();
        _unitManager.ClearDeadUnits();

        //다친 유닛 회복
        _commanderCamp.RecoveryAllUnits();

        //라운드 증가
        _typeBattleRound++;

        _isReady = false;
        _isAutoBattle = false;

        ActivateFormation();
    }

    /// <summary>
    /// 진형배치 단계 적용
    /// </summary>
    private void ActivateFormation()
    {
        _typeBattleField = TYPE_BATTLE_STEP.Setting;

        _uiGame?.UpdateUnits();
        _uiGame?.ActivateUnitCardPanel();
        _uiGame?.SetBattleRound(_typeBattleRound);

        SetEnemyFormation();
    }

    /// <summary>
    /// 적 진형 배치
    /// </summary>
    private void SetEnemyFormation()
    {
        if (_firstTypeTeam == TYPE_BATTLE_TEAM.Right)
            CreateEnemyUnits();
    }

    /// <summary>
    /// 게임 결과
    /// </summary>
    /// <returns></returns>
    private TYPE_BATTLE_RESULT GameResult()
    {
        return _commanderCamp.GameResult();
    }

    /// <summary>
    /// 체력 증감
    /// 리팩토링 필요
    /// </summary>
    public static void IncreaseHealth(int damageValue, TYPE_BATTLE_TEAM typeTeam)
    {
        _commanderCamp.DecreaseHealth(typeTeam, damageValue);
    }

    /// <summary>
    /// 명령 단계인지 확인
    /// </summary>
    /// <returns></returns>
    public bool IsOrder() => _typeBattleField == TYPE_BATTLE_STEP.Setting;

    /// <summary>
    /// 보급력 가능한지 확인
    /// </summary>
    /// <param name="uCard"></param>
    /// <param name="nowTypeTeam"></param>
    /// <returns></returns>
    public bool IsSupply(UnitCard uCard, TYPE_BATTLE_TEAM typeTeam = TYPE_BATTLE_TEAM.Left) => _commanderCamp.IsSupply(uCard, typeTeam);

    /// <summary>
    /// 게임 종료 여부
    /// </summary>
    /// <returns></returns>
    private bool IsGameEnd()
    {
        return _commanderCamp.IsGameEnd();
    }

    /// <summary>
    /// AI 배치
    /// </summary>
    /// <param name="cActor"></param>
    public void CreateUnitActorForAI(ICommanderActor cActor)
    {
        var blocks = FieldManager.GetTeamUnitBlocksFromVertical(cActor.typeTeam);
        var list = cActor.unitDataArray.ToArray();//.OrderBy(uCard => uCard.typeUnitClass).ToArray();


        //List<UnitCard> cards = new List<UnitCard>();
        //for (int i = 0; i < list.Length; i++)
        //{
        //    if (list[i].typeUnitClass == TYPE_UNIT_CLASS.Charger || list[i].typeUnitClass == TYPE_UNIT_CLASS.HeavySoldier)
        //        cards.Insert(0, list[i]);
        //    else
        //        cards.Add(list[i]);
        //}

        //list = cards.ToArray();

        for (int i = 0; i < list.Length; i++)
        {
            for(int j = 0; j < blocks.Length; j++)
            {
                var block = blocks[j];
                var uCard = list[i];

                //사망한 병사 포메이션은 무시
                var formationCells = new List<Vector2Int>();
                var uKeys = new List<int>();

                for (int k = 0; k < uCard.UnitKeys.Length; k++)
                {
                    if (!uCard.IsDead(uCard.UnitKeys[k]))
                    {
                        formationCells.Add(uCard.FormationCells[k]);
                        uKeys.Add(uCard.UnitKeys[k]);
                    }
                }

                //포메이션 블록 가져오기
                var formationBlocks = FieldManager.GetFormationBlocks(block.coordinate, formationCells.ToArray(), cActor.typeTeam);

                if (uKeys.Count == formationBlocks.Length)
                {
                    bool isCheck = false;
                    for (int k = 0; k < formationBlocks.Length; k++)
                    {
                        if (formationBlocks[k].IsHasGroundUnitActor())
                        {
                            isCheck = true;
                            break;
                        }
                    }

                    if (!isCheck && cActor.IsSupply(uCard))
                    {
                        cActor.UseSupply(uCard);
                        _unitManager.CreateUnits(uCard, uKeys.ToArray(), formationBlocks, cActor.typeTeam);
                        break;
                    }
                }                
            }
        }
    }


    #region ##### Test #####

    #if UNITY_EDITOR && UNITY_INCLUDE_TESTS



    /// <summary>
    /// 테스트 전장 배치
    /// </summary>
    private CommanderCamp InitializeTestGame()
    {
        BattleFieldOutpost.InitializeBattleFieldOutpost();

        Debug.LogWarning("BattleField TestMode");

        var dataArrayL = DataStorage.Instance.GetAllDataArrayOrZero<UnitData>();
        var dataArrayR = DataStorage.Instance.GetAllDataArrayOrZero<UnitData>();

        dataArrayL = dataArrayL.Where(data => data.SkeletonDataAsset != null && data.Icon != null && data.Tier == 3).ToArray();
        dataArrayR = dataArrayR.Where(data => data.SkeletonDataAsset != null && data.Icon != null && data.Tier == 3).ToArray();

        var uCardsL = UnitCard.Create(dataArrayL);// _unitManager.GetRandomUnitCards(20);//_unitManager.GetUnitCards("UnitData_SpearSoldier", "UnitData_Archer", "UnitData_Assaulter");
        var uCardsR = UnitCard.Create(dataArrayR); //_unitManager.GetRandomUnitCards(20);//_unitManager.GetUnitCards("UnitData_SpearSoldier", "UnitData_Archer", "UnitData_Assaulter");

        var leftCommandActor = CommanderActor.Create(CommanderCard.Create(DataStorage.Instance.GetDataOrNull<CommanderData>("Raty")), uCardsL, 0);
        leftCommandActor.SetTeam(TYPE_BATTLE_TEAM.Left);

        var rightCommandActor = CommanderActor.Create(CommanderCard.Create(DataStorage.Instance.GetDataOrNull<CommanderData>("Raty")), uCardsR, 0);
        rightCommandActor.SetTeam(TYPE_BATTLE_TEAM.Right);

        var commanderCamp = CommanderCamp.Create(leftCommandActor, rightCommandActor);
        return commanderCamp;
    }



    /// <summary>
    /// 다음턴 테스트
    /// </summary>
    /// <param name="typeBattleTurnL"></param>
    /// <param name="typeBattleTurnR"></param>
    public void NextTurnTester(TYPE_BATTLE_TURN typeBattleTurnL, TYPE_BATTLE_TURN typeBattleTurnR)
    {
        if (_battleCoroutine == null)
        {
            _battleCoroutine = StartCoroutine(TurnTestCoroutine(typeBattleTurnL, typeBattleTurnR));
        }
    }

    /// <summary>
    /// 테스트용 턴 코루틴
    /// </summary>
    /// <param name="battleTurnsLeft"></param>
    /// <param name="battleTurnsRight"></param>
    /// <returns></returns>
    IEnumerator TurnTestCoroutine(TYPE_BATTLE_TURN battleTurnsLeft, TYPE_BATTLE_TURN battleTurnsRight)
    {
        _uiGame.ActivateBattlePanel();

        StartCoroutine(_unitManager.ActionUnits(TYPE_BATTLE_TEAM.Left, battleTurnsLeft));
        StartCoroutine(_unitManager.ActionUnits(TYPE_BATTLE_TEAM.Right, battleTurnsRight));

        yield return null;

        while (_unitManager.isRunning)
        {
            yield return null;
        }

        TurnUnitActor();

        _battleCoroutine = null;

        yield return null;
    }




    /// <summary>
    /// 다음 턴 테스트
    /// </summary>
    public void NextRoundTest()
    {
        NextRound();
    }

    /// <summary>
    /// 현재 팀 - 테스트
    /// </summary>
    /// <returns></returns>
    public TYPE_BATTLE_TEAM GetNowTeam()
    {
        return _dropTeam;
    }

    /// <summary>
    /// 현재 팀 정하기 - 테스트
    /// </summary>
    /// <param name="typeTeam"></param>
    public void SetNowTeam(TYPE_BATTLE_TEAM typeTeam) => _dropTeam = typeTeam;


    /// <summary>
    /// 필드에 유닛 배치 - 테스트
    /// </summary>
    public void CreateFieldUnit(TYPE_BATTLE_TEAM typeTeam)
    {
        var cActor = _commanderCamp.GetCommanderActor(typeTeam);

        var blocks = FieldManager.GetTeamUnitBlocksFromHorizental(typeTeam);

        for (int i = 0; i < blocks.Length; i++)
        {
            var block = blocks[i];
            if (block != null && !block.IsHasUnitActor())
            {
                var uCard = cActor.unitDataArray[Random.Range(0, cActor.unitDataArray.Length)];

                if (uCard != null)
                {
                    var uCardTmp = UnitCard.Create(uCard.UnitData);
                    var uKey = uCardTmp.UnitKeys[0];
                    _unitManager.CreateUnit(uCardTmp, uKey, block, typeTeam);
                }                
            }
        }
    }


    /// <summary>
    /// 테스트용 유닛 생성기 - 테스트
    /// </summary>
    /// <param name="typeTeam"></param>
    /// <param name="unit"></param>
    public void CreateFieldUnitInTest(TYPE_BATTLE_TEAM typeTeam, UnitData unit)
    {
        var blocks = FieldManager.GetTeamUnitBlocksFromHorizental(typeTeam);

        for (int i = 0; i < blocks.Length; i++)
        {
            var block = blocks[i];
            if (block != null && !block.IsHasGroundUnitActor())
            {
                var uCardTmp = UnitCard.CreateTest(unit);
                var uKey = uCardTmp.UnitKeys[0];
                _unitManager.CreateUnit(uCardTmp, uKey, block, typeTeam);
            }
        }
    }



    /// <summary>
    /// 필드에 있는 모든 유닛을 제거합니다 - 테스트
    /// </summary>
    public void ClearAllUnits(bool isIncludeCastle = false)
    {
        _unitManager.ClearAllUnits(isIncludeCastle);
    }

#endif

    #endregion


    #region ##### Unit #####


    /// <summary>
    /// 개인 병사 생성하기
    /// </summary>
    /// <param name="uCard"></param>
    /// <param name="uKey"></param>
    /// <param name="fieldBlock"></param>
    /// <param name="typeTeam"></param>
    /// <returns></returns>
    public IUnitActor CreateUnit(UnitCard uCard, int uKey, IFieldBlock fieldBlock, TYPE_BATTLE_TEAM typeTeam)
    {
        return _unitManager.CreateUnit(uCard, uKey, fieldBlock, typeTeam);
    }


    /// <summary>
    /// 유닛 드래그
    /// </summary>
    /// <param name="uCard"></param>
    public bool DragUnitCard(UnitCard uCard)
    {
        if (IsSupply(uCard))
        {
            if (BattleFieldSettings.SingleFormation)
            {
                var card = UnitCard.Create(uCard.UnitData);
                card.SetFormation(new Vector2Int[] { new Vector2Int(0, 0) });
                _unitManager.DragUnitActor(card, _dropTeam);
            }
            else
            {
                _unitManager.DragUnitActor(uCard, _dropTeam);
            }
            _uiGame?.ActivateUnitSetting(true);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 유닛 드래그
    /// </summary>
    /// <param name="uActor"></param>
    public void ModifiedUnitActor(IUnitActor uActor)
    {
        _unitManager.ModifiedUnitActor(uActor);
    }

    /// <summary>
    /// 분대 배치하기
    /// </summary>
    /// <param name="uCard"></param>
    /// <returns></returns>
    public bool DropUnitCard(UnitCard uCard)
    {
        _uiGame?.ActivateUnitSetting(false);

        var leftCommanderActor = _commanderCamp.GetCommanderActor(TYPE_BATTLE_TEAM.Left);
        if (_unitManager.DropUnitActor())
        {
            _commanderCamp.UseSupply(TYPE_BATTLE_TEAM.Left, uCard);
            SkillData.CastSkills(leftCommanderActor, TYPE_SKILL_CAST.DeployCast);
            _uiGame?.SetSupply(leftCommanderActor.nowSupplyValue, leftCommanderActor.GetSupplyRate());
            return true;
        }
        return false;
    }

    /// <summary>
    /// 유닛 반납
    /// </summary>
    /// <param name="uActor"></param>
    public void ReturnUnitActor(IUnitActor uActor)
    {
        _unitManager.ReturnUnitActor(uActor);
    }

    /// <summary>
    /// 변경하는 유닛 취소
    /// </summary>
    public void CancelModifiedUnitActor()
    {
        _unitManager.CancelModifiedUnitActor();
    }

    #endregion


    #region ##### Event #####
    private void SupplyEvent(TYPE_BATTLE_TEAM typeTeam, int value, float rate)
    {
        _uiGame?.ShowSupply(typeTeam, value, rate);
    }

    private void CastleHealthEvent(TYPE_BATTLE_TEAM typeTeam, int value, float rate)
    {
        _uiGame?.ShowHealth(typeTeam, value, rate);
    }

    #endregion


}
