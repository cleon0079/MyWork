using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableItem : MonoBehaviour
{
    [SerializeField] int index;
    bool isTableIn = false;

    public bool GetTableIn() {
        return isTableIn;
    }

    public void SetTableIn(bool set) {
        this.isTableIn = set;
    }

    public int GetIndex() {
        return index;
    }

    public void CantMove() {
        this.gameObject.layer = LayerMask.NameToLayer("Default");
        Destroy(this.GetComponent<Rigidbody>());
    }
}
