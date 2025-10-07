# SolitaireDemo
This is a simple implementation of Klondike Solitaire (classic patience) built in Unity 6.0 as a 2D demo project. It features a draggable card system, deck drawing, column stacking with basic rules validation, a trash/waste pile, and an undo mechanism.

Features

Deck Management: Shuffle and draw from a 52-card deck using ScriptableObjects for card configs (rank, suit, sprite).
Drag & Drop: Click and drag cards to columns or trash; uses Physics2D colliders for precise drop detection.
Column Rules: Basic validation for alternating colors and descending ranks (expandable).
Trash/Waste Pile: Discard invalid moves; supports reshuffling if needed.
Undo System: Interface-based (IUndoable) for reverting last moves across Deck, Columns, and Trash.

Known Issues

Sometimes cards may appear in the wrong sorting order
