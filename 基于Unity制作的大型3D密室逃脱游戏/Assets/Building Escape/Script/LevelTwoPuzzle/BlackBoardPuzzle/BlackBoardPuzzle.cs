using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackBoardPuzzle : MonoBehaviour
{
    DragObject dragObject;
    BlackBoardPuzzleCheck puzzleCheck;
    bool isPuzzleIn = false;

    bool startCount = false;
    float timer = 0;

    [SerializeField] int index;
    bool isRight = false;

    private void Start()
    {
        dragObject = FindObjectOfType<DragObject>();
        puzzleCheck = this.transform.parent.parent.GetComponent<BlackBoardPuzzleCheck>();
    }

    private void FixedUpdate()
    {
        if (startCount)
        {
            timer += Time.fixedDeltaTime;
            if (timer >= 2f)
            {
                timer = 0;
                startCount = false;
                puzzleCheck.SetTrigger(true);
                isPuzzleIn = false;
            }
        }

        if (this.transform.parent.childCount > 1)
        {
            isPuzzleIn = true;
            if (this.transform.parent.GetChild(1).GetComponent<BlackBoardItem>().GetIndex() == index)
            {
                isRight = true;
                this.transform.parent.GetChild(1).GetComponent<BlackBoardItem>().CantMove();
                this.gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("BlackBoard") && !isPuzzleIn && puzzleCheck.GetTrigger() && !isRight)
        {
            other.transform.SetParent(this.transform.parent);  


            if (this.transform.parent.childCount > 1)
            {
                other.transform.DOLocalMove(Vector3.zero, 1f);
                other.transform.DOLocalRotate(Vector3.zero, 1f);

                dragObject.StopDrag();
                other.GetComponent<Rigidbody>().velocity = Vector3.zero;

                Destroy(other.GetComponent<Rigidbody>());
                other.transform.GetComponent<MeshCollider>().convex = false;

                other.GetComponent<BlackBoardItem>().SetPuzzleIn(true);
            }
            puzzleCheck.Check();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.CompareTag("BlackBoard") && !isPuzzleIn && puzzleCheck.GetTrigger() && !isRight)
        {
            other.transform.SetParent(this.transform.parent);


            if (this.transform.parent.childCount > 1)
            {
                other.transform.DOLocalMove(Vector3.zero, 1f);
                other.transform.DOLocalRotate(Vector3.zero, 1f);

                dragObject.StopDrag();
                other.GetComponent<Rigidbody>().velocity = Vector3.zero;

                Destroy(other.GetComponent<Rigidbody>());
                other.transform.GetComponent<MeshCollider>().convex = false;

                other.GetComponent<BlackBoardItem>().SetPuzzleIn(true);
            }
            puzzleCheck.Check();
        }
    }

    public bool GetPuzzle() {
        return isRight;
    }

    public void SetCount(bool count) {
        this.startCount = count;
    }

    public void SetIndex(int index) {
        this.index = index;
    }
}
