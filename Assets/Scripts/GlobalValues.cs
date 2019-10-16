public static class GlobalValues
{
    private static int currentHealth, maxHealth, money;
    private static BattleHandler.Deck deck;
    private static MapHandler.Town[] towns;
    private static MapHandler.Town townPlayerAt;

    public enum Scene { Battle, Map, Town }
    private static Scene currentScene;

    public static Scene CurrentScene
    {
        get
        {
            return currentScene;
        }
        set
        {
            currentScene = value;
        }
    }

    public static int CurrentHealth
    {
        get
        {
            return currentHealth;
        }
        set
        {
            currentHealth = value;
        }
    }
    public static MapHandler.Town TownPlayerAt
    {
        get
        {
            return townPlayerAt;
        }
        set
        {
            townPlayerAt = value;
        }
    }

    public static MapHandler.Town[] Towns
    {
        get
        {
            return towns;
        }
        set
        {
            towns = value;
        }
    }

    public static int MaxHealth
    {
        get
        {
            return maxHealth;
        }
        set
        {
            maxHealth = value;
        }
    }

    public static int Money
    {
        get
        {
            return money;
        }
        set
        {
            money = value;
        }
    }

    public static BattleHandler.Deck Deck
    {
        get
        {
            return deck;
        }
        set
        {
            deck = value;
        }
    }

}