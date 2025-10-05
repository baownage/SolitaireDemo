using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class DeckManager : MonoBehaviour
{

    [SerializeField] private Transform NewCardTransform;
    [SerializeField] private List<CardConfig> Deck;
    [SerializeField] private GameObject CardPrefab;

    private List<CardConfig> _currentDeck;

    void Awake()
    {
        _currentDeck = new();
    }

    void Start()
    {
        ResetDeck();
    }

    private void ResetDeck()
    {
        _currentDeck.Clear();
        foreach (var cardConfig in Deck)
        {
            _currentDeck.Add(cardConfig);
        }

        ShuffleDeck(_currentDeck);
    }

    [Button]
    public void DrawCard()
    {
        var cardToDraw = _currentDeck[^1];
        var newCard = CardPool.Instance.GetCard();
        newCard.transform.position = NewCardTransform.position;
        newCard.SetConfig(cardToDraw);
        newCard.SetSortingOrder(52 - _currentDeck.Count);
        _currentDeck.RemoveAt(_currentDeck.Count - 1);
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
        ResetDeck();
        CardPool.Instance.ReturnAll();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
