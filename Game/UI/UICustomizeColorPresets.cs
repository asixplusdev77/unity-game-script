using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Data.Json;
using Engine.Events;
using Engine.Utility;

public class UICustomizeColorPresets : UICustomizeSelectObject {
    
    public string type = "character";
    public Camera cameraCustomize;
    public GameObject colorWheelPanel;
    public Dictionary<string, UICheckbox> checkboxes;

    public override void OnEnable() {
        base.OnEnable();
        
        Messenger<string, bool>.AddListener(
            CheckboxEvents.EVENT_ITEM_CHANGE, 
            OnCheckboxChangedEventHandler);
        
        Messenger<string, string>.AddListener(
            GameCustomMessages.customColorPresetChanged, 
            OnCustomColorPresetChanged);
        
        Messenger<Color>.AddListener(GameCustomMessages.customColorChanged, OnCustomColorChanged);
    }
    
    public override void OnDisable() {
        base.OnDisable();        
        
        Messenger<string, bool>.RemoveListener(
            CheckboxEvents.EVENT_ITEM_CHANGE, 
            OnCheckboxChangedEventHandler);
        
        Messenger<string, string>.RemoveListener(
            GameCustomMessages.customColorPresetChanged,
            OnCustomColorPresetChanged);
        
        Messenger<Color>.RemoveListener(GameCustomMessages.customColorChanged, OnCustomColorChanged);
    }

    public override void Start() {
        Load();   
    }
    
    public override void Load() {
        base.Load();

        Init();
    }

    public void Init() {

        checkboxes = new Dictionary<string, UICheckbox>();
        
        foreach (AppContentAssetCustomItem customItem 
                in AppContentAssetCustomItems.Instance.GetListByType(type)) {
                        
            foreach (AppContentAssetCustomItemProperty prop in customItem.properties) {

                checkboxes.Add(prop.code, gameObject.Get<UICheckbox>(prop.code));
            }
        }
    }
    
    public virtual void OnCustomColorChanged(Color color) { 
        
        GameAudio.PlayEffect(GameAudioEffects.audio_effect_ui_button_1); 
        
        currentProfileCustomItem = GameProfileCharacters.currentCustom;

        foreach (AppContentAssetCustomItem customItem 
                in AppContentAssetCustomItems.Instance.GetListByType(type)) {

            Dictionary<string, Color> colors = new Dictionary<string, Color>();

            foreach (AppContentAssetCustomItemProperty prop in customItem.properties) {

                bool update = false;

                foreach (KeyValuePair<string,UICheckbox> pair in checkboxes) {
                    if (pair.Value == null) {
                        LogUtil.Log("Checkbox not found:" + pair.Key);
                        continue;
                    }

                    if (pair.Value.isChecked 
                        && prop.code == pair.Key) {
                        update = true;
                    }
                }

                Color colorTo = currentProfileCustomItem.GetCustomColor(prop.code);

                if (update) {
                    color.a = 1;
                    colorTo = color;
                }

                colors.Add(prop.code, colorTo);
            }        
            
            currentProfileCustomItem = GameCustomController.UpdateColorPresetObject(currentProfileCustomItem, currentObject, type, colors);
            
            GameCustomController.SaveCustomItem(currentProfileCustomItem);
        }
    }
    
    public override void OnButtonClickEventHandler(string buttonName) {
        
        if (UIUtil.IsButtonClicked(buttonCycleLeft, buttonName)) {
            ChangePresetPrevious();
        }
        else if (UIUtil.IsButtonClicked(buttonCycleRight, buttonName)) {
            ChangePresetNext();
        }
    }
    
    public void OnCustomColorPresetChanged(string code, string name) {
        
        //UIUtil.SetLabelValue(labelCurrentDisplayName, name);
    }
    
    void OnCheckboxChangedEventHandler(string checkboxName, bool selected) {
        
        //LogUtil.Log("OnCheckboxChangedEventHandler:", " checkboxName:" + checkboxName + " selected:" + selected);
        if (checkboxes != null) {

            foreach (KeyValuePair<string,UICheckbox> pair in checkboxes) {
                if (UIUtil.IsCheckboxChecked(pair.Value, checkboxName)) {
                    checkboxes[pair.Key].isChecked = selected;
                }
            }
        }
    }

    public void ChangePresetNext() {
        ChangePreset(currentIndex + 1);
    }
    
    public void ChangePresetPrevious() {
        ChangePreset(currentIndex - 1);
    }
    
    public void ChangePreset(int index) {
        
        int countPresets = 
            AppColorPresets.Instance.GetListByType(type).Count;
        
        if (index < -1) {
            index = countPresets - 1;    
        }
        
        if (index > countPresets - 1) {
            index = -1;
        }
        
        currentIndex = index;
        
        if (index > -2 && index < countPresets) {

            if (initialProfileCustomItem == null) {
                initialProfileCustomItem = GameProfileCharacters.currentCustom;
            }

            currentProfileCustomItem = GameProfileCharacters.currentCustom;

            if (index == -1) {                

                UIUtil.SetLabelValue(labelCurrentDisplayName, "My Previous Colors");

                GameCustomController.UpdateColorPresetObject(
                    initialProfileCustomItem, currentObject, type);
            }
            else {
                AppColorPreset preset = 
                    AppColorPresets.Instance.GetListByType(type)[currentIndex];
                
                // change character to currently selected texture preset
                
                currentProfileCustomItem = 
                    GameCustomController.UpdateColorPresetObject(
                        currentProfileCustomItem, currentObject, preset);
                
                GameCustomController.SaveCustomItem(currentProfileCustomItem);
                
                UIUtil.SetLabelValue(labelCurrentDisplayName, preset.display_name);
            }
        }
    }
    
    public override void Update() {
        if (currentObject) {
            currentObject.transform.Rotate(0f, -50 * Time.deltaTime, 0f);
        }
    }

}
