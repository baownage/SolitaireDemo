using System.Collections.Generic;
using UnityEngine;

public class TrashHolder : MonoBehaviour, IUndoable
{
    private List<Card> _cards;

    void Awake()
    {
        _cards = new();
    }

    public void AddCard(Card card)
    {
        card.transform.position = transform.position;
        card.SetStartPosition();
        card.SetSortingOrder(_cards.Count);
        _cards.Add(card);
    }

    public void Undo(List<Card> cards, UndoManager.Move.PreviousLocation previousLocation, bool flipped = false)
    {
        if (previousLocation != UndoManager.Move.PreviousLocation.Trash) return;

        foreach (var card in cards)
        {
            AddCard(card);
            card.GetColumn()?.RemoveCard(card);
        }
    }
}
