using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour, IUndoable
{
    [SerializeField] private List<CardColumn> Columns;

    public void AddCardToColumn(int index, Card card, bool faceUp)
    {
        index = Mathf.Clamp(index, 0, Columns.Count - 1);
        Columns[index].AddCard(card, faceUp);
    }

    public void Undo(List<Card> cards, UndoManager.Move.PreviousLocation previousLocation, bool flipped = false)
    {
        if ((int)previousLocation is < 3 or > 9) return;

        int columnIndex = (int)previousLocation - 3;

        if (flipped)
        {
            Columns[columnIndex].FlipLast(false);
        }

        foreach (var card in cards)
        {
            foreach (var column in Columns)
            {
                column.RemoveCard(card);
            }
            
            AddCardToColumn(columnIndex, card, true);
        }
    }
}
