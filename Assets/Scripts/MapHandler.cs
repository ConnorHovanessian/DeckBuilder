using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapHandler : MonoBehaviour
{
    public readonly int NUMBER_OF_TOWNS_TO_SPAWN = 8;
    public readonly int MINIMUM_DISTANCE_BETWEEN_TOWNS = 30;

    private Town[] towns;
    private Town townPlayerAt;
    public static System.Random rng = new System.Random();

    private void Awake()
    {
        //If this is the first time map is loaded, instantiate global values
        if(GlobalValues.MaxHealth == 0)
        {
            Debug.Log("Instantiating Global Values");
            GlobalValues.CurrentScene = GlobalValues.Scene.Map;
            GlobalValues.MaxHealth = 80;
            GlobalValues.CurrentHealth = GlobalValues.MaxHealth;
            GlobalValues.Money = 100;
            GlobalValues.Deck = InitializeDeck();
            GlobalValues.Towns = InitializeTowns();
            GlobalValues.TownPlayerAt = townPlayerAt;
        }
        //If not, we load in our GlobalValues
        else
        {
            GlobalValues.CurrentScene = GlobalValues.Scene.Map;
            Debug.Log("Loading Global Values");
            towns = GlobalValues.Towns;
            foreach(Town t in towns)
            {
                Instantiate(MapAssets.GetInstance().Town, new Vector3(t.GetLocation().x, t.GetLocation().y, t.GetLocation().z), Quaternion.identity);
            }
            townPlayerAt = GlobalValues.TownPlayerAt;
            Instantiate(MapAssets.GetInstance().RedCircle, new Vector3(townPlayerAt.GetLocation().x, townPlayerAt.GetLocation().y - 1, townPlayerAt.GetLocation().z), Quaternion.identity);
        }

        /*
        SpawnTowns(NUMBER_OF_TOWNS_TO_SPAWN);
        foreach (Town t in towns)
            t.LogTown();

        townPlayerAt = FindPoorestTown();
        Instantiate(MapAssets.GetInstance().RedCircle, new Vector3(townPlayerAt.GetLocation().x, townPlayerAt.GetLocation().y-1, townPlayerAt.GetLocation().z), Quaternion.identity);
        */
    }

    private Town[] InitializeTowns()
    {
        SpawnTowns(NUMBER_OF_TOWNS_TO_SPAWN);
        townPlayerAt = FindPoorestTown();
        Instantiate(MapAssets.GetInstance().RedCircle, new Vector3(townPlayerAt.GetLocation().x, townPlayerAt.GetLocation().y - 1, townPlayerAt.GetLocation().z), Quaternion.identity);
        return towns;
    }

    private BattleHandler.Deck InitializeDeck()
    {
        List<GameObject> testDeck = new List<GameObject>();
        testDeck.Add(BattleAssets.GetInstance().Stab);
        testDeck.Add(BattleAssets.GetInstance().Stab);
        testDeck.Add(BattleAssets.GetInstance().Stab);
        testDeck.Add(BattleAssets.GetInstance().Block);
        testDeck.Add(BattleAssets.GetInstance().Block);
        testDeck.Add(BattleAssets.GetInstance().Block);
        return (new BattleHandler.Deck(testDeck));
    }

    void Update()
    {
        //Check if a town was clicked on
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 cursorPosition = findCursorPosition();
            if (cursorPosition.x < -9000)
            {
                Debug.Log("cursor position negative");
                return;
            }
            Town closest = towns[0];
            foreach(Town town in towns)
            {
                if(  DistanceBetween(cursorPosition, closest.GetLocation()) > DistanceBetween(cursorPosition, town.GetLocation()) && DistanceBetween(cursorPosition, town.GetLocation()) < 100)
                    closest = town;
            }

            Debug.Log("Distance to nearest town = " + DistanceBetween(cursorPosition, closest.GetLocation()));
            Debug.Log("Cursor Position: " + cursorPosition.x + ", " + cursorPosition.y);
            closest.LogTown();
            if (DistanceBetween(cursorPosition, closest.GetLocation()) <= 4.1 && closest != townPlayerAt)
            {
                TravelToTown(closest);
            }
            else if((DistanceBetween(cursorPosition, closest.GetLocation()) <= 4.1 && closest == townPlayerAt))
            {
                EnterTown(closest);
            }
        }
    }

    private void EnterTown(Town t)
    {
        StartCoroutine(LoadYourAsyncScene("Town"));
    }

    private void TravelToTown(Town t)
    {
        //Save data to pass into next scene
        GlobalValues.TownPlayerAt = t;
        // Use a coroutine to load the Scene in the background
        StartCoroutine(LoadYourAsyncScene("Battle"));
    }

    IEnumerator LoadYourAsyncScene(String str)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(str);
        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public Vector3 findCursorPosition()
    {
        Vector2 cubeRay = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D cubeHit = Physics2D.Raycast(cubeRay, Vector2.zero);

        if (cubeHit)
        {
            Debug.Log("We hit " + cubeHit.collider.name);
            return new Vector3(cubeHit.point.x, cubeHit.point.y, 0); 
        }
        else
        {
            return (new Vector3(-9999,-9999,-9999));
        }
    }

    public double DistanceBetween(Vector3 v1, Vector3 v2)
    {
        double distance = Math.Sqrt((double)(v1.x - v2.x) * (v1.x - v2.x) + (v1.y - v2.y) * (v1.y - v2.y));
        return distance;
    }

    private Town FindPoorestTown()
    {
        Town t = towns[0];
        for(int i = 1; i < towns.Length; i++)
        {
            if (towns[i].GetEconomy().GetOverallWealth() > t.GetEconomy().GetOverallWealth())
                t = towns[i];
        }
        return t;
    }

    private double FindDistanceFromPlayer(Town t)
    {
        float x = townPlayerAt.GetLocation().x;
        float y = townPlayerAt.GetLocation().y;
        double distance = Math.Sqrt((double)(x - t.GetLocation().x) * (x - t.GetLocation().x) + (y - t.GetLocation().y) * (y - t.GetLocation().y));
        return (distance);
    }

    private void SpawnTowns(int numTowns)
    {
        towns = new Town[numTowns];
        for (int i = 0; i < numTowns; i++)
        {
            towns[i] = SpawnTown();
            Vector3 loc = towns[i].GetLocation();
            
            Instantiate(MapAssets.GetInstance().Town, new Vector3(loc.x, loc.y, loc.z), Quaternion.identity);
        }
    }

    private Town SpawnTown()
    {
        float maxx = 150f;
        float maxy = 90f;

        float randomx = (float) (rng.NextDouble() * maxx) - (maxx/2);
        float randomy = (float) (rng.NextDouble() * maxy) - (maxy/2);

        while(IsValidPosition(randomx,randomy) != true)
        {
            randomx = (float)(rng.NextDouble() * maxx) - (maxx / 2);
            randomy = (float)(rng.NextDouble() * maxy) - (maxy / 2);
        }
        
        Town town = new Town("Boston", new Vector3(randomx, randomy,0f));
        return town;
    }

    private bool IsValidPosition(float x, float y)
    {
        foreach(Town town in towns)
        {
            if(town != null)
            {
                town.LogTown();
                double distance = Math.Sqrt((double)(x - town.GetLocation().x) * (x - town.GetLocation().x) + (y - town.GetLocation().y) * (y - town.GetLocation().y));
                if (distance < Math.Abs(MINIMUM_DISTANCE_BETWEEN_TOWNS)) return false;
            }
        }
        return true;
    }

    public class Town
    {
        Vector3 location;
        string name;
        Economy localEconomy;

        public Town(string name, Vector3 location)
        {
            this.name = name;
            this.location = location;
            this.localEconomy = new Economy();
        }

        public void SetLocation(Vector3 vector)
        {
            this.location = vector;
        }

        public Vector3 GetLocation()
        {
            return this.location;
        }

        public Economy GetEconomy()
        {
            return this.localEconomy;
        }

        public void SetName(String str)
        {
            this.name = str;
        }
        public void LogTown()
        {
            string[] prices = this.localEconomy.GetPrices();

            Debug.Log(this.name + " " + this.location);

            foreach (string price in prices)
                Debug.Log(price);

        }

    }

    public class Economy
    {
        int slopPrice;
        int kikiPrice;
        int boubaPrice;

        int slopMin = 10;
        int kikiMin = 25;
        int boubaMin = 25;

        int slopMax = 30;
        int kikiMax = 75;
        int boubaMax = 75;

        public Economy()
        {
            slopPrice = rng.Next(slopMin, slopMax);
            kikiPrice = rng.Next(kikiMin, kikiMax);
            boubaPrice = rng.Next(boubaMin, boubaMax);
        }

        public string[] GetPrices()
        {
            string[] ret = new string[3];
            ret[0] = "slopPrice: " + slopPrice;
            ret[1] = "kikiPrice: " + kikiPrice;
            ret[2] = "boubaPrice: " + boubaPrice;
            return ret;
        }

        public int[] GetPricesInt()
        {
            int[] ret = new int[3];
            ret[0] = slopPrice;
            ret[1] = kikiPrice;
            ret[2] = boubaPrice;
            return ret;
        }

        public double GetOverallWealth()
        {
            double a = this.slopPrice / (slopMin + .5*(slopMax-slopMin));
            double b = this.kikiPrice / (kikiMin + .5 * (kikiMax - kikiMin));
            double c = this.boubaPrice / (boubaMin + .5 * (boubaMax - boubaMin));

            return ((a + b + c) / 3);
        }

    }

}
