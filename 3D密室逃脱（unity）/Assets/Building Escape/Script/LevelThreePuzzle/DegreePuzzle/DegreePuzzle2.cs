using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DegreePuzzle2 : MonoBehaviour
{
    DragObject dragObject;
    Degreepuzzlechecker puzzleCheck;
    public bool isPuzzleIn = false;

    bool startCount = false;
    float timer = 0;

    [SerializeField] int index;
    bool isRight = false;

    // Start is called before the first frame update
    void Start()
    {
        dragObject = FindObjectOfType<DragObject>();
        puzzleCheck = this.transform.parent.parent.GetComponent<Degreepuzzlechecker>();
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

       
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("BrokenDegree") && !isPuzzleIn && puzzleCheck.GetTrigger() && !isRight)
        {
            
            other.transform.SetParent(this.transform.parent);  


            if (this.transform.parent.childCount > 1)
            {
                other.transform.DOLocalMove(Vector3.zero, 1f);
                other.transform.DOLocalRotate(Vector3.zero + new Vector3(-90, 0, -180), 1f);

                dragObject.StopDrag();
                other.GetComponent<Rigidbody>().velocity = Vector3.zero;

                Destroy(other.GetComponent<Rigidbody>());
                other.transform.GetComponent<MeshCollider>().convex = false;

                other.GetComponent<DegreeItem>().SetPuzzleIn(true);
            }
        if (this.transform.parent.childCount > 1)
        {
            isPuzzleIn = true;
            if (this.transform.parent.GetChild(1).GetComponent<DegreeItem>().GetIndex() == index)
            {
                isRight = true;
                Debug.Log("dsasd");
                this.transform.parent.GetChild(1).GetComponent<DegreeItem>().CantMove();
                this.gameObject.SetActive(false);
            }
        }
            Debug.Log("Check1");
            puzzleCheck.Check();
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.transform.CompareTag("BrokenDegree") && !isPuzzleIn && puzzleCheck.GetTrigger() && !isRight)
        {
            other.transform.SetParent(this.transform.parent);

            if (this.transform.parent.childCount > 1)
            {
                other.transform.DOLocalMove(Vector3.zero, 1f);
                other.transform.DOLocalRotate(Vector3.zero + new Vector3(-90, 0, -180), 1f);

                dragObject.StopDrag();
                other.GetComponent<Rigidbody>().velocity = Vector3.zero;

                Destroy(other.GetComponent<Rigidbody>());
                other.transform.GetComponent<MeshCollider>().convex = false;

                other.GetComponent<DegreeItem>().SetPuzzleIn(true);
            }
             if (this.transform.parent.childCount > 1)
        {
            isPuzzleIn = true;
            if (this.transform.parent.GetChild(1).GetComponent<DegreeItem>().GetIndex() == index)
            {
                isRight = true;
                Debug.Log("dsasd");
                this.transform.parent.GetChild(1).GetComponent<DegreeItem>().CantMove();
                this.gameObject.SetActive(false);
            }
        }
            Debug.Log("Check2");
            puzzleCheck.Check();
        }
    }
    // Update is called once per frame
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
