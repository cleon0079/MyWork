using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public enum Index
    {
        BlackBoardPuzzle,
        TablePuzzle,
        BookPuzzle,
        Prize,
        DegreePuzzle,
        TablePuzzlePrize,
        BookPuzzlePrize

    }

    [SerializeField] Sprite image;
    [SerializeField] Index type;
    [SerializeField] bool isRight;

    public Sprite Image { get { return image; } set { image = value; } }
    public Index Type { get { return type; } set { type = value; } }
    public bool IsRight { get { return isRight; } set { isRight = value; } }

    public Item() {
        
    }
}