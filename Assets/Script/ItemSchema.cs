using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[CreateAssetMenu(fileName = "Schema", menuName = "ItemSchema")]
public class ItemSchema: ScriptableObject{
	
	public string ItemSchema_name = "";
	public List<Sprite> ItemSchema_sprites = new List<Sprite>();

}
