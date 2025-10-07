using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class DeckManager : MonoBehaviour, IUndoable
{

    [SerializeField] private Transform NewCardTransform;
    [SerializeField] private List<CardConfig> Deck;
    [SerializeField] private GameObject CardPrefab;
    [SerializeField] private Board Board;

    private List<CardConfig> _currentDeck;
    private List<CardConfig> _standbyDeck;

    private List<Card> _drawnCards;

    void Awake()
    {
        _currentDeck = new();
        _standbyDeck = new();
        _drawnCards = new();
    }

    void Start()
    {
        ResetDeck();
        InitBoard();
        UndoManager.Instance.Clear();
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
                bool faceUp = j == cardsToPlace - 1;

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
        if (_currentDeck.Count == 0)
        {
            for (int i = _standbyDeck.Count - 1; i >= 0; i--)
            {
                _currentDeck.Add(_standbyDeck[i]);
            }

            _standbyDeck.Clear();

            foreach (var card in _drawnCards)
            {
                if (card.IsPlaced) continue;

                CardPool.Instance.Return(card);
            }

            _drawnCards.Clear();
            return null;
        }
        var cardToDraw = _currentDeck[^1];
        var newCard = CardPool.Instance.GetCard();
        PlaceCardToStandby(newCard);
        newCard.SetConfig(cardToDraw);
        _drawnCards.Add(newCard);
        _currentDeck.RemoveAt(_currentDeck.Count - 1);

        UndoManager.Instance.AddMove(new List<Card>() { newCard }, UndoManager.Move.PreviousLocation.Deck);

        return newCard;
    }

    private void PlaceCardToStandby(Card newCard)
    {
        _standbyDeck.Add(newCard.GetConfig());
        newCard.transform.position = NewCardTransform.position;
        newCard.SetStartPosition();
        newCard.SetColumn(null);
        newCard.SetSortingOrder(_standbyDeck.Count);
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

    public void Undo(List<Card> cards, UndoManager.Move.PreviousLocation previousLocation, bool flipped = false)
    {
        if (previousLocation == UndoManager.Move.PreviousLocation.Deck)
        {
            var card = cards[0];
            var cardConfig = card.GetConfig();

            _standbyDeck.Remove(cardConfig);
            _currentDeck.Add(cardConfig);

            CardPool.Instance.Return(card);
        }

        if (previousLocation == UndoManager.Move.PreviousLocation.DeckStandby)
        {
            var card = cards[0];
            PlaceCardToStandby(card);
        }
    }
}
