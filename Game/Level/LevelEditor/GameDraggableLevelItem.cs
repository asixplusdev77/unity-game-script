using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Engine.Events;

public class GameDraggableLevelItem : MonoBehaviour {
	
	public GameObject dragColliderObject;
	public GameObject gameLevelItemObject;
	public GameObject dragHolder;
	
	public GameLevelItemAsset gameLevelItemAsset;		
			
	void Awake() {
		
	}
	
	public virtual void Start() {
		Init();
	}
	
	public virtual void Init() {
		
		if(dragColliderObject != null) {
			dragColliderObject.tag = "drag";
		}
		
		LoadData();
		
		if(gameLevelItemAsset != null) {
			LoadSprite(gameLevelItemAsset.asset_code);
		}
	}
	
	void OnEnable() {		
		Messenger<GameDraggableEditEnum>.AddListener(GameDraggableEditorMessages.EditState, OnEditStateHandler);
	}
	
	void OnDisable() {		
		Messenger<GameDraggableEditEnum>.RemoveListener(GameDraggableEditorMessages.EditState, OnEditStateHandler);
	}
	
	void OnEditStateHandler(GameDraggableEditEnum state) {
		if(state == GameDraggableEditEnum.StateEditing) {
			
		}
		else if(state == GameDraggableEditEnum.StateNotEditing) {
			
		}
	}
	
	public void ShowAllGameLevelItems() {
		if(gameLevelItemObject != null) {
			foreach(Transform t in gameLevelItemObject.transform) {
				t.gameObject.Show();
			}
		}
	}
	
	public void HideAllGameLevelItems() {
		if(gameLevelItemObject != null) {
			foreach(Transform t in gameLevelItemObject.transform) {
				t.gameObject.Hide();
			}
		}
	}
	
	public void LoadSprite(string spriteCode) {
		
		if(gameLevelItemObject != null) {
			
			RemoveGameLevelItems();
			
			GameObject go = GameDraggableEditor.LoadSprite(
				gameLevelItemObject, spriteCode, Vector3.one);
			if(go != null) {
				GameLevelSprite gameLevelSprite = go.GetComponent<GameLevelSprite>();
				if(gameLevelSprite != null) {
					gameLevelSprite.gameDraggableLevelItem = this;
				}
				//go.transform.parent.transform.parent.transform.parent.transform.parent.position = Vector3.zero;
				//go.transform.parent.transform.parent.transform.parent.transform.parent.localPosition = Vector3.zero;
			}
		}
	}
	
	public void RemoveGameLevelItems() {
		if(gameLevelItemObject != null) {
			// clear current or placeholder...
			foreach(Transform t in gameLevelItemObject.transform) {
				Destroy (t.gameObject);
			}
		}
	}
	
	public void LoadSpriteEffect(string spriteEffectCode) {
		
		if(gameLevelItemAsset != null) {
			GameObject go = GameDraggableEditor.LoadSpriteEffect(
				gameLevelItemObject, gameLevelItemAsset.destroy_effect_code, Vector3.one.WithX(3).WithY(3).WithZ(3) * .1f);
			if(go != null) {
				PackedSprite sprite = go.GetComponent<PackedSprite>();
				if(sprite != null) {
					sprite.PlayAnim(0);
				}
				go.transform.parent = gameLevelItemObject.transform;
			}
		}
	}
		
	public void DestroyGameLevelItemSprite() {
		RemoveGameLevelItems();
		if(gameLevelItemAsset != null) {
			LoadSpriteEffect(gameLevelItemAsset.destroy_effect_code);
		}
		Invoke("DestroyMe",.3f);
	}
	
	public void DestroyMeAnimated() {
		DestroyGameLevelItemSprite();
		Invoke("DestroyMe", .6f);
	}
	
	public void DestroyMe() {
		GameLevelItems.Current.level_items.RemoveAll(item => item.uuid == gameLevelItemAsset.uuid);		
		
		GameShooterController.Instance.ProcessStatDestroy();
		
		Destroy (gameObject);
	}
	
	public void LoadData() {
		if(gameLevelItemAsset != null) {
			LoadSprite(gameLevelItemAsset.asset_code);
			//StartCoroutine(LoadDataCo());			
		}
	}
	
	IEnumerator LoadDataCo() {
		LoadSprite(gameLevelItemAsset.asset_code);
		yield break;		
	}
	
	void Update() {
		if(dragHolder != null) {
			if(GameDraggableEditor.isEditing) {
				
				dragHolder.Show();
			}
			else if(!GameDraggableEditor.isEditing) {
								
				dragHolder.Hide();
			}
			
		}
	}
	
}
