using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TownHandler : MonoBehaviour
{

    private static TownHandler instance;
    public static TownHandler GetInstance()
    {
        return instance;
    }

    private MapHandler.Town townPlayerAt;
    public int money;
    private int slopPrice, kikiPrice, boubaPrice;

    private Text moneyText;
    private BattleHandler.Deck ShopDeck;

    private GameObject shopZone;
    public GameObject GetShopZone()
    {
        return shopZone;
    }

    private void Awake()
    {
        instance = this;
        shopZone = GameObject.Find("ShopZone");
        moneyText = GameObject.Find("MoneyText").GetComponent<Text>();
        LoadGlobalValues();
        LoadPrices();
        PopulateShopZone();
    }


    void Start()
    {
        AddPrices();
        UpdateMoney(0);
    }

    void Update()
    {
        GlobalValues.Money = money;

        if ((Input.GetKeyDown(KeyCode.E)))
            StartCoroutine(LoadYourAsyncScene());
    }

    IEnumerator LoadYourAsyncScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Map");
        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    void PopulateShopZone()
    {
        List<GameObject> tempDeck = new List<GameObject>();
        tempDeck.Add(Instantiate(BattleAssets.GetInstance().Slop, new Vector3(0, 0, 0), Quaternion.identity));
        tempDeck.Add(Instantiate(BattleAssets.GetInstance().Kiki, new Vector3(0, 0, 0), Quaternion.identity));
        tempDeck.Add(Instantiate(BattleAssets.GetInstance().Bouba, new Vector3(0, 0, 0), Quaternion.identity));
        ShopDeck = new BattleHandler.Deck(tempDeck);
        ParentDeckToShop(ShopDeck);

    }

    public void AddPrices()
    {
        int[] prices = townPlayerAt.GetEconomy().GetPricesInt();
        int i = 0;
        foreach(GameObject card in ShopDeck.GetCards())
        {
            card.GetComponent<Card>().price = prices[i];

            Text txt = Instantiate(BattleAssets.GetInstance().PriceText, new Vector3(0, 0, 0), Quaternion.identity);
            txt.transform.SetParent(card.transform);
            txt.transform.localPosition = new Vector3(465, -120, 0);
            txt.text = prices[i].ToString();
            i++;
        }
    }

    public void UpdateMoney(int moneyChange)
    {
        money += moneyChange;
        moneyText.text = "LOONIES: " + money.ToString();
    }

    public void UpdateMoney(int? moneyChange)
    {
        money += (int) moneyChange;
        moneyText.text = "LOONIES: " + money.ToString();
    }

    //Parent all cards in a deck to the hand
    public void ParentDeckToShop(BattleHandler.Deck deck)
    {
        foreach (GameObject card in deck.GetCards())
        {
            card.transform.SetParent(shopZone.transform);
            card.GetComponent<Draggable>().OnCardClicked += HandleCardClicked;
        }
    }

    public void HandleCardClicked(GameObject card)
    {
        Debug.Log("Card Clicked Event Success: " + card);
        if(money >= card.GetComponent<Card>().price)
        {
            UpdateMoney(-1*card.GetComponent<Card>().price);

            List<GameObject> cards = new List<GameObject>();
            BattleAssets.GetInstance().GetCards(cards);

            foreach(GameObject c in cards)
            {
                //Must iterate through all cards here because we need to add a prefab, not the already instanced card in the shop since it will be deleted. 
                Debug.Log(c.GetComponent<Card>().cardName + card.GetComponent<Card>().cardName);
                if (c.GetComponent<Card>().cardName == card.GetComponent<Card>().cardName)
                    GlobalValues.Deck.Add(c);

            }

        }
        
    }

    void LoadPrices()
    {
        if (GlobalValues.TownPlayerAt == null) return;
        int[] prices = townPlayerAt.GetEconomy().GetPricesInt();
        slopPrice = prices[0];
        kikiPrice = prices[1];
        boubaPrice = prices[2];
    }

    void LoadGlobalValues()
    {
        if (GlobalValues.TownPlayerAt == null) return;
        GlobalValues.CurrentScene = GlobalValues.Scene.Town;
        townPlayerAt = GlobalValues.TownPlayerAt;
        money = GlobalValues.Money;
    }

}
