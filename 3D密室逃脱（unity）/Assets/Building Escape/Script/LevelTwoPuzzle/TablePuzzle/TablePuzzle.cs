using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TablePuzzle : MonoBehaviour
{
    DragObject dragObject;
    [SerializeField] bool isChair = false;
    [SerializeField] bool isTable = false;

    [SerializeField] int index;
    bool isRight = false;

    [SerializeField] Transform container;

    void Start()
    {
        dragObject = FindObjectOfType<DragObject>();
    }

    private void OnTriggerEnter(Collider other)
    {
        CheckChair(other);

        if (other.transform.CompareTag("Table") && !isTable && !isRight)
        {
            isTable = true;
            other.transform.SetParent(this.transform);

            other.transform.DOLocalMove(Vector3.zero, 1f);
            other.transform.DOLocalRotate(Vector3.zero, 1f);

            dragObject.StopDrag();
            other.transform.GetComponent<Rigidbody>().velocity = Vector3.zero;


            other.GetComponent<TableItem>().SetTableIn(true);

            if (other.GetComponent<TableItem>().GetIndex() == index)
            {
                isRight = true;
                other.GetComponent<TableItem>().CantMove();
            }

            this.transform.parent.GetComponent<TablePuzzleCheck>().Check();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Chair") && isChair)
        {
            other.transform.SetParent(container);
            other.transform.DOKill();
            isChair = false;
        }

        if (other.transform.CompareTag("Table") && isTable)
        {
            other.transform.SetParent(container);
            other.transform.DOKill();
            isTable = false;
        }
    }

    void CheckChair(Collider other) {
        if (other.transform.CompareTag("Chair") && !isChair)
        {
            isChair = true;
            other.transform.SetParent(this.transform);

            other.transform.DOLocalMove(new Vector3(0, 0, -1.5f), 1f);
            other.transform.DOLocalRotate(Vector3.zero, 1f);

            other.transform.GetComponent<Rigidbody>().velocity = Vector3.zero;

            dragObject.StopDrag();


        }
    }

    public void SetIndex(int index)
    {
        this.index = index;
    }

    public bool GetPuzzle()
    {
        return isRight;
    }
}
