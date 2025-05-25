using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperPuzzleItem : MonoBehaviour
{
    [SerializeField] int id;
    public bool isPuzzleIn = false;
    [SerializeField] TYPE type;

    public bool GetPuzzleIn() {
        return isPuzzleIn;
    }

    public void SetPuzzleIn(bool puzzleIn) {
        isPuzzleIn = puzzleIn;
    }

    public int GetIndex() {
        return id;
    }

    public void SetPuzzleId(int id) {
        this.id = id;
    }

    public TYPE GetItemType() {
        return type;
    }

    public void CantMove()
    {
        this.gameObject.layer = LayerMask.NameToLayer("Default");
        Destroy(this.GetComponent<Rigidbody>());
        Destroy(this.GetComponent<MeshCollider>());
    }

    public enum TYPE {
        One,
        Two,
        Three
    }
}
