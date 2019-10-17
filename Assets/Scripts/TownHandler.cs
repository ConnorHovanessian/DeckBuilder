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
    private BattleHandler.Deck fullDeck;
    private BattleHandler.Deck shopDeck;
    private BattleHandler.Deck sellDeck;

    private GameObject sellZone;
    private GameObject shopZone;
    public GameObject GetShopZone()
    {
        return shopZone;
    }

    private void Awake()
    {
        instance = this;
        shopZone = GameObject.Find("ShopZone");
        sellZone = GameObject.Find("SellZone");
        moneyText = GameObject.Find("MoneyText").GetComponent<Text>();
        fullDeck = GlobalValues.Deck;
        LoadGlobalValues();
        LoadPrices();
        PopulateShopZone();
        PopulateSellZone();
    }


    void Start()
    {
        AddPrices();
        AddSellPrices();
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
        shopDeck = new BattleHandler.Deck(tempDeck);
        ParentDeck(shopDeck, shopZone);
    }

    void PopulateSellZone()
    {
        BattleHandler.Deck fullDeck = GlobalValues.Deck;

        List<string> cardsAdded = new List<string>();
        List<GameObject> tempDeck = new List<GameObject>();
        foreach (GameObject card in fullDeck.GetCards())
        {
            if (card.GetComponent<Card>().cardName != "Stab" && card.GetComponent<Card>().cardName != "Block" && !cardsAdded.Contains(card.GetComponent<Card>().cardName))
            {
                tempDeck.Add(Instantiate(card, new Vector3(0, 0, 0), Quaternion.identity));
                cardsAdded.Add(card.GetComponent<Card>().cardName);
            }
        }
        sellDeck = new BattleHandler.Deck(tempDeck);
        ParentDeck(sellDeck, sellZone);
    }

    public void AddSellPrices()
    {
        IDictionary<string, int> dict = townPlayerAt.GetEconomy().GetPricesDictionary();
        foreach (GameObject card in sellDeck.GetCards())
        {
            card.GetComponent<Card>().price = dict[card.GetComponent<Card>().cardName];

            Text txt = Instantiate(BattleAssets.GetInstance().PriceText, new Vector3(0, 0, 0), Quaternion.identity);
            txt.transform.SetParent(card.transform);
            txt.transform.localPosition = new Vector3(465, -120, 0);
            txt.text = card.GetComponent<Card>().price.ToString();
        }
    }

    public void AddPrices()
    {
        IDictionary<string, int> dict = townPlayerAt.GetEconomy().GetPricesDictionary();
        foreach(GameObject card in shopDeck.GetCards())
        {
            card.GetComponent<Card>().price = dict[card.GetComponent<Card>().cardName];

            Text txt = Instantiate(BattleAssets.GetInstance().PriceText, new Vector3(0, 0, 0), Quaternion.identity);
            txt.transform.SetParent(card.transform);
            txt.transform.localPosition = new Vector3(465, -105, 0);
            txt.text = card.GetComponent<Card>().price.ToString();
        }
    }

    public void UpdateMoney(int moneyChange)
    {
        money += moneyChange;
        moneyText.text = "LOONIES: " + money.ToString();
    }

    public void UpdateMoney(int? moneyChange)
    {
        if (moneyChange == null) return;
        money += (int) moneyChange;
        moneyText.text = "LOONIES: " + money.ToString();
    }

    //Parent all cards in a deck to the GameObject
    public void ParentDeck(BattleHandler.Deck deck, GameObject zone)
    {
        foreach (GameObject card in deck.GetCards())
        {
            card.transform.SetParent(zone.transform);
            card.GetComponent<Draggable>().OnCardClicked += HandleCardClicked;
        }
    }

    public void HandleCardClicked(GameObject card)
    {
        Debug.Log("Card Clicked Event Success: " + card);
        //We buy a card if we have money
        if(money >= card.GetComponent<Card>().price && card.transform.parent == shopZone.transform)
        {
            UpdateMoney(-1*card.GetComponent<Card>().price);

            List<GameObject> cards = new List<GameObject>();
            BattleAssets.GetInstance().GetCards(cards);

            foreach(GameObject c in cards)
            {
                //Must iterate through all cards here because we need to add a prefab, not the already instanced card in the shop since it will be deleted upon next scene load. 
                Debug.Log(c.GetComponent<Card>().cardName + card.GetComponent<Card>().cardName);
                if (c.GetComponent<Card>().cardName == card.GetComponent<Card>().cardName)
                    GlobalValues.Deck.Add(c);
            }
        }
        //We sell a card if we have money
        else if (card.transform.parent == sellZone.transform && fullDeck.Contains(card))
        {
            UpdateMoney(1 * card.GetComponent<Card>().price);
            GlobalValues.Deck.Remove(card);
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
