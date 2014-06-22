using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

public class BaseGameUIPanelBackgrounds : GameUIPanelBase {
    
    public static GameUIPanelBackgrounds Instance;
	
	public GameObject containerObject;

	public GameObject backgroundPlainObject;
	public GameObject backgroundStarryObject;
	public GameObject backgroundStarryTopObject;
	public GameObject backgroundScaryObject;
	public GameObject backgroundGlowObject;
	public GameObject backgroundCloudLeftObject;
    public GameObject backgroundCloudRightObject;
    
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

    public virtual void OnButtonClickEventHandler(string buttonName) {

    }
	
	public override void Start() {
		Init();
	}
	
	public override void Init() {
		base.Init();

        AnimateInCenter(0,0);		
		//LoadData();
		//AnimateIn();
	}
	
	public virtual void LoadData() {
		StartCoroutine(LoadDataCo());
	}
	
	IEnumerator LoadDataCo() {
		yield break;
	}
	
    public virtual void ShowBackgroundPlain() {
		if(backgroundPlainObject != null) {
			UITweenerUtil.FadeTo(backgroundPlainObject, 
				UITweener.Method.Linear, UITweener.Style.Once, durationShow, durationDelayShow, 1f);
		}
	}
	
    public virtual void HideBackgroundPlain() {
		if(backgroundPlainObject != null) {
			UITweenerUtil.FadeTo(backgroundPlainObject, 
				UITweener.Method.Linear, UITweener.Style.Once, durationHide, durationDelayHide, 0f);
		}
	}
		
    public virtual void ShowBackgroundStarry() {
		if(backgroundStarryObject != null) {
			UITweenerUtil.FadeTo(backgroundStarryObject, 
				UITweener.Method.Linear, UITweener.Style.Once, durationShow, durationDelayShow, 1f);
		}
	}
	
    public virtual void HideBackgroundStarry() {
		if(backgroundStarryObject != null) {
			UITweenerUtil.FadeTo(backgroundStarryObject, 
				UITweener.Method.Linear, UITweener.Style.Once, durationHide, durationDelayHide, 0f);
		}
	}
	
    public virtual void ShowBackgroundStarryTop() {
		if(backgroundStarryTopObject != null) {
			UITweenerUtil.FadeTo(backgroundStarryTopObject, 
				UITweener.Method.Linear, UITweener.Style.Once, durationShow, durationDelayShow, 1f);
		}
	}
	
    public virtual void HideBackgroundStarryTop() {
		if(backgroundStarryTopObject != null) {
			UITweenerUtil.FadeTo(backgroundStarryTopObject, 
				UITweener.Method.Linear, UITweener.Style.Once, durationHide, durationDelayHide, 0f);
		}
	}
	
    public virtual void ShowBackgroundScary() {
		if(backgroundScaryObject != null) {
			UITweenerUtil.FadeTo(backgroundScaryObject, 
				UITweener.Method.Linear, UITweener.Style.Once, durationShow, durationDelayShow, 1f);
		}
	}
	
    public virtual void HideBackgroundScary() {
		if(backgroundScaryObject != null) {
			UITweenerUtil.FadeTo(backgroundScaryObject, 
				UITweener.Method.Linear, UITweener.Style.Once, durationHide, durationDelayHide, 0f);
		}
	}
	
    public virtual void ShowBackgroundGlow() {
		if(backgroundGlowObject != null) {
			UITweenerUtil.FadeTo(backgroundGlowObject, 
				UITweener.Method.Linear, UITweener.Style.Once, durationShow, durationDelayShow, 1f);
		}
	}
	
    public virtual void HideBackgroundGlow() {
		if(backgroundGlowObject != null) {	
			
			UITweenerUtil.FadeTo(backgroundGlowObject, 
				UITweener.Method.Linear, UITweener.Style.Once, durationHide, durationDelayHide, 0f);
		}
	}
	
    public virtual void ShowCloudLeft() {
		if(backgroundCloudLeftObject != null) {
			UITweenerUtil.MoveTo(backgroundCloudLeftObject, 
				UITweener.Method.EaseInOut, UITweener.Style.Once, durationShow, durationDelayShow, Vector3.zero.WithX(0));	
			
			UITweenerUtil.FadeTo(backgroundCloudLeftObject, 
				UITweener.Method.Linear, UITweener.Style.Once, durationShow, durationDelayShow, 1f);
		}
	}
	
    public virtual void HideCloudLeft() {
		if(backgroundCloudLeftObject != null) {			
			UITweenerUtil.MoveTo(backgroundCloudLeftObject, 
				UITweener.Method.EaseInOut, UITweener.Style.Once, durationHide, durationDelayHide, Vector3.zero.WithX(leftClosedX));	
			
			UITweenerUtil.FadeTo(backgroundCloudLeftObject, 
				UITweener.Method.Linear, UITweener.Style.Once, durationHide, durationDelayHide, 0f);
		}
	}
	
    public virtual void ShowCloudRight() {
		if(backgroundCloudRightObject != null) {
			UITweenerUtil.MoveTo(backgroundCloudRightObject, 
				UITweener.Method.EaseInOut, UITweener.Style.Once, durationShow, durationDelayShow, Vector3.zero.WithX(0));		
			
			UITweenerUtil.FadeTo(backgroundCloudRightObject, 
				UITweener.Method.Linear, UITweener.Style.Once, durationShow, durationDelayShow, 1f);
		}
	}
	
    public virtual void HideCloudRight() {
		if(backgroundCloudRightObject != null) {
			UITweenerUtil.MoveTo(backgroundCloudRightObject, 
				UITweener.Method.EaseInOut, UITweener.Style.Once, durationHide, durationDelayHide, Vector3.zero.WithX(rightClosedX));			
			
			UITweenerUtil.FadeTo(backgroundCloudRightObject, 
				UITweener.Method.Linear, UITweener.Style.Once, durationHide, durationDelayHide, 0f);
		}
	}
	
	
	public override void AnimateIn() {
		
		base.AnimateIn();
		
		ShowBackgroundPlain();
		ShowBackgroundGlow();
		ShowBackgroundScary();
		
		HideBackgroundStarry();
		HideBackgroundStarryTop();
		HideCloudLeft();
		HideCloudRight();
	}
	
    public virtual void AnimateInStarry() {
		
		base.AnimateIn();
		
		ShowBackgroundPlain();
		ShowBackgroundGlow();
		ShowBackgroundStarry();
		ShowBackgroundStarryTop();
		
		ShowCloudLeft();
		ShowCloudRight();
		
		HideBackgroundScary();
	}
	
    public virtual void AnimateInScary() {
		
		base.AnimateIn();
		
		ShowBackgroundPlain();
		ShowBackgroundGlow();
		ShowBackgroundScary();
		
		HideBackgroundStarry();
		HideBackgroundStarryTop();
	}
	
	public override void AnimateOut() {
		
		base.AnimateOut();
		
		HideBackgroundGlow();
		HideBackgroundStarry();
		HideBackgroundStarryTop();
		HideBackgroundScary();
		HideBackgroundPlain();
		HideCloudLeft();
		HideCloudRight();
	}
	
    public virtual void Update() {

        if(GameConfigs.isGameRunning) {
            return;
        }

        if(!isVisible) {
            return;
        }
		
		var ry = 0f;
		//var rx = 0f;
		if(Context.Current.isMobile) {
			ry =-Input.acceleration.y + Screen.height/2;	
			//rx =-Input.acceleration.x + Screen.width/2;	
		}
		else {
			ry =-Input.mousePosition.y + Screen.height/2;	
			//rx =-Input.acceleration.x + Screen.width/2;				
		}
	
		if(backgroundStarryObject != null) {
			backgroundStarryObject.transform.Rotate(Vector3.forward * (ry * .005f) * Time.deltaTime);
		}
		
		if(backgroundStarryTopObject != null) {
			backgroundStarryTopObject.transform.Rotate(Vector3.forward * (ry * .0045f) * Time.deltaTime);
		}
	}
	
	/*

    void OnButtonClickEventHandler(string buttonName) {
		LogUtil.Log("OnButtonClickEventHandler: " + buttonName);
		
		if(buttonName == buttonNorahGlowObject.name 
			|| buttonName == buttonNorahStaticObject.name) {
			LogUtil.Log("Norah Clicked: " + buttonName);
		}

    }
	
    void OnListItemClickEventHandler(string listName, string listIndex, bool selected) {
        LogUtil.Log("OnListItemClickEventHandler: listName:" + listName + " listIndex:" + listIndex.ToString() + " selected:" + selected.ToString());

	}

    void OnListItemSelectEventHandler(string listName, string selectName) {
        LogUtil.Log("OnListItemSelectEventHandler: listName:" + listName + " selectName:" + selectName );

        if(listName == "ListState") {

        }
    }

    void OnSliderChangeEventHandler(string sliderName, float sliderValue) {
        LogUtil.Log("OnSliderChangeEventHandler: sliderName:" + sliderName + " sliderValue:" + sliderValue );

        // Change appstate

        if(sliderName == "AudioEffectsSlider") {
            //GameProfiles.Current.SetAudioEffectsVolume(sliderValue);
        }
    }
    
    void OnCheckboxChangeEventHandler(string checkboxName, bool selected) {
        LogUtil.Log("OnCheckboxChangeEventHandler: checkboxName:" + checkboxName + " selected:" + selected );
        
        // Change appstate
        
        if(checkboxName == "DeviceModeBestCheckbox") {
            //CameraDevice.Instance.SetFocusMode(
        }
    }
    */
	
}
