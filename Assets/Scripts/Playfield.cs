using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Playfield : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{

    public delegate void PlayCardEventHandler(GameObject card);
    public event PlayCardEventHandler OnCardPlayed;

    private static Playfield instance;

    public static Playfield GetInstance()
    {
        return instance;
    }
    private void Awake()
    {
        instance = this;
    }

    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;

        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        if (d != null)
        {
            d.placeholderParent = this.transform;
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;
        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        if (d != null && d.placeholderParent == this.transform)
        {
            d.placeholderParent = d.parentToReturnTo;
        }
    }
    public void OnDrop(PointerEventData eventData)
    {
        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        if (d != null)
        {
            //d.parentToReturnTo = this.transform;
        }
        GameObject card = eventData.pointerDrag.gameObject;
        OnCardPlayed?.Invoke(card);
    }
}