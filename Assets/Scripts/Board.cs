using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private List<CardColumn> Columns;

    void Start()
    {
        
    }

    public void AddCardToColumn(int index, Card card, bool faceUp)
    {
        index = Mathf.Clamp(index, 0, Columns.Count - 1);
        Columns[index].AddCard(card, faceUp);
    }
}
