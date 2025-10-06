using System.Collections.Generic;
using UnityEngine;

public class CardColumn : MonoBehaviour
{
    private List<Card> _cards;

    void Awake()
    {
        _cards = new();
    }

    public void AddCard(Card card, bool faceUp = true)
    {
        card.gameObject.transform.position = new Vector3(transform.position.x, transform.position.y - 0.54f * _cards.Count, 0f);
        card.SetStartPosition();
        card.Flip(faceUp);
        
        _cards.Add(card);
    }
}
