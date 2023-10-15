using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[CreateAssetMenu(fileName = "Stage", menuName = "StageBase")]
public class StageMaster: ScriptableObject{
	public List<StageGroup> group = new List<StageGroup>();
}

[System.Serializable]
public class StageGroup{
	public List<EnemyBase> enemies = new List<EnemyBase>();
}
