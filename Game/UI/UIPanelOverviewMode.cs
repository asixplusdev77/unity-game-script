#define DEV
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Engine.Events;

public class UIPanelOverviewMode : UIPanelBase {

    public static UIPanelOverviewMode Instance;
    public GameObject containerOverview;
    public GameObject containerOverviewGameplayTips;
    public GameObject containerTutorial;
    
    public GameObject containerTips;
    public GameObject containerTipsMode;
    public GameObject containerTipsGameplay;

    // OVERVIEW

    public UILabel labelOverviewTip;
    public UILabel labelOverviewType;
    public UILabel labelOverviewStatus;

    public UIImageButton buttonOverviewReady;
    public UIImageButton buttonOverviewTutorial;
    public UIImageButton buttonOverviewTips;
    public UIImageButton buttonOverviewMode;

    //public UIPanelTips tips

    // GLOBAL

    public AppOverviewFlowState flowState = AppOverviewFlowState.Mode;

    public void Awake() {
        
        if (Instance != null && this != Instance) {
            //There is already a copy of this script running
            //Destroy(gameObject);
            return;
        }
        
        Instance = this;    
    }
    
    public static bool isInst {
        get {
            if (Instance != null) {
                return true;
            }
            return false;
        }
    }
    
    public override void Init() {
        base.Init();
    }
    
    public override void Start() {
        Init();
    }

    // EVENTS
    
    public override void OnEnable() {

        base.OnEnable();

        Messenger<string>.AddListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);

        Messenger.AddListener(GameDraggableEditorMessages.GameLevelItemsLoaded, OnGameLevelItemsLoadedHandler);
    }
    
    public override void OnDisable() {

        base.OnDisable();

        Messenger<string>.RemoveListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);

        Messenger.RemoveListener(GameDraggableEditorMessages.GameLevelItemsLoaded, OnGameLevelItemsLoadedHandler);
    }

    void OnButtonClickEventHandler(string buttonName) {
        if (UIUtil.IsButtonClicked(buttonOverviewReady, buttonName)) {
            Ready();
        }
        else if (UIUtil.IsButtonClicked(buttonOverviewMode, buttonName)) {
            ChangeTipsState(AppOverviewFlowState.Mode);
        }
        else if (UIUtil.IsButtonClicked(buttonOverviewTutorial, buttonName)) {
            ShowTutorial();
        }
        else if (UIUtil.IsButtonClicked(buttonOverviewTips, buttonName)) {
            ChangeTipsState(AppOverviewFlowState.GameplayTips);
        }
    }
        
    public void ContentPause() {
        GameController.GameRunningStateContent();
    }
    
    public void ContentRun() {
        GameController.GameRunningStateRun();
    }

    public void Ready() {
        HideAll();
    }

    public void ChangeTipsState(AppOverviewFlowState flowStateTo) {
        flowState = flowStateTo;
        UpdateTipsStates();
    }


    public void UpdateTipsStates() {
    
        if(flowState == AppOverviewFlowState.GameplayTips) {
            ShowTipsObjectGameplay();
        }
        else {            
            ShowTipsObjectMode();  
        }
    }
    
    public void ShowTipsObjectGameplay() {
        UIUtil.HideButton(buttonOverviewMode);
        UIUtil.ShowButton(buttonOverviewTips);
        ShowTipsObject("gameplay");    
    }

    public void ShowTipsObjectMode() {
        UIUtil.HideButton(buttonOverviewTips);
        UIUtil.ShowButton(buttonOverviewMode);
        string currentAppContentState = AppContentStates.Current.code;
        ShowTipsObject(currentAppContentState);
    }

    public void HideTipsObjects() {        
        if(containerTips != null) {            
            foreach(UIPanelTips tips in containerTips.GetComponentsInChildren<UIPanelTips>(true)) {   
                tips.gameObject.Hide();             
                UITweenerUtil.FadeTo(tips.gameObject, UITweener.Method.Linear, UITweener.Style.Once, .4f, 0f, 0f);
            }
        }    
    }

    public void ShowTipsObject(string objName) {

        if(containerTips != null) {

            HideTipsObjects();

            foreach(UIPanelTips tips in containerTips.GetComponentsInChildren<UIPanelTips>(true)) {
                
                if(!string.IsNullOrEmpty(objName) && tips.name.Contains(objName)) {
                    tips.gameObject.Show();
                    UITweenerUtil.FadeTo(tips.gameObject, UITweener.Method.Linear, UITweener.Style.Once, 0f, 0f, 0f);
                    tips.ShowTipsFirst();
                    UITweenerUtil.FadeTo(tips.gameObject, UITweener.Method.Linear, UITweener.Style.Once, .5f, .6f, 1f);

                }
            }   
        }
    }
    
    public void ShowTutorial() {
        
        HideStates();

        flowState = AppOverviewFlowState.Tutorial;
        
        UIPanelDialogBackground.ShowDefault();
        
        UIUtil.SetLabelValue(labelOverviewType, AppContentStates.Current.display_name);
        
        //Debug.Log("UIPanelModeTypeChoice:ShowOverview:flowState:" + flowState);
        
        AnimateInBottom(containerTutorial);
        
        ContentPause();
        
        UIColors.UpdateColors();

    }
    
    public void HideTutorial() {
        
        AnimateOutBottom(containerOverview, 0f, 0f);
        
        ContentRun();
    }
    
    public void ShowTips() {

        HideStates();

        flowState = AppOverviewFlowState.GameplayTips;
        
        UIPanelDialogBackground.ShowDefault();
        
        UIUtil.SetLabelValue(labelOverviewType, AppContentStates.Current.display_name);
        
        //Debug.Log("UIPanelModeTypeChoice:ShowOverview:flowState:" + flowState);
        
        AnimateInBottom(containerOverviewGameplayTips);
        
        ContentPause();
        
        UIColors.UpdateColors();

    }
    
    public void HideTips() {
        
        AnimateOutBottom(containerOverview, 0f, 0f);
        
        ContentRun();
    }

    public void OnGameLevelItemsLoadedHandler() {

        Debug.Log("OnGameLevelItemsLoadedHandler");

        if (AppModeTypes.Instance.isAppModeTypeGameChoice) {

            Debug.Log("OnGameLevelItemsLoadedHandler2");
        }
    }

    public void ShowOverview() {

        HideStates();

        flowState = AppOverviewFlowState.Mode;

        UIPanelDialogBackground.ShowDefault();

        UIUtil.SetLabelValue(labelOverviewType, AppContentStates.Current.display_name);

        AnimateInBottom(containerOverview);

        ContentPause();

        UIColors.UpdateColors();
    }

    public void HideOverview() {

        AnimateOutBottom(containerOverview, 0f, 0f);

        ContentRun();
    }

    public void ShowCurrentState() {        
        ShowOverview();
    }
    
    public void HideStates() {        

        UIPanelDialogBackground.HideAll();
        
        UIPanelDialogRPGHealth.HideAll();
        UIPanelDialogRPGEnergy.HideAll();

        HideOverview();
        HideTutorial();
        HideTips();
    }

    // SHOW/LOAD

    public static void ShowDefault() {
        if (isInst) {
            Instance.AnimateIn();
        }
    }

    public static void HideAll() {
        if (isInst) {
            Instance.AnimateOut();
        }
    }

    public void Reset() {
        flowState = AppOverviewFlowState.Mode;
    }

    public static void LoadData() {
        if (Instance != null) {
            Instance.loadData();
        }
    }

    public void loadData() {
        //Debug.Log("UIPanelModeTypeChoice:loadData");
        StartCoroutine(loadDataCo());
    }
    
    IEnumerator loadDataCo() {
        yield return new WaitForSeconds(1f);

        Reset();

        ShowCurrentState();
        
        UpdateTipsStates();
    }

    public override void AnimateIn() {
        base.AnimateIn();

        loadData();
    }

    public override void AnimateOut() {
        base.AnimateOut();

        HideStates();
    }

    public void Update() {

    }
    
}
