using System;
using System.Collections;
using System.Collections.Generic;

using Engine;
using Engine.Data;
using Engine.Game.Controllers;
using Engine.Networking;
using Engine.Utility;

using UnityEngine;

public enum GamePlayerControllerAnimationType {
    legacy,
    mecanim
}

/*
public class BaseGamePlayerAnimationType {
    public static string walk = "walk";
    public static string run = "run";
    public static string attack = "attack";
    public static string attackAlt = "attack-alt";
    public static string attackRight = "attack-right";
    public static string attackLeft = "attack-left";
    public static string speed = "speed";
    public static string jump = "jump";
    public static string defend = "defend";
    public static string defendAlt = "defend-alt";
    public static string defendRight = "defend-right";
    public static string defendLeft = "defend-left";
    public static string hit = "hit";
    public static string die = "die";
    public static string death = "death";
    public static string skill = "skill";
    public static string magic = "magic";
    public static string use = "use";
    public static string mount = "mount";
    public static string idle = "idle";
    public static string emo = "emo";
    public static string strafe = "strafe";
    public static string strafeLeft = "strafe-left";
    public static string strafeRight = "strafe-right";
    public static string boost = "boost";
    public static string spin = "spin";
}

*/


public class GamePlayerAnimationDataItem : GameDataObject {

}

public class BaseGamePlayerAnimationData {

    public GameCharacter gameCharacter;
    public Dictionary<string,GamePlayerAnimationDataItem> items;

    public BaseGamePlayerAnimationData() {
        Reset();
    }

    public void Reset() {
        items = new Dictionary<string, GamePlayerAnimationDataItem>();        
    }

    public void LoadCharacterAnimations(string characterCode) {
        gameCharacter = GameCharacters.Instance.GetById(characterCode);

        foreach (GameDataAnimation item in gameCharacter.data.animations) {

            GamePlayerAnimationDataItem itemData = new GamePlayerAnimationDataItem();

            itemData.code = GetDataAnimation(item.type);
            itemData.type = item.type;
            itemData.valFloat = Time.time;

            items.Set(item.type,itemData); 
        }
    }

    public string GetDataAnimation(string type) {

        string code = "idle";

        if (gameCharacter != null) {
            
            GameDataAnimation data = 
                gameCharacter.data.GetAnimationByType(
                    type);
            
            if (data != null) {
                code = data.code;
            }
        }
        
        return code;
    }       
}

public class BaseGamePlayerControllerAnimationData {   

    public bool initialized = false;
    public float runSpeedScale = 1.2f;
    public float walkSpeedScale = 1.0f;
    public bool isRunningClampAnimation = false;
    public bool isRunning = false;
    public bool isDead = false;
    public GamePlayerController gamePlayerController;
    public GamePlayerThirdPersonController thirdPersonController;
    public NavMeshAgent navAgent;
    public Animator animator;
    public Avatar avatar;
    public RuntimeAnimatorController animationController;
    public AnimationState animationRun;
    public AnimationState animationWalk;
    public AnimationState animationIdle;
    public AnimationState animationHit;
    public AnimationState animationDie;
    public AnimationState animationJump;
    public AnimationState animationAttack;
    public AnimationState animationAttackAlt;
    public AnimationState animationAttackLeft;
    public AnimationState animationAttackRight;
    public GameObject actor;
}

public class BaseGamePlayerControllerAnimation : GameObjectBehavior {

    public GamePlayerControllerAnimationType animationType = 
        GamePlayerControllerAnimationType.legacy;
    public GamePlayerControllerAnimationData animationData;
 
    public bool isLegacy {
        get {
            if (animationType == GamePlayerControllerAnimationType.legacy) { 
                return true;
            }
            return false;
        }
    }
 
    public bool isMecanim {
        get {
            if (animationType == GamePlayerControllerAnimationType.mecanim) { 
                return true;
            }
            return false;
        }
    }
 
    public virtual void Awake() {

    }

    public virtual void Start() {

    }
 
    public virtual void Init() {
        
        animationData = new GamePlayerControllerAnimationData();
        
        animationData.gamePlayerController = GetComponent<GamePlayerController>();
        animationData.thirdPersonController = animationData.gamePlayerController.controllerData.thirdPersonController;
        animationData.navAgent = GetComponent<NavMeshAgent>();

        Reset();
    }
     
    public virtual void ResetPlayState() {     
        if (!isLegacy && animationData.animator != null) {
            //animationData.animator.SetFloat(GameDataActionKeys.idle, 0f);

            ResetFloat(GameDataActionKeys.speed);
            ResetFloat(GameDataActionKeys.death);
            ResetFloat(GameDataActionKeys.strafe);
            ResetFloat(GameDataActionKeys.jump);
            ResetFloat(GameDataActionKeys.attack);
            ResetFloat(GameDataActionKeys.hit);
        }
    }

    public virtual void HandleAnimatorState() {        
        ResetPlayState();
    }
 
    public virtual void Reset() {
        if (animationData.actor != null) {

            FindAnimatedActor();
                 
            // LEGACY TYPE   
            if (animationData.actor.animation != null) {
                         
                // By default loop all animations
                animationData.actor.animation.wrapMode = WrapMode.Loop;
                animationData.actor.animation.Stop();
             
                if (animationData.animationRun == null) {
                    if (animationData.actor.animation["run"] != null) {
                        animationData.animationRun = animationData.actor.animation["run"];
                        animationData.animationRun.layer = 1;
                    }
                }
             
                if (animationData.animationWalk == null) {
                    if (animationData.actor.animation["walk"] != null) {
                        animationData.animationWalk = animationData.actor.animation["walk"];
                        animationData.animationWalk.layer = 1;
                    }
                }
             
                if (animationData.animationIdle == null) {
                    if (animationData.actor.animation["idle"] != null) {
                        animationData.animationIdle = animationData.actor.animation["idle"];
                        animationData.animationIdle.layer = 1;
                    }
                }
             
                if (animationData.animationHit == null) {
                    if (animationData.actor.animation["hit"] != null) {
                        animationData.animationHit = animationData.actor.animation["hit"];
                        animationData.animationHit.layer = 2;
                    }
                }
             
                if (animationData.animationAttack == null) {
                    if (animationData.actor.animation["attack"] != null) {
                        animationData.animationAttack = animationData.actor.animation["attack"];
                        animationData.animationAttack.layer = 2;
                    }                    
                }
             
                if (animationData.animationAttackAlt == null) {
                    if (animationData.actor.animation["attack-alt"] != null) {
                        animationData.animationAttackAlt = animationData.actor.animation["attack-alt"];
                        animationData.animationAttackAlt.layer = 2;
                    }                    
                }
             
                if (animationData.animationAttackLeft == null) {
                    if (animationData.actor.animation["attack-left"] != null) {
                        animationData.animationAttackLeft = animationData.actor.animation["attack-left"];
                        animationData.animationAttackLeft.layer = 2;
                    }                    
                }
             
                if (animationData.animationAttackRight == null) {
                    if (animationData.actor.animation["attack-right"] != null) {
                        animationData.animationAttackRight = animationData.actor.animation["attack-right"];
                        animationData.animationAttackRight.layer = 2;
                    }                    
                }
             
                AnimationStatePlay(animationData.animationIdle);
            }
                     
        }

        ResetPlayState();
     
        animationData.isRunning = true;
        animationData.isDead = false;
    }
 
    public virtual void AnimationPlay(Animation ani) {
        if (ani == null) {
            return;
        }
        ani.Play();
    }
 
    public virtual void AnimationCrossFade(Animation ani, Animation aniTo) {
        if (ani == null) {
            return;
        }
     
        if (aniTo == null) {
            return;
        }
     
        if (animationData.actor == null) {
            return;
        }
     
        if (animationData.actor.animation == null) {
            return;
        }

        if (animationData.actor.animation[aniTo.name] != null) {
            ani.CrossFade(aniTo.name);
        }
    }
 
    public virtual void AnimationBlend(Animation ani, Animation aniTo) {
        AnimationBlend(ani, aniTo, .5f, .5f);
    }
 
    public virtual void AnimationBlend(Animation ani, Animation aniTo, float targetWeight) {
        AnimationBlend(ani, aniTo, targetWeight, .5f);
    }
 
    public virtual void AnimationBlend(Animation ani, Animation aniTo, float targetWeight, float fadeLength) {
        if (ani == null) {
            return;
        }
     
        if (aniTo == null) {
            return;
        }
     
        if (animationData.actor == null) {
            return;
        }
     
        if (animationData.actor.animation == null) {
            return;
        }

        if (animationData.actor.animation[aniTo.name] != null) {
            ani.Blend(aniTo.name, targetWeight, fadeLength);
        }
    }
 
    public virtual void AnimationStatePlay(AnimationState ani) {
        if (ani == null) {
            return;
        }
     
        if (animationData.actor == null) {
            return;
        }
     
        if (animationData.actor.animation == null) {
            return;
        }
     
        animationData.actor.animation.Play(ani.name);
    }

    public virtual void AnimationStateCrossFade(AnimationState ani, AnimationState aniTo) {
        if (ani == null) {
            return;
        }
     
        if (aniTo == null) {
            return;
        }
     
        if (animationData.actor == null) {
            return;
        }
     
        if (animationData.actor.animation == null) {
            return;
        }

        if (animationData.actor.animation[aniTo.name] != null) {
            animationData.actor.animation.CrossFade(aniTo.name);
        }
    }
 
    public virtual void AnimationStateBlend(AnimationState ani, AnimationState aniTo) {
        AnimationStateBlend(ani, aniTo, .8f, .5f);
    }
 
    public virtual void AnimationStateBlend(AnimationState ani, AnimationState aniTo, float targetWeight) {
        AnimationStateBlend(ani, aniTo, targetWeight, .5f);
    }
 
    public virtual void AnimationStateBlend(AnimationState ani, AnimationState aniTo, float targetWeight, float fadeLength) {
        if (ani == null) {
            return;
        }
     
        if (aniTo == null) {
            return;
        }
     
        if (animationData.actor == null) {
            return;
        }
     
        if (animationData.actor.animation == null) {
            return;
        }

        if (animationData.actor.animation[aniTo.name] != null) {
            animationData.actor.animation.Blend(aniTo.name, targetWeight, fadeLength);
        }
    }
 
    public virtual void ResetAnimatedActor(GameObject actorItem) {
        if (actorItem != null) {
            animationData.actor = actorItem;
            animationData.animator = null;
            FindAnimatedActor();
        }
    }
 
    public virtual void FindAnimatedActor() {

        if (animationData.actor != null) {

            // MECANIM
            if (animationData.animator == null && animationData.actor.animation == null) {
                foreach (Animator anim in animationData.actor.GetComponentsInChildren<Animator>()) {
                    if (anim.runtimeAnimatorController != null 
                        && anim.avatar != null) {
                        animationData.animator = anim;
                        animationData.actor = anim.gameObject;
                        animationType = GamePlayerControllerAnimationType.mecanim;
                        animationData.avatar = anim.avatar;
                        animationData.animationController = anim.runtimeAnimatorController;
                    }
                }
            }
         
            // LEGACY TYPE
            if (animationData.actor.animation == null && animationData.animator == null) {

                foreach (Animation anim in animationData.actor.GetComponentsInChildren<Animation>()) {
                    animationData.actor = anim.gameObject;
                    animationType = GamePlayerControllerAnimationType.legacy;
                }
            }
        }
        else {
            Debug.LogWarning("FindAnimatedActor:WARNING:" + " animationData.actor IS NULL" + animationData.gamePlayerController.uniqueId); 
        }
    }

    public virtual void Update() {

        if (!GameConfigs.isGameRunning || GameConfigs.isUIRunning) {
            return;
        }
     
        if (animationData.isDead) {
            return;
        }
             
        if (animationData.isRunning) {
                     
            FindAnimatedActor();
         
            float currentSpeed = 0f;
         
            if (animationData.thirdPersonController != null) {
                currentSpeed = animationData.thirdPersonController.GetSpeed();
            }
         
            if (animationData.gamePlayerController != null) {

                if (animationData.gamePlayerController.contextState == GamePlayerContextState.ContextFollowAgent
                    || animationData.gamePlayerController.contextState == GamePlayerContextState.ContextFollowAgentAttack
                    || animationData.gamePlayerController.contextState == GamePlayerContextState.ContextRandom) {

                    
                    if (!animationData.gamePlayerController.IsPlayerControlled) {
                        //LogUtil.Log("currentSpeed11:" + currentSpeed);
                    }

                    if (animationData.navAgent != null) {
                        if (animationData.navAgent.enabled) {                       
                            //currentSpeed = navAgent.velocity.magnitude + 20;
                         
                            if (animationData.navAgent.velocity.magnitude > 0f) {
                                currentSpeed = 15f;
                            }
                            else {
                                currentSpeed = 0;    
                            }
                         
                            if (animationData.navAgent.remainingDistance < 
                                animationData.navAgent.stoppingDistance + 1) {
                                currentSpeed = 0;
                            }
                         
                            if (currentSpeed < animationData.navAgent.speed) {
                                //currentSpeed = 0;
                            }
                        }
                    }
                }
            }
         
            float walkSpeed = 5f;
         
            //LogUtil.Log("currentSpeed:" + currentSpeed);
            if (animationData.thirdPersonController != null) {
                walkSpeed = animationData.thirdPersonController.walkSpeed;
                //LogUtil.Log("currentSpeed:" + thirdPersonController.walkSpeed);
            }
         
            if (animationData.actor == null || (animationData.actor.animation == null && animationData.animator == null)) {
                Debug.Log("animationData NULL:" + " uniqueId:" + animationData.gamePlayerController.uniqueId);
                return;
            }
         
            if (isLegacy) {
                if (animationData.actor != null) {
                    if (animationData.actor.animation != null) {
                        if (animationData.actor.animation["run"] != null) {
                            animationData.actor.animation["run"].normalizedSpeed = animationData.runSpeedScale;
                        }
                        if (animationData.actor.animation["walk"] != null) {
                            animationData.actor.animation["walk"].normalizedSpeed = animationData.walkSpeedScale;
                        }
                    }
                }
            }
         
            // Fade in run
            if (currentSpeed > walkSpeed) {
             
                if (isLegacy) {
                    if (animationData.actor != null) {
                        if (animationData.actor.animation != null) {
                            if (animationData.actor.animation["run"] != null) {
                                if (animationData.actor.animation["run"] != null) {
                                    animationData.actor.animation["run"].blendMode = AnimationBlendMode.Blend;
                                 
                                    if (animationData.thirdPersonController == null) {                      
                                        animationData.actor.animation["run"].normalizedSpeed = animationData.runSpeedScale;
                                        //animationData.actor.animation["run"].time = 0f;
                                        animationData.actor.animation.CrossFade("run", .5f);   
                                    }
                                    else {                       
                                 
                                        if (animationData.thirdPersonController.verticalInput2 != 0f 
                                            || animationData.thirdPersonController.horizontalInput2 != 0f) {
                                         
                                            // if angle between axis is over 120 and less than 240 reverse run
                                            float angleTo = Vector3.Angle(
                                                animationData.thirdPersonController.movementDirection, 
                                                animationData.thirdPersonController.aimingDirection);
                                         
                                            if (angleTo > 120 && angleTo < 240) {
                                                animationData.actor.animation["run"].normalizedSpeed = -animationData.runSpeedScale * .9f;                               
                                            }
                                            else {   
                                                animationData.actor.animation["run"].normalizedSpeed = animationData.runSpeedScale;
                                            }
                                         
                                            //animationData.actor.animation["run"].time = animationData.actor.animation["run"].length;
                                            animationData.actor.animation.Blend("run");    
                                        }
                                        else {
                                            animationData.actor.animation["run"].normalizedSpeed = animationData.runSpeedScale;
                                            //animationData.actor.animation["run"].time = 0f;
                                            animationData.actor.animation.CrossFade("run", .5f);                       
                                        }
                                    }
                                }
                            }
                        }
                    }
                    // We fade out jumpland quick otherwise we get sliding feet
                    if (animationData.actor.animation["jump"] != null) {
                        animationData.actor.animation.CrossFade("jump", 0);
                    }
                }
                else if (isMecanim) {
                    SetFloat(GameDataActionKeys.speed, currentSpeed);
                }

                SendMessage("SyncAnimation", "run", SendMessageOptions.DontRequireReceiver);
            }
         // Fade in walk
            else if (currentSpeed > 0.1) {
             
                if (isLegacy) {
                    if (animationData.actor != null) {
                        if (animationData.actor.animation != null) {
                            if (animationData.actor.animation["jump"] != null) {
                                if (animationData.actor.animation["jump"] != null) {
                                    animationData.actor.animation.CrossFade("jump");
                                }
                                // We fade out jumpland realy quick otherwise we get sliding feet
                                animationData.actor.animation.Blend("jump", 0);
                            }
                            if (animationData.actor.animation["walk"] != null) {
                                if (animationData.actor.animation["walk"] != null) {
                                    animationData.actor.animation["walk"].blendMode = AnimationBlendMode.Blend;
                                    if (animationData.thirdPersonController.verticalInput2 != 0f 
                                        || animationData.thirdPersonController.horizontalInput2 != 0f) {
                                        // if angle between axis is over 120 and less than 240 reverse run
                                        float angleTo = Vector3.Angle(
                                            animationData.thirdPersonController.movementDirection, 
                                            animationData.thirdPersonController.aimingDirection);
                                                                 
                                        if (angleTo > 120 && angleTo < 240) {
                                            animationData.actor.animation["walk"].normalizedSpeed = -animationData.walkSpeedScale * .9f;                             
                                        }
                                        else {   
                                            animationData.actor.animation["walk"].normalizedSpeed = animationData.walkSpeedScale;
                                        }
                                        animationData.actor.animation.Blend("walk");   
                                    }
                                    else {
                                        animationData.actor.animation["walk"].normalizedSpeed = animationData.walkSpeedScale;
                                        //animationData.actor.animation["run"].time = 0f;
                                        animationData.actor.animation.CrossFade("walk", .5f);                      
                                    }
                                 
                                 
                                    SendMessage("SyncAnimation", "walk", SendMessageOptions.DontRequireReceiver);
                                }
                            }
                        }
                    }
                }
                else if (isMecanim) {
                    SetFloat(GameDataActionKeys.speed, currentSpeed);
                }
            }
         // Fade out walk and run
            else {
                if (isLegacy) {
                    AnimateIdle();
                }
                else if (isMecanim) {
                    SetFloat(GameDataActionKeys.speed, currentSpeed);
                }
            }
         
            // JUMPING
         
            bool isJumping = false;
            bool isCapeFlying = false;
            bool hasJumpReachedApex = false;
            bool isGroundedWithTimeout = false;
         
            if (animationData.thirdPersonController != null) {
                isJumping = animationData.thirdPersonController.IsJumping();
                isCapeFlying = animationData.thirdPersonController.IsCapeFlying();
                hasJumpReachedApex = animationData.thirdPersonController.HasJumpReachedApex();
                isGroundedWithTimeout = animationData.thirdPersonController.IsGroundedWithTimeout();
            }
         
            if (isJumping) {
                if (isCapeFlying) {
                    AnimateJump();
                }
                else if (hasJumpReachedApex) {
                    AnimateJump();
                }
                else {
                    AnimateJump();
                }
            }
         // We fell down somewhere
            else if (!isGroundedWithTimeout) {
                //animationData.actor.animation.CrossFade("ledgefall", 0.2f);
                //SendMessage("SyncAnimation", "ledgefall", SendMessageOptions.DontRequireReceiver);
            }
         // We are not falling down anymore
            else {
                //animationData.actor.animation.Blend("ledgefall", 0.0f, 0.2f);
            }
        }
    }    

 
    // --------------
    // JUMP
 
    public virtual void AnimateJump() {
        if (animationData.actor == null) {
            return;
        }
     
        if (isLegacy) {
            
            if (animationData.actor.animation != null) {
                if (animationData.actor.animation[GameDataActionKeys.jump] != null) {
                    animationData.actor.animation.CrossFade(GameDataActionKeys.jump, 0.2f);
                    SendMessage("SyncAnimation", GameDataActionKeys.jump, 
                     SendMessageOptions.DontRequireReceiver);
                }
            }
        }
        else if (isMecanim) {
            PlayOneShotFloat(GameDataActionKeys.jump);
        }
     
        //animationData.actor.animation.CrossFade("jetpackjump", 0.2f);
        //SendMessage("SyncAnimation", "jetpackjump", SendMessageOptions.DontRequireReceiver);
        //animationData.actor.animation.CrossFade("jumpfall", 0.2f);
        //SendMessage("SyncAnimation", "jumpfall", SendMessageOptions.DontRequireReceiver);
    }
 
    public virtual void ResetClampAnimation() {
        animationData.isRunningClampAnimation = false; 
    }
 
    public virtual void PauseAnimationUpdate(float duration) {
        Invoke("ResetClampAnimation", duration);
    }

    public virtual void DidLand() {


        if (isLegacy) {
            //animationData.actor.animation.Play("jumpland");
            //SendMessage("SyncAnimation", "jumpland", SendMessageOptions.DontRequireReceiver);
            if (animationData.actor == null) {
                return;
            }

            if (animationData.actor.animation != null) {
                if (animationData.actor.animation[GameDataActionKeys.jump] != null) {
                    animationData.actor.animation.Play(GameDataActionKeys.jump);
                    SendMessage("SyncAnimation", "jumpland", SendMessageOptions.DontRequireReceiver);
                }
            }
        }
        else if (isMecanim) {
            PlayOneShotFloat(GameDataActionKeys.jump, 1f);
        }

    }
 
    // --------------
    // IDLE
 
    public virtual void AnimateIdle() {

        if (isLegacy) {
            
            if (animationData.actor == null) {
                return;
            }

            if (animationData.actor.animation != null) {
                if (animationData.actor.animation[GameDataActionKeys.idle] != null) {
                    if (animationData.actor.animation[GameDataActionKeys.idle] != null
                        && !animationData.isRunningClampAnimation) {
                        animationData.actor.animation.CrossFade(GameDataActionKeys.idle);
                        SendMessage("SyncAnimation", GameDataActionKeys.idle,
                         SendMessageOptions.DontRequireReceiver);
                    }
                }
            }
        }
        else if (isMecanim) {
            ResetPlayState();
        }
    }
 
    // --------------
    // HIT
 
    public virtual void AnimateHit() {   

        if (isLegacy) {    
            if (animationData.actor == null) {
                return;
            }
            if (animationData.actor.animation["hit"] != null) {
                animationData.actor.animation.CrossFade("hit", 0.1f);
                SendMessage("SyncAnimation", "hit", SendMessageOptions.DontRequireReceiver);
            }
        }
        else if (isMecanim) {
            if (animationData.animator != null) {                
                PlayOneShotFloat(GameDataActionKeys.attack, 1f);
            }
        }
    }
 
    // --------------
    // ACTIONS - ATTACK
 
    public virtual void Attack() {       
        DidAttack(); 
    }
 
    public virtual void AttackAlt() {        
        DidAttackAlt();  
    }
 
    public virtual void AttackLeft() {       
        DidAttackLeft(); 
    }
 
    public virtual void AttackRight() {      
        DidAttackRight();    
    }

    public virtual void DidAttack() {        
        DidAttack(GameDataActionKeys.attack);
    }
 
    public virtual void DidAttackAlt() {
        DidAttack("attack-alt");
    }
 
    public virtual void DidAttackLeft() {        
        DidAttack("attack-left");
    }
 
    public virtual void DidAttackRight() {       
        DidAttack("attack-right");
    }
 
    public virtual void DidAttack(string animationName) {
     
        if (animationData.isDead) {
            return;
        }

        if (isLegacy) {
            if (animationData.actor != null) {
                if (animationData.actor.animation != null) {
                    if (animationData.actor.animation[animationName] != null) {
                        animationData.isRunningClampAnimation = true;
                        PauseAnimationUpdate(.5f);
                        animationData.actor.animation.Play(animationName, PlayMode.StopAll);
                        //if (animationData.actor.animation["hit"] != null) {
                        //    animationData.actor.animation.Play("hit", PlayMode.StopAll);
                        //}
                    }
                }
            }
        }
        else {
            if (!animationData.isDead) {
                // animator.SetFloat(GameDataActionKeys.speed, .8f);
                //PlayOneShotFloat(GameDataActionKeys.attack);
            }
        }

        /*
        //LogUtil.Log("GamePlayerControllerAnimation:DidAttack:" + animationName);
     
        isRunningClampAnimation = true;
        PauseAnimationUpdate(1f);            
     
        //float currentSpeed = 0f;
        //float walkSpeed = 0f;
     
        if(thirdPersonController != null) {
            //currentSpeed = thirdPersonController.GetSpeed();
            //walkSpeed = thirdPersonController.walkSpeed;
        }
     
        if(animationData.actor.animation != null) {
            if(animationData.actor.animation[animationName]) {
             
                animationData.actor.animation[animationName].blendMode = AnimationBlendMode.Additive;
             
                if(thirdPersonController != null) {
                    if(thirdPersonController.verticalInput2 == 0 && thirdPersonController.horizontalInput2 == 0) {
                        animationData.actor.animation.CrossFade(animationName);            
                    }
                    else if(thirdPersonController.verticalInput2 < .5f 
                     && thirdPersonController.horizontalInput2 < .5f
                     && thirdPersonController.verticalInput2 > -.5f
                     && thirdPersonController.horizontalInput2 > -.5f) {
                        animationData.actor.animation.Blend(animationName, .8f);           
                    }
                    else {
                        animationData.actor.animation.Blend(animationName, .7f); 
                 
                    }
                }
                else {
                    animationData.actor.animation.Blend(animationName, .7f); 
             
                }
            }
        }
        */
    }

    // DEFEND


    public virtual void Defend() {
        DidDefend();
    }
 
    public virtual void DefendAlt() {
        DidDefendAlt();
    }

    public virtual void DefendLeft() {
        DidDefendLeft();
    }

    public virtual void DefendRight() {
        DidDefendRight();
    }

    public virtual void DidDefend() {
        DidDefend(GameDataActionKeys.defend);
    }
 
    public virtual void DidDefendAlt() {
        DidDefend(GameDataActionKeys.defend_alt);
    }
 
    public virtual void DidDefendLeft() {
        DidDefend(GameDataActionKeys.defend_left);
    }
 
    public virtual void DidDefendRight() {
        DidDefend(GameDataActionKeys.defend_right);
    }
 
    public virtual void DidDefend(string animationName) {
     
        if (animationData.isDead) {
            return;
        }

        if (isLegacy) {
            
            if (animationData.actor != null) {
                if (animationData.actor.animation != null) {
                    if (animationData.actor.animation[animationName] != null) {
                        animationData.isRunningClampAnimation = true;
                        PauseAnimationUpdate(.5f);
                        animationData.actor.animation.Play(animationName, PlayMode.StopAll);
                        
                        //if (animationData.actor.animation["hit"] != null) {
                        //    animationData.actor.animation.Play("hit", PlayMode.StopAll);
                        //}
                    }
                }
            }
        }
        else {
            if (!animationData.isDead) {
                SetFloat(GameDataActionKeys.speed, .8f);
            }
        }

        //LogUtil.Log("GamePlayerControllerAnimation:DidAttack:" + animationName);

        /*
        isRunningClampAnimation = true;
        PauseAnimationUpdate(1f);

        //float currentSpeed = 0f;
        //float walkSpeed = 0f;
     
        if(thirdPersonController != null) {
            //currentSpeed = thirdPersonController.GetSpeed();
            //walkSpeed = thirdPersonController.walkSpeed;
        }

        if(isLegacy) {

            if(animationData.actor.animation != null) {
                if(animationData.actor.animation[animationName]) {
    
                    animationData.actor.animation[animationName].blendMode = AnimationBlendMode.Additive;
                 
                    if(thirdPersonController != null) {
                        if(thirdPersonController.verticalInput2 == 0 && thirdPersonController.horizontalInput2 == 0) {
                            animationData.actor.animation.CrossFade(animationName);
                        }
                        else if(thirdPersonController.verticalInput2 < .5f
                         && thirdPersonController.horizontalInput2 < .5f
                         && thirdPersonController.verticalInput2 > -.5f
                         && thirdPersonController.horizontalInput2 > -.5f) {
                            animationData.actor.animation.Blend(animationName, .8f);
                        }
                        else {
                            animationData.actor.animation.Blend(animationName, .7f);
                     
                        }
                    }
                    else {
                        animationData.actor.animation.Blend(animationName, .7f);
    
                    }
                }
            }
        }
        else {

        }
        */
    }
 
    // --------------
    // ACTIONS - DIE 
 
    public virtual void Die() {  
        DidDie();
    }
 
    public virtual void DidDie() {   

        if (isLegacy) {
            if (animationData.actor != null) {
                if (animationData.actor.animation != null) {
                    if (animationData.actor.animation[GameDataActionKeys.death] != null) {
                        animationData.actor.animation.Play(GameDataActionKeys.death, PlayMode.StopAll);
                        animationData.isRunningClampAnimation = true;
                    }
                }
            }
        }
        else {
            
            if (animationData.animator != null) {
                if (!animationData.isDead) {
                    HandleAnimatorState();
                    animationData.isRunningClampAnimation = true;
                    PauseAnimationUpdate(.5f);
                    SetFloat(GameDataActionKeys.death, 1f);
                    animationData.isDead = true;
                }                
            }
        }
    }

    
    // --------------
    // ACTIONS - IDLE    
    
    public virtual void Idle() {
        DidIdle();
    }
    
    public virtual void DidIdle() {
        if (animationData.isDead) {
            return;
        }
        
        if (isLegacy) {
            if (animationData.actor != null) {
                if (animationData.actor.animation != null) {
                    if (animationData.actor.animation[GameDataActionKeys.idle] != null) {
                        animationData.isRunningClampAnimation = true;
                        //PauseAnimationUpdate(1f);  
                        animationData.actor.animation.Play(GameDataActionKeys.idle, PlayMode.StopAll);
                    }
                }
            }
        }
        else {
            ResetFloat(GameDataActionKeys.attack);
            ResetFloat(GameDataActionKeys.hit);
            ResetFloat(GameDataActionKeys.strafe);
            ResetFloat(GameDataActionKeys.speed);
            ResetFloat(GameDataActionKeys.run);
            ResetFloat(GameDataActionKeys.walk);

            PlayOneShotFloat(GameDataActionKeys.idle);
        }
    }
 
    // --------------
    // ACTIONS - JUMP    
 
    public virtual void Jump() {
        DidJump();
    }
    
    public virtual void DidJump() {
        if (animationData.isDead) {
            return;
        }
     
        if (isLegacy) {
            if (animationData.actor != null) {
                if (animationData.actor.animation != null) {
                    if (animationData.actor.animation[GameDataActionKeys.jump] != null) {
                        animationData.isRunningClampAnimation = true;
                        //PauseAnimationUpdate(1f);  
                        animationData.actor.animation.Play(GameDataActionKeys.jump, PlayMode.StopAll);
                    }
                }
            }
        }
        else {
            PlayOneShotFloat(GameDataActionKeys.jump);
        }
    }


    // --------------

    // ACTIONS - STRAFE LEFT

    public virtual void StrafeLeft() {
        DidStrafeLeft();
    }

    public virtual void DidStrafeLeft() {
        if (animationData.isDead) {
            return;
        }

        if (isLegacy) {
            if (animationData.actor != null) {
                if (animationData.actor.animation != null) {
                    if (animationData.actor.animation["strafe_left"] != null) {
                        animationData.isRunningClampAnimation = true;
                        //PauseAnimationUpdate(1f);
                        animationData.actor.animation.Play("strafe_left", PlayMode.StopAll);
                    }
                }
            }
        }
        else {
            PlayOneShotFloat(GameDataActionKeys.strafe, -1f);
        }
    }

    // --------------

    // ACTIONS - STRAFE RIGHT

    public virtual void StrafeRight() {
        DidStrafeRight();
    }

    public virtual void DidStrafeRight() {
        if (animationData.isDead) {
            return;
        }

        if (isLegacy) {
            
            if (animationData.actor != null) {
                if (animationData.actor.animation != null) {
                    if (animationData.actor.animation["strafe_right"] != null) {
                        animationData.isRunningClampAnimation = true;
                        //PauseAnimationUpdate(1f);
                        animationData.actor.animation.Play("strafe_right", PlayMode.StopAll);
                    }
                }
            }
        }
        else {
            PlayOneShotFloat(GameDataActionKeys.strafe);
        }
    }


    // --------------

    // ACTIONS - BOOST

    public virtual void Boost() {
        DidBoost();
    }

    public virtual void DidBoost() {
        if (animationData.isDead) {
            return;
        }

        if (isLegacy) {
            
            if (animationData.actor != null) {
                if (animationData.actor.animation != null) {
                    if (animationData.actor.animation["boost"] != null) {
                        animationData.isRunningClampAnimation = true;
                        //PauseAnimationUpdate(1f);
                        animationData.actor.animation.Play("boost", PlayMode.StopAll);
                    }
                }
            }
        }
        else {
            PlayOneShotFloat(GameDataActionKeys.boost);
        }
    }


    // --------------

    // ACTIONS - SPIN

    public virtual void Spin() {
        DidSpin();
    }

    public virtual void DidSpin() {
        if (animationData.isDead) {
            return;
        }

        if (isLegacy) {
            
            if (animationData.actor != null) {
                if (animationData.actor.animation != null) {
                    if (animationData.actor.animation["spin"] != null) {
                        animationData.isRunningClampAnimation = true;
                        //PauseAnimationUpdate(1f);
                        animationData.actor.animation.Play("spin", PlayMode.StopAll);
                    }
                }
            }
        }
        else {
            PlayOneShotFloat(GameDataActionKeys.spin);
        }
    }
     
    // --------------
    // ACTIONS - SKILL   
 
    public virtual void Skill() {
        DidSkill();
    }
 
    public virtual void DidSkill() {
        if (animationData.isDead) {
            return;
        }
     
        animationData.isRunningClampAnimation = true;
        PauseAnimationUpdate(1f);    
        //LogUtil.Log("DidSkill:");
        float currentSpeed = 0f;
        float walkSpeed = 0f;
     
        if (animationData.thirdPersonController != null) {
            currentSpeed = animationData.thirdPersonController.GetSpeed();
            walkSpeed = animationData.thirdPersonController.walkSpeed;
        }
     
        // Fade in run
        if (currentSpeed > walkSpeed) {
            if (animationData.actor.animation["skill"] != null) {
                animationData.actor.animation.Blend("skill");
                SendMessage("SyncAnimation", "run", SendMessageOptions.DontRequireReceiver);
            }
        }
     // Fade in walk
        else if (currentSpeed > 0.1) {
            if (animationData.actor.animation["skill"] != null) {
                animationData.actor.animation.Blend("skill");
                SendMessage("SyncAnimation", "walk", SendMessageOptions.DontRequireReceiver);
            }
        }
     // Fade out walk and run
        else {
            if (animationData.actor.animation["skill"] != null) {
                animationData.actor.animation.Play("skill");
                SendMessage("SyncAnimation", "idle", SendMessageOptions.DontRequireReceiver);
            }
        }
        //SendMessage("SyncAnimation", "run", SendMessageOptions.DontRequireReceiver);
    }
 
    public virtual void PlayOneShotBool(string paramName) {
        StartCoroutine(PlayOneShotBoolCo(paramName));
    }
 
    public virtual IEnumerator PlayOneShotBoolCo(string paramName) {
     
        if (!isLegacy) {
            
            SyncAnimator();

            if (animationData.animator != null
                && animationData.animator.enabled
                && animationData.animator.gameObject.activeInHierarchy
                && animationData.animator.gameObject.activeSelf) {
                animationData.animator.SetBool(paramName, true);
                yield return null;
                animationData.animator.SetBool(paramName, false);    
            }
        }
    }
    
    public void SetBool(string key, bool val) {
        
        if (isLegacy) {
        }
        else if (isMecanim) {
            
            SyncAnimator();
                        
            if (animationData.animator != null
                && animationData.animator.enabled
                && animationData.animator.gameObject.activeInHierarchy
                && animationData.animator.gameObject.activeSelf) {
                animationData.animator.SetBool(key, val);
            }
        }
    }

    public virtual void ResetFloat(string paramName) { 
        if (!isLegacy) {
            
            SyncAnimator();

            if (animationData.animator != null
                && animationData.animator.enabled) {
                animationData.animator.SetFloat(paramName, 0.0f);
            }
        }
    }

    public void SyncAnimator() {       
        if (animationData == null) {
            return;
        }

        if (isMecanim) {

            if (animationData.animator == null) {
                FindAnimatedActor();
                Debug.Log("animationData.animator NULL:" + 
                    " uniqueId:" + animationData.gamePlayerController.uniqueId);
            }
        }
    }
        
    public void SetFloat(string key, float val) {
        
        if (isLegacy) {
        }
        else if (isMecanim) {

            SyncAnimator();

            if (animationData.animator != null
                && animationData.animator.enabled
                && animationData.animator.gameObject.activeInHierarchy
                && animationData.animator.gameObject.activeSelf) {
                animationData.animator.SetFloat(key, val);
            }
        }
    }
 
    public virtual void PlayOneShotFloat(string paramName) {
        StartCoroutine(PlayOneShotFloatCo(paramName));
    }
 
    public virtual IEnumerator PlayOneShotFloatCo(string paramName) {
     
        if (!isLegacy) {
            
            SyncAnimator();

            if (animationData.animator != null
                && animationData.animator.enabled
                && animationData.animator.gameObject.activeInHierarchy
                && animationData.animator.gameObject.activeSelf) {
                animationData.animator.SetFloat(paramName, 1.0f);
                yield return null;
                animationData.animator.SetFloat(paramName, 0.0f);
            }
        }
    }

    public virtual void PlayOneShotFloat(string paramName, float val) {
        StartCoroutine(PlayOneShotFloatCo(paramName, val));
    }
 
    public virtual IEnumerator PlayOneShotFloatCo(string paramName, float val) {
     
        if (!isLegacy) {
            
            SyncAnimator();

            if (animationData.animator != null
                && animationData.animator.enabled
                && animationData.animator.gameObject.activeInHierarchy
                && animationData.animator.gameObject.activeSelf) {
                animationData.animator.SetFloat(paramName, val);
                yield return null;
                animationData.animator.SetFloat(paramName, 0.0f);
            }
        }
    }
 
     
    // --------------
    // ACTIONS - HIT 
 
    public virtual void ApplyDamage() {
        AnimateHit();
    }
 
    // --------------
    // ACTIONS - EXTRA   
 
    public virtual void ButtStomp() {
        DidButtStomp();
    }

    public virtual void DidButtStomp() {
        //animationData.actor.animation.CrossFade("buttstomp", 0.1f);
        //SendMessage("SyncAnimation", "buttstomp", SendMessageOptions.DontRequireReceiver);
        //animationData.actor.animation.CrossFadeQueued("jumpland", 0.2f);
    }
 
    public virtual void WallJump() {
        DidWallJump();
    }

    public virtual void DidWallJump() {
        // Wall jump animation is played without fade.
        // We are turning the character controller 180 degrees around when doing a wall jump so the animation accounts for that.
        // But we really have to make sure that the animation is in full control so 
        // that we don't do weird blends between 180 degree apart rotations
        
        if (animationData.actor.animation != null) {
            if (animationData.actor.animation["walljump"] != null) {
                animationData.actor.animation.Play("walljump");
                SendMessage("SyncAnimation", "walljump");
            }
        }
    }
}