using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using System.Reflection;

public class DragItem: MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler{

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

		transform.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
		isDropEnable = true;
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
