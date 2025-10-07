using System.Collections.Generic;

public interface IUndoable
{
    void Undo(List<Card> cards, UndoManager.Move.PreviousLocation previousLocation, bool flipped = false);
} 
