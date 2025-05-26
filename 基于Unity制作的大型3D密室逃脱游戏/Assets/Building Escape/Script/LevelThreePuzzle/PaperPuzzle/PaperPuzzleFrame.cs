using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PaperPuzzleFrame : MonoBehaviour
{
    private bool[] isPieceIn;
    private bool isDone = false;
    private List<PaperPuzzleItem> inFrame = new List<PaperPuzzleItem>();

    private void Start()
    {
        isPieceIn = new bool[9];
        for (int i = 0; i < isPieceIn.Length; i++)
        {
            isPieceIn[i] = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PaperPuzzlePiece") && !isPieceIn[other.transform.GetComponent<PaperPuzzleItem>().GetIndex()])
        {
            other.transform.SetParent(this.transform.GetChild(other.transform.GetComponent<PaperPuzzleItem>().GetIndex()));

            other.transform.localScale = Vector3.one;

            other.transform.DOLocalMove(Vector3.zero, 1f);
            other.transform.DOLocalRotate(Vector3.zero, 1f);

            Destroy(other.GetComponent<Rigidbody>());
            other.transform.GetComponent<MeshCollider>().convex = false;
            other.transform.GetComponent<PaperPuzzleItem>().SetPuzzleIn(true);

            isPieceIn[other.transform.GetComponent<PaperPuzzleItem>().GetIndex()] = true;
            inFrame.Add(other.transform.GetComponent<PaperPuzzleItem>());

            CheckDone();
        }
    }

    public void removePuzzle(PaperPuzzleItem item) {
        inFrame.Remove(item);
        isPieceIn[item.GetIndex()] = false;
    }

    void CheckDone() {
        isDone = IsDone<PaperPuzzleItem>(inFrame);
        if (isDone)
        {
            for (int i = 0; i < this.transform.childCount; i++)
            {
                if (this.transform.GetChild(i).childCount != 0)
                {
                    this.transform.GetChild(i).GetChild(0).GetComponent<PaperPuzzleItem>().CantMove();
                }
            }
            this.transform.parent.parent.GetComponent<PaperPuzzlePlayerIn>().Done();
        }
    }

    bool IsDone<T>(List<PaperPuzzleItem> frameItems) {
        if (frameItems.Count != this.transform.childCount) 
        {
            return false;
        }

        PaperPuzzleItem fristItem = frameItems[0];
        foreach (PaperPuzzleItem item in inFrame)
        {
            if (item.GetItemType() != fristItem.GetItemType())
            {
                return false;
            }
        }

        return true;
    }
}
