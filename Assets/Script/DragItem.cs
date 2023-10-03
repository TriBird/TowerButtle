using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using System.Reflection;

public class DragItem: MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler{

	public GameMaster master;
	public int itemindex = -1;

	private Transform LastTriggered_Trans = null;
	private bool isDropEnable = false;
	private Vector3 beforeDragPosition = new Vector3();

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

		master.ItemGetHundler(itemindex);

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
				master.StatusAdder_ATK(1);
				yield break;
			}
		}
	}

	public void OnTriggerEnter2D(Collider2D other){
		LastTriggered_Trans = other.transform;
	}

	public void OnTriggerExit2D(Collider2D other){
		LastTriggered_Trans = null;
	}

	void Start() {
		
	}

}
