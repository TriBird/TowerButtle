using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameMaster: MonoBehaviour{

	public GameObject Damage_Obj, Effect_Obj, Reward_Obj, Bubble_Obj;
	public Transform Status_Trans, Player_Trans, Enemy_Trans, DropContainer_Trans;

	private int Attack = 100;
	private Coroutine Cronus_Routine = null;
	private List<GameObject> DeleteResearvedObjects = new List<GameObject>();

	void Start() {
		EnemyAppear();
	}

	public void StatusAdder_ATK(int addnum){
		Attack += addnum;
		Status_Trans.Find("ATK").GetComponent<Text>().text = "Attack " + Attack;
	}

	public void CalcStatus(){

	}

	public void EnemyAppear(){
		Enemy_Trans.gameObject.SetActive(true);
		Enemy_Trans.DOLocalMoveX(335f, 0.5f);

		Cronus_Routine = StartCoroutine(Cronus());
	}

	/// <summary>
	/// Animation when enemy damaging
	/// </summary>
	/// <returns></returns>
	public IEnumerator Anim_Damaging(){
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

	/// <summary>
	/// Main game routine
	/// </summary>
	/// <returns></returns>
	public IEnumerator Cronus(){
		while(true){
			yield return new WaitForSeconds(2f);
			StartCoroutine(Anim_Damaging());

			// proc:: damage calc
			if(true){
				// Enemy_Trans.gameObject.SetActive(false);
				Enemy_Trans.localPosition = new Vector3(900f, -420f, 0f);
				MakeRewardItem();
				yield break;
			}
		}
	}

	/// <summary>
	/// Make reward three items
	/// </summary>
	public void MakeRewardItem(){
		DeleteResearvedObjects = new List<GameObject>();
		
		for(int i=0; i<3; i++){
			GameObject reward = Instantiate(Reward_Obj, DropContainer_Trans);
			GameObject bubble = Instantiate(Bubble_Obj, DropContainer_Trans);
			reward.transform.localPosition = new Vector3(950f + 350f * i, 500f, -10f);
			bubble.transform.localPosition = new Vector3(950f + 350f * i, 500f, -10f);
			reward.GetComponent<DragItem>().master = this;
			reward.GetComponent<DragItem>().itemindex = i;

			DeleteResearvedObjects.Add(reward);
			DeleteResearvedObjects.Add(bubble);
		}
	}

	public void ItemGetHundler(int index){
		DeleteResearvedObjects.RemoveAt(index*2);

		// remove no-selected objects
		foreach(GameObject obj in DeleteResearvedObjects) Destroy(obj);
		DeleteResearvedObjects = new List<GameObject>();

		EnemyAppear();
	}

	public void Killed(){

	}

}
