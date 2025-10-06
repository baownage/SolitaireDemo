using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class DeckManager : MonoBehaviour
{

    [SerializeField] private Transform NewCardTransform;
    [SerializeField] private List<CardConfig> Deck;
    [SerializeField] private GameObject CardPrefab;
    [SerializeField] private Board Board;

    private List<CardConfig> _currentDeck;
    private List<CardConfig> _standbyDeck;

    void Awake()
    {
        _currentDeck = new();
        _standbyDeck = new();
    }

    void Start()
    {
        ResetDeck();
        InitBoard();
    }

    private void ResetDeck()
    {
        _currentDeck.Clear();
        _standbyDeck.Clear();
        foreach (var cardConfig in Deck)
        {
            _currentDeck.Add(cardConfig);
        }

        ShuffleDeck(_currentDeck);
    }

    private void InitBoard()
    {
        int cardsToPlace = 1;
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < cardsToPlace; j++)
            {
                bool faceUp = false;
                if (j == cardsToPlace - 1) faceUp = true;

                var card = DrawCard();
                _standbyDeck.RemoveAt(_standbyDeck.Count - 1);
                Board.AddCardToColumn(i, card, faceUp);
            }
            cardsToPlace++;
        }
    }

    [Button]
    public Card DrawCard()
    {
        var cardToDraw = _currentDeck[^1];
        var newCard = CardPool.Instance.GetCard();
        newCard.transform.position = NewCardTransform.position;
        newCard.SetStartPosition();
        newCard.SetConfig(cardToDraw);
        newCard.SetSortingOrder(52 - _currentDeck.Count);
        _currentDeck.RemoveAt(_currentDeck.Count - 1);
        _standbyDeck.Add(cardToDraw);

        return newCard;
    }

    private void ShuffleDeck<T>(List<T> list)
    {
        int n = list.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }

    [Button]
    private void DebugReset()
    {
        CardPool.Instance.ReturnAll();
        ResetDeck();
        InitBoard();
    }
}
