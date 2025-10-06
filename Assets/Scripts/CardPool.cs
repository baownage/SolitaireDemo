using System;
using System.Collections.Generic;
using UnityEngine;

public class CardPool : MonoBehaviour
{
    [SerializeField] private GameObject CardPrefab;

    private List<Card> _deactiveObjects;
    private List<Card> _activeObjects;

    private const int PoolStartSize = 52;

    public static CardPool Instance;


    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _deactiveObjects = new();
        _activeObjects = new();
    }

    void Start()
    {
        InitPool();
    }

    private void InitPool()
    {
        for (int i = 0; i < PoolStartSize; i++)
        {
            InstantiateCard();
        }
    }

    private void InstantiateCard()
    {
        var newCardObject = Instantiate(CardPrefab, transform);
        var card = newCardObject.GetComponent<Card>();
        card.gameObject.SetActive(false);
        _deactiveObjects.Add(card);
    }

    public Card GetCard()
    {
        if (_deactiveObjects.Count == 0)
        {
            InstantiateCard();
        }

        var card = _deactiveObjects[^1];
        card.gameObject.SetActive(true);
        card.transform.SetParent(transform.parent);
        _activeObjects.Add(card);
        _deactiveObjects.RemoveAt(_deactiveObjects.Count - 1);

        return card;
    }

    public void Return(Card card)
    {
        _activeObjects.Remove(card);
        _deactiveObjects.Add(card);
        card.Flip();
        card.gameObject.SetActive(false);
        card.transform.SetParent(transform);
    }

    public void ReturnAll()
    {
        while (_activeObjects.Count > 0)
        {
            var card = _activeObjects[^1];
            Return(card);
        }
    }
}
