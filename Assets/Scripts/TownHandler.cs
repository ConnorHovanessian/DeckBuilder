using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownHandler : MonoBehaviour
{
    private MapHandler.Town townPlayerAt;
    public int money;
    private int slopPrice, kikiPrice, boubaPrice;

    void Start()
    {
        townPlayerAt = GlobalValues.TownPlayerAt;
        money = GlobalValues.Money;
        int[] prices = townPlayerAt.GetEconomy().GetPricesInt();

        slopPrice = prices[0];
        kikiPrice = prices[1];
        boubaPrice = prices[2];

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
