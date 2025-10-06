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
        card.SetColumn(this);

        _cards.Add(card);
    }

    public void Refresh()
    {
        bool allFaceDown = true;
        foreach (var card in _cards)
        {
            if (card.IsFaceUp)
            {
                allFaceDown = false;
                break;
            }
        }

        if (allFaceDown && _cards.Count > 0)
        {
            _cards[^1].Flip(true);
        }
    }

    public void RemoveCard(Card card)
    {
        _cards.Remove(card);
    }
}
