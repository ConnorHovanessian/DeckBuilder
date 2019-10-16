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

    private void Start()
    {
        GameObject[] Cards = new GameObject[] { Stab, Slam, Block, Kiki, Bouba, Slop };
    }

    public void GetCards(List<GameObject> list)
    {
        list.Add(Stab);
        list.Add(Slam);
        list.Add(Kiki);
    }

    public Sprite idle1;

    public Text SelfHPBar;
    public Text EnemyHPBar;
    public Text PriceText;

    public GameObject EmptyCard;
    //Attacks
    public GameObject Stab;
    public GameObject Slam;
    //Skills
    public GameObject Block;
    //Consumables
    public GameObject Kiki;
    public GameObject Bouba;
    public GameObject Slop;

    public GameObject[] Cards;

}
