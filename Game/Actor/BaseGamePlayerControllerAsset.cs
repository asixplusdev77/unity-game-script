using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Animation;
using Engine.Events;
using Engine.UI;
using Engine.Utility;


public class BaseGamePlayerControllerAsset : GameObjectBehavior {

    public float currentTimeBlock = 0.0f;
    public float actionInterval = 5.0f;
    
    public virtual void Start() {
    
    }
    
    public virtual void PlayOneShot(Animator animator, string paramName) {
        StartCoroutine(PlayOneShotCo(animator, paramName));
    }
    
    public virtual IEnumerator PlayOneShotCo ( Animator animator, string paramName ) {
        animator.SetBool( paramName, true );
        yield return null;
        animator.SetBool( paramName, false );
    }
    
    public virtual void RunAnimations() {
        //foreach(Animator anim in gameObject.GetComponentsInChildren<Animator>()) {
        //    anim.SetFloat("speed", v);
        //    anim.SetFloat("strafe", h);
        //    anim.SetFloat("jump", u);
            //Avatar avatar = anim.avatar;
            //RuntimeAnimatorController controller = anim.runtimeAnimatorController;
    
            //LogUtil.Log("avatar:" + avatar.name);
            //LogUtil.Log("controller:" + controller.name);
        //}
    }
    
    public virtual void Update() {

        if(!GameConfigs.isGameRunning) {
            return;
        }

        currentTimeBlock += Time.deltaTime;
    
        if(currentTimeBlock > actionInterval) {
    
            actionInterval = 5;
            currentTimeBlock = 0.0f;
    
            transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, .5f);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.identity, .5f);
    
        }
    }
}
