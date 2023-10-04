using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using System.Reflection;

public class DragItem: MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler{

	public GameMaster master_instance;
	public int itemindex = -1;
	public ItemSchema _schema;
	public int _level = 1;

	private bool isCollisionEnable = true;
	private Transform LastTriggered_Trans = null;
	private bool isDropEnable = false;
	private Vector3 beforeDragPosition = new Vector3();

	private void Start() {
		
	}

	/// <summary>
	/// Instance Maker
	/// </summary>
	public void DragItemMake(GameMaster master, int index, int level, ItemSchema schema){
		master_instance = master;
		itemindex = index;
		_level = level;
		_schema = schema;

		transform.GetComponent<SpriteRenderer>().sprite = _schema.ItemSchema_sprites[_level];
		transform.localScale = new Vector3(75, 75);
		Destroy(transform.GetComponent<PolygonCollider2D>());
		gameObject.AddComponent<PolygonCollider2D>();
	}
	
	public void OnTriggerEnter2D(Collider2D other){
		LastTriggered_Trans = other.transform;
	}

	public void OnTriggerExit2D(Collider2D other){
		LastTriggered_Trans = null;
	}

	public void OnCollisionEnter2D(Collision2D collisionInfo){
		if(!isCollisionEnable) return;

		if(isDropEnable && collisionInfo.transform.GetComponent<DragItem>()){
			DragItem di = collisionInfo.transform.GetComponent<DragItem>();
			if(di._schema.ItemSchema_name == _schema.ItemSchema_name && di._level == _level){
				di.isCollisionEnable = false;

				_level += 1;
				transform.GetComponent<SpriteRenderer>().sprite = _schema.ItemSchema_sprites[_level];
				Destroy(transform.GetComponent<PolygonCollider2D>());
				gameObject.AddComponent<PolygonCollider2D>();
				Destroy(di.gameObject);
			}
		}
	}
	
	// -------------- //
	// Drag Interface //
	// -------------- //

	public void OnBeginDrag(PointerEventData eventData){
		beforeDragPosition = transform.localPosition;
	}

	public void OnDrag(PointerEventData eventData){
		if(isDropEnable) return;

		var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		pos.z = 0;
		transform.position = pos;
	}

	public void OnEndDrag(PointerEventData eventData)	{
		// rollback to before position when dragendpos is except droparea.
		if(LastTriggered_Trans is null){
			transform.localPosition = beforeDragPosition;
			return;
		}
		if(LastTriggered_Trans.tag != "DropArea"){
			transform.localPosition = beforeDragPosition;
			return;
		}

		master_instance.ItemGetHundler(itemindex);

		transform.GetComponent<PolygonCollider2D>().isTrigger = false;
		transform.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
		isDropEnable = true;

		StartCoroutine(StopAudit());
	}

	public IEnumerator StopAudit(){
		while(true){
			yield return new WaitForSeconds(0.5f);

			float vel = transform.GetComponent<Rigidbody2D>().velocity.magnitude;

			if (vel < 0.001){
				master_instance.StatusAdder_ATK(1);
				yield break;
			}
		}
	}

}
