using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Engine.Data.Json;
using Engine.Events;
using Engine.Utility;

public class UIGameRPGHealth : UIGameRPGObject {

    public override void Start() {
        incrementValue = .01;
        profileValue = 1;
        lastValue = 0;
        UpdateValue();
    }

    public override void UpdateValue() {
        //if(useGlobal) {
        //    profileValue = GameProfileRPGs.Current.GetGamePlayerProgressHealth(1);
        //}
        //else {
        profileValue = GameProfileCharacters.currentProgress.GetGamePlayerProgressHealth(1);
        //}
    }

    public override void UpdateInterval() {
        if(lastTime > 1f) {
            lastTime = 0f;
            UpdateValue();
        }
    }

    public override void HandleUpdate(bool updateTimeInterval) {

        lastTime += Time.deltaTime;

        if(updateTimeInterval) {
            UpdateInterval();
        }

        base.HandleUpdate(false);
    }

    public override void Update() {

        HandleUpdate(true);

        if(UIGameKeyCodes.isActionHealthAdd) {
            GameProfileCharacters.currentProgress.AddGamePlayerProgressHealth(incrementValue);
        }
        else if(UIGameKeyCodes.isActionHealthSubtract) {
            GameProfileCharacters.currentProgress.SubtractGamePlayerProgressHealth(incrementValue);
        }
    }
}