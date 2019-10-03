using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapAssets : MonoBehaviour
{
    private static MapAssets instance;

    public static MapAssets GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = this;
    }

    public GameObject Town;
    public GameObject RedCircle;

}
