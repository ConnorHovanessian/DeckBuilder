public static class GlobalValues
{
    private static int currentHealth, maxHealth, money;
    private static BattleHandler.Deck deck;

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