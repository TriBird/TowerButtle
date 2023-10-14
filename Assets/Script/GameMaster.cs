using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameMaster: MonoBehaviour{

	public GameObject Damage_Obj, Effect_Obj, Reward_Obj, Bubble_Obj;
	public Transform Status_Trans, Player_Trans, Enemy_Trans, DropContainer_Trans, Buttle_Trans;

	public List<ItemSchema> ItemSchema_List;
	public List<EnemyBase> Enemy_List;
	public StageMaster stageMaster;

	private int Floor = 0;
	private int CurrentEnemyIndex = 0;
	private Enemy EnemyInstance = null;
	private int Attack = 100;
	private int Defence = 0;
	private Coroutine Cronus_Routine = null;
	private Coroutine EnemyCronus_Routine = null;
	private List<GameObject> DeleteResearvedObjects = new List<GameObject>();

	private List<int> ATK_table = new List<int>(){1,3,7,15};
	private List<int> DEF_table = new List<int>(){1,3,7,15};

	void Start() {
		EnemyAppear();
	}

	public void StatusAdder_ATK(int addnum){
		Attack += addnum;
		Status_Trans.Find("ATK").GetComponent<Text>().text = "ATK " + Attack;
	}

	public void StatusAdder_DEF(int addnum){
		Defence += addnum;
		Status_Trans.Find("DEF").GetComponent<Text>().text = "DEF " + Defence;
	}

	public void CalcStatus(){
		Attack = 100;
		Defence = 0;

		foreach(Transform tmp in DropContainer_Trans){
			DragItem ditem = tmp.GetComponent<DragItem>();
			if(ditem is null) continue;
			if(!ditem.isDropEnable) continue;

			if(ditem._schema.ItemSchema_name == "Sword") StatusAdder_ATK(ATK_table[ditem._level]);
			if(ditem._schema.ItemSchema_name == "Armor") StatusAdder_DEF(DEF_table[ditem._level]);
		}
	}

	public void EnemyAppear(){
		Floor++;

		Enemy_Trans.gameObject.SetActive(true);
		Enemy_Trans.DOLocalMoveX(335f, 0.5f);

		StageGroup group = stageMaster.group[Mathf.FloorToInt(Floor / 5f)];
		EnemyInstance = new Enemy(group.enemies[Random.Range(0, group.enemies.Count)]);
		// EnemyInstance = new Enemy(Enemy_List[CurrentEnemyIndex]);
		Enemy_Trans.GetComponent<Image>().sprite = EnemyInstance.ebase.EnemySprite;
		Enemy_Trans.Find("HPBar/Current").GetComponent<Image>().fillAmount = (float)EnemyInstance.CurrentHItPoint / EnemyInstance.ebase.EnemyHitPoint;

		Cronus_Routine = StartCoroutine(Cronus());
		EnemyCronus_Routine = StartCoroutine(EnemyCronus());
	}

	public IEnumerator EnemyCronus(){
		while(true){
			yield return new WaitForSeconds(EnemyInstance.ebase.EnemySpeed / 100f);

			Vector2 pos = Enemy_Trans.localPosition;
			Sequence sequence = DOTween.Sequence();
			sequence.Append(Enemy_Trans.DOLocalMoveX(pos.x-50f, 0.2f));
			sequence.Append(Enemy_Trans.DOLocalMoveX(pos.x, 0.1f));

			//enemy attack
			GameObject damage_txt = Instantiate(Damage_Obj, Buttle_Trans);
			damage_txt.GetComponent<DamageCtrl>().master = this;
			damage_txt.transform.localPosition = Player_Trans.localPosition;
		}
	}

	/// <summary>
	/// Animation when enemy damaging
	/// </summary>
	/// <returns></returns>
	public IEnumerator Anim_Damaging(int damage){
		Player_Trans.DOLocalRotate(new Vector3(0, 360f, 0), 0.5f, RotateMode.FastBeyond360);
		yield return new WaitForSeconds(0.3f);

		Enemy_Trans.DOShakePosition(0.3f, 10, 10);

		GameObject obj = Instantiate(Effect_Obj, Enemy_Trans);
		obj.transform.localPosition = new Vector3(-50f, 50f);

		GameObject dmg = Instantiate(Damage_Obj, Buttle_Trans);
		dmg.GetComponent<DamageCtrl>().master = this;
		dmg.transform.localPosition = Enemy_Trans.localPosition;
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
			yield return new WaitForSeconds(1.0f);

			// proc:: damage calc
			int damage = Attack;
			EnemyInstance.CurrentHItPoint -= damage;
			StartCoroutine(Anim_Damaging(damage));

			Enemy_Trans.Find("HPBar/Current").GetComponent<Image>().fillAmount = (float)EnemyInstance.CurrentHItPoint / EnemyInstance.ebase.EnemyHitPoint;
			
			// judge dead
			if(EnemyInstance.CurrentHItPoint <= 0){
				yield return new WaitForSeconds(0.8f);
				// Enemy_Trans.gameObject.SetActive(false);
				Enemy_Trans.localPosition = new Vector3(900f, -420f, 0f);
				StartCoroutine(MakeRewardItem());
				StopCoroutine(EnemyCronus_Routine);
				yield break;
			}
		}
	}

	/// <summary>
	/// Make reward three items
	/// </summary>
	public IEnumerator MakeRewardItem(){
		DeleteResearvedObjects = new List<GameObject>();
		
		for(int i=0; i<3; i++){
			ItemSchema schema = ItemSchema_List[Random.Range(0, ItemSchema_List.Count)];

			GameObject reward = Instantiate(Reward_Obj, DropContainer_Trans);
			GameObject bubble = Instantiate(Bubble_Obj, DropContainer_Trans);
			float posy = 400f;
			if(i == 1) posy = 500f;
			reward.transform.localPosition = new Vector3(950f + 350f * i, posy, -10f);
			bubble.transform.localPosition = new Vector3(950f + 350f * i, posy, -10f);
			reward.GetComponent<DragItem>().DragItemMake(this, i, 0, schema);

			DeleteResearvedObjects.Add(reward);
			DeleteResearvedObjects.Add(bubble);

			yield return new WaitForSeconds(0.2f);
		}

		yield break;
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
