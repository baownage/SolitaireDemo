using System;
using UnityEngine;
using UnityEngine.Events;

public class ClickController : MonoBehaviour
{
    private Camera _camera;

    public static event Action OnMouseUp;
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

        card.Drag();
    }

    private void HandleDeckClick(Collider2D collider)
    {
        if (!collider.gameObject.TryGetComponent<DeckManager>(out var deck)) return;
        
        deck.DrawCard();
    }
}
