using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownClickable : MonoBehaviour
{
    public delegate void TownClickedEventHandler();
    public event TownClickedEventHandler OnTownClicked;

    private static TownClickable instance;

    private void Awake()
    {
        Debug.Log("Setting Instance");
        instance = this;
    }

    public static TownClickable GetInstance()
    {
        return instance;
    }

    void OnMouseDown()
    {
        OnTownClicked?.Invoke();
        if(OnTownClicked != null) Debug.Log("OnTownClicked not null");
        else Debug.Log("OnTownClicked is null");
    }
}