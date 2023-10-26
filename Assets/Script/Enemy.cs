using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;
using System.Reflection;

public class Enemy{
	public EnemyBase ebase;
	public int CurrentHitPoint = 0;
	public int CurrentShieldPoint = 0;

	public Enemy(EnemyBase ebase){
		this.ebase = ebase;
		this.CurrentHitPoint = ebase.EnemyHitPoint;
	}
}
