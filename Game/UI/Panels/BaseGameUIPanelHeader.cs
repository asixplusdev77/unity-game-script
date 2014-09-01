using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

public class BaseGameUIPanelHeader : GameUIPanelBase {

    public static GameUIPanelHeader Instance;
		
	/*
	easeInQuad
	easeOutQuad
	easeInOutQuad
	easeInCubic
	easeOutCubic
	easeInOutCubic
	easeInQuart
	easeOutQuart
	easeInOutQuart
	easeInQuint
	easeOutQuint
	easeInOutQuint
	easeInSine
	easeOutSine
	easeInOutSine
	easeInExpo
	easeOutExpo
	easeInOutExpo
	easeInCirc
	easeOutCirc
	easeInOutCirc
	linear
	spring
	easeInBounce
	easeOutBounce
	easeInOutBounce
	easeInBack
	easeOutBack
	easeInOutBack
	easeInElastic
	easeOutElastic
	easeInOutElastic
	
	*/
	
	public GameObject coinObject;
	public GameObject backObject;
	public GameObject backerObject;
	public GameObject titleObject;

    public GameObject containerCharacters;
    public GameObject containerCharacter;
	
	public UIImageButton buttonCoins;
	public UIImageButton buttonBack;
	
    public UILabel labelSection;
    
    public static bool isInst {
        get {
            if(Instance != null) {
                return true;
            }
            return false;
        }
    }
    
    public virtual void Awake() {
        
    }
	
	public override void Start() {
		Init();
	}
	
	public override void Init() {
		base.Init();	
		loadData();
		
		base.AnimateIn();	
	}
	
    public override void OnEnable() {

        Messenger<string>.AddListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);

        Messenger<string>.AddListener(
            UIControllerMessages.uiPanelAnimateIn,
            OnUIControllerPanelAnimateIn);

        Messenger<string>.AddListener(
            UIControllerMessages.uiPanelAnimateOut,
            OnUIControllerPanelAnimateOut);

        Messenger<string, string>.AddListener(
            UIControllerMessages.uiPanelAnimateType,
            OnUIControllerPanelAnimateType);
    }

    public override void OnDisable() {

        Messenger<string>.RemoveListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);

        Messenger<string>.RemoveListener(
            UIControllerMessages.uiPanelAnimateIn,
            OnUIControllerPanelAnimateIn);

        Messenger<string>.RemoveListener(
            UIControllerMessages.uiPanelAnimateOut,
            OnUIControllerPanelAnimateOut);

        Messenger<string, string>.RemoveListener(
            UIControllerMessages.uiPanelAnimateType,
            OnUIControllerPanelAnimateType);
    }

    public override void OnUIControllerPanelAnimateIn(string classNameTo) {

        if(className == classNameTo) {
            AnimateIn();
        }
    }

    public override void OnUIControllerPanelAnimateOut(string classNameTo) {

        if(className == classNameTo) {
            AnimateOut();
        }
    }

    public override void OnUIControllerPanelAnimateType(string classNameTo, string code) {
        
        HideCharacters();

        if(className == classNameTo) {
           //

            if(code.Contains("-internal")) {
                AnimateInInternal();
            }
        }
    }

    public virtual void OnButtonClickEventHandler(string buttonName) {
		//LogUtil.Log("OnButtonClickEventHandler: " + buttonName);
		
		if(buttonName == buttonCoins.name) {
			GameUIController.ShowProductCurrency();
		}
		else if(buttonName == buttonBack.name) {
			
			bool gotoMain = true;
			/*
			if(BaseGameUIPanelStore.Instance.panelMode == UIAppPanelMode.ModeList) {
				gotoMain = false;
				BaseGameUIPanelStore.Instance.AnimateInMain();
			}
			else if(BaseGameUIPanelSettings.Instance.panelMode == UIAppPanelMode.ModeList) {
				gotoMain = false;
				//BaseGameUIPanelSettings.Instance.ShowMain();
			}
			*/


			//if(GameUIController.Instance.uiVisible) {
			//	GameUIController.ShowMain();
			//}
			
			if(GameUIController.Instance.uiVisible) {
				if(gotoMain) {
					//GameUIController.ShowMain();
				}
			}
		}

    }
	
	public override void AnimateIn() {

        backgroundDisplayState = GameBackgroundDisplayState.None;
		
		base.AnimateIn();
	}
	
	public virtual void AnimateInMain() {
		
		AnimateIn();
		
		showMain();
	}
	
	public virtual void AnimateInInternal() {
		
		AnimateIn();
		
		showFull();
	}
	
	public override void AnimateOut() {
		base.AnimateOut();
		
		HideBackButtonObject();
		HideBackerObject();
		HideCoinsObject();
		HideTitleObject();

        HideCharacter();
	}
	
	public static void HideTitle() {
        if(GameUIPanelHeader.Instance != null) {
            GameUIPanelHeader.Instance.hideTitle();
		}
	}
	
    public virtual void hideTitle() {
		UIUtil.HideLabel(labelSection);
	}
	
	
	public static void ShowTitle(string title) {
		if(GameUIPanelHeader.Instance != null) {
            GameUIPanelHeader.Instance.showTitle(title);
		}
	}
	
    public virtual void showTitle(string title) {
		UIUtil.ShowLabel(labelSection);
		UIUtil.SetLabelValue(labelSection, title);
	}
	
	public static void ShowFull() {
        if(GameUIPanelHeader.Instance != null) {
            GameUIPanelHeader.Instance.showFull();
		}
	}
	
    public virtual void showFull() {
		ShowCoinsObject();
		ShowBackerObject();
		ShowBackButtonObject();
		ShowTitleObject();
	}
	
	public static void ShowMain() {
        if(GameUIPanelHeader.Instance != null) {
            GameUIPanelHeader.Instance.showMain();
		}
	}
	
    public virtual void showMain() {
		ShowCoinsObject();
		HideBackerObject();
		HideBackButtonObject();
		HideTitleObject();
	}
	
	public static void ShowNone() {
        if(GameUIPanelHeader.Instance != null) {
            GameUIPanelHeader.Instance.showNone();
		}
	}
	
    public virtual void showNone() {
		AnimateOut();
	}

    // characters

    public static void HideCharacters() {
        HideCharacter();
    }
        
    // characters 
    
    public static void ShowCharacter() {
        if(GameUIPanelHeader.Instance != null) {
            GameUIPanelHeader.Instance.showCharacter();
        }
    }
    
    public virtual void showCharacter() {
        StartCoroutine(showCharacterCo());
    }

    public IEnumerator showCharacterCo() {
        yield return new WaitForSeconds(.55f);
        ShowPanelTop(containerCharacter, false);
    }
    
    public static void HideCharacter() {
        if(GameUIPanelHeader.Instance != null) {
            GameUIPanelHeader.Instance.hideCharacter();
        }
    }
    
    public virtual void hideCharacter() {
        HidePanelTop(containerCharacter, false);
    }

	
	//public void ShowObjectDelayed(GameObject obj, float delay) {
	//	StartCoroutine(ShowObjectDelayedCo(obj, delay));
	//}
	
	//public void HideObjectDelayed(GameObject obj, float delay) {
	//	StartCoroutine(HideObjectDelayedCo(obj, delay));
	//}
		
	IEnumerator ShowObjectDelayedCo(GameObject obj, float delay) {
		yield return new WaitForSeconds(delay);
		ShowObject(obj);
	}
	
	IEnumerator HideObjectDelayedCo(GameObject obj, float delay) {
		yield return new WaitForSeconds(delay);
		HideObject(obj);
	}
	
	//public void ShowObject(GameObject obj) {
	//	if(obj != null) {
	//		obj.Show();
	//	}
	//}
	
	//public void HideObject(GameObject obj) {
	//	if(obj != null) {
	//		obj.Hide();
	//	}
	//}	
	
    public virtual void ShowBackButtonObject() {
		if(backObject != null) {
			ShowObject(backObject);

			//iTween.FadeTo(backObject, iTween.Hash(
			//	"easetype", "linear",
			//	"alpha", 1f,
			//	"time", .5f,
			//	"delay", 0f
			//));


			UITweenerUtil.FadeTo(backObject,
				UITweener.Method.EaseInOut, UITweener.Style.Once, 1f, .3f, 1f);
			foreach(Transform t in backObject.transform) {
				
				UITweenerUtil.FadeTo(t.gameObject,
					UITweener.Method.EaseInOut, UITweener.Style.Once, 1f, .3f, 1f);
			}
		}
	}
	
    public virtual void HideBackButtonObject() {
		if(backObject != null) {
			//iTween.FadeTo(backObject, iTween.Hash(
			//	"easetype", "linear",
			//	"alpha", 0f,
			//	"time", .5f,
			//	"delay", 0f
			//));

			UITweenerUtil.FadeTo(backObject,
				UITweener.Method.EaseInOut, UITweener.Style.Once, .3f, .3f, 0f);
			
			foreach(Transform t in backObject.transform) {
				
				UITweenerUtil.FadeTo(t.gameObject,
					UITweener.Method.EaseInOut, UITweener.Style.Once, .3f, .3f, 0f);
			}
			
			HideObjectDelayed(backObject, .3f);
		}
	}
	
    public virtual void ShowBackerObject() {
		if(backerObject != null) {
			UITweenerUtil.FadeTo(backerObject, 
				UITweener.Method.EaseInOut, UITweener.Style.Once, 1f, .3f, 1f);
		}
	}
	
    public virtual void HideBackerObject() {
		if(backerObject != null) {
			UITweenerUtil.FadeTo(backerObject, 
				UITweener.Method.EaseInOut, UITweener.Style.Once, .3f, .3f, 0f);
		}
	}
	
	
    public virtual void ShowTitleObject() {
		if(titleObject != null) {
			UITweenerUtil.FadeTo(titleObject, 
				UITweener.Method.EaseInOut, UITweener.Style.Once, 1f, .3f, 1f);
		}
	}
	
    public virtual void HideTitleObject() {
		if(titleObject != null) {
			UITweenerUtil.FadeTo(titleObject, 
				UITweener.Method.EaseInOut, UITweener.Style.Once, .3f, .3f, 0f);
		}
	}
	
    public virtual void ShowCoinsObject() {
		if(coinObject != null) {
			UITweenerUtil.FadeTo(coinObject, 
				UITweener.Method.EaseInOut, UITweener.Style.Once, 1f, .3f, 1f);
		}
	}
	
    public virtual void HideCoinsObject() {
		if(coinObject != null) {
			UITweenerUtil.FadeTo(coinObject, 
				UITweener.Method.EaseInOut, UITweener.Style.Once, .3f, .3f, 0f);
		}
	}
	
	public static void LoadData() {
        if(GameUIPanelHeader.Instance != null) {
            GameUIPanelHeader.Instance.loadData();
		}
	}
	
    public virtual void loadData() {
		StartCoroutine(loadDataCo());
	}
	
	IEnumerator loadDataCo() {
		
		yield return new WaitForSeconds(1f);
	}
	
}
