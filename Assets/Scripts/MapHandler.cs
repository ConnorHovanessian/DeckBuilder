using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MapHandler : MonoBehaviour
{
    public readonly int NUMBER_OF_TOWNS_TO_SPAWN = 8;
    public readonly int MINIMUM_DISTANCE_BETWEEN_TOWNS = 30;

    private Town[] towns;
    private Town townPlayerAt;
    public static System.Random rng = new System.Random();

    private void Awake()
    {
        SpawnTowns(NUMBER_OF_TOWNS_TO_SPAWN);
        foreach (Town t in towns)
            t.LogTown();

        townPlayerAt = FindPoorestTown();
        Instantiate(MapAssets.GetInstance().RedCircle, new Vector3(townPlayerAt.GetLocation().x, townPlayerAt.GetLocation().y-1, townPlayerAt.GetLocation().z), Quaternion.identity);
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

    private class Town
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

        public double GetOverallWealth()
        {
            double a = this.slopPrice / (slopMin + .5*(slopMax-slopMin));
            double b = this.kikiPrice / (kikiMin + .5 * (kikiMax - kikiMin));
            double c = this.boubaPrice / (boubaMin + .5 * (boubaMax - boubaMin));

            return ((a + b + c) / 3);
        }

    }

}
