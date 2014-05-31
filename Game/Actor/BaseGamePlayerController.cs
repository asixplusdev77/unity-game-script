using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Animation;
using Engine.Data.Json;
using Engine.Events;
using Engine.Game;
using Engine.Game.Actor;
using Engine.Game.Controllers;
using Engine.Utility;

public enum GamePlayerControllerState {
    ControllerAgent = 0,
    ControllerPlayer = 1,
    ControllerUI = 2,
    ControllerNetwork = 3,
    ControllerNotSet = 4,
}

public enum GamePlayerActorState {
    ActorMe,
    ActorEnemy,
    ActorFriend
}

public enum GamePlayerContextState {
    ContextInput = 0,
    ContextInputVehicle,
    ContextFollowAgent,
    ContextFollowAgentAttack,
    ContextFollowAgentAttackVehicle,
    ContextFollowInput,
    ContextScript,
    ContextRandom,
    ContextUI,
    ContextNetwork,
    ContextNotSet
}

public class BaseGamePlayerSlots {
    public static string slotPrimary = "primary";
    public static string slotSecondary = "secondary";
    public static string slotExtra = "extra";
}

public class BaseGamePlayerRuntimeData {
    public double health = 1f;
    public double energy = 1f;
    public double speed = 1f;
    public double scale = 1f;
    public double attack = 1f;
    public double defense = 1f;
    public double scores = 0;
    public double score = 0;
    public double coins = 0;
    public double hits = 8;
    public double hitCount = 0;
    public double hitLimit = 10;
    public double mass = 1;
    public double evades = 0;
    public double ammo = 10;
    public double collectedAmmo = 0;
    public double saves = 0;
    public double savesLaunched = 0;

    public virtual float totalScoreValue {
        get {
            return (float)(score + (coins * 50) + (scores * 500));
        }
    }
}

public class BaseGamePlayerControllerData {  
    public bool loadingCharacter = false;
    public bool gameModelVisible = true;
    public bool paused = true;

    // player
    public bool visible = true;
    public bool initialized = false;
    public bool dying = false;
    public float lastDie = 0f;
    public string lastCharacterCode = null;
    
    // animation
    public GamePlayerControllerAnimation gamePlayerControllerAnimation;
        
    // gameplay    
    public float lastAirCheck = 0f;
    public float lastUpdateCommon = 0f;
    public float lastAttackTime = 0;
    public float lastBoostTime = 0;
    public float lastStrafeLeftTime = 0;
    public float lastSpinTime = 0;
    public float lastStrafeRightTime = 0;
    public float lastAudioPlayedAttack = 0f;
    public float lastAudioPlayedHit = 0f;
    public float lastAudioPlayedDie = 0f;
    public float lastNetworkContainerFind = 0f;
    public bool navAgentRunning = true;
    public float currentTimeBlock = 0.0f;
    public float actionInterval = .33f;
    public bool initLoaded = false;
    public float lastCollision = 0f;
    public float lastHit = 0f;
    public Vector3 positionPlayer;
    public Vector3 positionTackler;
    public float lastTackle = 0f;
    public float incrementScore = 1f;
    
    // effects
    
    // effects - warps
    
    public bool effectWarpEnabled = false;
    public float effectWarpStart = 0f;
    public float effectWarpEnd = 200f;
    public float effectWarpCurrent = 0f;
    
    // effects - lines
    
    public float lastPlayerEffectsTrailUpdate = 0f;
    public TrailRenderer trailRendererBoost;
    public TrailRenderer trailRendererGround;
    
    // effects - follow
    
    public float lastPlayerEffectsGroundUpdate = 0f;
    public float lastPlayerEffectsBoostUpdate = 0f;
    public Color gamePlayerEffectsGroundColorCurrent;
    public Color gamePlayerEffectsGroundColorLast;
    public Color gamePlayerEffectsBoostColorCurrent;
    public Color gamePlayerEffectsBoostColorLast;

    // navigation/movement
    public NavMeshAgent navMeshAgent;
    public GamePlayerNavMeshAgentFollowController navMeshAgentFollowController;
    public GamePlayerNavMeshAgentController navMeshAgentController;
    public CharacterController characterController;
    public GamePlayerThirdPersonController thirdPersonController;

    // physics
    public Vector3 impact = Vector3.zero;

    // networking
    public Gameverses.GameNetworkAniStates currentNetworkAniState = Gameverses.GameNetworkAniStates.walk;
    public Gameverses.GameNetworkAniStates lastNetworkAniState = Gameverses.GameNetworkAniStates.run;
        
    // RPG
    public GameProfileRPGItem currentRPGItem;
    public GameProfilePlayerProgressItem currentPlayerProgressItem;
    public double rpgModifierDefault = .4f;
    public float lastRPGModTime = 0f;
    public bool playerSpin = false;
    public GamePlayerRuntimeRPGData runtimeRPGData = new GamePlayerRuntimeRPGData();
    
    // LAUNCHING
    public Vectrosity.VectorLine lineAim = null;
    public Vector3 positionStart = Vector3.zero;
    public Vector3 positionRelease = Vector3.zero;
    public Vector3 positionLastTouch = Vector3.zero;
    public Vector3 currentStartPoint = Vector3.zero;
    public Vector3 currentEndPoint = Vector3.zero;
    public Vector3 currentPosition = Vector3.zero;
    public Vector3 currentAimPosition = Vector3.zero;
    public ParticleSystem gamePlayerEffectAim;

    // EVADING
    public float distanceToPlayerControlledGamePlayer;
    public float distanceEvade = 5f;
    public bool isWithinEvadeRange = false;
    public bool lastIsWithinEvadeRange = false;
    public float distanceRandomDie = 50f;
    public float timeMinimumRandomDie = 25f;
    public float lastRandomDie = 5f;
    public bool isInRandomDieRange = false;
    public bool lastIsInRandomDieRange = false;    
    
    // IDLE ACTIONS AFTER INACTION
    public float delayIdleActions = 3.0f;
    public float lastIdleActions = 0f;
    public float lastCharacterLoadedCheck = 0f;
    public GameObject currentPrefabNameObjectItem;
    public string currentPrefabNameObject = "";
    public GamePlayerAttributes gamePlayerAttributes = new GamePlayerAttributes();
    
    // audio
    public GameObject audioObjectFootsteps;
    public AudioClip audioObjectFootstepsClip;
    public AudioSource audioObjectFootstepsSource;

    // materials
    
    //float controllerData.lastUpdate = 0f;
    public List<SkinnedMeshRenderer> renderers;
    public GamePlayerController collisionController = null;

    // powerup modifiers

    // speed

    public float modifierItemSpeedCurrent = 1.0f;
    public float modifierItemSpeedMin = 1.0f;
    public float modifierItemSpeedMax = 3.0f;
    public float modifierItemSpeedLerpTime = 5f;
    public float modifierItemSpeedLerp = 0f;

    // scale
        
    public float modifierItemScaleCurrent = 1.0f;
    public float modifierItemScaleMin = 1.0f;
    public float modifierItemScaleMax = 3.0f;
    public float modifierItemScaleLerpTime = 5f;
    public float modifierItemScaleLerp = 0f;

    // items
    public GamePlayerItemsData itemsData = new GamePlayerItemsData();

    // mounts
    public GamePlayerMountData mountData = new GamePlayerMountData();

}

public class BaseGamePlayerRuntimeRPGData {
    public double modifierSpeed = .5;
    public double modifierHealth = .5;
    public double modifierEnergy = .5;
    public double modifierMagic = .5;
    public double modifierBoost = .5;
    public double modifierPower = .5;
    public double modifierAttack = .5;
    public double modifierDefend = .5;
    public double modifierScale = .5;
}

public class GamePlayerItemsData : BaseGamePlayerItemsData {
    
    public GamePlayerItemsData() {
        
    }
}

public class BaseGamePlayerItemsData {    
    
    public BaseGamePlayerItemsData() {
        
    }
    
    // ---------------------------------------------------
    // ITEMS
    
    // powerups
    
    // boost
}

public class GamePlayerMountData : BaseGamePlayerMountData {

    public GamePlayerMountData() {
    
    }
}

public class BaseGamePlayerMountData {    
        
    public BaseGamePlayerMountData() {
        
    }

    // ---------------------------------------------------
    // MOUNTS

    // MOUNTS - vehicle

    public GameObjectMountVehicle mountVehicle;

    public bool isMountedVehicle {

        get {
            if (isMountedVehicleObject) {
                return true;
            }

            return false;
        }
    }

    public bool isMountedVehicleObject {
        get {            
            if (mountVehicle != null) {
                return true;
            }
            return false;
        }
    }

    public void MountVehicle(GameObject go, GameObjectMountVehicle mount) {
        if (!isMountedVehicleObject) {
            mountVehicle = mount;
            mountVehicle.Mount(go);
        }
    }

    public void UnmountVehicle() {
        if (isMountedVehicleObject) {
            mountVehicle.Unmount();
            mountVehicle = null;
        }
    }
    
    public void SetMountVehicleAxis(float h, float v) {
        if (mountVehicle != null) {
            mountVehicle.SetMountVehicleAxis(h, v);
        }
    }

    public void SetMountWeaponRotator(Vector3 rt) {
        if (mountVehicle != null) {
            mountVehicle.SetMountWeaponRotator(rt);
        }
    }
    
    public void SetMountWeaponRotator(Quaternion qt) {
        if (mountVehicle != null) {
            mountVehicle.SetMountWeaponRotator(qt);
        }
    }
}

public class BaseGamePlayerController : GameActor {
 
    //public string uuid = "";
    public string characterCode = "character-player-boy-1";
    public Transform currentTarget;
    public bool paused = true;
 
    // asset
    public GamePlayerControllerAsset gamePlayerControllerAsset;
    public GameCustomPlayer gameCustomPlayer;
    public GameCustomEnemy gameCustomEnemy;
     
    // player effects
    public GameObject gamePlayerEffectParticleObjects;
    public ParticleSystem gamePlayerEffectWarp;
    public ParticleSystem gamePlayerEffectCircleFollow;
    public ParticleSystem gamePlayerEffectCircle;
    public ParticleSystem gamePlayerEffectCircleStars;
    public ParticleSystem gamePlayerEffectAttack;
    public ParticleSystem gamePlayerEffectSkill;
    public ParticleSystem gamePlayerEffectHit;
    public ParticleSystem gamePlayerEffectDeath; 
 
    // appearance/context
    public ActorShadow actorShadow;
 
    // models/objects
    public GameObject gamePlayerModel;
    public GameObject gamePlayerHolder;
    public GameObject gamePlayerShadow;
    public GameObject gamePlayerEnemyTarget;
    public GameObject gamePlayerModelTarget;
    public GameObject gamePlayerModelHolder;
    public GameObject gamePlayerModelHolderModel;
    public GameObject gamePlayerModelHolderWeapons;
    public GameObject gamePlayerModelHolderWeaponsHolder;
    public GameObject gamePlayerModelHolderItems;
    public GameObject gamePlayerModelHolderSkills;
    public Vector3 initialGamePlayerWeaponContainer = Vector3.zero;
    public Vector3 currentGamePlayerWeaponContainer = Vector3.zero;

    //gamePlayerModelHolderWeapons.transform.position
 
    // attack
    public GameObject weaponObject;
 
    // skill
    public GameObject skillObject;
 
    // states
    public GamePlayerControllerState controllerState = GamePlayerControllerState.ControllerNotSet;
    public GamePlayerContextState contextState = GamePlayerContextState.ContextNotSet;

    // controller runtime state
    public GamePlayerControllerData controllerData;
 
    // runtime data
    public GamePlayerRuntimeData runtimeData;
    public GamePlayerItemsData itemsData;

    // initialize
    public float initialMaxWalkSpeed = 5f;
    public float initialMaxTrotSpeed = 15f;
    public float initialMaxRunSpeed = 20f;
    public float initialMaxJumpHeight = .5f;
    public float initialMaxExtraJumpHeight = 1f;
    public float characterSlopeLimit = 45;
    public float characterStepOffset = .3f;
    public float characterRadius = 1f;
    public float characterHeight = 2.5f;
    public Vector3 characterCenter = new Vector3(0f, 0f, 0f);
    
    GameObject gameObjectLoad = null;
     
    // weapons
    public Dictionary<string, GamePlayerWeapon> weapons = new Dictionary<string, GamePlayerWeapon>();
    public GamePlayerWeapon weaponPrimary;
    public GamePlayerWeapon weaponSecondary;
 
    // network
    public Gameverses.GameNetworkPlayerContainer currentNetworkPlayerContainer;
 
    // CAMS
    public GameCameraSmoothFollow gameCameraSmoothFollow;
    public GameCameraSmoothFollow gameCameraSmoothFollowGround;
    //
     
    public GameObject gamePlayerEffectsBoost;
    public GameObject gamePlayerEffectsContainer;
    public GameObject gamePlayerEffectsGround;
    public GameObject gamePlayerTrailContainer;
    public GameObject gamePlayerTrailGround;
    public GameObject gamePlayerTrailBoost;

    // If this is an enemy see if we should attack
    
    public float attackRange = 12f;  // within 6 yards    
    public float attackDistance = 10f;
    
    public float lastStateEvaded = 0f;

    // quality settings
        
    public float currentFPS = 60f;

    // --------------------------------------------------------------------
    // INIT
 
    public virtual void Awake() {

    }
 
    public override void Start() {
        //Init(controllerState);
    }
 
    public override void OnEnable() {
        //MessengerObject<InputTouchInfo>.AddListener(MessengerObjectMessageType.OnEventInputDown, OnInputDown);
        //MessengerObject<InputTouchInfo>.AddListener(MessengerObjectMessageType.OnEventInputUp, OnInputUp);
          
        Messenger<string, Vector3>.AddListener("input-axis", OnInputAxis);//"input-axis-" + axisName, axisInput);
     
        Messenger<string, string>.AddListener(GamePlayerMessages.PlayerAnimation, OnPlayerAnimation);
     
        Gameverses.GameMessenger<string, Gameverses.GameNetworkAniStates>.AddListener(Gameverses.GameNetworkPlayerMessages.PlayerAnimation, OnNetworkPlayerAnimation);
        Gameverses.GameMessenger<string, float>.AddListener(Gameverses.GameNetworkPlayerMessages.PlayerInputAxisHorizontal, OnNetworkPlayerInputAxisHorizontal);
        Gameverses.GameMessenger<string, float>.AddListener(Gameverses.GameNetworkPlayerMessages.PlayerInputAxisVertical, OnNetworkPlayerInputAxisVertical);
        Gameverses.GameMessenger<string, float>.AddListener(Gameverses.GameNetworkPlayerMessages.PlayerSpeed, OnNetworkPlayerSpeed);
     
        Gameverses.GameMessenger<Gameverses.GameNetworkingAction, Vector3, Vector3>.AddListener(Gameverses.GameNetworkingMessages.ActionEvent, OnNetworkActionEvent);
    }
 
    public override void OnDisable() {
        //MessengerObject<InputTouchInfo>.RemoveListener(MessengerObjectMessageType.OnEventInputDown, OnInputDown);
        //MessengerObject<InputTouchInfo>.RemoveListener(MessengerObjectMessageType.OnEventInputUp, OnInputUp);
        Messenger<string, string>.RemoveListener(GamePlayerMessages.PlayerAnimation, OnPlayerAnimation);
     
        Messenger<string, Vector3>.RemoveListener("input-axis", OnInputAxis);//"input-axis-" + axisName, axisInput); 
     
        Gameverses.GameMessenger<string, Gameverses.GameNetworkAniStates>.RemoveListener(Gameverses.GameNetworkPlayerMessages.PlayerAnimation, OnNetworkPlayerAnimation);
        Gameverses.GameMessenger<string, float>.RemoveListener(Gameverses.GameNetworkPlayerMessages.PlayerInputAxisHorizontal, OnNetworkPlayerInputAxisHorizontal);
        Gameverses.GameMessenger<string, float>.RemoveListener(Gameverses.GameNetworkPlayerMessages.PlayerInputAxisVertical, OnNetworkPlayerInputAxisVertical);
        Gameverses.GameMessenger<string, float>.RemoveListener(Gameverses.GameNetworkPlayerMessages.PlayerSpeed, OnNetworkPlayerSpeed);
     
        Gameverses.GameMessenger<Gameverses.GameNetworkingAction, Vector3, Vector3>.RemoveListener(Gameverses.GameNetworkingMessages.ActionEvent, OnNetworkActionEvent);
 
    }

    public virtual void SetRuntimeData(GamePlayerRuntimeData data) {
        if (data == null) {
            data = new GamePlayerRuntimeData();
        }
        
        runtimeData = data;
        
        // TODO sync if needed... to update 
        // runtime expensive states that can't be polled.
    }
 
    public virtual void SetControllerData(GamePlayerControllerData data) {
        if (data == null) {
            data = new GamePlayerControllerData();
        }
     
        controllerData = data;
     
        // TODO sync if needed... to update 
        // runtime expensive states that can't be polled.
    }
    
    public virtual void SetItemsData(GamePlayerItemsData data) {
        if (data == null) {
            data = new GamePlayerItemsData();
        }
        
        itemsData = data;
        
        // TODO sync if needed... to update 
        // runtime expensive states that can't be polled.
    }
     
    public virtual void Init(
        GamePlayerControllerState controllerStateTo, 
        GamePlayerContextState contextStateTo) {
        
        SetUp(controllerStateTo, contextStateTo);

    }

    // SPEED

    public virtual float gamePlayerMoveSpeed {
        get {
            return GamePlayerMoveSpeed();
        }
    }

    public virtual float GamePlayerMoveSpeed() {

        float currentSpeed = 0f;

        if(controllerData == null) {
            return currentSpeed;
        }
        
        if (controllerData.thirdPersonController != null) {
            currentSpeed = controllerData.thirdPersonController.GetSpeed();
        }
        
        if (contextState == GamePlayerContextState.ContextFollowAgent
            || contextState == GamePlayerContextState.ContextFollowAgentAttack
            || contextState == GamePlayerContextState.ContextRandom) {

            if (controllerData.navMeshAgent != null) {

                if (controllerData.navMeshAgent.enabled) {                       
                    //currentSpeed = navAgent.velocity.magnitude + 20;
                    
                    if (controllerData.navMeshAgent.velocity.magnitude > 0f) {
                        currentSpeed = 15f;
                    }
                    else {
                        currentSpeed = 0;    
                    }
                    
                    if (controllerData.navMeshAgent.remainingDistance < 
                        controllerData.navMeshAgent.stoppingDistance + 1) {
                        currentSpeed = 0;
                    }
                    
                    if (currentSpeed < 
                        controllerData.navMeshAgent.speed) {
                        //currentSpeed = 0;
                    }
                }
            }
        }

        return currentSpeed;
    }

    // REWARDS / ITEMS
    
    public virtual void HandleItemStateCurrency(double val) {    

        runtimeData.coins += val;
    }
    
    public virtual void HandleItemStateHitCount(double val) {         
        runtimeData.hitCount += val;
    }
    
    public virtual void HandleItemStateHealth(double val) {         
        HandleItemStateHitCount(-1);
        runtimeData.health += (float)val;
    }

    public virtual void HandleItemStateSpeedModifier(double val, double duration) {         
        controllerData.modifierItemSpeedCurrent *= (float)val;
        controllerData.modifierItemSpeedLerpTime = (float)duration;
        ;
        controllerData.modifierItemSpeedLerp = 0f;
    }
    
    public virtual void HandleItemStateScaleModifier(double val, double duration) {         
        controllerData.modifierItemScaleCurrent *= (float)val;
        controllerData.modifierItemScaleLerpTime = (float)duration;
        ;
        controllerData.modifierItemScaleLerp = 0f;
                
        Debug.Log("HandleItemStateScaleModifier::" + " val:" + val + " duration:" + duration);
        Debug.Log("HandleItemStateScaleModifier::" + " controllerData.modifierItemScaleCurrent:" + controllerData.modifierItemScaleCurrent);
        Debug.Log("HandleItemStateScaleModifier::" + " controllerData.modifierItemScaleLerpTime:" + controllerData.modifierItemScaleLerpTime);
        Debug.Log("HandleItemStateScaleModifier::" + " controllerData.modifierItemScaleLerp:" + controllerData.modifierItemScaleLerp);
    }

    public virtual void HandleItemUse(GameItem gameItem) {
        
        GameDataObjectItem data = gameItem.data;
        
        if (data == null) {
            return;
        }
        
        float modifier = 1f;

        GameDataItemRPG rpg = new GameDataItemRPG();

        if (data.HasRPGs()) {
            rpg = data.GetRPG();            
            
            Debug.Log("HandleItemUse::" + " rpg:" + rpg.ToJson());

            HandleItemStateSpeedModifier(rpg.speed, rpg.duration);
            HandleItemStateScaleModifier(rpg.scale, rpg.duration);
        }
        
        // rewards
        
        if (data.HasRewards()) {
            
            List<GameDataItemReward> items = data.rewards;
            
            bool broadcastEvent = false;
            object broadcastVal = null;
            
            foreach (GameDataItemReward item in items) {
                
                broadcastEvent = false;
                broadcastVal = null;
                
                if (item.val == null) {
                    continue;
                }
                
                if (item.code == GameDataItemReward.xp) {

                    double val = item.valDouble * modifier;   
                    
                    GamePlayerProgress.SetStatXP(val);
                    
                    GameProfileCharacters.Current.CurrentCharacterAddGamePlayerProgressXP(val);
                    
                    broadcastEvent = true;
                    broadcastVal = val;
                }
                else if (item.code == GameDataItemReward.currency) {
                    
                    double val = item.valDouble * modifier;   
                    
                    GamePlayerProgress.SetStatCoins(val);
                    GamePlayerProgress.SetStatCoinsPickup(val);     
                    
                    HandleItemStateCurrency(val);
                    
                    broadcastEvent = true;
                    broadcastVal = val;
                }
                else if (item.code == GameDataItemReward.health) {
                    
                    double val = item.valDouble * modifier;  
                    
                    HandleItemStateHealth(val);

                    GameProfileCharacters.Current.CurrentCharacterAddGamePlayerProgressEnergy(val); // refill
                    GameProfileCharacters.Current.CurrentCharacterAddGamePlayerProgressHealth(val); // refill                        
                    
                    broadcastEvent = true;
                    broadcastVal = val;
                }
                
                if (broadcastEvent) {        
                    Messenger<string, object>.Broadcast(GameMessages.item, item.code, broadcastVal);
                }
                
            }
        }
        
        // sounds
        
        GameAudio.PlayEffect(GameAudioEffects.audio_effect_ui_button_1);
        
        if (data.HasSounds()) {
            
            List<GameDataSound> items = data.sounds;
            
            foreach (GameDataSound item in items) {
                item.PlaySoundType(GameDataSound.reward);
            }
        }           
    }

 
    // --------------------------------------------------------------------
    // EFFECTS

    // LINE RENDERERS

    public virtual void PlayerEffectTrailGroundFadeIn() {
        UITweenerUtil.FadeTo(gamePlayerTrailGround,
            UITweener.Method.Linear, UITweener.Style.Once, 1f, .5f, 1f);
    }

    public virtual void PlayerEffectTrailGroundFadeOut() {
        UITweenerUtil.FadeTo(gamePlayerTrailGround,
            UITweener.Method.Linear, UITweener.Style.Once, 1f, .5f, 0f);
    }

    public virtual void PlayerEffectTrailBoostFadeIn() {
        UITweenerUtil.FadeTo(gamePlayerTrailBoost,
            UITweener.Method.Linear, UITweener.Style.Once, 1f, .5f, 1f);
    }

    public virtual void PlayerEffectTrailBoostFadeOut() {
        UITweenerUtil.FadeTo(gamePlayerTrailBoost,
            UITweener.Method.Linear, UITweener.Style.Once, 1f, .5f, 0f);
    }

    public virtual bool CheckTrailRendererBoost() {
        if (controllerData.trailRendererBoost == null && gamePlayerTrailBoost != null) {
            controllerData.trailRendererBoost = gamePlayerTrailBoost.Get<TrailRenderer>();
            return true;
        }
        return false;
    }

    public virtual bool CheckTrailRendererGround() {
        if (controllerData.trailRendererGround == null && gamePlayerTrailGround != null) {
            controllerData.trailRendererGround = gamePlayerTrailGround.Get<TrailRenderer>();
            return true;
        }
        return false;
    }

    public virtual void PlayerEffectTrailBoostTime(float time) {
        if (gamePlayerTrailBoost != null) {
            CheckTrailRendererBoost();

            if (controllerData.trailRendererBoost != null) {
                controllerData.trailRendererBoost.time = time;
            }
        }
    }

    public virtual void PlayerEffectTrailGroundTime(float time) {
        if (gamePlayerTrailGround != null) {
            CheckTrailRendererGround();

            if (controllerData.trailRendererGround != null) {
                controllerData.trailRendererGround.time = time;
            }
        }
    }

    public virtual void HandlePlayerEffectTrailGroundTick() {
        if (gamePlayerTrailGround != null) {

            // UPDATE color randomly
            // TODO add other conditions to get colors, health, power etc
            // Currently randomize player colors for effect

            CheckTrailRendererGround();
            
            if (controllerData.trailRendererGround != null) {

                //Color colorTo = GameCustomController.GetRandomizedColorFromContextEffects();
                //controllerData.trailRendererGround.gameObject.ColorTo(colorTo, 1f, 0f);
            }
        }
    }
    
    public virtual void HandlePlayerEffectTrailBoostTick() {
        if (gamePlayerTrailBoost != null) {
            
            // UPDATE color randomly
            // TODO add other conditions to get colors, health, power etc
            // Currently randomize player colors for effect
            
            CheckTrailRendererBoost();
            
            if (gamePlayerTrailBoost != null) {
                
                //Color colorTo = GameCustomController.GetRandomizedColorFromContextEffects();
                //controllerData.trailRendererBoost.gameObject.ColorTo(colorTo, 1f, 0f);
            }
        }
    }

    
    // EFFECTS FOLLOW - GROUND/BACK/HEAD ETC

    public bool playerEffectsGroundShow {
        get {
            
            if (FPSDisplay.isUnder20FPS) {
                return false;
            }

            return true;
        }
    }

    public virtual void PlayerEffectsGroundFadeIn() {

        if (!playerEffectsGroundShow) {
            return;
        }

        UITweenerUtil.FadeTo(gamePlayerEffectsGround,
                             UITweener.Method.Linear, UITweener.Style.Once, 1f, .5f, 1f);
    }
    
    public virtual void PlayerEffectsGroundFadeOut() {
        
        if (!playerEffectsGroundShow) {
            return;
        }

        UITweenerUtil.FadeTo(gamePlayerEffectsGround,
                             UITweener.Method.Linear, UITweener.Style.Once, 1f, .5f, 0f);
    }
    
    public virtual void PlayerEffectEffectsBoostFadeIn() {
        UITweenerUtil.FadeTo(gamePlayerEffectsBoost,
                             UITweener.Method.Linear, UITweener.Style.Once, 1f, .5f, 1f);
    }
    
    public virtual void PlayerEffectEffectsBoostFadeOut() {
        UITweenerUtil.FadeTo(gamePlayerEffectsBoost,
                             UITweener.Method.Linear, UITweener.Style.Once, 1f, .5f, 0f);
    }
    
    public virtual void PlayerEffectsBoostTime(float time) {
        if (gamePlayerEffectsBoost != null) {
            gamePlayerEffectsBoost.SetParticleSystemEmissionRate(time, true);
        }
    }

    public virtual void PlayerEffectsGroundTime(float time) {
        
        if (!playerEffectsGroundShow) {
            return;
        }

        if (gamePlayerEffectsGround != null) {
            gamePlayerEffectsGround.SetParticleSystemEmissionRate(time, true);
        }
    }

    public virtual void HandlePlayerEffectsObjectTick(
        ref GameObject effectsObject, ref Color colorCurrent, ref Color colorLast, ref float lastTime, float speed) {
                
        // UPDATE color randomly
        // TODO add other conditions to get colors, health, power etc
        // Currently randomize player colors for effect

        float updateTime = 0;
        bool immediate = lastTime < 0 ? true : false;
        
        
        if (lastTime < 5 && lastTime >= 0) {    
            lastTime += Time.deltaTime; 
        }
        else {             
            lastTime = 0;
            colorCurrent = GameCustomController.GetRandomizedColorFromContextEffects();
        }
        
        if (immediate) {
            updateTime = 1;
        }
        else {
            updateTime = lastTime / speed;                
        }
        
        colorLast = 
            Color.Lerp(colorLast, 
                       colorCurrent, 
                       updateTime);
        
        effectsObject.SetParticleSystemStartColor(colorLast, true);
    }

    public virtual void HandlePlayerEffectsTick() {
        
        if (controllerData.lastPlayerEffectsTrailUpdate + 4 < Time.time) {
            controllerData.lastPlayerEffectsTrailUpdate = Time.time;
            HandlePlayerEffectTrailBoostTick();            
            HandlePlayerEffectTrailGroundTick();  
        }

        HandlePlayerEffectsGroundTick();
        HandlePlayerEffectsBoostTick();
    }
    
    public virtual void HandlePlayerEffectsGroundTick() {
        
        if (!playerEffectsGroundShow) {
            return;
        }

        if (gamePlayerEffectsGround != null) {

            HandlePlayerEffectsObjectTick(ref gamePlayerEffectsGround, 
                                          ref controllerData.gamePlayerEffectsGroundColorCurrent, 
                                          ref controllerData.gamePlayerEffectsGroundColorLast, 
                                          ref controllerData.lastPlayerEffectsGroundUpdate,
                                          5);


        }
    }

    public virtual void HandlePlayerEffectsBoostTick() {
        if (gamePlayerEffectsBoost != null) {
            
            HandlePlayerEffectsObjectTick(ref gamePlayerEffectsBoost, 
                                          ref controllerData.gamePlayerEffectsBoostColorCurrent, 
                                          ref controllerData.gamePlayerEffectsBoostColorLast, 
                                          ref controllerData.lastPlayerEffectsBoostUpdate,
                                          3);
        }
    }  

    // WARP
     
    public virtual void PlayerEffectWarpFadeOut() {
        PlayerEffectWarpAnimate(200, 0);
    }
 
    public virtual void PlayerEffectWarpFadeIn() {
        PlayerEffectWarpAnimate(0, 200);
    }
 
    public virtual void PlayerEffectWarpAnimate(float fromEmission, float toEmission) {
        controllerData.effectWarpStart = fromEmission;
        controllerData.effectWarpEnd = toEmission;
        controllerData.effectWarpEnabled = true;
    }
 
    public virtual void HandlePlayerEffectWarpAnimateTick() {
        if (controllerData.effectWarpEnabled && controllerData.visible) {
            float fadeSpeed = 200f;
            if (controllerData.effectWarpCurrent < controllerData.effectWarpEnd) {
                controllerData.effectWarpCurrent += (Time.deltaTime * fadeSpeed);
                SetPlayerEffectWarp(controllerData.effectWarpCurrent);
            }
            else if (controllerData.effectWarpCurrent > controllerData.effectWarpEnd) {
                controllerData.effectWarpCurrent -= (Time.deltaTime * fadeSpeed);
                SetPlayerEffectWarp(controllerData.effectWarpCurrent);
            }
            else {
                controllerData.effectWarpEnabled = false;
                controllerData.effectWarpCurrent = controllerData.effectWarpEnd;
            }
        }
    }
 
    public virtual void SetPlayerEffectWarp(float rate) {
        if (gamePlayerEffectWarp != null) {
            gamePlayerEffectWarp.emissionRate = rate;
        }
    }
 
    public virtual void ShowPlayerEffectWarp() {
        if (gamePlayerEffectWarp != null) {
            SetPlayerEffectWarp(200);
        }
    }
 
    public virtual void HidePlayerEffectWarp() {
        if (gamePlayerEffectWarp != null) {
            SetPlayerEffectWarp(0);
        }
    }

    // PLAYER CIRCLE INDICATOR GROUND
 
    public virtual void ShowPlayerEffectCircleFollow() {
        if (gamePlayerEffectCircleFollow != null) {
            gamePlayerEffectCircleFollow.Play();
        }
    }
 
    public virtual void HidePlayerEffectCircleFollow() {
        if (gamePlayerEffectCircleFollow != null) {
            gamePlayerEffectCircleFollow.Pause();
        }
    }
 
    public virtual void ShowPlayerEffectCircle() {
        if (gamePlayerEffectCircle != null) {
            gamePlayerEffectCircle.Play();
        }
    }
 
    public virtual void HidePlayerEffectCircle() {
        if (gamePlayerEffectCircle != null) {
            gamePlayerEffectCircle.Pause();
        }
    }
 
    public virtual void ShowPlayerEffectCircleStars() {
        if (gamePlayerEffectCircleStars != null) {
            gamePlayerEffectCircleStars.Play();
        }
    }
 
    public virtual void HidePlayerEffectCircleStars() {
        if (gamePlayerEffectCircleStars != null) {
            gamePlayerEffectCircleStars.Pause();
        }
    }
 
    // --------------------------------------------------------------------
    // RUNTIME STATES
    public virtual void HandlePlayerAliveState() {
        
        if (controllerData.lastCharacterLoadedCheck + 1 < Time.time) {
            controllerData.lastCharacterLoadedCheck = Time.time;
            
            if (!isCharacterLoaded) {
                //LoadCharacter(characterCode);
            }
        }        

        if (runtimeData.health <= 0f) {
            Die();
        }
    }

    public virtual void HandlePlayerAliveStateLate() {

        UpdatePhysicsState();
    }

    public virtual void HandlePlayerAliveStateFixed() {

    }

    public virtual void HandlePlayerInactionState() {
        
        if (!IsPlayerControlled) {
            return;
        }

        // update player controlled players to look at player and animate it inactive

        bool update = false;

        if (controllerData != null) {
            if (controllerData.lastIdleActions + UnityEngine.Random.Range(3, 7) < Time.time) {
                controllerData.lastIdleActions = Time.time;
                if (controllerData.thirdPersonController != null) {
                    if (controllerData.thirdPersonController.moveSpeed == 0f) {
                        update = true;
                    }
                }
            }

            if (!update) {
                return;
            }
        }

        // Look at camera

        OnInputAxis(GameTouchInputAxis.inputAxisMove, Vector3.zero.WithY(-1));

        // Randomize animations that are ok for idle

        int randomize = UnityEngine.Random.Range(0, 5);

        if (randomize == 0) {
            for (int i = 0; i < UnityEngine.Random.Range(1, 4); i++) {
                Idle();
            }
        }
        else if (randomize == 1) {
            for (int i = 0; i < UnityEngine.Random.Range(1, 4); i++) {
                Idle();
            }
        }
        else if (randomize == 2) {
            for (int i = 0; i < UnityEngine.Random.Range(1, 4); i++) {
                //StrafeLeft();
                Idle();
            }
        }
        else if (randomize == 3) {
            for (int i = 0; i < UnityEngine.Random.Range(1, 4); i++) {
                //StrafeRight();
                Idle();
            }
        }


    }
 
    // --------------------------------------------------------------------
    // CHARACTER
     
    public virtual bool isMe {
        get {
            if (uniqueId == UniqueUtil.Instance.currentUniqueId) {
                return true;
            }
            return false;
        }
    }
 
    public virtual bool isDead {
        get {
            return !isAlive;
        }
    }
 
    public virtual bool isAlive {
        get {
            return runtimeData.health > 0f ? true : false;
        }
    }
  
    public virtual bool IsPlayerControlled {
        get {
            if (controllerState == GamePlayerControllerState.ControllerPlayer
                || contextState == GamePlayerContextState.ContextInput
                || contextState == GamePlayerContextState.ContextInputVehicle
                || contextState == GamePlayerContextState.ContextFollowInput
                || uniqueId == UniqueUtil.Instance.currentUniqueId) {
                return true;
            }
            return false;
        }
    }
 
    public virtual bool IsAgentState() {
        if (controllerState == GamePlayerControllerState.ControllerAgent) {
            return true;
        }
        return false;
    }
 
    public virtual bool IsPlayerState() {
        if (controllerState == GamePlayerControllerState.ControllerPlayer) {
            return true;
        }
        return false;
    }
 
    public virtual bool IsNetworkPlayerState() {
        if (controllerState == GamePlayerControllerState.ControllerNetwork) {
            return true;
        }
        return false;
    }
 
    public virtual bool IsUIState() {
        if (controllerState == GamePlayerControllerState.ControllerUI) {
            return true;
        }
        return false;
    }
 
    public virtual void ChangePlayerState(GamePlayerControllerState controllerStateTo) {
        //if (controllerStateTo != controllerState) {
        controllerState = controllerStateTo;
         
        InitControls();
         
        if (controllerState == GamePlayerControllerState.ControllerAgent) {
            if (controllerData.navMeshAgent != null) {
                // TODO load script or look for character input.
                controllerData.navMeshAgent.enabled = true;
            }
        }
        else if (controllerState == GamePlayerControllerState.ControllerPlayer) {
            if (controllerData.navMeshAgent != null) {
                controllerData.navMeshAgent.Stop();
                //navMeshAgent.enabled = false;
            }
        }
        else if (controllerState == GamePlayerControllerState.ControllerNetwork) {
            if (controllerData.navMeshAgent != null) {
                controllerData.navMeshAgent.Stop();
                //navMeshAgent.enabled = false;
            }
            ChangeContextState(GamePlayerContextState.ContextNetwork);
        }
        else if (controllerState == GamePlayerControllerState.ControllerUI) {
            if (controllerData.navMeshAgent != null) {
                controllerData.navMeshAgent.Stop();
                //navMeshAgent.enabled = false;  
                if (controllerData.thirdPersonController != null) {
                    controllerData.thirdPersonController.getUserInput = true;
                }                    
            }
        }
        //}
    }
     
    public virtual GamePlayerController GetController(Transform transform) {
        if (transform != null) {
            GamePlayerController gamePlayerController = transform.GetComponentInChildren<GamePlayerController>();
            if (gamePlayerController != null) {
                return gamePlayerController;
            }
        }
        return null;
    }

    public virtual void LoadCharacterAttachedSounds() {
        // TODO footsteps over different terrain
        // Foosteps, breathing etc.

        if (controllerData.audioObjectFootsteps == null) {
            controllerData.audioObjectFootsteps = GameAudio.PlayEffectObject(transform, "audio_footsteps_default", true);
            if (controllerData.audioObjectFootsteps != null) {
                if (controllerData.audioObjectFootsteps.audio != null) {
                    controllerData.audioObjectFootstepsSource = controllerData.audioObjectFootsteps.audio;

                    if (controllerData.audioObjectFootstepsClip == null && controllerData.audioObjectFootstepsSource.clip != null) {
                        controllerData.audioObjectFootstepsClip = controllerData.audioObjectFootsteps.audio.clip;
                    }
                }
            }
        }
    }

    public virtual void HandleCharacterAttachedSounds() {
    
        if (!GameConfigs.isGameRunning) {
            if (controllerData != null) {
                if (controllerData.audioObjectFootstepsSource != null) {
                    controllerData.audioObjectFootstepsSource.StopIfPlaying();
                }
            }
            return;
        }

        LoadCharacterAttachedSounds();

        controllerData.audioObjectFootstepsSource.volume = (float)GameProfiles.Current.GetAudioEffectsVolume();

        if (gamePlayerMoveSpeed > .1f) {
            ////controllerData.audioObjectFootstepsSource.volume = 1f;
            float playSpeed = Mathf.InverseLerp(0, initialMaxRunSpeed, gamePlayerMoveSpeed) + .8f;
            //LogUtil.Log("playSpeed", playSpeed);
            controllerData.audioObjectFootstepsSource.pitch = playSpeed;
        }
        else {
            controllerData.audioObjectFootstepsSource.pitch = 0f;
        }    
    }

    public virtual bool isCharacterLoaded {
        get {
            return gamePlayerModelHolderModel.transform.childCount > 0;   
        }
    }
     
    public virtual void LoadCharacter(string characterCodeTo) {

        characterCode = characterCodeTo;

        if (controllerData == null) {
            controllerData = new GamePlayerControllerData();
        }

        //LogUtil.Log("LoadCharacter:prefabNameObject:" + prefabNameObject);
        if (controllerData.lastCharacterCode != characterCode 
            || controllerData.lastCharacterCode == null) {

            controllerData.lastCharacterCode = characterCode;

            if (gameObject.activeInHierarchy) {
                StartCoroutine(LoadCharacterObject());
            }
        }
    }
 
    public virtual IEnumerator LoadCharacterObject() {
        
        if (controllerData.loadingCharacter) {
            yield break;
        }   

        GameCharacter gameCharacter = 
            GameCharacters.Instance.GetById(characterCode);

        if(gameCharacter == null) {
            yield break;
        }

        controllerData.loadingCharacter = true;

        string prefabCode = gameCharacter.data.GetModel().code;
        
        //LogUtil.Log("LoadCharacter:path:" + path);
        if(!string.IsNullOrEmpty(prefabCode)) {
            if (gamePlayerModelHolderModel.transform.childCount > 0) {
                // Remove all current characters
                foreach (Transform t in gamePlayerModelHolderModel.transform) {
                    // Pool safely destroys either way
                    GameObjectHelper.DestroyGameObject(
                        t.gameObject, GameConfigs.usePooledGamePlayers);

                    //LogUtil.Log("LoadCharacter:destroy pooled:t.name:" + t.name);
                }
            }

            gameObjectLoad = AppContentAssets.LoadAsset(prefabCode);

            if (gameObjectLoad != null) {           

                if (IsPlayerControlled) {                    
                    if (!gameObjectLoad.Has<GameCustomPlayer>()) {
                        gameCustomPlayer = gameObjectLoad.AddComponent<GameCustomPlayer>();
                    }
                    else {
                        gameCustomPlayer = gameObjectLoad.GetComponent<GameCustomPlayer>();
                    }

                    if(gamePlayerEffectsContainer != null) {
                        gamePlayerEffectsContainer.Show();
                    }

                }
                else {
                    
                    if(gamePlayerEffectsContainer != null) {
                        gamePlayerEffectsContainer.Hide();
                    }

                    if (!gameObjectLoad.Has<GameCustomEnemy>()) {
                        gameCustomEnemy = gameObjectLoad.AddComponent<GameCustomEnemy>();
                    }
                    else {
                        gameCustomEnemy = gameObjectLoad.GetComponent<GameCustomEnemy>();
                    }
                }
                                
                if (!IsPlayerControlled 
                    && GameAIController.generateType == GameAICharacterGenerateType.team) {
                    // apply team colors and textures

                    GameTeam team = GameTeams.Current;

                    if (team != null) {
                        if (team.data != null) {
                                                                                    
                            GameCustomInfo customInfo = new GameCustomInfo();

                            customInfo.actorType = GameCustomActorTypes.enemyType;
                            customInfo.presetColorCode = team.data.GetColorPreset().code;//GetColorPresetCode();
                            customInfo.presetTextureCode = team.data.GetTexturePreset().code;
                            customInfo.type = GameCustomTypes.teamType;
                            customInfo.teamCode = team.code;

                            gameCustomEnemy.Load(customInfo);
                        }
                    }                    
                }

                gameObjectLoad.transform.parent = gamePlayerModelHolderModel.transform;
                gameObjectLoad.transform.localScale = Vector3.one;
                gameObjectLoad.transform.position = Vector3.zero;
                gameObjectLoad.transform.localPosition = Vector3.zero;
                gameObjectLoad.transform.rotation = gamePlayerModelHolderModel.transform.rotation;
                gameObjectLoad.transform.localRotation = gamePlayerHolder.transform.localRotation;
                                
                initialScale = transform.localScale;

                // load items

                foreach (GamePlayerObjectItem objectItem 
                        in gameObjectLoad.GetComponentsInChildren<GamePlayerObjectItem>(true)) {

                    if (IsPlayerControlled) {
                        objectItem.gameObject.Show();
                    }
                    else {
                        objectItem.gameObject.Hide(); 
                    }
                }
                
                //LogUtil.Log("LoadCharacter:create game object:gameObjectLoad.name:" + gameObjectLoad.name);

                foreach (Transform t in gameObjectLoad.transform) {
                    //t.localRotation = gamePlayerModelHolderModel.transform.rotation;
                    GamePlayerIndicator.AddIndicator(GameHUD.Instance.containerOffscreenIndicators, 
                                                     t.gameObject, "bot1");
                    break;

                }
             
                if (controllerData.gamePlayerControllerAnimation != null) {
                    controllerData.gamePlayerControllerAnimation.ResetAnimatedActor(gamePlayerModelHolderModel);
                    controllerData.gamePlayerControllerAnimation.Reset();
                }

                if (!gameObjectLoad.Has<GamePlayerControllerAsset>()) {
                    gamePlayerControllerAsset = gameObjectLoad.AddComponent<GamePlayerControllerAsset>();
                }

            }
        }

        ChangePlayerState(controllerState);
             
        LoadWeapons();

        controllerData.loadingCharacter = false;
        
        ResetPosition();       
        
        InitControls();
        
        paused = false; 
        
        HidePlayerEffectWarp();
        
        if (IsPlayerControlled) {
            GetPlayerProgress();
            controllerData.lastPlayerEffectsTrailUpdate = 0;
            HandlePlayerEffectsTick();
        }
        
        controllerData.initialized = true;

    }
 
    // --------------------------------------------------------------------
    // WEAPONS   

    public List<string> weaponInventory;
    public int weaponInventoryIndex = 0;

    public virtual void LoadInventory() {

        ////LogUtil.Log("LoadInventory");
    
        if (weaponInventory == null) {
            weaponInventory = new List<string>();
        }

        weaponInventory.Clear();

        foreach (GameWeapon weapon in GameWeapons.Instance.GetAll()) {
            weaponInventory.Add(weapon.code);
        }
        
        // TODO load from data
        SetItemsData(new GamePlayerItemsData());
    }
    
    public virtual void UnloadWeapons() {
        if (gamePlayerModelHolderWeaponsHolder != null) {
            gamePlayerModelHolderWeaponsHolder.DestroyChildren();
        }
        
        if (weapons == null) {
            weapons = new Dictionary<string, GamePlayerWeapon>();
        }

        weapons.Clear();
    }
 
    public virtual void LoadWeapons() {
        
        //LogUtil.Log("LoadWeapons");
        if (gamePlayerModelHolderWeaponsHolder != null) {
            initialGamePlayerWeaponContainer = gamePlayerModelHolderWeaponsHolder.transform.position;
            currentGamePlayerWeaponContainer = gamePlayerModelHolderWeaponsHolder.transform.position;

            LoadInventory();

            LoadWeapon(weaponInventory[weaponInventoryIndex]);
        }
    }
    
    public virtual void LoadWeaponNext() {
        LoadWeapon(weaponInventoryIndex + 1);
    }
    
    public virtual void LoadWeaponPrevious() {
        LoadWeapon(weaponInventoryIndex - 1);
    }

    public virtual void LoadWeapon(int index) {
        if (index < 0) {
            index = weaponInventory.Count - 1;
        }
        else if (index > weaponInventory.Count - 1) {
            index = 0;
        }

        weaponInventoryIndex = index;            
        LoadWeapon(weaponInventory[weaponInventoryIndex]);
    }

    public virtual void LoadWeapon(string code) {
        
        UnloadWeapons();

        if (!IsPlayerControlled) {
            return; // TODO enemy weapons
        }

        GameWeapon gameWeaponData = GameWeapons.Instance.GetByCode(code);

        if (gameWeaponData == null) {
            LogUtil.LogWarning("LoadWeapon: NULL gameWeaponData");
            return;
        }

        if (gameWeaponData.data == null) {
            LogUtil.LogWarning("LoadWeapon: NULL gameWeaponData.data");
            return;
        }

        GameDataModel dataModel = gameWeaponData.data.GetModel();
        
        if (dataModel == null) {
            LogUtil.LogWarning("LoadWeapon: NULL dataModel");
            return;
        }

        GameObject go = AppContentAssets.LoadAsset("weapon", dataModel.code);

        if (go == null) {
            return;
        }

        go.transform.parent = gamePlayerModelHolderWeaponsHolder.transform;
        go.ResetPosition();
        go.ResetRotation();

        if (GameConfigs.isGameRunning && IsPlayerControlled) {
            UINotificationDisplayTip.Instance.QueueTip(
                "Weapon Loaded:" + gameWeaponData.display_name,
                gameWeaponData.description);
                
        }

        if (go != null && weapons.Count == 0) {
            
            foreach (GamePlayerWeapon weapon in 
                    gamePlayerModelHolderWeaponsHolder.GetComponentsInChildren<GamePlayerWeapon>()) {

                weapon.gameWeaponData = gameWeaponData;
                                
                LogUtil.Log("LoadWeapon:weapon.name:" + weapon.name);

                weapons.Add(GamePlayerSlots.slotPrimary, weapon);
                weaponPrimary = weapon;                
                break;
            }
        }
    }


 
    // --------------------------------------------------------------------
    // EVENTS

    public virtual void OnInputAxis(string name, Vector3 axisInput) {
                     
        if (!GameConfigs.isGameRunning) {
            return;
        }

        if(controllerData == null) {
            return;
        }
     
        // main
     
        //LogUtil.Log("OnInputAxis:" + name + "input:" + axisInput);
     
        if (name == GameTouchInputAxis.inputAxisMove) {
         
            //LogUtil.Log("OnInputAxis:" + name + "input:" + axisInput);
         
            if (controllerData.thirdPersonController != null) {
             
                //LogUtil.Log("OnInputAxis:" + name + "input:" + axisInput);
                
                if (!axisInput.IsBiggerThanDeadzone(axisDeadZone)) {
                    axisInput.x = 0f;
                    axisInput.y = 0f;
                }

                //if(!GameController.isFingerNavigating) {
                HandleThirdPersonControllerAxis(axisInput);
            }
        }
        else if (name == GameTouchInputAxis.inputAxisAttack) {
         
            //LogUtil.Log("OnInputAxis:" + name + "input:" + axisInput);
         
            if (controllerData.thirdPersonController != null) {
             
                //LogUtil.Log("OnInputAxis ATTACK:" + name + "input:" + axisInput);
                
                if (!axisInput.IsBiggerThanDeadzone(axisDeadZone)) {
                    axisInput.x = 0f;
                    axisInput.y = 0f;
                }
                
                controllerData.thirdPersonController.horizontalInput2 = axisInput.x;
                controllerData.thirdPersonController.verticalInput2 = axisInput.y;

            }
        }
        else if (name == GameTouchInputAxis.inputAxisMoveHorizontal) {
                
            //LogUtil.Log("OnInputAxis:" + name + "input:" + axisInput);
            
            if (controllerData.thirdPersonController != null) {
                    
                //LogUtil.Log("OnInputAxis:" + name + "input:" + axisInput);
                
                if (axisInput.IsBiggerThanDeadzone(axisDeadZone)) {

                    controllerData.thirdPersonController.horizontalInput = axisInput.x;
                    controllerData.thirdPersonController.verticalInput = 0f;//controllerData.thirdPersonController.verticalInput;
                }

                if (axisInput.y > .7f) {
                    //LogUtil.Log("axisInput.y:" + axisInput.y);
                    Jump();
                }
                else {
                    JumpStop();
                }

            }
        }
        else if (name == GameTouchInputAxis.inputAxisMoveVertical) {
                
            //LogUtil.Log("OnInputAxis:" + name + "input:" + axisInput);
            
            if (controllerData.thirdPersonController != null) {
                
                if (axisInput.IsBiggerThanDeadzone(axisDeadZone)) {
                    //LogUtil.Log("OnInputAxis:" + name + "input:" + axisInput);
                    controllerData.thirdPersonController.horizontalInput = 0f;//axisInput.x;
                    controllerData.thirdPersonController.verticalInput = axisInput.y;
                }
            }
        }
        else if (name == GameTouchInputAxis.inputAxisAttack2DSide2) {
                
            //LogUtil.Log("OnInputAxis:" + name + "input:" + axisInput);
            
            if (controllerData.thirdPersonController != null) {
                    
                //LogUtil.Log("OnInputAxis:" + name + "input:" + axisInput);
                
                //controllerData.thirdPersonController.horizontalInput = axisInput.x;
                //controllerData.thirdPersonController.verticalInput = 0f;
                
                if (axisInput.IsBiggerThanDeadzone(axisDeadZone)) {
                    
                    if (controllerState == GamePlayerControllerState.ControllerPlayer) {
                        gamePlayerModelHolderModel
                                .transform.LookAt(-Vector3.zero.WithX(axisInput.x).WithY(axisInput.y));
                    }
                }
                else {
                    //GameController.CurrentGamePlayerController.gamePlayerModel.transform.rotation
                    //       = Quaternion.Euler(Vector3.zero);
                }
                            
                    
            }
        }
        else if (name == GameTouchInputAxis.inputAxisAttack2DSide) {
            
            //LogUtil.Log("OnInputAxis:" + name + "input:" + axisInput);
            
            if (controllerData.thirdPersonController != null) {
                
                if (axisInput.IsBiggerThanDeadzone(axisDeadZone)) {
                    //LogUtil.Log("OnInputAxis ATTACK:" + name + "input:" + axisInput);
                    
                    controllerData.thirdPersonController.horizontalInput2 = -axisInput.x;
                    controllerData.thirdPersonController.verticalInput2 = 0f;//axisInput.y;
                    
                    //UpdateAim(axisInput.x, axisInput.y);
                }
            }
        }
    }

    public virtual void HandleThirdPersonControllerAxis(Vector3 axisInput) {
        if (controllerData.mountData.isMountedVehicle) {

            controllerData.mountData.SetMountVehicleAxis(axisInput.x, axisInput.y);
        }
        else {

            controllerData.thirdPersonController.horizontalInput = axisInput.x;
            controllerData.thirdPersonController.verticalInput = axisInput.y;
        }
    }
        
    public virtual void SetThirdPersonControllerAxisAlt(Vector3 axisInput) {
        
    }
 
    public virtual void OnNetworkActionEvent(Gameverses.GameNetworkingAction actionEvent, Vector3 pos, Vector3 direction) {
         
        if (!GameConfigs.isGameRunning) {
            return;
        }
    
        if (actionEvent.uuidOwner == uniqueId) {
            AnimatePlayer(actionEvent.code);
        }
    }
     
    public virtual void OnNetworkPlayerAnimation(string uid, Gameverses.GameNetworkAniStates aniState) {
             
        if (!GameConfigs.isGameRunning) {
            return;
        }
     
        if (uniqueId == uid && !isMe) {
            if (controllerData.lastNetworkAniState != controllerData.currentNetworkAniState) {
                controllerData.lastNetworkAniState = controllerData.currentNetworkAniState;
             
                if (controllerData.currentNetworkAniState == Gameverses.GameNetworkAniStates.walk) {
                 
                }
                else if (controllerData.currentNetworkAniState == Gameverses.GameNetworkAniStates.run) {
                 
                }
                else if (controllerData.currentNetworkAniState == Gameverses.GameNetworkAniStates.attack1) {
                 
                }
                else if (controllerData.currentNetworkAniState == Gameverses.GameNetworkAniStates.attack2) {
                 
                }
                else if (controllerData.currentNetworkAniState == Gameverses.GameNetworkAniStates.death) {
                 
                }
                else if (controllerData.currentNetworkAniState == Gameverses.GameNetworkAniStates.skill1) {
                 
                }
                else if (controllerData.currentNetworkAniState == Gameverses.GameNetworkAniStates.skill2) {
                 
                }
            }
        }
    }
 
    public virtual void OnNetworkPlayerInputAxisHorizontal(string uid, float horizontalInput) {
        if (uniqueId == uid && !isMe) {
            if (controllerData.thirdPersonController != null) {
                controllerData.thirdPersonController.horizontalInput = horizontalInput;
            }
        }
    }
 
    public virtual void OnNetworkPlayerInputAxisVertical(string uid, float verticalInput) {
        if (uniqueId == uid && !isMe) {
            if (controllerData.thirdPersonController != null) {
                controllerData.thirdPersonController.verticalInput = verticalInput;
            }
        }
    }
 
    public virtual void OnNetworkPlayerSpeed(string uid, float speed) {
        if (uniqueId == uid && !isMe) {
            if (controllerData.thirdPersonController != null) {
                controllerData.thirdPersonController.moveSpeed = speed;
            }            
        }
    }
 
    public override void OnInputDown(InputTouchInfo touchInfo) {
        LogUtil.Log("OnInputDown GameActor");        
     
    }
    
    // --------------------------------------------------------------------
    // MOUNT VEHICLE


    public virtual void SetControllersState(bool running) {

        if (controllerData.characterController == null) {
            return;
        }

        controllerData.characterController.enabled = running;
    }

    public virtual void MountNearest() {
        
        if (controllerData.mountData == null) {
            return;
        }

        if (controllerData.mountData.isMountedVehicle) {
            Unmount();
        }
        else {
            MountNearest<GameObjectMountVehicle>();
        }
    }

    public virtual void MountNearest<T>() where T : Component {

        List<T> mounts = new List<T>();
        T nearest = default(T);
        bool found = false;
        float mountRange = 10f;
        float shortestDistance = mountRange * 2;

        foreach (Collider collide in Physics.OverlapSphere(transform.position, mountRange)) {

            Transform t = collide.transform;

            if (t != null) {

                if (t.name.ToLower().Contains("mount")) {

                    T mountObject = t.gameObject.Get<T>();

                    if (mountObject != null) {
                       
                        float currentDistance = Vector3.Distance(
                            transform.position,
                            mountObject.transform.position);

                        if (currentDistance < shortestDistance) {
                            found = true;
                            shortestDistance = currentDistance; 
                            nearest = mountObject;
                        }
                    }
                    
                }
            }
        }

        if (found && nearest != null) {
            Mount(nearest.gameObject);
        }
    }

    public virtual void Mount(GameObject go) {
        if (go.Has<GameObjectMountVehicle>()) {        // MOUNT VEHICLES     
            if (!controllerData.mountData.isMountedVehicleObject) {
                controllerData.mountData.MountVehicle(gameObject, 
                    go.Get<GameObjectMountVehicle>());

                if (controllerData.gameModelVisible) {
                    gamePlayerModelHolderModel.Hide();
                    controllerData.gameModelVisible = false;
                }

                GameObjectMountWeaponHolder weaponHolder = controllerData.mountData.mountVehicle.GetWeaponHolder();

                if (weaponHolder != null) {
                    currentGamePlayerWeaponContainer = weaponHolder.transform.position;
                    //gamePlayerModelHolderWeaponsHolder.transform.position = currentGamePlayerWeaponContainer;
                    
                    gamePlayerModelHolderWeaponsHolder.transform.parent = weaponHolder.transform;
                    gamePlayerModelHolderWeaponsHolder.transform.position = weaponHolder.transform.position;
                    gamePlayerModelHolderWeaponsHolder.transform.rotation = weaponHolder.transform.rotation;
                }

                SetControllersState(false);

                StopNavAgent();
            }
        }
    }

    public virtual void Unmount() {
        if (controllerData.mountData.isMountedVehicleObject) {
            controllerData.mountData.UnmountVehicle();
                        
            if (!controllerData.gameModelVisible) {
                gamePlayerModelHolderModel.Show();
                controllerData.gameModelVisible = true;
            }
                        
            //gamePlayerModelHolderWeaponsHolder.transform.position = initialGamePlayerWeaponContainer;
            
            gamePlayerModelHolderWeaponsHolder.transform.parent = gamePlayerModelHolderWeapons.transform;
            gamePlayerModelHolderWeaponsHolder.transform.localPosition = Vector3.zero;
            gamePlayerModelHolderWeaponsHolder.transform.rotation = gamePlayerModelHolderWeapons.transform.rotation;
            
            SetControllersState(true);

            StartNavAgent();
        }
    }

    public bool controllerReady {
        get{
            if(paused) {
                return false;
            }

            if(!gameObject.activeSelf) {
                return false;
            }

            if(!paused && controllerData != null) {
                if(!controllerData.loadingCharacter) {
                    return true;
                }
            }

            return false;
        }
    }
 
    // --------------------------------------------------------------------
    // COLLISIONS/TRIGGERS

    public virtual void HandleCollision(Collision collision) {
                                
        if (!GameConfigs.isGameRunning) {
            return;
        }

        if (controllerData.lastCollision + .2f < Time.time) {
            controllerData.lastCollision = Time.time;
        }
        else {
            return;
        }
        
        if(!controllerReady) {
            return;
        }

        if (collision.contacts.Length > 0) {
            foreach (ContactPoint contact in collision.contacts) {
                //Debug.DrawRay(contact.point, contact.normal, Color.white);
                     
                Transform t = contact.otherCollider.transform;
                if (t.parent != null) {
                    string parentName = t.parent.name;

                    // TODO make name recursion by depth limit, for now check three above.
                    string parentParentName = "";
                    string parentParentParentName = "";
                    if (t.parent.parent != null) {
                        parentParentName = t.parent.parent.name;                        
                        if (t.parent.parent.parent != null) {
                            parentParentParentName = t.parent.parent.parent.name;
                        }
                    }

                    bool isObstacle = parentName.Contains("GameObstacle");                  

                    bool isLevelObject = parentName.Contains("GameItemObject")
                        || parentParentName.Contains("GameItemObject")
                        || parentParentParentName.Contains("GameItemObject");                

                    bool isPlayerObject = 
                        parentName.Contains("HelmetContainer")
                        || parentName.Contains("Helmet")
                        || parentName.Contains("Facemask")
                        || t.name.Contains("Helmet")
                        || t.name.Contains("Facemask")
                        || parentName.Contains("HitCollider")
                        || parentName.Contains("GamePlayerCollider");
                    //|| t.name.Contains("GamePlayerObject")
                    //|| t.name.Contains("GamePlayerEnemy")
                    //|| t.name.Contains("GameEnemy");  

                    if (isLevelObject) {
                        GameLevelSprite sprite = t.gameObject.FindTypeAboveRecursive<GameLevelSprite>();
                        if (sprite == null) {
                            sprite = t.parent.gameObject.GetComponentInChildren<GameLevelSprite>();
                        }
                        if (sprite != null) {                        
                            isLevelObject = true;
                        }
                        else { 
                            isLevelObject = false;
                        }
                    }
                                     
                    if (isObstacle || isLevelObject) {
                        if (IsPlayerState()) {
                            AudioAttack();
                            Score(1);
                            GamePlayerProgress.SetStatHitsObstacles(1f);
                        }
                    }
                    else if (isPlayerObject) {

                        // handle stat

                        //if (IsPlayerControlled) {
                        controllerData.collisionController = GameController.GetGamePlayerControllerObject(
                            t.gameObject, false);

                        if (controllerData.collisionController != null) {
                            
                            if(!controllerData.collisionController.controllerReady) {
                                break;
                            }

                            if (!IsPlayerControlled) {
                                // we hit a player, so we are an enemy
                                GamePlayerProgress.SetStatHitsReceived(1f);

                            }
                            else {
                                // we hit an enemy, so we are the player
                                GamePlayerProgress.SetStatHits(1f);
                            }
                        }
                        //}

                        // handle hit

                        float power = .1f;

                        runtimeData.health -= power;

                        //contact.normal.magnitude

                        Hit(power);

                        //GamePlayerProgress.Instance.ProcessProgressSpins

                        //GameProfileCharacters.currentProgress.SubtractGamePlayerProgressHealth(power); // TODO get by skill upgrade
                        //GameProfileCharacters.currentProgress.SubtractGamePlayerProgressEnergy(power/2f); // TODO get by skill upgrade

                        Vector3 normal = contact.normal;
                        float magnitude = contact.point.sqrMagnitude;
                        float hitPower = (magnitude * (float)runtimeData.mass) / 110;
                        //LogUtil.Log("hitPower:" + hitPower);
                        AddImpact(normal, Mathf.Clamp(hitPower, 0f, 80f));
                    }
                }
                break;
            }
        }
     
        //if (collision.relativeVelocity.magnitude > 2)
        //    audio.Play();      
    }
 
    //GamePlayerController gamePlayerControllerHit;
        
    //void OnCollisionEnter(Collision collision) {
    //    if(!GameController.shouldRunGame) {
    //            return;
    //    }
        
    //GameObject target = collision.collider.gameObject;
    //LogUtil.Log("hit object:" + target);
        
    //if(target != null) {
    //    gamePlayerControllerHit = target.GetComponent<GamePlayerController>();
         
    //   if(gamePlayerControllerHit != null) {
             
    //DeviceUtil.Vibrate();
    //       LogUtil.Log("hit another game player");
    //   }
    //}
    // }

    public float lastCollision = 0f;
    public float intervalCollision = .2f;
    //private ParticleSystem.CollisionEvent[] collisionEvents = new ParticleSystem.CollisionEvent[16];
    
    public virtual void OnParticleCollision(GameObject other) {

        if(!controllerReady) {
            return;
        }
        
        if (!GameConfigs.isGameRunning) {
            return;
        }
        
        if (lastCollision + intervalCollision < Time.time) {
            //lastCollision = Time.time;
        }
        else {
            // return;
        }

            
        /*
            ParticleSystem particleSystem;
            particleSystem = other.GetComponent<ParticleSystem>();
            int safeLength = particleSystem.safeCollisionEventSize;
            if (collisionEvents.Length < safeLength)
                collisionEvents = new ParticleSystem.CollisionEvent[safeLength];
            
            int numCollisionEvents = particleSystem.GetCollisionEvents(gameObject, collisionEvents);
            int i = 0;
            while (i < numCollisionEvents) {
                if (gameObject.rigidbody) {
                    Vector3 pos = collisionEvents[i].intersection;
                    Vector3 force = collisionEvents[i].velocity * 10;
                    gamePlayerController.gameObject.rigidbody.AddForce(force);
                }
                i++;
            }
            */
            
        //if(gamePlayerController.IsPlayerControlled) {
        //}
        //else {
            
        if (other.name.Contains("projectile")) {
            LogUtil.Log("OnParticleCollision:" + other.name);

            // todo lookup projectile and power to subtract.

            float projectilePower = 1;
            
            float power = projectilePower / 10f;

            if (IsPlayerControlled) {
                // 1/20th power for friendly fire
                power = power / 20f;
            }
            
            runtimeData.health -= power;
            
            //contact.normal.magnitude
            
            Hit(power);
        }
            
        /*
            int safeLength = particleSystem.safeCollisionEventSize;
            if (collisionEvents.Length < safeLength)
                collisionEvents = new ParticleSystem.CollisionEvent[safeLength];
            
            int numCollisionEvents = particleSystem.GetCollisionEvents(other, collisionEvents);
            int i = 0;
            while (i < numCollisionEvents) {
                if (other.rigidbody) {
                    Vector3 pos = collisionEvents[i].intersection;
                    Vector3 force = collisionEvents[i].velocity * 10;
                    rigidbody.AddForce(force);
                }
                i++;
            }
            */
        //}
    }
     
    public virtual void OnTriggerEnter(Collider collider) {
        
        if(!controllerReady) {
            return;
        }

        // Check if we hit an actual destroyable sprite
        if (!GameController.shouldRunGame) {
            return;
        }
                     
        if (!GameConfigs.isGameRunning) {
            return;
        }
     
        if (controllerState == GamePlayerControllerState.ControllerPlayer) {
     
            string colliderName = collider.name;
         
            if (colliderName.Contains("GameGoalZone")) {
                LogUtil.Log("GameGoalZone: " + colliderName);
                GameController.GamePlayerGoalZone(collider.transform.gameObject);
            }
            else if (colliderName.Contains("GameBadZone")) {
                LogUtil.Log("GameBadZone: " + colliderName);
                GameController.GamePlayerOutOfBounds();
            }
            else if (colliderName.Contains("GameZone")) {
                LogUtil.Log("GameZone: " + colliderName);
                GameController.GamePlayerGoalZoneCountdown(collider.transform.gameObject);
            }
        }
        
        //GameObject target = collider.gameObject;
     
        //LogUtil.Log("hit object:" + target);
        
        //if(target != null) {
        //    GamePlayerController gamePlayerController = target.GetComponent<GamePlayerController>();
         
        //   if(gamePlayerController != null) {
             
        //DeviceUtil.Vibrate();
        //       LogUtil.Log("hit another game player");
        //   }
        //}
    
    }
 
    // --------------------------------------------------------------------
    // ANIMATION

    public virtual void AnimatePlayer(string animationName) {
        if (animationName == GamePlayerAnimationType.skill) {
            InputSkill();
        }
        else if (animationName == GamePlayerAnimationType.attack) {
            InputAttack();
        }
        else if (animationName == GamePlayerAnimationType.attackAlt) {
            InputAttackAlt();
        }
        else if (animationName == GamePlayerAnimationType.attackRight) {
            InputAttackRight();
        }
        else if (animationName == GamePlayerAnimationType.attackLeft) {
            InputAttackLeft();
        }
        else if (animationName == GamePlayerAnimationType.defend) {
            InputDefend();
        }
        else if (animationName == GamePlayerAnimationType.defendAlt) {
            InputDefendAlt();
        }
        else if (animationName == GamePlayerAnimationType.defendRight) {
            InputDefendRight();
        }
        else if (animationName == GamePlayerAnimationType.defendLeft) {
            InputDefendLeft();
        }
        else if (animationName == GamePlayerAnimationType.death
            || animationName == GamePlayerAnimationType.die) {
            InputDie();
        }
        else if (animationName == GamePlayerAnimationType.jump) {
            InputJump();
        }
        else if (animationName == GamePlayerAnimationType.strafeLeft) {
            InputStrafeLeft();
        }
        else if (animationName == GamePlayerAnimationType.strafeRight) {
            InputStrafeRight();
        }
        else if (animationName == GamePlayerAnimationType.use) {
            InputUse();
        }
        else if (animationName == GamePlayerAnimationType.mount) {
            InputMount();
        }
    }
 
    public virtual void OnPlayerAnimation(string animationName, string uniqueId) {
             
        if (!GameConfigs.isGameRunning) {
            return;
        }
     
        LogUtil.Log("OnPlayerAnimation: " + animationName);
     
        /*
     if(Network.isClient || Network.isServer) {
     
         // call them over the network
         
         if(IsPlayerControlled) {
             Gameverses.GameNetworkingAction actionEvent = new Gameverses.GameNetworkingAction();
             actionEvent.uuid = UniqueUtil.Instance.CreateUUID4();
             actionEvent.uuidOwner = uuid;
             actionEvent.code = animationName;
             actionEvent.type = Gameverses.GameNetworkingPlayerTypeMessages.PlayerTypeAction;
             
             
             //Gameverses.GameversesGameAPI.Instance.SendActionMessage(actionEvent, Vector3.zero, Vector3.zero);
         }
     }
     else  {
     */
        if (IsPlayerControlled) {
            if (controllerData.gamePlayerControllerAnimation != null) {
                AnimatePlayer(animationName);
            }
        }
        //}
    }
 
    // --------------------------------------------------------------------
    // STATE/RESET

    public virtual void ResetPositionAir(float y) {    

        //if(IsPlayerControlled) {
        if (controllerData.lastAirCheck > 1f) {
            controllerData.lastAirCheck = 0;

            gameObject.transform.position = Vector3.Lerp(
            gameObject.transform.position, 
            gameObject.transform.position.WithY(y), 
            1 * Time.deltaTime);            
        }

        controllerData.lastAirCheck += Time.deltaTime;
        //}
    }

    public virtual void ResetPosition() {

        foreach (Transform t in gamePlayerModelHolderModel.transform) {
            t.position.Reset();
            t.localPosition.Reset();
            t.rotation.Reset();
            t.localRotation.Reset();
            t.rotation = Quaternion.Euler(0f, 0f, 0f);
        }

        if(IsPlayerControlled) {
            gameObject.ResetPosition();
        }

        //transform.position = Vector3.zero.WithY(1.5f);
    }

    
    public virtual void SetUp(
        GamePlayerControllerState controllerStateTo, 
        GamePlayerContextState contextStateTo) {        
        
        if(controllerState != controllerStateTo 
           || controllerState == GamePlayerControllerState.ControllerNotSet) {
           
            controllerState = controllerStateTo;
            contextState = contextStateTo;
        }

        Reset();
    }
         
    public virtual void Reset() {

        if(IsPlayerControlled) {
            
            SetControllerData(new GamePlayerControllerData());
        }

        SetControllerData(new GamePlayerControllerData());
        SetRuntimeData(new GamePlayerRuntimeData());
                
        if (IsPlayerControlled) {
            uniqueId = UniqueUtil.Instance.currentUniqueId;
        }
        else {
            uniqueId = UniqueUtil.Instance.CreateUUID4();
        }
        
        LoadCharacter(characterCode);  

    }
 
    public virtual void Remove() {
        if (!IsPlayerControlled) {

            foreach (Transform t in gamePlayerModelHolderModel.transform) {
                GameObjectHelper.DestroyGameObject(t.gameObject, GameConfigs.usePooledGamePlayers);
            }

            GameObjectHelper.DestroyGameObject(gameObject, GameConfigs.usePooledGamePlayers);
        }
    }
 
    // --------------------------------------------------------------------
    // COMBAT/HIT/ATTACK

    //MountNearest
        
    public virtual void SendMount() {
        Messenger<string, string>.Broadcast(
            GamePlayerMessages.PlayerAnimation,
            GamePlayerAnimationType.mount,
            UniqueUtil.Instance.currentUniqueId);
    }
 
    public virtual void SendUse() {
        Messenger<string, string>.Broadcast(
            GamePlayerMessages.PlayerAnimation,
            GamePlayerAnimationType.use,
            UniqueUtil.Instance.currentUniqueId);
    }
    
    public virtual void SendJump() {
        Messenger<string, string>.Broadcast(
            GamePlayerMessages.PlayerAnimation,
            GamePlayerAnimationType.jump,
            UniqueUtil.Instance.currentUniqueId);
    }

    public virtual void SendStrafeLeft() {
        Messenger<string, string>.Broadcast(
            GamePlayerMessages.PlayerAnimation,
            GamePlayerAnimationType.strafeLeft,
            UniqueUtil.Instance.currentUniqueId);
    }

    public virtual void SendStrafeRight() {
        Messenger<string, string>.Broadcast(
            GamePlayerMessages.PlayerAnimation,
            GamePlayerAnimationType.strafeRight,
            UniqueUtil.Instance.currentUniqueId);
    }

    public virtual void SendBoost() {
        Messenger<string, string>.Broadcast(
            GamePlayerMessages.PlayerAnimation,
            GamePlayerAnimationType.boost,
            UniqueUtil.Instance.currentUniqueId);
    }

    public virtual void SendSkill() {
        Messenger<string, string>.Broadcast(
            GamePlayerMessages.PlayerAnimation,
            GamePlayerAnimationType.skill,
            UniqueUtil.Instance.currentUniqueId);
    }
    
    public virtual void SendAttack() {
        Messenger<string, string>.Broadcast(
            GamePlayerMessages.PlayerAnimation,
            GamePlayerAnimationType.attack,
            UniqueUtil.Instance.currentUniqueId);
    }

    public virtual void SendAttackAlt() {
        Messenger<string, string>.Broadcast(
            GamePlayerMessages.PlayerAnimation,
            GamePlayerAnimationType.attackAlt,
            UniqueUtil.Instance.currentUniqueId);
    }

    public virtual void SendAttackRight() {
        Messenger<string, string>.Broadcast(
            GamePlayerMessages.PlayerAnimation,
            GamePlayerAnimationType.attackRight,
            UniqueUtil.Instance.currentUniqueId);
    }

    public virtual void SendAttackLeft() {
        Messenger<string, string>.Broadcast(
            GamePlayerMessages.PlayerAnimation,
            GamePlayerAnimationType.attackLeft,
            UniqueUtil.Instance.currentUniqueId);
    }

    public virtual void SendDefend() {
        Messenger<string, string>.Broadcast(
            GamePlayerMessages.PlayerAnimation,
            GamePlayerAnimationType.defend,
            UniqueUtil.Instance.currentUniqueId);
    }

    public virtual void SendDefendAlt() {
        Messenger<string, string>.Broadcast(
            GamePlayerMessages.PlayerAnimation,
            GamePlayerAnimationType.defendAlt,
            UniqueUtil.Instance.currentUniqueId);
    }

    public virtual void SendDefendRight() {
        Messenger<string, string>.Broadcast(
            GamePlayerMessages.PlayerAnimation,
            GamePlayerAnimationType.defendRight,
            UniqueUtil.Instance.currentUniqueId);
    }

    public virtual void SendDefendLeft() {
        Messenger<string, string>.Broadcast(
            GamePlayerMessages.PlayerAnimation,
            GamePlayerAnimationType.defendLeft,
            UniqueUtil.Instance.currentUniqueId);
    }
 
    // ANIMATIONS/TYPES
 
    public virtual void InputAttack() {
        Attack();
    }
 
    public virtual void InputAttackAlt() {
        AttackAlt();
    }
 
    public virtual void InputAttackLeft() {
        AttackLeft();
    }
 
    public virtual void InputAttackRight() {
        AttackRight();
    }

    public virtual void InputDefend() {
        //Defend();
    }
    
    public virtual void InputDefendAlt() {
        //DefendAlt();
    }
    
    public virtual void InputDefendLeft() {
        //DefendLeft();
    }
    
    public virtual void InputDefendRight() {
        //DefendRight();
    }
 
    public virtual void InputJump() {
        Jump();
    }

    public virtual void InputBoost() {
        Boost();
    }
 
    public virtual void InputUse() {
        // USE

    }
    
    public virtual void InputMount() {
        // MOUNT
        MountNearest();        
    }

    public virtual void InputSkill() {
        Skill();
    }

    public virtual void InputMagic() {
        //Magic();
    }
 
    public virtual void InputStrafeLeft() {
        StrafeLeft();
    }
 
    public virtual void InputStrafeRight() {
        StrafeRight();
    }
 
    public virtual void InputDie() {
        Die();
    }
 
    public virtual void InputSpin() {
        //Skill();
        // Straightarm
    }

    public virtual void Hit(float power) {
        if (!GameConfigs.isGameRunning) {
            return;
        }
     
        if (IsPlayerControlled) {          
            GameHUD.Instance.ShowHitOne((float)(1.5 - runtimeData.health));
            Score(2 * power);
            DeviceUtil.Vibrate();
        }
        else {
            //bool allow = false;

            if (controllerData.lastHit + .3f < Time.time) {
                controllerData.lastHit = Time.time;
                //allow = true;
            }
            else {
                return;
            }

            Score(-1);
        }
     
        //GameUIPanelOverlays.Instance.ShowOverlayWhiteFlash();
        //GameHUD.Instance.ShowHitOne();
             
        if (controllerState == GamePlayerControllerState.ControllerAgent) {
            power = power * 1;
        }
        else if (IsPlayerControlled) {
            power = power * 1;
        }
     
        runtimeData.health -= power * .1f;

        AudioHit();

        if (runtimeData.health < 0) {
            Die();
        }
    }
    
    public virtual void Idle() {
        if (isDead) {
            return;
        }
        
        //controllerData.thirdPersonController.Idle();
        
        controllerData.gamePlayerControllerAnimation.DidIdle();
    }

    public virtual void Jump() {
        if (isDead) {
            return;
        }
        
        controllerData.thirdPersonController.Jump();
        
        controllerData.gamePlayerControllerAnimation.DidJump();
        
        if (gamePlayerEffectSkill != null) {
            gamePlayerEffectSkill.Emit(1);
        }
    }
    
    public virtual void JumpStop() {
        if (isDead) {
            return;
        }
        
        controllerData.thirdPersonController.JumpStop();
    }

    // SKILL
 
    public virtual void Skill() {
        if (isDead) {
            return;
        }
     
        controllerData.gamePlayerControllerAnimation.DidSkill();
     
        if (gamePlayerEffectSkill != null) {
            gamePlayerEffectSkill.Emit(1);
        }
    }

    // STRAFE

    // STRAFE LEFT

    public virtual void DidStrafeLeft(Vector3 dir) {
        StrafeLeft(dir);
    }

    public virtual void DidStrafeLeft(float power) {
        StrafeLeft(power);
    }

    public virtual void DidStrafeLeft(Vector3 dir, float power) {
        StrafeLeft(dir, power);
    }

    public virtual void StrafeLeft() {
        Vector3 dir = transform.TransformPoint(transform.localPosition.WithX(-1));//Vector3.zero.WithX(-1);
        float power = 10f + 5f * (float)controllerData.runtimeRPGData.modifierAttack;
        StrafeLeft(dir, power);
    }

    public virtual void StrafeLeft(Vector3 dir) {
        float power = 10f + 5f * (float)controllerData.runtimeRPGData.modifierAttack;
        StrafeLeft(dir, power);
    }

    public virtual void StrafeLeft(float power) {
        Vector3 dir = transform.TransformPoint(transform.localPosition.WithX(-1));//Vector3.zero.WithX(-1);
        StrafeLeft(dir, power);
    }

    public virtual void StrafeLeft(Vector3 dir, float power) {
        //LogUtil.Log("GamePlayerController:DidStrafeLeft:");

        if(!controllerReady) {
            return;
        }

        if (isDead) {
            return;
        }

        if (Time.time > controllerData.lastStrafeLeftTime + 1f) {
            controllerData.gamePlayerControllerAnimation.DidStrafeLeft();

            GamePlayerProgress.Instance.ProcessProgressTotal(GameStatCodes.cuts, 1f);
            GamePlayerProgress.Instance.ProcessProgressTotal(GameStatCodes.cutsLeft, 1f);

            controllerData.lastStrafeLeftTime = Time.time;
            StartCoroutine(DidStrafeLeftCo(dir, power));
        }
    }

    public virtual IEnumerator DidStrafeLeftCo(Vector3 dir, float power) {
        AddForce(dir, power, false);
        yield return new WaitForEndOfFrame();
    }

    // STRAFE RIGHT


    public virtual void DidStrafeRight(Vector3 dir) {
        StrafeRight(dir);
    }

    public virtual void DidStrafeRight(float power) {
        StrafeRight(power);
    }

    public virtual void DidStrafeRight(Vector3 dir, float power) {
        StrafeRight(dir, power);
    }

    public virtual void StrafeRight() {
        Vector3 dir = transform.localPosition.WithX(1);
        float power = 10f + 5f * (float)controllerData.runtimeRPGData.modifierAttack;
        StrafeRight(dir, power);
    }

    public virtual void StrafeRight(Vector3 dir) {
        float power = 10f + 5f * (float)controllerData.runtimeRPGData.modifierAttack;
        StrafeRight(dir, power);
    }

    public virtual void StrafeRight(float power) {
        Vector3 dir = transform.localPosition.WithX(1);
        StrafeRight(dir, power);
    }

    public virtual void StrafeRight(Vector3 dir, float power) {
        //LogUtil.Log("GamePlayerController:DidStrafeRight:");

        if (isDead) {
            return;
        }
        if (Time.time > controllerData.lastStrafeRightTime + 1f) {

            controllerData.gamePlayerControllerAnimation.DidStrafeRight();

            GamePlayerProgress.Instance.ProcessProgressTotal(GameStatCodes.cuts, 1f);
            GamePlayerProgress.Instance.ProcessProgressTotal(GameStatCodes.cutsRight, 1f);

            controllerData.lastStrafeRightTime = Time.time;
            StartCoroutine(DidStrafeRightCo(dir, power));
        }
    }

    public virtual IEnumerator DidStrafeRightCo(Vector3 dir, float power) {
        AddForce(dir, power, false);
        yield return new WaitForEndOfFrame();
    }

    // BOOST

    public virtual void DidBoost(Vector3 dir) {
        Boost(dir);
    }

    public virtual void DidBoost(float power) {
        Boost(power);
    }

    public virtual void DidBoost(Vector3 dir, float power) {
        Boost(dir, power);
    }

    public virtual void Boost() {
        Vector3 dir = transform.forward;
        float power = 10f + 5f * (float)controllerData.runtimeRPGData.modifierAttack;
        Boost(dir, power);
    }

    public virtual void Boost(Vector3 dir) {
        float power = 10f + 5f * (float)controllerData.runtimeRPGData.modifierAttack;
        Boost(dir, power);
    }

    public virtual void Boost(float power) {
        Vector3 dir = transform.forward;
        Boost(dir, power);
    }

    public virtual void Boost(Vector3 dir, float power) {
        if (isDead) {
            return;
        }
        //LogUtil.Log("GamePlayerController:Boost:");
        if (Time.time > controllerData.lastBoostTime + 1f) {
            controllerData.lastBoostTime = Time.time;

            controllerData.gamePlayerControllerAnimation.DidBoost();
            GamePlayerProgress.SetStatBoosts(1f);
            StartCoroutine(DidBoostCo(dir, power));
        }
    }

    public virtual IEnumerator DidBoostCo(Vector3 dir, float power) {
        AddForce(dir, power, false);
        yield return new WaitForEndOfFrame();
    }

    // SPIN

    public virtual void DidSpin(Vector3 dir) {
        Spin(dir);
    }

    public virtual void DidSpin(float power) {
        Spin(power);
    }

    public virtual void DidSpin(Vector3 dir, float power) {
        Spin(dir, power);
    }

    public virtual void Spin() {
        Vector3 dir = transform.localPosition.WithZ(-1);
        float power = 10f + 5f * (float)controllerData.runtimeRPGData.modifierAttack;
        Boost(dir, power);
    }

    public virtual void Spin(Vector3 dir) {
        float power = 10f + 5f * (float)controllerData.runtimeRPGData.modifierAttack;
        Boost(dir, power);
    }

    public virtual void Spin(float power) {
        Vector3 dir = transform.localPosition.WithZ(-1);
        Boost(dir, power);
    }

    public virtual void Spin(Vector3 dir, float power) {
        if (isDead) {
            return;
        }
        //LogUtil.Log("GamePlayerController:Spin:");
        if (Time.time > controllerData.lastSpinTime + 1f) {
            controllerData.lastSpinTime = Time.time;

            //UITweenerUtil.RotateTo(gamePlayerModelHolder, UITweener.Method.Linear, UITweener.Style.Once, .2f, 0f, Vector3.zero.WithY(179));
            //UITweenerUtil.RotateTo(gamePlayerModelHolder, UITweener.Method.Linear, UITweener.Style.Once, .2f, .2f, Vector3.zero.WithY(290));
            //UITweenerUtil.RotateTo(gamePlayerModelHolder, UITweener.Method.Linear, UITweener.Style.Once, .2f, .4f, Vector3.zero.WithY(0));

            controllerData.playerSpin = true;

            //iTween.RotateTo(gamePlayerModelHolderModel, iTween.Hash("y", Vector3.zero.WithY(179).y, "time", .2f, "delay", 0f, "easetype", "linear", "space", "local"));
            //iTween.RotateTo(gamePlayerModelHolderModel, iTween.Hash("y", Vector3.zero.WithY(290).y, "time", .2f, "delay", .21f, "easetype", "linear", "space", "local"));
            //iTween.RotateTo(gamePlayerModelHolderModel, iTween.Hash("y", Vector3.zero.WithY(0).y, "time", .2f, "delay", .41f, "easetype", "linear", "space", "local"));

            controllerData.gamePlayerControllerAnimation.DidSpin();
            GamePlayerProgress.SetStatSpins(1f);

            StartCoroutine(DidSpinCo(dir, power));
        }
    }

    public virtual IEnumerator DidSpinCo(Vector3 dir, float power) {
        AddForce(dir, power, false);
        yield return new WaitForEndOfFrame();
    }

    // DIE

    public virtual void DidDie(Vector3 dir) {
        Die(dir);
    }

    public virtual void DidDie(float power) {
        Die(power);
    }

    public virtual void DidDie(Vector3 dir, float power) {
        Die(dir, power);
    }

    public virtual void Die() {
        Vector3 dir = Vector3.zero.WithZ(1);
        float power = 50f;
        Die(dir, power);
    }

    public virtual void Die(Vector3 dir) {
        float power = 10f;
        Die(dir, power);
    }

    public virtual void Die(float power) {
        Vector3 dir = Vector3.zero.WithZ(1);
        Die(dir, power);
    }
             
    public virtual void Die(Vector3 dir, float power) {
        if (!GameConfigs.isGameRunning) {
            return;
        }
     
        if (controllerData.lastDie + 3f < Time.time) {
            controllerData.lastDie = Time.time;
        }
        else {
            return;
        }
                     
        if (isDead && controllerData.dying) {
            return;
        }
                
        if (controllerData.thirdPersonController != null) {
            controllerData.thirdPersonController.controllerData.removing = true;
        }
        
        controllerData.gamePlayerControllerAnimation.DidDie();
                

        controllerData.dying = true;

        if (IsPlayerControlled) {
            GamePlayerProgress.SetStatDeaths(1f);
        }
        else {
            GamePlayerProgress.SetStatKills(1f);
        }
     
        /*
        if (gamePlayerEffectDeath != null) {
            gamePlayerEffectDeath.Emit(1);
        }
        */
     
        if (IsPlayerControlled) {
            PlayerEffectWarpFadeIn();
        }
                
        runtimeData.health = 0;
     
        AudioDie();
        
        StopNavAgent();
        
        ResetPositionAir(0f);
     
        // TODO FADE OUT CLEANLY
        /*
        // fade out 
        UnityEngine.SkinnedMeshRenderer[] skinRenderersCharacter 
         = gamePlayerHolder.GetComponentsInChildren<SkinnedMeshRenderer>();
     
        foreach (SkinnedMeshRenderer skinRenderer in skinRenderersCharacter) {
         
            UITweenerUtil.FadeTo(skinRenderer.gameObject, UITweener.Method.Linear, UITweener.Style.Once, 1f, 2f, 0f);        
        }
        */
     
        Invoke("Remove", 3);

    }

    public virtual void StartNavAgent() {
        
        if (!IsPlayerControlled || gameObject.Has<CharacterController>()) {
            if (controllerData.navMeshAgent != null) {
                controllerData.navMeshAgent.enabled = true;
                controllerData.navMeshAgent.Resume();
            }
            if (controllerData.navMeshAgentController != null) {
                controllerData.navMeshAgentController.StartAgent();
            }
            if (controllerData.navMeshAgentFollowController != null) {
                controllerData.navMeshAgentFollowController.StartAgent();
            }
        }
    }

    public virtual void StopNavAgent() {

        if (controllerData.navMeshAgent != null) {
            if (controllerData.navMeshAgent.enabled) {
                controllerData.navMeshAgent.Stop(true);
                controllerData.navMeshAgent.enabled = false;
            }
        }
        if (controllerData.navMeshAgentController != null) {
            controllerData.navMeshAgentController.StopAgent();
        }
        if (controllerData.navMeshAgentFollowController != null) {
            controllerData.navMeshAgentFollowController.StopAgent();
        }
    }
 
    public virtual void AttackAlt() {    
        if (isDead) {
            return;
        }    
        controllerData.gamePlayerControllerAnimation.DidAttackAlt();
        Invoke("AttackEffect", .5f);
    }
 
    public virtual void AttackLeft() {       
        if (isDead) {
            return;
        }
        controllerData.gamePlayerControllerAnimation.DidAttackLeft();
        Invoke("AttackEffect", .5f);
    }
 
    public virtual void AttackRight() {      
        if (isDead) {
            return;
        }
        controllerData.gamePlayerControllerAnimation.DidAttackRight();
        Invoke("AttackEffect", .5f);
    }
 
    public virtual void AttackEffect() {
        if (isDead) {
            return;
        }
        if (gamePlayerEffectAttack != null) {
            gamePlayerEffectAttack.Emit(1);
        }
    }
 
    public virtual void DidAttack() {    
        //LogUtil.Log("GamePlayerController:DidAttack:");
        Attack();
    }
 
    public virtual void Attack() {       
        if (isDead) {
            return;
        }
        if (Time.time > controllerData.lastAttackTime + 1f) {
            controllerData.lastAttackTime = Time.time;
            StartCoroutine(DidAttackCo());
        }
    }
 
    public virtual IEnumerator DidAttackCo() {
        yield return new WaitForSeconds(.5f);
        ActionAttack();
    }

    public virtual void CastAttack() {      

        if(controllerReady) {
            return;
        }
             
        if (!GameConfigs.isGameRunning) {
            return;
        }
     
        // if (Physics.Linecast(transform.position, controllerData.thirdPersonController.aimingDirection)) {
            
        //}
     
        //if(prefabName.IndexOf("norah") > -1) {
        //   distance = 300f;
        //}
        //else if(prefabName.IndexOf("jaime") > -1) {
        //   distance = 30f;
        //}
        //else {
        //   distance = 6f;
        //}
     
        RaycastHit[] hits;
     
        Vector3 directionAttack = transform.forward;
        if (controllerData.thirdPersonController != null) {
            directionAttack = controllerData.thirdPersonController.aimingDirection;
        }        
     
        Debug.DrawRay(transform.position, directionAttack * attackDistance);

        hits = Physics.RaycastAll(transform.position, directionAttack, attackDistance);
        int i = 0;
        while (i < hits.Length) {
            RaycastHit hit = hits[i];
            Transform hitObject = hit.transform;
         
            if(hitObject.name.IndexOf("Game") > -1) {
                if (hitObject != null) {
                    GamePlayerController playerController = GetController(hitObject);
                    if (playerController != null) {

                        Debug.Log("CastAttack:" + " currentUUID:" + uniqueId + " otherID:" + playerController.uniqueId);
                                             
                        ScoreAttack();
                     
                        playerController.Hit(1f);
                     
                        playerController.InputAttack();
                    }
                }
            }
         
            // Renderer renderer = hit.collider.renderer;
            //if (renderer) {
            //    renderer.material.shader = Shader.Find("Transparent/Diffuse");
            //    renderer.material.color.a = 0.3F;
            //}
            i++;
        }
     
    }
 
    public virtual void ScoreAttack() {
        ScoreAttack(10);
    }
 
    public virtual void ScoreAttack(double score) {
        runtimeData.score += score;
    }
 
    public virtual void ActionAttack() {
                     
        //LogUtil.Log("GamePlayerController:ActionAttack:");

        //controllerData.thirdPersonController.ApplyAttack();
     
        //gamePlayerControllerAnimation.DidAttack();

        controllerData.gamePlayerControllerAnimation.DidAttack();
     
        Invoke("AttackEffect", .5f);
     
        //LogUtil.Log("Attacking:");
     
        // shoot ray for type of character
     
        CastAttack();

        AudioAttack();

        // Fire weapons
     
        if (weaponPrimary != null) {
            weaponPrimary.Attack();
        }
             
     
        // TODO wire up to weapons
     
     
        /*
     //LoadWeapons();
     
     if(weapons != null) {
         
         LogUtil.Log("Attacking: weapons:" + weapons);
         
         LogUtil.Log("Attacking: weapons.Count:" + weapons.Count);
         
         if(weapons.Count > 0) {
             //weapons[GamePlayerSlots.slotPrimary].AttackPrimary();
             //LogUtil.Log("Attacking: AttackPrimary:" + weapons[GamePlayerSlots.slotPrimary]);
         }
     }
     */
    }
    
    public virtual void ShootOne() {
             
        if (!GameConfigs.isGameRunning) {
            return;
        }
        
        Shoot(1);       
    }
    
    public virtual void Shoot(int number) {
             
        if (!GameConfigs.isGameRunning) {
            return;
        }
        
        AnimateShoot(); 
        
        //GameController.ProcessStatShot();
        
        runtimeData.savesLaunched += number;
        Messenger<double>.Broadcast(GameMessages.launch, number);
        Messenger<double>.Broadcast(GameMessages.ammo, -number);
    }
        
    public virtual void FindGamePlayerCamera() {
        if (gameCameraSmoothFollow == null || gameCameraSmoothFollowGround == null) {
            foreach (GameCameraSmoothFollow cam in ObjectUtil.FindObjects<GameCameraSmoothFollow>()) {
                if (cam.name.Contains("Ground")) {
                    gameCameraSmoothFollowGround = cam;
                }
                else {
                    gameCameraSmoothFollow = cam;
                }
            }
        }
    }

    float axisDeadZone = .05f;
    
    public virtual void UpdateAim(float x, float y) {
        
        FindGamePlayerCamera();
        
        GameObject model = gamePlayerModelHolder;
        float cameraAdjustment = 8f;
        axisDeadZone = .05f;
        
        if (Math.Abs(x) > axisDeadZone
            && Math.Abs(y) > axisDeadZone) {
            
            controllerData.currentPosition = model.transform.position;
            
            controllerData.currentAimPosition = -controllerData.currentPosition
                .WithX(controllerData.currentPosition.x + (x * 100))
                .WithY(controllerData.currentPosition.y + (y * 100));
            
            //float angle = Vector3.Angle(controllerData.currentPosition, controllerData.currentAimPosition);
            float dist = Vector3.Distance(controllerData.currentPosition, controllerData.currentAimPosition);
            
            model.transform.localScale = Mathf.Clamp(dist * .1f, .5f, 1.3f) * Vector3.one;  
            
            Vector3 lookAtPos = model.transform.position + (controllerData.currentAimPosition * 10);
            
            model.transform.LookAt(lookAtPos);
            
            float amount = Mathf.Abs(dist);
            
            if (controllerData.gamePlayerEffectAim != null) {
                controllerData.gamePlayerEffectAim.enableEmission = true;
                controllerData.gamePlayerEffectAim.emissionRate = amount * 2;
                controllerData.gamePlayerEffectAim.startLifetime = amount / 400f;
                controllerData.gamePlayerEffectAim.startSpeed = amount;
                controllerData.gamePlayerEffectAim.Play();
            }
            
            //controllerData.lineAim..SetLine3D(Color.white, model.transform.position, lookAtPos);
            
            //LogUtil.Log("UpdateAim:controllerData.currentAimPosition:", controllerData.currentAimPosition);
            
            if (gameCameraSmoothFollow != null) {
                gameCameraSmoothFollow.offset.x = 
                    (gameCameraSmoothFollow.offsetInitial.x + -(x * cameraAdjustment));
                
                gameCameraSmoothFollow.offset.y = 
                    (gameCameraSmoothFollow.offsetInitial.y + -(y * cameraAdjustment));
                
                gameCameraSmoothFollow.SetZoom(-((x + y) * cameraAdjustment));
            }
            if (gameCameraSmoothFollowGround != null) {
                gameCameraSmoothFollowGround.offset.x = 
                    (gameCameraSmoothFollowGround.offsetInitial.x + -(x * cameraAdjustment));
                
                gameCameraSmoothFollowGround.offset.y = 
                    (gameCameraSmoothFollowGround.offsetInitial.y + -(y * cameraAdjustment));
                
                gameCameraSmoothFollowGround.SetZoom(-((x + y) * cameraAdjustment));
            }           
            
        }
        else {                  
            //AnimateShoot();           
            model.transform.localScale = Vector3.one;
            model.transform.rotation = gamePlayerHolder.transform.rotation;
            //model.transform.LookAt(gamePlayerModelHolder.transform.position);
            
            if (controllerData.gamePlayerEffectAim != null) {
                controllerData.gamePlayerEffectAim.enableEmission = false;
                controllerData.gamePlayerEffectAim.emissionRate = 1;
                controllerData.gamePlayerEffectAim.Stop();
            }
            
            if (gameCameraSmoothFollow != null) {
                gameCameraSmoothFollow.Reset();
            }
            if (gameCameraSmoothFollowGround != null) {
                gameCameraSmoothFollowGround.Reset();
            }
        }
    }
    
    public virtual void AnimateShoot() {
        
        if (gamePlayerModelHolderModel != null) {
            foreach (Animation anim in gamePlayerModelHolderModel.GetComponentsInChildren<Animation>()) {
                if (anim != null) {
                    //gamePlayerControllerObject.animation.Play("emo_06");
                    //anim["emo_09"].normalizedSpeed = 2f;
                    anim["emo_09"].normalizedSpeed = 2f;
                    anim.Play("emo_09", PlayMode.StopAll);
                    //anim.Blend("emo_08");
                    //gamePlayerControllerObject.animation.Play("emo_10");
                    //gamePlayerControllerObject.animation.CrossFade("emo_09");
                    break;
                }
            }
        }
    }

    public virtual void Scores(double val) {

        if (!GameConfigs.isGameRunning) {
            return;
        }
        
        runtimeData.scores += val;

        Messenger<double>.Broadcast(GameMessages.scores, val);

        GamePlayerProgress.SetStatScores(val);
    }
 
    public virtual void Score(double val) {
             
        if (!GameConfigs.isGameRunning) {
            return;
        }
     
        runtimeData.score += val;
        
        Messenger<double>.Broadcast(GameMessages.score, val);
        
        GamePlayerProgress.SetStatScore(val);
    }
        
    public virtual void Ammo(double val) {
             
        if (!GameConfigs.isGameRunning) {
            return;
        }
        
        runtimeData.ammo += val;
        runtimeData.collectedAmmo += val;

        Messenger<double>.Broadcast(GameMessages.ammo, val);

        GamePlayerProgress.SetStatAmmo(val);
    }
    
    public virtual void Save(double valAdd) {
             
        if (!GameConfigs.isGameRunning) {
            return;
        }
        
        runtimeData.saves += valAdd;
        Messenger<double>.Broadcast(GameMessages.save, valAdd);
    }
 
    public virtual void Tackle(GamePlayerController gamePlayerControllerTo) {
        Tackle(gamePlayerControllerTo, 1f);
    }

    public virtual void Tackle(GamePlayerController gamePlayerControllerTo, float power) {
     
        if (gamePlayerControllerTo == null) {
            return;
        }

        //bool allowTackle = false;

        if (controllerData.lastTackle + .1f < Time.time) {
            controllerData.lastTackle = Time.time;
            //allowTackle = true;
        }
        else {
            return;
        }

        //transform.LookAt(gamePlayerControllerTo.transform);

        controllerData.positionPlayer = transform.position;
        controllerData.positionTackler = gamePlayerControllerTo.transform.position;
     
        controllerData.gamePlayerControllerAnimation.Attack();

        //Attack();
     
        AddImpact(controllerData.positionTackler - controllerData.positionPlayer, power, false);
     
    }

    public virtual void AddForce(Vector3 dir, float force) {
        AddImpact(dir, force, false);
    }

    public virtual void AddForce(Vector3 dir, float force, bool damage) {
        AddImpact(dir, force, damage);
    }

    public virtual void AddImpact(Vector3 dir, float force) {
        AddImpact(dir, force, true);
    }

    public virtual void UpdatePlayerProgress(float energy, float health) {
        StartCoroutine(UpdatePlayerProgressCo(energy, health));
    }

    public virtual IEnumerator UpdatePlayerProgressCo(float energy, float health) {
        yield return new WaitForEndOfFrame();
        GameProfileCharacters.Current.CurrentCharacterAddGamePlayerProgressEnergyAndHealthRuntime(
            energy, health);
    }
 
    // call this function to add an controllerData.impact force:
    public virtual void AddImpact(Vector3 dir, float force, bool damage) {

        dir.Normalize();
     
        if (dir.y < 0) {
            dir.y = 0;//-dir.y; // reflect down force on the ground
        }

        if (damage) {
            force = Mathf.Clamp(force, 0f, 100f);
        }

        controllerData.impact += dir.normalized * force / (float)runtimeData.mass;

        if (damage) {
            runtimeData.hitCount++;

            if (IsPlayerControlled && damage) {

                HandlePlayerEffectsStateChange();

                UpdatePlayerProgress(
                    (float)(-.01f * Mathf.Clamp(force / 10f, .3f, 1f)),
                    (float)(-.01f * Mathf.Clamp(force / 10f, .3f, 1f)));
            }
        }
        //LogUtil.Log("AddImpact:name:", transform.name + "controllerData.impact:" + controllerData.impact.x);
    }
 
    public virtual void UpdatePhysicsState() {
        
        if(!controllerReady) {
            return;
        }

        StartCoroutine(UpdatePhysicStateCo());
    }

    public virtual IEnumerator UpdatePhysicStateCo() {
        
        //Vectrosity.VectorLine.SetLine (Color.red, transform.position, controllerData.impact);
        
        // apply the controllerData.impact force:
        //if (controllerData.impact.magnitude > 0.3f) {
        
        if (controllerData.characterController.enabled) {
            controllerData.characterController.Move(controllerData.impact * Time.deltaTime);
        }
        //}

        yield return new WaitForFixedUpdate();

        UpdatePlayerEffectsState();
        
        yield return new WaitForFixedUpdate();
        
        // consumes the controllerData.impact energy each cycle:
        controllerData.impact = Vector3.Lerp(controllerData.impact, Vector3.zero, 5 * Time.deltaTime);
        
        yield return new WaitForFixedUpdate();
    }

    public virtual void HandlePlayerEffectsStateChange() {
        controllerData.lastPlayerEffectsTrailUpdate = 0;
        controllerData.lastPlayerEffectsBoostUpdate = -1f;
        controllerData.lastPlayerEffectsGroundUpdate = -1f;
        UpdatePlayerEffectsState();
    }

    public virtual void  UpdatePlayerEffectsState() { 
        
        if(!controllerReady) {
            return;
        }

        float trailTime =
            (Math.Abs(controllerData.impact.x) +
            Math.Abs(controllerData.impact.y) +
            Math.Abs(controllerData.impact.z)) * 5f;
        
        if (IsPlayerControlled) {
            PlayerEffectTrailBoostTime(trailTime * controllerData.thirdPersonController.moveSpeed);
            PlayerEffectTrailGroundTime(-trailTime + controllerData.thirdPersonController.moveSpeed);

            HandlePlayerEffectsTick();
        }
        
        // consumes the controllerData.impact energy each cycle:
        controllerData.impact = Vector3.Lerp(controllerData.impact, Vector3.zero, 5 * Time.deltaTime);
    }


    // --------------------------------------------------------------------
    // AUDIO
 
    public virtual void AudioAttack() {
        if (!GameConfigs.isGameRunning) {
            return;
        }
     
        if (controllerData.lastAudioPlayedAttack + 1 > Time.time) {
            return;
        }
        else {
            controllerData.lastAudioPlayedAttack = Time.time;
        }

        if (controllerState == GamePlayerControllerState.ControllerPlayer) {
            //GameAudio.PlayEffect(transform, "attack-sword-hit-1");
            int randAudio = UnityEngine.Random.Range(1, 5);
            GameAudio.PlayEffect(transform, "audio_football_hit_good_" + randAudio.ToString());
        }
        else {
            //if(isBotZombie) {
            int randAudio = UnityEngine.Random.Range(1, 5);
            GameAudio.PlayEffect(transform, "audio_football_hit_good_" + randAudio.ToString());
            //}
        }

    }

    public virtual void AudioHit() {
        if (!GameConfigs.isGameRunning) {
            return;
        }
     
        if (controllerData.lastAudioPlayedHit + 1 > Time.time) {
            return;
        }
        else {
            controllerData.lastAudioPlayedHit = Time.time;
        }

        if (controllerState == GamePlayerControllerState.ControllerPlayer) {
            //GameAudio.PlayEffect(transform, "hit-grunt-3");
            int randAudio = UnityEngine.Random.Range(1, 5);
            GameAudio.PlayEffect(transform, "audio_football_hit_good_" + randAudio.ToString());
        }
        else {
            //if(isBotZombie) {
            int randAudio = UnityEngine.Random.Range(1, 5);
            GameAudio.PlayEffect(transform, "audio_football_hit_good_" + randAudio.ToString());
            //}
        }

    }

    public virtual void AudioDie() {
        if (!GameConfigs.isGameRunning) {
            return;
        }
     
        if (controllerData.lastAudioPlayedDie + 1 > Time.time) {
            return;
        }
        else {
            controllerData.lastAudioPlayedDie = Time.time;
        }

        if (controllerState == GamePlayerControllerState.ControllerPlayer) {

            GameAudioController.Instance.PlayOh();
            GameAudioController.Instance.PlayWhistle();

            //GameAudio.PlayEffect(transform, "hit-grunt-3");
            int randAudio = UnityEngine.Random.Range(1, 3);
            GameAudio.PlayEffect(transform, "audio_football_grunts_" + randAudio.ToString());
        }
        else {
            //if(isBotZombie) {
            int randAudio = UnityEngine.Random.Range(1, 3);
            GameAudio.PlayEffect(transform, "audio_football_grunts_" + randAudio.ToString());
            //}
        }
    }    
 
    // --------------------------------------------------------------------
    // NETWORK
 
 

    public virtual void UpdateNetworkContainer(string uid) {
     
        uniqueId = uid;
        
        if (!AppConfigs.featureEnableNetworking || !GameConfigs.useNetworking) {
            return;
        }
     
        FindNetworkContainer(uniqueId);      
     
        if (currentNetworkPlayerContainer != null) {
            currentNetworkPlayerContainer.networkViewObject.observed = currentNetworkPlayerContainer;
            currentNetworkPlayerContainer.gamePlayer = gameObject;
            if (controllerData.thirdPersonController != null) {
                currentNetworkPlayerContainer.currentSpeedNetwork = controllerData.thirdPersonController.moveSpeed;
                currentNetworkPlayerContainer.verticalInputNetwork = controllerData.thirdPersonController.verticalInput;
                currentNetworkPlayerContainer.horizontalInputNetwork = controllerData.thirdPersonController.horizontalInput;
            }
            currentNetworkPlayerContainer.running = true;                
        }    
    }
 
    public virtual Gameverses.GameNetworkPlayerContainer FindNetworkContainer(string uid) {
     
        if (!AppConfigs.featureEnableNetworking || !GameConfigs.useNetworking) {
            return null;
        }
     
        if (currentNetworkPlayerContainer != null) {
            if (currentNetworkPlayerContainer.uniqueId == uid) {
                return currentNetworkPlayerContainer;
            }
        }
     
        if (Time.time > controllerData.lastNetworkContainerFind + 5f) {
            controllerData.lastNetworkContainerFind = Time.time;
            if (GameController.Instance.gameState == GameStateGlobal.GameStarted) {
                foreach (Gameverses.GameNetworkPlayerContainer playerContainer 
                         in ObjectUtil.FindObjects<Gameverses.GameNetworkPlayerContainer>()) {
                    if (playerContainer.uniqueId == uid) {
                        currentNetworkPlayerContainer = playerContainer;
                        return currentNetworkPlayerContainer;
                    }
                }
            }
        }
     
        return null;
    }
 
    public virtual bool HasNetworkContainer(string uid) {

        foreach (Gameverses.GameNetworkPlayerContainer playerContainer 
                 in ObjectUtil.FindObjects<Gameverses.GameNetworkPlayerContainer>()) {
            if (playerContainer.uniqueId == uid) {
                currentNetworkPlayerContainer = playerContainer;
                return true;
            }
        }
     
        return false;
    }
 
    public virtual void UpdateNetworkContainerFromSource(string uid) {
     
        uniqueId = uid;
     
        FindNetworkContainer(uniqueId);
     
        if (currentNetworkPlayerContainer != null) {
            if (controllerData.thirdPersonController != null) {
                currentNetworkPlayerContainer.currentSpeedNetwork = controllerData.thirdPersonController.moveSpeed;
                currentNetworkPlayerContainer.verticalInputNetwork = controllerData.thirdPersonController.verticalInput;
                currentNetworkPlayerContainer.horizontalInputNetwork = controllerData.thirdPersonController.horizontalInput;
            }
            currentNetworkPlayerContainer.running = true;                
        }    
    }
 
    public virtual void ChangeContextState(GamePlayerContextState contextStateTo) {
        //if (contextStateTo != contextState) {
            contextState = contextStateTo;
         
            if (controllerData.thirdPersonController != null) {
                controllerData.thirdPersonController.isNetworked = false;
            }
         
            if (contextState == GamePlayerContextState.ContextFollowAgent
                || contextState == GamePlayerContextState.ContextFollowAgentAttack
                || contextState == GamePlayerContextState.ContextRandom
                || contextState == GamePlayerContextState.ContextScript) {
                if (controllerData.navMeshAgent != null) {
                    // TODO load script or look for character input.
                    controllerData.navMeshAgent.enabled = true;
                }
            }
            else if (contextState == GamePlayerContextState.ContextInput
                || contextState == GamePlayerContextState.ContextInputVehicle
                || contextState == GamePlayerContextState.ContextFollowInput) {
                if (controllerData.navMeshAgent != null) {
                    controllerData.navMeshAgent.Stop();
                    //navMeshAgent.enabled = false;
                }
            }
            else if (contextState == GamePlayerContextState.ContextNetwork) {
                if (controllerData.navMeshAgent != null) {
                    controllerData.navMeshAgent.Stop();
                    //navMeshAgent.enabled = false;
                }
                controllerData.thirdPersonController.isNetworked = true;
            }
            else if (contextState == GamePlayerContextState.ContextUI) {
                if (controllerData.navMeshAgent != null) {
                    controllerData.navMeshAgent.Stop();
                    //navMeshAgent.enabled = false;
                }
            }
        //}
    }
 
    // --------------------------------------------------------------------
    // INPUT
 
    public override bool HitObject(GameObject go, InputTouchInfo inputTouchInfo) {
        Ray screenRay = Camera.main.ScreenPointToRay(inputTouchInfo.position3d);
        RaycastHit hit;
     
        if (Physics.Raycast(screenRay, out hit, Mathf.Infinity) && hit.transform != null) {
            if (hit.transform.gameObject == go) {
                LogUtil.Log("HitObject GameActor");
                return true;
            }
        }
        return false;
    }
 
    public override void OnInputUp(InputTouchInfo touchInfo) {
        //LogUtil.Log("OnInputDown GameActor");
    }

    // HANDLE PROGRESS

    public virtual void GetPlayerProgress() {
        controllerData.currentRPGItem = GameProfileCharacters.Current.GetCurrentCharacterRPG();
        controllerData.currentPlayerProgressItem = GameProfileCharacters.Current.GetCurrentCharacterProgress();
    }

    public virtual void HandleItemProperties() {    

        //float tParam = 0f;
        //float valToBeLerped = 15f;
        //float speed = 0.3f;
        //if (tParam < 1) {
        //    tParam += Time.deltaTime * speed; //This will increment tParam based on Time.deltaTime multiplied by a speed multiplier
        //    //valToBeLerped = Mathf.Lerp(0, 3, tParam);
        //}

        // speed

        if(controllerData.modifierItemSpeedLerp < 1f) {

            controllerData.modifierItemSpeedLerp += Time.deltaTime / (controllerData.modifierItemSpeedLerpTime * 1000);
                    
                controllerData.modifierItemSpeedCurrent = Mathf.Lerp(
                    controllerData.modifierItemSpeedCurrent, 
                    controllerData.modifierItemSpeedMin, 
                    controllerData.modifierItemSpeedLerp);     

            controllerData.modifierItemSpeedCurrent = Mathf.Clamp(
                controllerData.modifierItemSpeedCurrent, 0, 5);
        }

        // scale
        
        if (controllerData.modifierItemScaleLerp < 1f) {
            
            controllerData.modifierItemScaleLerp += Time.deltaTime / (controllerData.modifierItemScaleLerpTime * 1000);
            
            controllerData.modifierItemScaleCurrent = Mathf.Lerp(
                controllerData.modifierItemScaleCurrent, 
                controllerData.modifierItemScaleMin, 
                controllerData.modifierItemScaleLerp);     
            
            controllerData.modifierItemScaleCurrent = Mathf.Clamp(
                controllerData.modifierItemScaleCurrent, 0, 5);
        }
    }

    public Vector3 initialScale = Vector3.one;

    public virtual void HandleRPGProperties() {

        if (IsPlayerControlled) {
            if (controllerData.currentRPGItem == null
                || controllerData.currentPlayerProgressItem == null
                || controllerData.lastRPGModTime < Time.time) {
                controllerData.lastRPGModTime = Time.time + 3f;
                GetPlayerProgress();
            }
    
            controllerData.runtimeRPGData.modifierSpeed = controllerData.currentRPGItem.GetSpeed();

            controllerData.runtimeRPGData.modifierEnergy = controllerData.currentRPGItem.GetEnergy()
                + controllerData.currentPlayerProgressItem.GetGamePlayerProgressEnergy();

            controllerData.runtimeRPGData.modifierHealth = controllerData.currentRPGItem.GetHealth()
                + controllerData.currentPlayerProgressItem.GetGamePlayerProgressHealth();
    
            controllerData.runtimeRPGData.modifierAttack = controllerData.currentRPGItem.GetAttack()
                + controllerData.currentPlayerProgressItem.GetGamePlayerProgressEnergy();

            controllerData.runtimeRPGData.modifierScale = controllerData.currentRPGItem.GetScale();

            //Debug.Log("modifierScale:" + controllerData.runtimeRPGData.modifierScale);
            //Debug.Log("modifierItemScaleCurrent:" + controllerData.modifierItemScaleCurrent);
                        
            Vector3 scalePos = initialScale * Mathf.Clamp((float)controllerData.runtimeRPGData.modifierScale
                * controllerData.modifierItemScaleCurrent, .4f, 2.4f);
            
            //Debug.Log("scalePos:" + scalePos);

            transform.localScale = scalePos;
    
            if (controllerData.thirdPersonController != null) {
                controllerData.thirdPersonController.walkSpeed = Mathf.Clamp(
                    5 * (float)(controllerData.runtimeRPGData.modifierSpeed + controllerData.runtimeRPGData.modifierEnergy), 4, 8)
                    * Mathf.Clamp(controllerData.modifierItemSpeedCurrent, .3f, 4f);
    
                controllerData.thirdPersonController.trotSpeed = Mathf.Clamp(
                    12 * (float)(controllerData.runtimeRPGData.modifierSpeed + controllerData.runtimeRPGData.modifierEnergy), 9, 14)
                    * Mathf.Clamp(controllerData.modifierItemSpeedCurrent, .3f, 4f);
    
                controllerData.thirdPersonController.runSpeed = Mathf.Clamp(
                    20 * (float)(controllerData.runtimeRPGData.modifierSpeed + controllerData.runtimeRPGData.modifierEnergy), 14, 34)
                    * Mathf.Clamp(controllerData.modifierItemSpeedCurrent, .3f, 4f);
    
                controllerData.thirdPersonController.inAirControlAcceleration = 3;
                controllerData.thirdPersonController.jumpHeight = .8f;
                controllerData.thirdPersonController.extraJumpHeight = 1f;
                controllerData.thirdPersonController.trotAfterSeconds = .5f;
                controllerData.thirdPersonController.getUserInput = false;
                controllerData.thirdPersonController.capeFlyGravity = 8f;
                controllerData.thirdPersonController.gravity = 16f;
            }
        }

    }

    // HANDLE CONTROLS
 
    public virtual void InitControls() {
     
        if (gamePlayerHolder != null) {
            
            // 
            // CHARACTER

            if (gameObject == null) {
                return;
            }

            if (controllerData == null) {
                return;
            }
         
            controllerData.characterController = gameObject.GetComponent<CharacterController>();

            if (controllerData.characterController == null) {
                controllerData.characterController = gameObject.AddComponent<CharacterController>();
            }
            
            controllerData.characterController.slopeLimit = 45;
            controllerData.characterController.stepOffset = .3f;
            controllerData.characterController.radius = 1.67f;
            controllerData.characterController.height = 2.42f;
            controllerData.characterController.center = new Vector3(0f, 1.79f, 0f);
            
            // 
            // PLAYER CONTROLLERS
                     
            if ((contextState == GamePlayerContextState.ContextInput
                || contextState == GamePlayerContextState.ContextInputVehicle
                || contextState == GamePlayerContextState.ContextFollowInput
                && !IsUIState())
                || IsNetworkPlayerState()) {
         
                    
                if (gameObject.Has<GamePlayerThirdPersonController>()) {

                    controllerData.thirdPersonController = 
                            gameObject.GetComponent<GamePlayerThirdPersonController>();
                }
                else {

                    controllerData.thirdPersonController = 
                            gameObject.AddComponent<GamePlayerThirdPersonController>();
                }

                controllerData.thirdPersonController.Init();

                HandleRPGProperties();
            }
            
            // 
            // AGENTS
         
            if (!IsUIState()) {

                controllerData.navMeshAgent = gameObject.GetComponent<NavMeshAgent>();

                if (controllerData.navMeshAgent == null) {
                    controllerData.navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
                }

                if (controllerData.navMeshAgent != null) {
                    //navMeshAgent.enabled = false;
                    //if(!IsPlayerControlled) {
                    controllerData.navMeshAgent.height = 3.19f;
                    controllerData.navMeshAgent.radius = 1.29f;
                    controllerData.navMeshAgent.baseOffset = -0.30f;
                    controllerData.navMeshAgent.stoppingDistance = 2f;
                    //}
                    
                    controllerData.navMeshAgent.speed = 
                        12 * (float)(controllerData.runtimeRPGData.modifierSpeed + 
                        controllerData.runtimeRPGData.modifierEnergy) + 
                        UnityEngine.Random.Range(1, 5);
                    
                    controllerData.navMeshAgent.acceleration = 
                        8 * (float)(controllerData.runtimeRPGData.modifierSpeed + 
                        controllerData.runtimeRPGData.modifierEnergy) + 
                        UnityEngine.Random.Range(1, 5);
                    
                    controllerData.navMeshAgent.angularSpeed = 
                        120 + UnityEngine.Random.Range(1, 5);

                }
            }
         
            if (contextState == GamePlayerContextState.ContextFollowAgent
                || contextState == GamePlayerContextState.ContextFollowAgentAttack
                && !IsUIState()) {

                controllerData.navMeshAgentFollowController = 
                    gameObject.GetComponent<GamePlayerNavMeshAgentFollowController>();
                
                if (controllerData.navMeshAgentFollowController == null) {
                    controllerData.navMeshAgentFollowController = 
                        gameObject.AddComponent<GamePlayerNavMeshAgentFollowController>();
                }

                controllerData.navMeshAgentFollowController.agent = controllerData.navMeshAgent;

                controllerData.navMeshAgentFollowController.targetFollow = 
                    GameController.CurrentGamePlayerController.gamePlayerEnemyTarget.transform;
            }
         
            if (contextState == GamePlayerContextState.ContextRandom
                && !IsUIState()) {

                controllerData.navMeshAgentController = 
                    gameObject.GetComponent<GamePlayerNavMeshAgentController>();
                
                if (controllerData.navMeshAgentController == null) {
                    controllerData.navMeshAgentController = 
                        gameObject.AddComponent<GamePlayerNavMeshAgentController>();
                }

                controllerData.navMeshAgentController.agent = controllerData.navMeshAgent;

                controllerData.navMeshAgentController.nextDestination = 
                    controllerData.navMeshAgentController.GetRandomLocation();
            }
            
            // 
            // ANIMATION

            //bool addedAnimation = false;
         
            if (gameObject.Has<GamePlayerControllerAnimation>()) {
                controllerData.gamePlayerControllerAnimation = 
                    gameObject.GetComponent<GamePlayerControllerAnimation>();
            }
            else {
                controllerData.gamePlayerControllerAnimation = 
                    gameObject.AddComponent<GamePlayerControllerAnimation>();  

                //addedAnimation = true;
            }
            
            controllerData.gamePlayerControllerAnimation.Init(); 

            if (gamePlayerModelHolderModel != null) {
                controllerData.gamePlayerControllerAnimation.ResetAnimatedActor(gamePlayerModelHolderModel);    
                controllerData.gamePlayerControllerAnimation.Reset();
            }
                      
            float smoothing = .8f;

            if (controllerData.thirdPersonController != null) {
                smoothing = controllerData.thirdPersonController.speedSmoothing;
            }
            else {
                smoothing = controllerData.navMeshAgent.velocity.magnitude + 10f;
            }

            controllerData.gamePlayerControllerAnimation.animationData.runSpeedScale = 
                (smoothing * .15f) * (float)(controllerData.runtimeRPGData.modifierSpeed + 
                controllerData.runtimeRPGData.modifierEnergy);// controllerData.thirdPersonController.trotSpeed / controllerData.thirdPersonController.walkSpeed / 2;

            controllerData.gamePlayerControllerAnimation.animationData.walkSpeedScale = 
                1f * (float)(controllerData.runtimeRPGData.modifierSpeed + 
                controllerData.runtimeRPGData.modifierEnergy);//controllerData.thirdPersonController.walkSpeed / controllerData.thirdPersonController.walkSpeed;

            controllerData.gamePlayerControllerAnimation.animationData.isRunning = true;

            // 
            // SHADOW
         
            if (actorShadow == null) {

                actorShadow = gameObject.AddComponent<ActorShadow>();
                actorShadow.objectParent = gamePlayerModelHolderModel;

                if (gamePlayerShadow != null) {
                    actorShadow.objectShadow = gamePlayerShadow;
                }
            }

            StartNavAgent();
                        
            if (controllerData.thirdPersonController != null) {
                controllerData.thirdPersonController.Reset();           
            }
            
            if (controllerData.gamePlayerControllerAnimation != null) {
                controllerData.gamePlayerControllerAnimation.Reset();
            }
            
            if (controllerState == GamePlayerControllerState.ControllerAgent) {
                controllerData.navMeshAgent.enabled = true;
            } 
        }
    }
 
    // --------------------------------------------------------------------
    // GAME STATE
 
    public virtual void CheckIfShouldRemove() {
        if (IsNetworkPlayerState()) {
            // if network container is gone remove the player...
         
            if (HasNetworkContainer(uniqueId)) {
                // no prob
            }
            else {
                // remove
             
                if (controllerData.thirdPersonController) {
                    controllerData.thirdPersonController.ApplyDie(true);
                }

                UITweenerUtil.FadeTo(
                    gameObject, 
                    UITweener.Method.EaseIn, UITweener.Style.Once, .3f, .5f, 0);

                Invoke("RemoveMe", 6);
            }
        }
    }
 
    public virtual void RemoveMe() {
        gamePlayerModelHolderModel.DestroyChildren(GameConfigs.usePooledGamePlayers);
        gameObject.DestroyGameObject(3f, GameConfigs.usePooledGamePlayers);
    }
 
    public virtual bool CheckVisibility() {
     
        if (controllerData.renderers == null) {
            controllerData.renderers = new List<SkinnedMeshRenderer>(); 
        }
     
        if (controllerData.renderers.Count == 0) {           
            foreach (SkinnedMeshRenderer rendererSkinned in gamePlayerHolder.GetComponentsInChildren<SkinnedMeshRenderer>()) {
                controllerData.renderers.Add(rendererSkinned);
            }
        }            
     
        controllerData.visible = false;
     
        if (controllerData.renderers.Count > 0) {
            foreach (SkinnedMeshRenderer rendererSkinned in controllerData.renderers) {
                if (rendererSkinned != null) {
                    if (!rendererSkinned.isVisible) {// || !rendererSkinned.IscontrollerData.visibleFrom(Camera.main)) {
                        controllerData.visible = false;
                    }
                    else {
                        controllerData.visible = true;
                        break;
                    }
                }        
            }
        }
     
        //LogUtil.Log("controllerData.visible:" + controllerData.visible);
     
        return controllerData.visible;
    }
     
    // --------------------------------------------------------------------
    // AGENTS
 
    public virtual void TurnOffNavAgent() {
        if (controllerData.navAgentRunning) {
            if (controllerData.navMeshAgent != null) {
                controllerData.navMeshAgent.Stop();
                controllerData.navAgentRunning = false;
            }        
        }
    }
 
    public virtual void SyncNavAgent() {
     
        if (IsAgentState()) {
         
            /*
         if(navMeshAgent != null) {
             if(navMeshAgent.enabled) {
                 if(navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance + 1) {
                     //navMeshAgent.Stop();
                 }
             }
         }
         */
            if (isDead) {
                controllerData.navAgentRunning = true;
            }
        }
        else if (IsPlayerState()) {
            TurnOffNavAgent();
        }
        else if (IsUIState()) {
            TurnOffNavAgent();
        }
        else if (IsNetworkPlayerState()) {
            TurnOffNavAgent();           
            CheckIfShouldRemove();
        }    
    }
 
    // --------------------------------------------------------------------
    // UPDATE/GAME TICK
 
    public virtual void UpdateCommonState() {
        
        currentFPS = FPSDisplay.GetCurrentFPS();
             
        if (Application.isEditor) {
            if (Input.GetKeyDown(KeyCode.M)) {
                PlayerEffectWarpFadeIn();
            }
            else if (Input.GetKeyDown(KeyCode.N)) {
                PlayerEffectWarpFadeOut();
            }
        }

        // visibility
        CheckVisibility();
     
        // fast stuff    
        HandlePlayerAliveState();
        HandlePlayerEffectWarpAnimateTick();
     
        if (IsAgentState()) {         
         
            //if(Input.GetMouseButtonDown(0)) {
         
            //   if(navMeshAgent != null) {
            //       if(navMeshAgent.enabled) {
                     
            //Vector3 worldPoint = new Vector3 (Input.mousePosition.x, Input.mousePosition.y,Camera.main.nearClipPlane);
            //var mousePos = Camera.main.ScreenToWorldPoint (worldPoint);
            //var ray = Camera.main.ScreenPointToRay(mousePos);
            //RaycastHit hit;
            //if(Physics.Raycast(ray.origin,ray.direction, out hit)){
            // print("world point on terrain: "+hit.point+", distance to point: "+hit.distance);
            //navMeshAgent.destination = hit.point;
            //}
            //       }   
            //   }
            //}
         
            if (runtimeData != null) {
                if (runtimeData.hitCount > UnityEngine.Random.Range(2, 4)) {
                    Die();
                }
            }            
         
            UpdateNetworkContainerFromSource(uniqueId);
        }
        else if (IsPlayerState()) {           
            if (controllerData.thirdPersonController.aimingDirection != Vector3.zero) {

                //gamePlayerHolder.transform.rotation = Quaternion.LookRotation(controllerData.thirdPersonController.aimingDirection);
                gamePlayerModelHolder.transform.rotation = 
                    Quaternion.LookRotation(controllerData.thirdPersonController.aimingDirection);

                if (controllerData.mountData.isMountedVehicle) {
                    controllerData.mountData.mountVehicle.SetMountWeaponRotator(
                        Quaternion.LookRotation(controllerData.thirdPersonController.aimingDirection));
                }

                foreach (Transform t in gamePlayerModelHolderModel.transform) {
                    t.localRotation = Quaternion.identity;
                }

                if (controllerData.thirdPersonController.aimingDirection.IsBiggerThanDeadzone(axisDeadZone)) {

                    if (controllerData.thirdPersonController.aimingDirection != controllerData.thirdPersonController.movementDirection) {
                        SendAttack();
                    }
                }
            }
            else {
                                
                if (controllerData.mountData.isMountedVehicle) {
                    //controllerData.mountData.mountVehicle.SetMountWeaponRotatorLocal(Vector3.zero);
                }

                foreach (Transform t in gamePlayerModelHolderModel.transform) {
                    t.localRotation = Quaternion.identity;
                }
            }
         
            if (runtimeData.hitCount > 10) {
                Die();
            }
         
            UpdateNetworkContainerFromSource(uniqueId);          
        }
        else if (IsUIState()) {       
         
        }
        else if (IsNetworkPlayerState()) {            

        }

        bool shouldBeGrounded = true;
        if (controllerData.thirdPersonController != null) {
            if (controllerData.thirdPersonController.IsJumping()) {
                shouldBeGrounded = false;
            }
        }

        if (shouldBeGrounded) {
            ResetPositionAir(0f);
        }
        
        if (IsPlayerControlled) {
            HandleItemProperties();
            HandleRPGProperties();
        }

        // periodic      
     
        bool runUpdate = false;
        if (Time.time > controllerData.lastUpdateCommon + 1f) {
            controllerData.lastUpdateCommon = Time.time;
            runUpdate = true;

            if (IsPlayerControlled) {
                Score(2 * 1);
            }
        }
     
        if (!runUpdate) {
            return;
        }        
                 
        SyncNavAgent();  
     
    }
 
    public virtual void UpdateVisibleState() {
     
        if (controllerData.thirdPersonController != null) {
            if (controllerData.thirdPersonController.IsJumping()) {
                if (controllerData.navMeshAgent != null) {
                    if (controllerData.navMeshAgent.enabled) {
                        controllerData.navMeshAgent.Stop(true);
                    }
                }
                
                if (gamePlayerShadow != null) {
                    /////gamePlayerShadow.Hide();
                }
            }
            else {
                if (controllerData.navMeshAgent != null) {
                    if (!controllerData.navMeshAgent.enabled) {
                        controllerData.navMeshAgent.enabled = true;
                    }
                    controllerData.navMeshAgent.Resume();
                }                       
                
                if (gamePlayerShadow != null) {
                    /////gamePlayerShadow.Show();
                }
            }
        }

        if (isCharacterLoaded) {
            actorShadow.gameObject.Show();
        }
        else {
            actorShadow.gameObject.Hide();
        }

        if (controllerData.dying) {
            //transform.position = Vector3.Lerp(transform.position, transform.position.WithY(1.3f), 1 + Time.deltaTime);
        }
     
        //bool runUpdate = false;

        if (controllerData.currentTimeBlock + controllerData.actionInterval < Time.time) {
            controllerData.currentTimeBlock = Time.time;
            // runUpdate = true;
        }
     
        if (controllerState == GamePlayerControllerState.ControllerAgent
            && (contextState == GamePlayerContextState.ContextFollowAgentAttack
            || contextState == GamePlayerContextState.ContextFollowAgent)
            && GameController.Instance.gameState == GameStateGlobal.GameStarted
            && isAlive) {

            //if(runUpdate) {
            GameObject go = GameController.CurrentGamePlayerController.gameObject;

            if (go != null) {
                
                controllerData.distanceToPlayerControlledGamePlayer = Vector3.Distance(
                    go.transform.position,
                    transform.position);

                // check distance for evades

                if (lastStateEvaded > .3f) {

                    lastStateEvaded += Time.deltaTime;


                    if (controllerData.distanceToPlayerControlledGamePlayer <= controllerData.distanceEvade) {
                        controllerData.isWithinEvadeRange = true;
                    }
                    else {
                        controllerData.isWithinEvadeRange = false;
                    }


                    if (controllerData.lastIsWithinEvadeRange != controllerData.isWithinEvadeRange) {
                        if (controllerData.lastIsWithinEvadeRange && !controllerData.isWithinEvadeRange) {
                            // evaded!
                            GamePlayerProgress.SetStatEvaded(1f);
                        }
                        controllerData.lastIsWithinEvadeRange = controllerData.isWithinEvadeRange;
                    }
                }

                // check attack/lunge range

                if (controllerData.distanceToPlayerControlledGamePlayer <= attackRange) {
                    //foreach(Collider collide in Physics.OverlapSphere(transform.position, attackRange)) {

                    // Turn towards player and attack!

                    GamePlayerController gamePlayerControllerHit
                        = GameController.GetGamePlayerControllerObject(go, true);

                    if (gamePlayerControllerHit != null
                        && !gamePlayerControllerHit.controllerData.dying) {


                        if (controllerData.distanceToPlayerControlledGamePlayer < attackRange / 2.5f) {
                            // LEAP AT THEM within three
                            Tackle(gamePlayerControllerHit, Mathf.Clamp(20f - controllerData.distanceToPlayerControlledGamePlayer / 2, 1f, 20f));
                        }
                        else {
                            // PURSUE FASTER
                            Tackle(gamePlayerControllerHit, 3.23f);
                        }
                    }
                }

                // CHECK RANDOM DIE RANGE

                bool shouldRandomlyDie = false;
                
                if (controllerData.distanceToPlayerControlledGamePlayer >= controllerData.distanceRandomDie) {
                    controllerData.isInRandomDieRange = true;
                }
                else {
                    controllerData.isInRandomDieRange = false;
                }

                if (controllerData.isInRandomDieRange) {
                    if (controllerData.lastRandomDie > UnityEngine.Random.Range(
                        controllerData.timeMinimumRandomDie, 
                        controllerData.timeMinimumRandomDie + controllerData.timeMinimumRandomDie / 2)) {
                        
                        controllerData.lastRandomDie = 0;
                        //shouldRandomlyDie = true;
                    }
                    
                    controllerData.lastRandomDie += Time.deltaTime;
                }
                
                //public float controllerData.distanceRandomDie = 30f;
                //public float controllerData.timeMinimumRandomDie = 5f;
                
                if (controllerData.lastIsInRandomDieRange != controllerData.isInRandomDieRange) {
                    if (controllerData.lastIsInRandomDieRange && !controllerData.isInRandomDieRange) {
                        // out of range random!
                        //GameController.CurrentGamePlayerController.Score(5);
                        //GamePlayerProgress.SetStatEvaded(1f);
                    }
                    controllerData.lastIsWithinEvadeRange = controllerData.isInRandomDieRange;
                }

                if (shouldRandomlyDie) {
                    runtimeData.hitCount += 10; 
                }
            }
            //}
        }
        else if (controllerState == GamePlayerControllerState.ControllerPlayer
            && GameController.Instance.gameState == GameStateGlobal.GameStarted) {
            float currentSpeed = 0;

            if (controllerData.mountData.isMountedVehicle) {
                currentSpeed = controllerData.mountData.mountVehicle.driver.currentSpeed;
            }
            else {
                currentSpeed = controllerData.thirdPersonController.moveSpeed;
            }
            //LogUtil.Log("currentSpeed:", currentSpeed);
            
            Vector3 pos = Vector3.zero;
            pos.z = Mathf.Clamp(currentSpeed / 3, .3f, 3.5f);
         
            if (gamePlayerEnemyTarget != null) {
                gamePlayerEnemyTarget.transform.localPosition = pos;
            }

            if (gamePlayerModelTarget != null) {
                gamePlayerModelTarget.transform.localPosition = pos;
            }

            if (controllerData.playerSpin) {
                // Clamps automatically angles between 0 and 360 degrees.
                float y = 360 * Time.deltaTime;

                gamePlayerModelHolder.transform.localRotation =
                    Quaternion.Euler(0, gamePlayerModelHolder.transform.localRotation.eulerAngles.y + y, 0);

                if (gamePlayerModelHolder.transform.localRotation.eulerAngles.y > 330) {
                    controllerData.playerSpin = false;
                    gamePlayerModelHolder.transform.localRotation =
                        Quaternion.Euler(0, 0, 0);
                }
            }
        }

        /*
     // periodic stuff
     
     bool runUpdate = false;
     if(Time.time > controllerData.lastUpdate + .3f) {
         controllerData.lastUpdate = Time.time;
         runUpdate = true;
     }
     
     if(!runUpdate) {
         return;
     }
     */
    }
 
    public virtual void UpdateOffscreenState() {
 
    }

    //void OnDrawGizmosSelected() {
    //    Vector3 p = camera.ScreenToWorldPoint(new Vector3(100, 100, camera.nearClipPlane));
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawSphere(p, 0.1F);
    //}

    public virtual void FixedUpdate() {
        
        if(!controllerReady) {
            return;
        }

        if (!controllerData.initialized) {
            return;
        }
     
        if (!GameController.IsGameRunning) {
            return;
        }

        HandlePlayerAliveStateFixed();
    }

    public virtual void LateUpdate() {
        
        if(!controllerReady) {
            return;
        }

        if (controllerData == null) {
            return;
        }

        if (!controllerData.initialized) {
            return;
        }
     
        if (!GameController.IsGameRunning) {
            return;
        }

        HandlePlayerAliveStateLate();
    }

    public virtual void UpdateAlways() {        
        HandleCharacterAttachedSounds(); // always run to turn off audio when not playing.
        HandlePlayerInactionState();
    }
 
    public override void Update() {   
        
        if(!controllerReady) {
            return;
        }

        if (GameConfigs.isUIRunning) {
            return;
        }
                 
        if (!GameConfigs.isGameRunning) {
            return;
        }
     
        if (controllerData != null && !controllerData.initialized) {
            return;
        }

        UpdateAlways();
        
        UpdateVisibleState();
        
        UpdateCommonState();

        if (IsPlayerControlled) {
            if (Input.GetKey(KeyCode.LeftControl)) {

                //LogUtil.Log("GamePlayer:moveDirection:" + GameController.CurrentGamePlayerController.controllerData.thirdPersonController.movementDirection);
                //LogUtil.Log("GamePlayer:aimDirection:" + GameController.CurrentGamePlayerController.controllerData.thirdPersonController.aimingDirection);
                //LogUtil.Log("GamePlayer:rotation:" + GameController.CurrentGamePlayerController.transform.rotation);
                //Vector3 point1 = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
                //Vector3 point2 = Camera.main.ScreenToWorldPoint(new Vector3(1, 0, 1));

                //LogUtil.Log("GamePlayer:point1:" + point1);
                //LogUtil.Log("GamePlayer:point2:" + point2);

                float power = 100f;
                if (Input.GetKey(KeyCode.V)) {
                    Boost(Vector3.zero.WithZ(1),
                        power);
                }
                else if (Input.GetKey(KeyCode.B)) {
                    Boost(Vector3.zero.WithZ(-1),
                        power);
                }
                else if (Input.GetKey(KeyCode.N)) {
                    StrafeLeft(Vector3.zero.WithX(-1),
                        power);
                }
                else if (Input.GetKey(KeyCode.M)) {
                    StrafeRight(Vector3.zero.WithX(1),
                        power);
                }
            }
        }
        
        if (Application.isEditor) {
            if (Input.GetKey(KeyCode.LeftControl)) {
                if (Input.GetKey(KeyCode.RightBracket)) {
                    if (!IsPlayerControlled) {
                        Die();
                    }       
                }
                else if (Input.GetKey(KeyCode.V)) {                  
                    LoadWeapon("weapon-machine-gun-1");

                    UINotificationDisplay.Instance.QueueTip(
                        "Machine Gun Enabled",
                        "Machine gun simulation trigger and action installed and ready.");
                }
                else if (Input.GetKey(KeyCode.B)) {                  
                    LoadWeapon("weapon-flame-thrower-1");

                    UINotificationDisplay.Instance.QueueTip(
                        "Flame Thrower Enabled",
                        "Flame thrower simulation trigger and action installed and ready.");
                }
                else if (Input.GetKey(KeyCode.N)) {                  
                    LoadWeapon("weapon-shotgun-1");
                    UINotificationDisplay.Instance.QueueTip(
                        "Shotgun Enabled",
                        "Shotgun simulation trigger and action installed and ready.");
                }
                else if (Input.GetKey(KeyCode.M)) {                  
                    LoadWeapon("weapon-rocket-launcher-1");

                    UINotificationDisplay.Instance.QueueTip(
                            "Rocket Launcher Enabled",
                            "Rocket launcher trigger and action installed and ready.");
                }
                else if (Input.GetKey(KeyCode.C)) {                  
                    LoadWeapon("weapon-rifle-1");

                    UINotificationDisplay.Instance.QueueTip(
                            "Rifle Enabled",
                            "Rifle simulation trigger and action installed and ready.");
                }
            }
        }

        //if(!controllerData.visible) {
        //UpdateOffscreenState();
        //return;
        //}
        //else {
        //UpdateVisibleState();
        //}

    }        
}

