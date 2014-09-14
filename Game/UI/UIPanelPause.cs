#define DEV
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

public class UIPanelPause : UIPanelBase {

    public GameObject listItemPrefab;
	
    public UIImageButton buttonResume;
    public UIImageButton buttonRestart;
    public UIImageButton buttonQuit;
    public UIImageButton buttonSettingsAudio;
	
	public static UIPanelPause Instance;
	
	public UISlider sliderMusicVolume;
	public UISlider sliderEffectsVolume;	

	public void Awake() {
		
        if (Instance != null && this != Instance) {
            //There is already a copy of this script running
            //Destroy(gameObject);
            return;
        }
		
        Instance = this;	
        
        panelTypes.Add(UIPanelBaseTypes.typeDialogHUD);
	}
	
	public static bool isInst {
		get {
			if(Instance != null) {
				return true;
			}
			return false;
		}
	}	
	
	public override void Init() {
		base.Init();	
		
		loadData();
	}	
	
	public override void Start() {
		Init();
	}
	
    public override void OnEnable() {
        base.OnEnable();

        Messenger<string>.AddListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);
		Messenger<string, float>.AddListener(SliderEvents.EVENT_ITEM_CHANGE, OnSliderChangeEventHandler);

    }
    
    public override void OnDisable() {
        base.OnDisable();

		Messenger<string>.RemoveListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);
		Messenger<string, float>.RemoveListener(SliderEvents.EVENT_ITEM_CHANGE, OnSliderChangeEventHandler);

    }
	
    public override void OnButtonClickEventHandler(string buttonName) {

	}
	
    void OnSliderChangeEventHandler(string sliderName, float sliderValue) {

        //LogUtil.Log("OnSliderChangeEventHandler: sliderName:" + sliderName + " sliderValue:" + sliderValue );
		
		bool changeAudio = true;
		
#if DEV	
		if(Application.isEditor) {
            //GameProfiles.Current.SetAudioMusicVolume(GameGlobal.volumeEditor);
            //GameProfiles.Current.SetAudioEffectsVolume(GameGlobal.volumeEditor);
			//changeAudio = false;
		}
#endif
		
		if(!changeAudio) {
			return;
		}

        if(sliderEffectsVolume != null) {
            if (sliderName == sliderEffectsVolume.name) {
    			//GameAudio.SetProfileEffectsVolume(sliderValue);
            }
        }

        if(sliderMusicVolume != null) {
            if (sliderName == sliderMusicVolume.name) {
			    //GameAudio.SetProfileAmbienceVolume(sliderValue);
            }
        }
    }

    public static void ShowDefault() {
        if(isInst) {
            Instance.AnimateIn();
        }
    }

    public static void HideAll() {
        if(isInst) {
            Instance.AnimateOut();
        }
    }
	
	public static void LoadData() {
		if(Instance != null) {
			Instance.loadData();
		}
	}


    public void UpdateAudioValues() {
        float effectsVolume = (float)GameProfiles.Current.GetAudioEffectsVolume();
        float musicVolume = (float)GameProfiles.Current.GetAudioMusicVolume();
        
        if(sliderMusicVolume != null) {
            sliderMusicVolume.sliderValue = musicVolume;
            sliderMusicVolume.ForceUpdate();

            GameAudio.SetProfileEffectsVolume(musicVolume);
        }
        
        if(sliderEffectsVolume != null) {
            sliderEffectsVolume.sliderValue = effectsVolume;
            sliderEffectsVolume.ForceUpdate();
        
            GameAudio.SetProfileAmbienceVolume(effectsVolume);
        }
    }
 
    public void loadData() {
        StartCoroutine(loadDataCo());
    }
    
    IEnumerator loadDataCo() {
    
        yield return new WaitForSeconds(1f);
    
        //UpdateAudioValues();
    }
	
}
