﻿using UnityEngine;
using System.Collections;

public class GameDamageBase : GameObjectBehavior {

	public GameObject Effect;
	[HideInInspector]
    public GamePlayerController gamePlayerController;
    public int Damage = 20;
	
	public string[] TargetTag = new string[1]{"Enemy"};
}

public class GameWeaponBase : MonoBehaviour {
	[HideInInspector]
    public GamePlayerController gamePlayerController;
	[HideInInspector]
	public GameObject Target;
	
    public string[] TargetTag = new string[1]{"Enemy"};
	public bool RigidbodyProjectile;
	public Vector3 TorqueSpeedAxis;
	public GameObject TorqueObject;

}

