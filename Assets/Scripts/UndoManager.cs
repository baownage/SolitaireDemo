using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class UndoManager : MonoBehaviour
{
    private List<Move> _moves;

    [SerializeField] private List<GameObject> UndoableGameObjects;
    [SerializeField] private Button UndoButton;

    public static UndoManager Instance;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _moves = new();
    }

    void Start()
    {
        UndoButton.onClick.AddListener(Undo);
    }

    public void AddMove(List<Card> cards, Move.PreviousLocation previousLocation, bool flipped = false)
    {
        var newMove = new Move();
        newMove.PrevLocation = previousLocation;
        newMove.Cards = cards;
        newMove.CardOnTopFaceDown = flipped;

        _moves.Add(newMove);
    }

    public void Undo()
    {
        if (_moves.Count == 0) return;

        var lastMove = _moves[^1];

        foreach (var undoable in UndoableGameObjects)
        {
            undoable.GetComponent<IUndoable>().Undo(lastMove.Cards, lastMove.PrevLocation, lastMove.CardOnTopFaceDown);
        }

        _moves.RemoveAt(_moves.Count - 1);
    }

    public void Clear()
    {
        _moves.Clear();
    }

    [Button]
    private void AutoCollect()
    {
        UndoableGameObjects = new();
        MonoBehaviour[] allMbs = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
        foreach (var mb in allMbs)
        {
            if (mb is IUndoable)
            {
                UndoableGameObjects.Add(mb.gameObject);
            }
        }
        Debug.Log($"Auto-collected {UndoableGameObjects.Count} IUndoable instances.");
    }


    public class Move
    {
        public PreviousLocation PrevLocation;
        public List<Card> Cards;
        public bool CardOnTopFaceDown;

        public enum PreviousLocation
        {
            Undefined = 0,
            Deck = 1,
            DeckStandby = 2,
            Column0 = 3,
            Column1 = 4,
            Column2 = 5,
            Column3 = 6,
            Column4 = 7,
            Column5 = 8,
            Column6 = 9,
            Trash = 10
        };

    }

}
