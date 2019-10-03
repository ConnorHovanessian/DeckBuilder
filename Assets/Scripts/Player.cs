using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private static Player instance;

    public static Player GetInstance()
    {
        return instance;
    }
    
    private void Awake()
    {
        instance = this;
    }


}
