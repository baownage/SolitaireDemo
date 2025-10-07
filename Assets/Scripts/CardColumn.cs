using System.Collections.Generic;
using UnityEngine;

public class CardColumn : MonoBehaviour
{

    [SerializeField] private BoxCollider2D Collider;

    private List<Card> _cards;

    private float _startYOffset;
    private float _startYSize;


    void Awake()
    {
        _cards = new();
        _startYOffset = Collider.offset.y;
        _startYSize = 1.86f;
    }


    public void AddCard(Card card, bool faceUp = true)
    {
        card.gameObject.transform.position = new Vector3(transform.position.x, transform.position.y - 0.54f * _cards.Count, 0f);
        card.SetStartPosition();
        card.Flip(faceUp);
        card.SetColumn(this);
        RefreshColliderSize();
        _cards.Add(card);
        
        card.SetSortingOrder(_cards.Count);
    }

    private void RefreshColliderSize()
    {
        var offset = _cards.Count * 0.54f;
        Collider.size = new Vector2(Collider.size.x, _startYSize + offset);
        Collider.offset = new Vector2(Collider.offset.x, -offset / 2f);
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
        RefreshColliderSize();
        Refresh();
    }

    public Card GetTopCard()
    {
        if (_cards.Count > 0) return _cards[^1];
        return null;
    }

    public List<Card> GetCardsFrom(Card card)
    {
        if (GetTopCard() == card)
        {
            return new List<Card> { card };
        }
        
        // Find card index in column
        int cardIndex = 0;
        for (int i = 0; i < _cards.Count; i++)
        {
            if (_cards[i] == card)
            {
                cardIndex = i;
                break;
            }
        }
    
        // Add all cards from index and return
        List<Card> cards = new();
        for (int i = cardIndex; i < _cards.Count; i++)
        {
            cards.Add(_cards[i]);
        }

        return cards;
    }
}
