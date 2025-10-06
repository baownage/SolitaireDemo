using System.Collections.Generic;
using UnityEngine;

public class TrashHolder : MonoBehaviour
{
    private List<Card> _cards;

    void Awake()
    {
        _cards = new();
    }

    public void AddCard(Card card)
    {
        card.transform.position = transform.position;
        card.SetStartPosition();
        card.SetSortingOrder(_cards.Count);
        _cards.Add(card);
    }
}
