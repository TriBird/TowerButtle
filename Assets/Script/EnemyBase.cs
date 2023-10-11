using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[CreateAssetMenu(fileName = "Enemy", menuName = "EnemybaseSchema")]
public class EnemyBase : ScriptableObject{

	public string EnemyName = "";
	public Sprite EnemySprite;

	public int EnemyHitPoint = 0;
	public int EnemyAttack = 0;
	public int EnemySpeed = 100;

}
