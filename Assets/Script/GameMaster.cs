using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;

public class GameMaster: MonoBehaviour{

	public GameObject Damage_Obj, Effect_Obj, Reward_Obj, Bubble_Obj;
	public Transform Status_Trans, Player_Trans, Enemy_Trans, DropContainer_Trans, Buttle_Trans, BlackTurn_Trans, Thunder_Trans;
	public Transform HP_Bar, ExpBar;

	public Sprite Boss_Sprite;

	public List<ItemSchema> ItemSchema_List;
	public ItemSchema Key_ItemSchema;
	public StageMaster stageMaster;

	public EnemyBase FireDragon_EB;

	private int Floor = 0;
	private int CurrentEnemyIndex = 0;
	private Enemy EnemyInstance = null;
	private int PlayerHitPoint = 1000;
	private int PlayerCurrentHitPoint = 0;
	private int Attack = 100;
	private int Defence = 0;
	private int CurrentEXP = 0;
	private int CurrentLevel = 1;
	private Coroutine Cronus_Routine = null;
	private Coroutine EnemyCronus_Routine = null;
	private List<GameObject> DeleteResearvedObjects = new List<GameObject>();
	private List<DragItem> BossKey_flags = new List<DragItem>(){null, null, null};

	private List<int> ATK_table = new List<int>(){1,3,7,15};
	private List<int> DEF_table = new List<int>(){1,3,7,15};

	void Start() {
		// Debug_LevelExp();

		PlayerCurrentHitPoint = PlayerHitPoint;
		HP_Bar.Find("Current").GetComponent<Image>().fillAmount = (float)PlayerCurrentHitPoint / PlayerHitPoint;

		// EnemyAppear();
		StartCoroutine(FireDragonAppear());
	}

	public void Debug_LevelExp(){
		for(int i=0; i<30; i++){
			print("Lv." + CurrentLevel + ":" + NextLevelExp());
			CurrentLevel++;
		}
	}

	public int NextLevelExp(){
		int a = 10;
		for(int i=0; i<CurrentLevel; i++){
			a = Mathf.CeilToInt(a * 1.1f);
		}
		int b = CurrentLevel * 15;

		return Mathf.FloorToInt((a + b) / 2f);
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

			// collect 3 gems to summon deamon king
			if(ditem._schema.ItemSchema_name == "KeyItem"){
				BossKey_flags[ditem._level] = ditem;
				if(!BossKey_flags.Exists(x => x == null)){
					StartCoroutine(BossAppear());
				}
			}
		}
	}

	
	public IEnumerator BossAppear(){
		if(Cronus_Routine != null) StopCoroutine(Cronus_Routine); 
		if(EnemyCronus_Routine != null) StopCoroutine(EnemyCronus_Routine);

		// black turn
		BlackTurn_Trans.GetComponent<CanvasGroup>().alpha = 0;
		BlackTurn_Trans.gameObject.SetActive(true);
		BlackTurn_Trans.GetComponent<CanvasGroup>().DOFade(1, 1.0f);

		yield return new WaitForSeconds(1.5f);

		// gem
		List<Vector2> GemPoint = new List<Vector2>(){
			new Vector2(1345f, 159f),
			new Vector2(1545f, 289f),
			new Vector2(1745f, 159f)
		};
		for(int i=0; i<3; i++){
			DragItem di = BossKey_flags[i];
			di.isCollisionEnable = false;
			di.transform.DOLocalMove(GemPoint[i], 2.0f);
			di.transform.GetComponent<Rigidbody2D>().simulated = false;
		}

		yield return new WaitForSeconds(1.5f);

		foreach(DragItem bosskey in BossKey_flags){
			Destroy(bosskey.gameObject);
		}

		// effect
		Thunder_Trans.gameObject.SetActive(true);
		Enemy_Trans.localPosition = new Vector2(135f, -100f);
		Enemy_Trans.GetComponent<Image>().sprite = Boss_Sprite;
		BlackTurn_Trans.GetComponent<CanvasGroup>().DOFade(0, 0.5f);

		yield return new WaitForSeconds(0.5f);

		Thunder_Trans.gameObject.SetActive(false);

		print("BossAppear");
	}

	public void EnemyAppear(){
		Floor++;

		EnemyDiscard();

		Enemy_Trans.gameObject.SetActive(true);
		Enemy_Trans.DOLocalMoveX(335f, 0.5f);

		if(Mathf.FloorToInt(Floor / 5f) >= stageMaster.group.Count){
			print("group is less than registration");
			return;
		}

		StageGroup group = stageMaster.group[Mathf.FloorToInt(Floor / 5f)];
		EnemyInstance = new Enemy(group.enemies[Random.Range(0, group.enemies.Count)]);
		Enemy_Trans.GetComponent<Image>().sprite = EnemyInstance.ebase.EnemySprite;
		Enemy_Trans.Find("HPBar/Current").GetComponent<Image>().fillAmount = (float)EnemyInstance.CurrentHitPoint / EnemyInstance.ebase.EnemyHitPoint;

		Cronus_Routine = StartCoroutine(Cronus());
		// EnemyCronus_Routine = StartCoroutine(EnemyCronus());
	}

	/// <summary>
	/// Initializa Enemy
	/// </summary>
	public void EnemyDiscard(){
		Enemy_Trans.Find("Shield").gameObject.SetActive(false);
		Enemy_Trans.Find("SkillName").gameObject.SetActive(false);
	}

	public void AttackMotion(){
		Vector2 pos = Enemy_Trans.localPosition;
		Sequence sequence = DOTween.Sequence();
		sequence.Append(Enemy_Trans.DOLocalMoveX(pos.x-50f, 0.2f));
		sequence.Append(Enemy_Trans.DOLocalMoveX(pos.x, 0.1f));
		sequence.SetLink(Enemy_Trans.gameObject);
	}

	public void Enemy_NormalAttack(){
		AttackMotion();

		GameObject damage_txt = Instantiate(Damage_Obj, Buttle_Trans);
		damage_txt.GetComponent<DamageCtrl>().master = this;
		damage_txt.transform.localPosition = Player_Trans.localPosition;
		damage_txt.GetComponent<DamageCtrl>().DamegeText = EnemyInstance.ebase.EnemyAttack.ToString();

		PlayerCurrentHitPoint -= EnemyInstance.ebase.EnemyAttack;
		HP_Bar.Find("Current").GetComponent<Image>().fillAmount = (float)PlayerCurrentHitPoint / PlayerHitPoint;
	}

	public IEnumerator EnemyCronus(){
		while(true){
			yield return new WaitForSeconds(EnemyInstance.ebase.EnemySpeed / 150f);

			Enemy_NormalAttack();
		}
	}

	public IEnumerator FireDragonAppear(){
		EnemyInstance = new Enemy(FireDragon_EB);
		Enemy_Trans.GetComponent<Image>().sprite = EnemyInstance.ebase.EnemySprite;

		// initial position
		EnemyDiscard();
		Enemy_Trans.localPosition = new Vector2(900f, -420f);

		// forward
		Enemy_Trans.gameObject.SetActive(true);
		Enemy_Trans.DOLocalMove(new Vector3(335f, -300f), 0.5f);

		yield return new WaitForSeconds(0.5f);

		Sequence evitation = DOTween.Sequence();
		evitation.Append(Enemy_Trans.DOLocalMoveY(-350f, 1.0f));
		evitation.Append(Enemy_Trans.DOLocalMoveY(-300f, 1.0f));
		evitation.SetLoops(-1);
		evitation.SetLink(Enemy_Trans.gameObject);

		Cronus_Routine = StartCoroutine(Cronus());
		EnemyCronus_Routine = StartCoroutine(FireDragonCronus());
	}

	public IEnumerator FireDragonCronus(){
		int routine_index = 0;

		while(true){
			yield return new WaitForSeconds(EnemyInstance.ebase.EnemySpeed / 150f);

			// 英傑の歌
			if(EnemyInstance.CurrentShieldPoint <= 0){
				AddEffect_Sheild(1000);
				continue;
			}

			switch(routine_index){
				case 0: // 通常
				case 2:
					Enemy_NormalAttack();
					break;
				case 1:
					SkillName_View("炎牙");
					
					break;
				case 3:
					SkillName_View("蒼炎");
					break;
				case 4:
					SkillName_View("咆哮");
					break;
				case 5:
					SkillName_View("ラスト・リベンジ");
					break;
			}
		}
	}

	public void AddEffect_Sheild(int value, string skillname = "シールド"){
		SkillName_View(skillname);
		EnemyInstance.SetShield(value);
		Enemy_Trans.Find("Shield").gameObject.SetActive(true);
	}

	public void SkillName_View(string text){
		Enemy_Trans.Find("SkillName").GetComponentInChildren<Text>().text = text;
		Enemy_Trans.Find("SkillName").gameObject.SetActive(true);

		DOVirtual.DelayedCall(1.0f, ()=>{
			Enemy_Trans.Find("SkillName").gameObject.SetActive(false);
		});
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
		dmg.GetComponent<DamageCtrl>().DamegeText = damage.ToString();
		dmg.transform.localPosition = Enemy_Trans.localPosition;
		yield return new WaitForSeconds(0.2f);

		Destroy(obj);
		yield return new WaitForSeconds(0.8f);

		Destroy(dmg);
	}

	/// <summary>
	/// Main game routine
	/// Discard when player is rewarding.
	/// </summary>
	/// <returns></returns>
	public IEnumerator Cronus(){
		while(true){
			yield return new WaitForSeconds(0.75f);

			// proc:: damage calc
			int damage = Attack;

			// if enemy has a shield, player attack the shield
			if(EnemyInstance.CurrentShieldPoint > 0){
				EnemyInstance.CurrentShieldPoint -= damage;
				// judge shield break
				if(EnemyInstance.CurrentShieldPoint < 0){
					Enemy_Trans.Find("Shield").gameObject.SetActive(false);
				}
			}else{
				EnemyInstance.CurrentHitPoint -= damage;
			}
			StartCoroutine(Anim_Damaging(damage));

			Enemy_Trans.Find("HPBar/Current").GetComponent<Image>().fillAmount = (float)EnemyInstance.CurrentHitPoint / EnemyInstance.ebase.EnemyHitPoint;
			Enemy_Trans.Find("Shield/HPBar/Current").GetComponent<Image>().fillAmount = (float)EnemyInstance.CurrentShieldPoint / EnemyInstance.MaxShieldPoint;
			
			// judge dead
			if(EnemyInstance.CurrentHitPoint <= 0){
				yield return new WaitForSeconds(1.0f);

				// StopCoroutine(EnemyCronus_Routine);
				Enemy_Trans.localPosition = new Vector3(900f, -420f, 0f);

				yield return new WaitForSeconds(0.1f);

				// process of experience points
				CurrentEXP += EnemyInstance.ebase.EnemyExp;
				int NextExp = NextLevelExp();
				if(NextExp <= CurrentEXP){
					GameObject dmg = Instantiate(Damage_Obj, Buttle_Trans);
					dmg.GetComponent<DamageCtrl>().master = this;
					dmg.GetComponent<DamageCtrl>().DamegeText = "Level Up!";
					dmg.transform.localPosition = Player_Trans.localPosition;

					StartCoroutine(MakeRewardItem());
					
					CurrentLevel++;
					CurrentEXP = 0;
				}else{
					EnemyAppear();
				}
				ExpBar.Find("Current").GetComponent<Image>().fillAmount = (float)CurrentEXP / NextExp;

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

			// appear key item for boss on 10% chance and more 20F
			ItemSchema schema = null;
			int level = 0; 
			bool isKey = false;
			if(Floor > 0 && Random.Range(0, 10) < 9){
				schema = Key_ItemSchema;

				// if player get the gem already
				List<int> lot = new List<int>();
				for(int j=0; j<3; j++){
					if(!BossKey_flags[j]) lot.Add(j);
				}
				level = lot[Random.Range(0, lot.Count)];

				isKey = true;
			}else{
				schema = ItemSchema_List[Random.Range(0, ItemSchema_List.Count)];
				level = 0;
			}

			GameObject reward = Instantiate(Reward_Obj, DropContainer_Trans);
			GameObject bubble = Instantiate(Bubble_Obj, DropContainer_Trans);
			float posy = 400f;
			if(i == 1) posy = 500f;
			reward.transform.localPosition = new Vector3(950f + 350f * i, posy, -10f);
			bubble.transform.localPosition = new Vector3(950f + 350f * i, posy, -10f);
			reward.GetComponent<DragItem>().DragItemMake(this, i, level, isKey, schema);

			DeleteResearvedObjects.Add(reward);
			DeleteResearvedObjects.Add(bubble);

			yield return new WaitForSeconds(0.1f);
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

}
