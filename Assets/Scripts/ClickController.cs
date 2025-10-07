using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ClickController : MonoBehaviour
{
    private Camera _camera;

    public static event Action OnMouseUp;
    
    // Unused in this project, however in a normal project this would definitely be used
    public static event Action OnMouseDown;

    void Awake()
    {
        _camera = Camera.main;
    }

    void Update()
    {
        // Showing both approaches here
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity);

            if (hit.collider == null) return;

            HandleDeckClick(hit.collider);
            HandleCardClick(hit.collider);

            OnMouseDown?.Invoke();
        }

        if (Input.GetMouseButtonUp(0))
        {
            OnMouseUp?.Invoke();
        }
    }

    private void HandleCardClick(Collider2D collider)
    {
        if (!collider.gameObject.TryGetComponent<Card>(out var card)) return;
        if (!card.IsFaceUp) return;

        var cardColumn = card.GetColumn();
        if (cardColumn == null)
        {
            card.Drag();
            return;
        }

        List<Card> cardsSelected = cardColumn.GetCardsFrom(card);
        if (cardsSelected.Count == 1)
        {
            cardsSelected[0].Drag();
            return;
        }

        for (var i = 0; i < cardsSelected.Count; i++)
        {
            var selectedCard = cardsSelected[i];

            Card nextCard = null;
            if (i < cardsSelected.Count - 1)
            {
                nextCard = cardsSelected[i + 1];
            }
            
            selectedCard.MultipleDrag(i == 0, nextCard);
        }
    }

    private void HandleDeckClick(Collider2D collider)
    {
        if (!collider.gameObject.TryGetComponent<DeckManager>(out var deck)) return;
        
        deck.DrawCard();
    }
}
