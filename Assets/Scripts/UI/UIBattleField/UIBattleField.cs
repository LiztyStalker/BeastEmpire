using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TYPE_BATTLE_TURN {None = -1, Forward, Charge, Guard, Backward }

public enum TYPE_BATTLE_RESULT { Victory, Draw, Defeat}

public class UIBattleField : MonoBehaviour
{
    private BattleFieldManager _battleFieldManager;

    private UIBattleStatusLayout _uiBattieStatusLayout;

    private UIBattleCommand _uiBattleCommandLayout;

    private UIBattleSupply _uiBattleSupplyLayout;

    private UIBattleSquadLayout _uiBattleSquadLayout;

    private UIUnitSelector _uiUnitSelector;

    private UIBattleTurnPanel _uiBattleTurnPanel;

    private UIBattleFieldMenu _uiBattleFieldMenu;

    [SerializeField]
    private GameObject _uiUnitSettingsPanel;

    [SerializeField]
    private Button _nextTurnButton;

    [SerializeField]
    private Button _helpButton;

    [SerializeField]
    private Button _menuButton;

    public void Initialize(BattleFieldManager battleFieldManager)
    {
        _battleFieldManager = battleFieldManager;

        SetComponent(ref _uiBattieStatusLayout);
        _uiBattieStatusLayout.Initialize();

        SetComponent(ref _uiBattleSquadLayout);
        _uiBattleSquadLayout.Initialize();
        _uiBattleSquadLayout.SetOnDragListener(DragUnit);
        _uiBattleSquadLayout.SetOnDropListener(DropUnit);
        _uiBattleSquadLayout.SetOnUnitInformationListener(ShowUnitInformationEvent);

        SetComponent(ref _uiUnitSelector);

        SetComponent(ref _uiBattieStatusLayout);
        _uiUnitSelector.Initialize();
        _uiUnitSelector.SetOnInformationListener(ShowUnitInformationEvent);
        _uiUnitSelector.SetOnDragListener(OnUnitModifiedClickedEvent);
        _uiUnitSelector.SetOnCancelListener(OnUnitCancelClickedEvent);
        _uiUnitSelector.SetOnReturnListener(OnUnitReturnClickedEvent);

        SetComponent(ref _uiBattleCommandLayout);
        _uiBattleCommandLayout.Initialize();

        SetComponent(ref _uiBattleSupplyLayout);

        SetComponent(ref _uiBattleTurnPanel);
        _uiBattleTurnPanel.SetAnimator(false);

        SetComponent(ref _uiBattleFieldMenu);
        _uiBattleFieldMenu.Initialize();
        _uiBattleFieldMenu.AddOnClosedListener(MenuClosedEvent);
        _uiBattleFieldMenu.SetOnRetryListener(RetryEvent);
        _uiBattleFieldMenu.SetOnReturnListener(ReturnEvent);
        _uiBattleFieldMenu.SetOnSurrenderListener(SurrenderEvent);

        ActivateUnitSetting(false);

        _nextTurnButton.onClick.AddListener(NextTurnEvent);
        _helpButton.onClick.AddListener(HelpEvent);
        _menuButton.onClick.AddListener(MenuEvent);

        AudioManager.ActivateAudio("BGMGrass", AudioManager.TYPE_AUDIO.BGM, true);
    }

    public void CleanUp()
    {
        _uiBattleFieldMenu.RemoveOnClosedListener(MenuClosedEvent);
        _uiBattleFieldMenu.CleanUp();

        _uiBattleSquadLayout.CleanUp();
        _uiUnitSelector.CleanUp();
        _uiBattleCommandLayout.CleanUp();

        _nextTurnButton.onClick.RemoveListener(NextTurnEvent);
    }

    /// <summary>
    /// ???? ?????????? ??????????
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="behaviour"></param>
    /// <returns></returns>
    private bool SetComponent<T>(ref T behaviour) where T : MonoBehaviour
    {
        behaviour = GetComponentInChildren<T>(true);
        return IsNull<T>(behaviour);
    }


    private bool IsNull<T>(MonoBehaviour gameObject)
    {
        if (gameObject == null)
        {
            Debug.LogError($"{typeof(T).Name}?? ???? ?? ????????");
            return true;
        }
        return false;
    }

    /// <summary>
    /// Supply ?????? ????????
    /// </summary>
    /// <param name="typeTeam"></param>
    /// <param name="value"></param>
    /// <param name="rate"></param>
    public void ShowSupply(TYPE_TEAM typeTeam, int value, float rate)
    {
        _uiBattleSupplyLayout.SetSupply(value, rate);
    }

    /// <summary>
    /// ???? ???? Health ?????? ????????
    /// </summary>
    /// <param name="typeTeam"></param>
    /// <param name="value"></param>
    /// <param name="rate"></param>
    public void ShowHealth(TYPE_TEAM typeTeam, int value, float rate)
    {
        _uiBattieStatusLayout.SetCastleHealth(typeTeam, value, rate);
    }

    /// <summary>
    /// ???????? ?????? ????????
    /// </summary>
    /// <param name="callback"></param>
    public void ShowAutoBattlePopup(System.Action<bool> callback)
    {
        var ui = UICommon.Current.GetUICommon<UIPopup>();
        ui.ShowOkAndCancelPopup("?????? ???? ???? ??????????????.\n???? ???? ???? ?????????????????\n?????? ?????? ???????? ???? ?????????????????",
            "????", 
            "????", 
            delegate { callback?.Invoke(true); },
            delegate { callback?.Invoke(false); }, 
            null,
            true);
    }

    /// <summary>
    /// ???? Zero ?????? ????????
    /// </summary>
    public void ShowUnitSettingIsZeroPopup()
    {
        var ui = UICommon.Current.GetUICommon<UIPopup>();
        ui.ShowApplyPopup("?????? ?????? ????????.\n?????? 1???? ???? ?????? ??????");
    }

    public void ShowIsNotEnoughCommandPopup()
    {
        var ui = UICommon.Current.GetUICommon<UIPopup>();
        ui.ShowApplyPopup("3???? ?????? ???????? ??????.");
    }

    /// <summary>
    /// ???? ???????? ???????? ??????
    /// </summary>
    public void UpdateUnits()
    {
        _uiBattleSquadLayout.UpdateUnits();
    }

    /// <summary>
    /// ???????? ??????
    /// </summary>
    /// <param name="uCard"></param>
    /// <returns></returns>
    private bool DragUnit(UnitCard uCard)
    {
        if(_battleFieldManager.IsSupply(uCard)){
            _battleFieldManager.DragUnit(uCard);
            return true;
        }
        return false;
    }

    /// <summary>
    /// ???????? ????
    /// </summary>
    /// <param name="uCard"></param>
    /// <returns></returns>
    private bool DropUnit(UnitCard uCard)
    {
        var boolean = _battleFieldManager.DropUnit(uCard);
        
        return boolean;
    }

    /// <summary>
    /// ???? ???? ???????? ????????
    /// </summary>
    /// <param name="typeBattleRound"></param>
    public void SetBattleRound(TYPE_BATTLE_ROUND typeBattleRound)
    {
        _uiBattieStatusLayout.SetBattleRound(typeBattleRound);
    }

    /// <summary>
    /// Supply ?????? ??????????
    /// </summary>
    /// <param name="value"></param>
    /// <param name="rate"></param>
    public void SetSupply(int value, float rate)
    {
        _uiBattleSupplyLayout.SetSupply(value, rate);
    }

    /// <summary>
    /// ?? ???? ?????? ??????????
    /// </summary>
    /// <param name="isActive"></param>
    public void SetTotalSquadCount(bool isActive)
    {
        _uiBattleSquadLayout.gameObject.SetActive(isActive);
        _uiUnitSelector.SetActive(isActive);
    }

    /// <summary>
    /// UnitCard[]?? ??????????
    /// </summary>
    /// <param name="unitDataArray"></param>
    public void SetUnitData(UnitCard[] unitDataArray)
    {
        _uiBattleSquadLayout.SetUnitData(unitDataArray);
    }


    //InputManager ???? ????
    //private void Update()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) { 
    //            var screenPosition = Input.mousePosition;
    //            if (_battleFieldManager.IsOrder())
    //                _uiUnitSelector.ShowSelectorMenu(TYPE_TEAM.Left, screenPosition);
    //        }
    //    }

    //    if (Input.GetKeyDown(KeyCode.Escape))
    //    {
    //        _uiUnitSelector.CloseMenu();

    //        if (UICommon.Current.IsCanvasActivated())
    //            UICommon.Current.NowCanvasHide();
    //    }
    //}


    public void ClickAction(Vector2 screenPosition)
    {
        if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            if (_battleFieldManager.IsOrder())
                _uiUnitSelector.ShowSelectorMenu(TYPE_TEAM.Left, screenPosition);
        }
    }

    public void EscapeAction()
    {
        _uiUnitSelector.CloseMenu();

        if (UICommon.Current.IsCanvasActivated())
            UICommon.Current.NowCanvasHide();
    }

    private void RetryGame()
    {
        AudioManager.InactiveAudio("BGMGrass", AudioManager.TYPE_AUDIO.BGM);
        LoadManager.SetNextSceneName("Test_BattleField");
        UnityEngine.SceneManagement.SceneManager.LoadScene(LoadManager.LoadSceneName);
        BattleFieldOutpost.Current.AllRecovery();
    }

    /// <summary>
    /// ?????????? ??????????
    /// </summary>
    private void ReturnMockGame()
    {
        AudioManager.InactiveAudio("BGMGrass", AudioManager.TYPE_AUDIO.BGM);
        LoadManager.SetNextSceneName("Test_MockGame");
        UnityEngine.SceneManagement.SceneManager.LoadScene(LoadManager.LoadSceneName);
    }

    /// <summary>
    /// ???????? ??????????
    /// </summary>
    private void ReturnMainTitle()
    {
        AudioManager.InactiveAudio("BGMGrass", AudioManager.TYPE_AUDIO.BGM);
        LoadManager.SetNextSceneName("Test_MainTitle");
        UnityEngine.SceneManagement.SceneManager.LoadScene(LoadManager.LoadSceneName);
    }

    /// <summary>
    /// ???????????? ?????? ??????
    /// </summary>
    public void ActivateUnitCardPanel()
    {
        _uiBattleSquadLayout.gameObject.SetActive(true);
        _nextTurnButton.gameObject.SetActive(true);
        _uiBattleSupplyLayout.gameObject.SetActive(true);
        _uiBattleCommandLayout.Hide();
        _uiBattleTurnPanel.SetAnimator(false);
    }

    /// <summary>
    /// ???????? ?????? ??????
    /// </summary>
    public void ActivateBattleRoundPanel()
    {
        _uiBattleSquadLayout.gameObject.SetActive(false);
        _nextTurnButton.gameObject.SetActive(true);
        _uiBattleSupplyLayout.gameObject.SetActive(false);
        _uiBattleCommandLayout.Show();
        _uiBattleTurnPanel.SetAnimator(false);
    }

    /// <summary>
    /// ???????? ?????? ??????
    /// </summary>
    public void ActivateBattlePanel()
    {
        _uiBattleSquadLayout.gameObject.SetActive(false);
        _nextTurnButton.gameObject.SetActive(false);
        _uiBattleSupplyLayout.gameObject.SetActive(false);
        _uiBattleCommandLayout.Hide();
        _uiBattleTurnPanel.SetAnimator(true);
    }   

    /// <summary>
    /// ???? ?????? ??????????
    /// </summary>
    private void NextTurnEvent()
    {
        if(_uiBattleCommandLayout.isActiveAndEnabled)
            _battleFieldManager.SetTypeBattleTurns(TYPE_TEAM.Left, _uiBattleCommandLayout.GetTypeBattleTurnArray());

        _nextTurnEvent?.Invoke();
        
    }

    private void MenuEvent()
    {
        Time.timeScale = 0f;
        _uiBattleFieldMenu.Show();
    }

    private void MenuClosedEvent()
    {
    }

    private void HelpEvent()
    {
        Time.timeScale = 0f;
        var ui = UICommon.Current.GetUICommon<UIHelpInformation>();
        ui.Show(4);
        ui.SetOnClosedListener(delegate
        {
            ReturnEvent();
            ui.SetOnClosedListener(null);
        }
        );
    }

    private void ReturnEvent()
    {
        ReturnTimeScale();
    }

    private void ReturnTimeScale()
    {
        Time.timeScale = 1f;
    }

    private void RetryEvent()
    {
        var ui = UICommon.Current.GetUICommon<UIPopup>();
        ui.ShowOkAndCancelPopup("?????? ?????? ???? ?????????????????", "??", "??????", delegate
        {
            RetryGame();
        }, null, closedCallback: ReturnTimeScale);
    }

    private void SurrenderEvent()
    {
        var ui = UICommon.Current.GetUICommon<UIPopup>();
        ui.ShowOkAndCancelPopup("?????? ???????? ?????? ?????????????????", "??", "??????", delegate
        {
            ReturnMockGame();
        }, null, closedCallback: ReturnTimeScale);
    }



    /// <summary>
    /// ?????? ????????
    /// </summary>
    /// <param name="typeBattleResult"></param>
    public void GameEnd(TYPE_BATTLE_RESULT typeBattleResult)
    {
        AudioManager.InactiveAudio("BGMGrass", AudioManager.TYPE_AUDIO.BGM);

        BattleFieldOutpost.Current.AllRecovery();

        var ui = UICommon.Current.GetUICommon<UIPopup>();
        switch (typeBattleResult)
        {
            case TYPE_BATTLE_RESULT.Defeat:
                AudioManager.ActivateAudio("BGMDefeat", AudioManager.TYPE_AUDIO.BGM, false);
                ui.ShowOkAndCancelPopup("????", "??????", "????", ReturnMockGame, delegate {
                    AudioManager.InactiveAudio("BGMDefeat", AudioManager.TYPE_AUDIO.BGM);
                    ReturnMainTitle();
                });
                break;
            case TYPE_BATTLE_RESULT.Victory:
                AudioManager.ActivateAudio("BGMVictory", AudioManager.TYPE_AUDIO.BGM, false);
                if (BattleFieldOutpost.Current.IsChallengeEnd())
                {
                    ui.ShowApplyPopup("?????????? ??????????????.\n?????????????? ??????????", "????", delegate
                    {
                        AudioManager.InactiveAudio("BGMVictory", AudioManager.TYPE_AUDIO.BGM);
                        ReturnMainTitle();
                    });
                }
                else
                {
                    BattleFieldOutpost.Current.AddChallengeLevel();
                    ui.ShowApplyPopup("????", "??????????", delegate
                    {
                        AudioManager.InactiveAudio("BGMVictory", AudioManager.TYPE_AUDIO.BGM);
                        ReturnMockGame();
                    });
                }
                break;
            case TYPE_BATTLE_RESULT.Draw:
                AudioManager.ActivateAudio("BGMDefeat", AudioManager.TYPE_AUDIO.BGM, false);
                ui.ShowOkAndCancelPopup("??????", "??????", "????", delegate {
                    AudioManager.InactiveAudio("BGMDefeat", AudioManager.TYPE_AUDIO.BGM);
                    ReturnMockGame();
                }, delegate {
                    AudioManager.InactiveAudio("BGMDefeat", AudioManager.TYPE_AUDIO.BGM);
                    ReturnMainTitle(); 
                });
                break;
        }
    }

    public void SetBattleTurnOrder(TYPE_TEAM typeTeam, TYPE_BATTLE_TURN typeBattleTurn)
    {
        _uiBattleTurnPanel.SetBattleTurnOrderText(typeTeam, typeBattleTurn);
    }

    public void ActivateUnitSetting(bool isActive)
    {
        _uiUnitSettingsPanel.SetActive(isActive);
    }

    #region ##### Listener #####

    /// <summary>
    /// ?????? ???? ??????
    /// </summary>
    private System.Action _nextTurnEvent;

    public void SetOnNextTurnListener(System.Action act) => _nextTurnEvent = act;



    /// <summary>
    /// ???? ???? ??????
    /// </summary>
    /// <param name="uActor"></param>
    /// <param name="screenPosition"></param>
    private void ShowUnitInformationEvent(UnitActor uActor, Vector2 screenPosition)
    {
        var ui = UICommon.Current.GetUICommon<UIUnitInformation>();
        ui.Show(uActor);
        ui.SetPosition(screenPosition);
    }

    /// <summary>
    /// ???? ???? ??????
    /// </summary>
    /// <param name="uCard"></param>
    /// <param name="screenPosition"></param>
    private void ShowUnitInformationEvent(UnitCard uCard, Vector2 screenPosition)
    {
        var ui = UICommon.Current.GetUICommon<UIUnitInformation>();
        ui.Show(uCard, screenPosition);
    }

    /// <summary>
    /// ???? ???? ???? ??????
    /// </summary>
    /// <param name="uActor"></param>
    public void OnUnitModifiedClickedEvent(UnitActor uActor)
    {
        _battleFieldManager.DragUnit(uActor);
    }

    /// <summary>
    /// ???? ???? ??????
    /// </summary>
    /// <param name="uActor"></param>
    public void OnUnitReturnClickedEvent(UnitActor uActor)
    {
        if (_uiBattleSquadLayout.ReturnUnit(uActor))
            _battleFieldManager.ReturnUnit(uActor);
    }

    /// <summary>
    /// ???? ???? ???? ??????
    /// </summary>
    public void OnUnitCancelClickedEvent()
    {
        _battleFieldManager.CancelChangeUnit();
    }


   
    #endregion
}

