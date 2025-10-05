using UnityEngine;

[CreateAssetMenu(fileName = "CardConfig", menuName = "Scriptable Objects/CardConfig")]
public class CardConfig : ScriptableObject
{
    public Rank Rank;
    public Suit Suit;
    public Sprite CardSprite;
}

public enum Rank
{
    Undefined = 0,
    Ace = 1,
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5,
    Six = 6,
    Seven = 7,
    Eight = 8,
    Nine = 9,
    Ten = 10,
    Jack = 11,
    Queen = 12,
    King = 13
}

public enum Suit
{
    Undefined = 0,
    Clubs = 1,
    Hearts = 2,
    Spades = 3,
    Diamonds = 4
}
