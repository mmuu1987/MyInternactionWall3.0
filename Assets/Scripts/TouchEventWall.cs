using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchEventWall : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler,IPointerClickHandler
{
    public bool IsEnable = true;

    public event Action<bool> TouchMoveEvent;

    public event Action<PointerEventData> DragMoveEvent;

    public event Action<PointerEventData> OnBeginDragEvent;


    public event Action<PointerEventData> OnEndDragEvent;

    public event Action<PointerEventData> OnClick;


    private Vector2 _beginDragPos;
    public void OnDrag(PointerEventData eventData)
    {
        if (!IsEnable) return;
       // Debug.Log("OnDrag");

        if (DragMoveEvent != null)
            DragMoveEvent(eventData);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!IsEnable) return;
        Debug.Log("OnBeginDrag");
        _beginDragPos = eventData.position;

        if (OnBeginDragEvent != null) OnBeginDragEvent(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!IsEnable) return;

        Vector2 pos = eventData.position;

        if (pos.x > _beginDragPos.x)
        {
            //isLeft = false;
            if (TouchMoveEvent != null) TouchMoveEvent(false);
            // Debug.Log("ÏòÓÒ»¬");
        }
        else
        {
            // Debug.Log("Ïò×ó»¬");
            if (TouchMoveEvent != null) TouchMoveEvent(true);
            //isLeft = true;
        }

        if (OnEndDragEvent != null) OnEndDragEvent(eventData);
        // Debug.Log("OnEndDrag");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
       // Debug.Log(eventData.position);
        if (OnClick != null) OnClick(eventData);
        
    }
}