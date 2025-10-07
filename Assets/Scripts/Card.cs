using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering;
using DG.Tweening;

public class Card : MonoBehaviour
{
    private const float CardHeight = 1.8f;
    private const float CardWidth = 1.4f;

    [SerializeField] private Rank Rank;
    [SerializeField] private Suit Suit;
    [SerializeField] private SortingGroup SortingGroup;
    [SerializeField] private SpriteRenderer FaceSpriteRenderer;
    [SerializeField] private CardConfig Config;
    [SerializeField] private GameObject BackFace;
    [SerializeField] private GameObject FrontFace;
    [SerializeField] private Collider2D Collider;
    [SerializeField] private LayerMask CardZoneLayer;

    [SerializeField] private float CardResetSpeed = 3f;

    private Vector3 _startPosition;
    private CardColumn _currentColumn;

    private bool _isBeingDragged;
    private int _previousSortOrder;

    public bool IsFaceUp => FrontFace.activeSelf;

    void OnEnable()
    {
        ClickController.OnMouseUp += ReleaseCard;
    }

    void OnDisable()
    {
        ClickController.OnMouseUp -= ReleaseCard;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isBeingDragged) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        transform.position = mousePos;
    }

    [Button]
    private void ApplyConfig()
    {
        if (Config == null) return;

        FaceSpriteRenderer.sprite = Config.CardSprite;
        Rank = Config.Rank;
        Suit = Config.Suit;
    }

    public void SetConfig(CardConfig config)
    {
        Config = config;
        ApplyConfig();
    }

    public void SetSortingOrder(int order)
    {
        SortingGroup.sortingOrder = order;
    }

    public void Drag()
    {
        _isBeingDragged = true;
        _previousSortOrder = SortingGroup.sortingOrder;
        SetSortingOrder(5000);
        transform.DOKill();
    }

    private void ReleaseCard()
    {
        if (!_isBeingDragged) return;

        _isBeingDragged = false;

        // Top left corner
        Vector2 pointA = transform.position;
        pointA.y += CardHeight / 2f;
        pointA.x -= CardWidth / 2f;

        // Bottom right corner
        Vector2 pointB = pointA;
        pointB.x += CardWidth;
        pointB.y -= CardHeight;

        Collider2D hitZone = Physics2D.OverlapArea(pointA, pointB, CardZoneLayer);
        if (hitZone == null)
        {
            ReturnToStartPosition();
            return;
        }


        if (hitZone.gameObject.TryGetComponent<TrashHolder>(out var trashHolder))
        {
            trashHolder.AddCard(this);
            if (_currentColumn != null)
            {
                _currentColumn.RemoveCard(this);
                _currentColumn.Refresh();
            }
            return;
        }

        if (hitZone.gameObject.TryGetComponent<CardColumn>(out var cardColumn))
        {
            if(_currentColumn != null) _currentColumn.RemoveCard(this);
            cardColumn.AddCard(this);
            return;
        }

        ReturnToStartPosition();
    }

    private void ReturnToStartPosition()
    {
        SetSortingOrder(_previousSortOrder);
        transform.DOMove(_startPosition, CardResetSpeed).SetSpeedBased();
    }

    public void SetStartPosition(Vector3 pos)
    {
        _startPosition = pos;
    }

    public void SetStartPosition()
    {
        _startPosition = transform.position;
    }

    public void Flip(bool faceUp = true)
    {
        FrontFace.SetActive(false);
        if (faceUp) FrontFace.SetActive(true);
        BackFace.SetActive(!FrontFace.activeSelf);
        Collider.enabled = FrontFace.activeSelf;
    }

    public void SetColumn(CardColumn cardColumn)
    {
        _currentColumn = cardColumn;
    }
}
