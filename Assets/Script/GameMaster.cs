using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameMaster: MonoBehaviour{

	public GameObject Damage_Obj, Effect_Obj;
	public Transform Status_Trans, Player_Trans, Enemy_Trans;

	private int Attack = 100;

	void Start() {
		EnemyAppear();
		StartCoroutine(Cronus());
	}

	public void StatusAdder_ATK(int addnum){
		Attack += addnum;
		Status_Trans.Find("ATK").GetComponent<Text>().text = "Attack " + Attack;
	}

	public void CalcStatus(){

	}

	public void EnemyAppear(){
		Enemy_Trans.DOLocalMoveX(335f, 0.5f);
	}

	public IEnumerator Damaging(){
		Player_Trans.DOLocalRotate(new Vector3(0, 360f, 0), 0.5f, RotateMode.FastBeyond360);
		yield return new WaitForSeconds(0.3f);

		Enemy_Trans.DOShakePosition(0.3f, 10, 10);

		GameObject obj = Instantiate(Effect_Obj, Enemy_Trans);
		obj.transform.localPosition = new Vector3(-50f, 50f);

		GameObject dmg = Instantiate(Damage_Obj, Enemy_Trans.parent);
		dmg.transform.localPosition = Enemy_Trans.localPosition;
		dmg.GetComponent<Text>().text = "9999";
		dmg.transform.DOLocalMoveY(dmg.transform.localPosition.y + 200f, 1.0f);
		dmg.transform.GetComponent<CanvasGroup>().DOFade(0, 1.0f).SetLink(dmg).SetDelay(0.3f);
		yield return new WaitForSeconds(0.3f);

		Destroy(obj);
		yield return new WaitForSeconds(1.0f);
		Destroy(dmg);
	}

	public IEnumerator Cronus(){
		while(true){
			yield return new WaitForSeconds(2f);
			StartCoroutine(Damaging());
		}
	}

	public void Killed(){

	}

	public void Reward(){

	}
}
