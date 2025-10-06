using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering;
using DG.Tweening;

public class Card : MonoBehaviour
{
    [SerializeField] private Rank Rank;
    [SerializeField] private Suit Suit;
    [SerializeField] private SortingGroup SortingGroup;
    [SerializeField] private SpriteRenderer FaceSpriteRenderer;
    [SerializeField] private CardConfig Config;
    [SerializeField] private GameObject BackFace;
    [SerializeField] private GameObject FrontFace;
    [SerializeField] private Collider2D Collider;

    [SerializeField] private float CardResetSpeed = 3f;

    private Vector3 _startPosition;

    private bool _isBeingDragged;

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
        transform.DOKill();
    }

    private void ReleaseCard()
    {
        _isBeingDragged = false;

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
}
