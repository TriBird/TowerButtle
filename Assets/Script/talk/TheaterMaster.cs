using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TheaterMaster: MonoBehaviour{

	public Transform Sentence_Parent;
	public GameObject SentenceBox_Prefab;

	private int CurrentProgress = 0;

	void Start() {
		
	}

	public void Next_Sentence(){
		foreach(Transform tmp in Sentence_Parent){
			tmp.DOLocalMoveY(tmp.localPosition.y + 170f, 0.3f);
		} 

		GameObject obj = Instantiate(SentenceBox_Prefab, Sentence_Parent);
		obj.transform.Find("Text").GetComponent<Text>().text = Prologue[CurrentProgress++].sentence;
		obj.transform.localPosition = new Vector3(0, -579f, 0f);

		if(CurrentProgress == Prologue.Count){

		}
	}

	public List<SentenceContainer> Prologue = new List<SentenceContainer>(){
		new SentenceContainer(0, "ここはある小さな王国。"),
		new SentenceContainer(0, "王様が好き勝手するせいで民は我慢の限界だった。"),
		new SentenceContainer(1, "もう許せない…"),
		new SentenceContainer(1, "自分の贅沢のためにおやつにまで課税するなんて…"),
		new SentenceContainer(1, "倒してやりたいけど王様めっちゃ強いらしいから"),
		new SentenceContainer(1, "封印された魔王を復活させて私の下僕にしちゃおう"),
		new SentenceContainer(0, "魔王さえ従えることができるという家宝の水晶を手に取り、"),
		new SentenceContainer(0, "少女は反逆を決意する。"),
		new SentenceContainer(0, "水晶は倒した魔物を力に変えて取り込む性質があるようだ。"),
		new SentenceContainer(0, "まずは３つの封印石を集まると復活するといわれる魔王を"),
		new SentenceContainer(0, "従えにいこう"),
	};

	public class SentenceContainer{
		public int chrid;
		public string sentence;

		public SentenceContainer(int id, string sen){
			chrid = id;
			sentence = sen;
		}
	}
}
