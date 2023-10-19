using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DamageCtrl: MonoBehaviour{

	public GameMaster master;

	public string DamegeText = "dummy";

	void Start() {
		GetComponent<Text>().text = DamegeText;
		transform.DOLocalMoveY(transform.localPosition.y + 200f, 1.0f);
		transform.GetComponent<CanvasGroup>().DOFade(0, 1.0f).SetLink(gameObject).SetDelay(0.3f).OnComplete(()=>{
			Destroy(gameObject);
		});	
	}
}
