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

    public bool IsBlack => Suit is Suit.Clubs or Suit.Spades;

    private Vector3 _startPosition;
    private Vector3 _mouseOffset;
    private CardColumn _currentColumn;
    private Camera _camera;

    private bool _isBeingDragged;
    
    // Used in multiple drag scenarios;
    private bool _isFirstCard;
    private Card _nextCard;

    public bool IsPlaced { get; private set; }
    
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

    private void Awake()
    {
        Init();
    }
    
    public void Init()
    {
        IsPlaced = false;
        _mouseOffset = Vector3.zero;
        _camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isBeingDragged) return;

        Vector3 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        transform.position = mousePos + _mouseOffset;
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

    public void Drag(bool isFirstCard = true)
    {
        _isFirstCard = isFirstCard;
        _isBeingDragged = true;
        _previousSortOrder = SortingGroup.sortingOrder;
        SetSortingOrder(5000);
        transform.DOKill();
    }

    public void MultipleDrag(bool isFirstCard, Card nextCard)
    {
        Vector3 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        
        _mouseOffset = transform.position - mousePos;

        _nextCard = nextCard;

        Drag(isFirstCard);
    }

    private void ReleaseCard()
    {
        if (!_isBeingDragged) return;
        if (!_isFirstCard)
        {
            _isBeingDragged = false;
            _isFirstCard = true;
            return;
        }

        _isBeingDragged = false;
        _isFirstCard = true;
        _mouseOffset = Vector3.zero;

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
            SendCardToTrash(trashHolder);
            return;
        }

        if (hitZone.gameObject.TryGetComponent<CardColumn>(out var cardColumn))
        {
            var topCard = cardColumn.GetTopCard();
            if((topCard == null && Rank == Rank.King) || (topCard != null && topCard.IsStackable(this)))
            {
                StackCardOnColumn(cardColumn);
                return;
            }
            
        }

        ReturnToStartPosition();
    }

    private void StackCardOnColumn(CardColumn cardColumn)
    {
        IsPlaced = true;
        if (_currentColumn != null) _currentColumn.RemoveCard(this);
        cardColumn.AddCard(this);
        
        if(_nextCard != null) _nextCard.StackCardOnColumn(cardColumn);
    }

    private void SendCardToTrash(TrashHolder trashHolder)
    {
        IsPlaced = true;
        trashHolder.AddCard(this);
        if (_currentColumn != null)
        {
            _currentColumn.RemoveCard(this);
            _currentColumn.Refresh();
        }
        
        if(_nextCard != null) _nextCard.SendCardToTrash(trashHolder);
    }

    private bool IsStackable(Card card)
    {
        // Same color can't stack
        if (IsBlack == card.IsBlack) return false;

        if ((int)card.Rank == (int)Rank - 1) return true;
        return false;
    }

    private void ReturnToStartPosition()
    {
        SetSortingOrder(_previousSortOrder);
        transform.DOMove(_startPosition, CardResetSpeed).SetSpeedBased();
        
        if(_nextCard != null) _nextCard.ReturnToStartPosition();
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
        IsPlaced = true;
        _currentColumn = cardColumn;
    }

    public CardColumn GetColumn()
    {
        return _currentColumn;
    }
}
