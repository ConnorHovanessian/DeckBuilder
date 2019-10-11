using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;


public class BattleHandler : MonoBehaviour
{
    private static BattleHandler instance;
    public static System.Random rng = new System.Random();
    
    public static BattleHandler GetInstance() {
        return instance;}
    
    public delegate void OnDeathEventHandler();
    public event OnDeathEventHandler OnDeath;

    public delegate void OnVictoryEventHandler();
    public event OnVictoryEventHandler OnVictory;

    private int BASE_CARD_DRAW = 5;

    private float MAX_PLAYER_HEALTH = 100f;
    private float CURRENT_PLAYER_HEALTH = 100f;
    private float MAX_ENEMY_HEALTH = 10f;
    private float CURRENT_ENEMY_HEALTH = 10f;
    
    private Deck fullDeck;
    private Deck drawPile;
    private Deck discardPile;

    private GameObject handZone;
    private GameObject gameOverScreen;
    private GameObject victoryScreen;

    public GameObject GetHandZone()
    {
        return handZone;
    }

    private void Awake()
    {
        instance = this;
        handZone = GameObject.Find("Hand");

        gameOverScreen = GameObject.Find("GameOverScreen");
        gameOverScreen.SetActive(false);

        victoryScreen = GameObject.Find("VictoryScreen");
        victoryScreen.SetActive(false);

        MAX_PLAYER_HEALTH = GlobalValues.MaxHealth;
        CURRENT_PLAYER_HEALTH = GlobalValues.CurrentHealth;
        UpdateHealthBars();

        /*
        List<GameObject> testDeck = new List<GameObject>();
        testDeck.Add(Instantiate(BattleAssets.GetInstance().Stab, new Vector3(0, 0, 0), Quaternion.identity));
        testDeck.Add(Instantiate(BattleAssets.GetInstance().Stab, new Vector3(0, 0, 0), Quaternion.identity));
        testDeck.Add(Instantiate(BattleAssets.GetInstance().Stab, new Vector3(0, 0, 0), Quaternion.identity));
        testDeck.Add(Instantiate(BattleAssets.GetInstance().Block, new Vector3(0, 0, 0), Quaternion.identity));
        testDeck.Add(Instantiate(BattleAssets.GetInstance().Block, new Vector3(0, 0, 0), Quaternion.identity));
        testDeck.Add(Instantiate(BattleAssets.GetInstance().Block, new Vector3(0, 0, 0), Quaternion.identity));
        fullDeck = new Deck(testDeck); 
        */

        Deck uninstantiatedDeck = GlobalValues.Deck;
        List<GameObject> emptyDeck = new List<GameObject>();
        List<GameObject> testDeck = new List<GameObject>();

        foreach (GameObject card in uninstantiatedDeck.GetCards())
        {
            testDeck.Add(Instantiate(card, new Vector3(0, 0, 0), Quaternion.identity));
        }

        fullDeck = new Deck(testDeck);
        fullDeck.ParentDeckToHand();

        discardPile = new Deck(emptyDeck);

        drawPile = fullDeck;
        drawPile.Shuffle();

        DrawHand(BASE_CARD_DRAW);

    }

    public void Start()
    {
        Playfield.GetInstance().OnCardPlayed += HandleCardPlayed;
    }


    private void DrawHand(int numToDraw)
    {
        List<GameObject> hand = drawPile.DrawCards(numToDraw);

        for (int i=0;i<hand.Count;i++)
        {
            hand[i].SetActive(true);
        }
    }
    
    private void DiscardHand()
    {

        for (int i = 0; i < handZone.transform.childCount; i++)
        {
            GameObject child = handZone.transform.GetChild(i).gameObject;

            if (child.activeSelf == true)
            {
                discardPile.Add(handZone.transform.GetChild(i).gameObject);
                child.SetActive(false);
            }
        }

    }

    private void EnemyTurn()
    {
    CURRENT_PLAYER_HEALTH -= 10f;
    UpdateHealthBars();
    
    }

    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.E)))
            EndTurn();
    }

    private void EndTurn()
    {
        Debug.Log("Before discard! DiscardPile:");
        foreach (GameObject c in BattleHandler.GetInstance().discardPile.GetCards())
          Debug.Log(c.ToString());

        DiscardHand();

        Debug.Log("After discard! DiscardPile:");
        foreach (GameObject c in BattleHandler.GetInstance().discardPile.GetCards())
            Debug.Log(c.ToString());

        EnemyTurn();
        DrawHand(BASE_CARD_DRAW);

        Debug.Log("After Draw! DiscardPile:");
        foreach (GameObject c in BattleHandler.GetInstance().discardPile.GetCards())
            Debug.Log(c.ToString());
    }

    //HandleCardPlayed deactivates the card, so reparenting, turning on raycasts, and cleanup of placeholder must happen here
    public void HandleCardPlayed(GameObject cardObject)
    {
        Card card = cardObject.GetComponent<Card>();
        Card.cardTarget target = card.target;
        if (target == Card.cardTarget.Enemy)
        {
            CURRENT_ENEMY_HEALTH -= card.damage;
        }
        else if (target == Card.cardTarget.Self)
        {
            CURRENT_PLAYER_HEALTH += card.block;
        }
        UpdateHealthBars();

        discardPile.Add(cardObject);
        Destroy(cardObject.GetComponent<Draggable>().GetPlaceholder());
        cardObject.transform.SetParent(BattleHandler.GetInstance().GetHandZone().transform);
        cardObject.GetComponent<CanvasGroup>().blocksRaycasts = true;
        cardObject.SetActive(false);
    }

    private void UpdateHealthBars()
    {
        BattleAssets.GetInstance().SelfHPBar.text = CURRENT_PLAYER_HEALTH + "/" + MAX_PLAYER_HEALTH;
        BattleAssets.GetInstance().EnemyHPBar.text = CURRENT_ENEMY_HEALTH + "/" + MAX_ENEMY_HEALTH;

        if (CURRENT_PLAYER_HEALTH <= 0f)
        {
            GameOver();
        }
        if (CURRENT_ENEMY_HEALTH <= 0f)
        {
            Victory();
        }
    }

    private void GameOver()
    {
        gameOverScreen.SetActive(true);
    }
    private void Victory()
    {
        victoryScreen.SetActive(true);
        GlobalValues.Deck.Add(BattleAssets.GetInstance().Slam);
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

    public class Deck
    {
        private List<GameObject> cards;

        public Deck(List<GameObject> cards)
        {
            this.cards = cards;
        }

        public List<GameObject> GetCards()
        {
            return cards;
        }

        public void Add(GameObject card)
        {
            cards.Add(card);
        }

        public void Add(List<GameObject> newCards)
        {
            foreach(GameObject card in newCards)
            {
                cards.Add(card);
            }
        }

        //Parent all cards in a deck to the hand
        public void ParentDeckToHand()
        {
            foreach(GameObject card in this.cards)
            {
                card.transform.SetParent(BattleHandler.GetInstance().handZone.transform);
                card.SetActive(false);
            }
        }

        public void Shuffle()
        {
            int n = cards.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                GameObject value = cards[k];
                cards[k] = cards[n];
                cards[n] = value;
            }
        }

        //Remove any nulls from a deck
        public void CleanDeck()
        {
            this.cards.RemoveAll(item => item == null);
        }

        public List<GameObject> DrawCards(int toDraw)
        {
            Debug.Log("DrawPile:"+ BattleHandler.GetInstance().drawPile.cards.Count + "\nDiscardPile:"+ BattleHandler.GetInstance().discardPile.cards.Count +"\nToDraw:"+toDraw);
            
            //If we can draw enough cards from the draw pile
            if (toDraw <= BattleHandler.GetInstance().drawPile.cards.Count)
            {
                Debug.Log("case 1, draw "+toDraw);
                
                List<GameObject> ret = new List<GameObject>();
                for (int i = 0; i < toDraw; i++)
                {
                    ret.Add(BattleHandler.GetInstance().drawPile.cards[0]);
                    BattleHandler.GetInstance().drawPile.cards.RemoveAt(0);
                }

                return ret;
            }

            //If we need to shuffle the discard pile back in
            else if(toDraw <= BattleHandler.GetInstance().drawPile.cards.Count + BattleHandler.GetInstance().discardPile.cards.Count)
            {
                Debug.Log("case 2, draw " + toDraw);

                //draw the entire draw pile
                int partialDraw = BattleHandler.GetInstance().drawPile.cards.Count;
                List <GameObject> partialHand = BattleHandler.GetInstance().drawPile.DrawCards(partialDraw);
                Debug.Log("Partial Hand:");
                foreach (GameObject c in partialHand)
                    Debug.Log(c.ToString());


                //Put the discard back into the draw pile and shuffle
                List<GameObject> addToDraw = BattleHandler.GetInstance().discardPile.cards;
                BattleHandler.GetInstance().drawPile.Add(addToDraw);
                BattleHandler.GetInstance().drawPile.Shuffle();
                //Debug.Log("New Draw Pile:");
                //foreach (GameObject c in BattleHandler.GetInstance().drawPile.cards)
                //    Debug.Log(c.ToString());
                BattleHandler.GetInstance().discardPile.cards = new List<GameObject>();

                //Draw remaining number of cards from the drawpile
                List<GameObject> partialHand2 = BattleHandler.GetInstance().drawPile.DrawCards(toDraw-partialDraw);
                Debug.Log("Partial Hand2:");
                foreach (GameObject c in partialHand2)
                    Debug.Log(c.ToString());

                //Combine both partial Draws
                foreach (GameObject c in partialHand2)
                {
                    partialHand.Add(c);
                }

                Debug.Log("Final Hand:");
                foreach (GameObject c in partialHand)
                    Debug.Log(c.ToString());

                return partialHand;
            }

            //If we don't have enough cards to draw, we draw all cards in the draw and discard piles
            else
            {
                Debug.Log("case 3, draw " + toDraw);
                //draw the entire draw pile
                int partialDraw = BattleHandler.GetInstance().drawPile.cards.Count;
                List<GameObject> partialHand = BattleHandler.GetInstance().drawPile.DrawCards(partialDraw);
                //draw the entire discard pile
                int partialDraw2 = BattleHandler.GetInstance().discardPile.cards.Count;
                List<GameObject> partialHand2 = BattleHandler.GetInstance().discardPile.DrawCards(partialDraw);
                partialHand.AddRange(partialHand2);
                return partialHand;
            }
            
        }

    }

}
