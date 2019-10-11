using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleAssets : MonoBehaviour
{
    private static BattleAssets instance;

    public static BattleAssets GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = this;
    }

    public Sprite idle1;

    public Text SelfHPBar;
    public Text EnemyHPBar;

    public GameObject EmptyCard;
    //Attacks
    public GameObject Stab;
    public GameObject Slam;
    //Skills
    public GameObject Block;

}
