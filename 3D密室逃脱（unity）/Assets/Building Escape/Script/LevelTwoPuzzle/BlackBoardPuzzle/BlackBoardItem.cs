using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackBoardItem : MonoBehaviour
{
    [SerializeField] int id;
    bool isPuzzleIn = false;

    public bool GetPuzzleIn() {
        return isPuzzleIn;
    }

    public void SetPuzzleIn(bool set) {
        this.isPuzzleIn = set;
    }

    public int GetIndex() {
        return id;
    }

    public void CantMove() {
        this.gameObject.layer = LayerMask.NameToLayer("Default");
        Destroy(this.GetComponent<Rigidbody>());
        Destroy(this.GetComponent<MeshCollider>());
    }
}
