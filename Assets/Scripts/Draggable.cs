using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public Transform parentToReturnTo = null;
    public Transform placeholderParent = null;
    GameObject placeholder = null;
    
    public delegate void ClickCardEventHandler(GameObject card);
    public event ClickCardEventHandler OnCardClicked;

    public GameObject GetPlaceholder()
    {
        return placeholder;
    }
    
    public void OnPointerClick(PointerEventData eventData) 
    {
        if (GlobalValues.CurrentScene == GlobalValues.Scene.Town)
        {
            Debug.Log("I was clicked in a shop");
            OnCardClicked?.Invoke(this.gameObject);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (GlobalValues.CurrentScene == GlobalValues.Scene.Battle)
        {
            placeholder = Instantiate(this.gameObject);
            placeholder.transform.SetParent(this.transform.parent);
            placeholder.GetComponent<CanvasGroup>().alpha = 0;

            placeholder.transform.SetSiblingIndex(this.transform.GetSiblingIndex());

            parentToReturnTo = this.transform.parent;
            placeholderParent = parentToReturnTo;
            this.transform.SetParent(this.transform.parent.parent);

            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (GlobalValues.CurrentScene == GlobalValues.Scene.Battle)
        {
            this.transform.position = eventData.position;

            if (placeholder.transform.parent != placeholderParent)
                placeholder.transform.SetParent(placeholderParent);

            int newSiblingIndex = placeholderParent.childCount;

            for (int i = 0; i < placeholderParent.childCount; i++)
            {
                if (this.transform.position.x < placeholderParent.GetChild(i).position.x)
                {
                    newSiblingIndex = i;

                    if (placeholder.transform.GetSiblingIndex() < newSiblingIndex)
                        newSiblingIndex--;

                    break;
                }
            }
            placeholder.transform.SetSiblingIndex(newSiblingIndex);
        }


    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if(GlobalValues.CurrentScene == GlobalValues.Scene.Battle)
        {
            this.transform.SetParent(BattleHandler.GetInstance().GetHandZone().transform);
            this.transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());
            Destroy(placeholder);
            GetComponent<CanvasGroup>().blocksRaycasts = true;
        }


    }
}
