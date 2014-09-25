using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

public class GameCustomPlayerDisplayContainer : MonoBehaviour {
    
    public GameObject containerPlayerDisplay;
    public UnityEngine.Object prefabPlayerDisplay;
    
    public void Awake() {
    }
    
    public void Start() {
        Init();
    }
    
    public void Init() {
        Load();
    }
    
    void OnEnable() {
        //Messenger.AddListener(UIColorsMessages.uiColorsUpdate, OnColorsUpdateHandler);
    }
    
    void OnDisable() {
        //Messenger.RemoveListener(UIColorsMessages.uiColorsUpdate, OnColorsUpdateHandler);
    }

    public void Load() {
        
        if(prefabPlayerDisplay != null
           && containerPlayerDisplay != null) {
            
            containerPlayerDisplay.DestroyChildren();
            
            GameObject go = PrefabsPool.Instantiate(prefabPlayerDisplay) as GameObject;
            
            if(go != null) {
                
                go.transform.parent = containerPlayerDisplay.transform;
                
                go.ResetObject();
                
                go.SetLayerRecursively(gameObject.layer);
            }
        }
    }
}