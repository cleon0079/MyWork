using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BlackBoardPuzzleCheck : MonoBehaviour
{
    [SerializeField] Transform targetPosition;
    [SerializeField] GameObject prizeGB;
    GameObject[] blackboards;
    Manager uimanager;
    int index = 0;

    public bool isFinish;

    bool canTrigger = true;

    private void Start()
    {
        uimanager = FindObjectOfType<Manager>();
        isFinish = false;
        blackboards = new GameObject[this.transform.childCount];
        for (int i = 0; i < this.transform.childCount; i++)
        {
            blackboards[i] = this.transform.GetChild(i).GetChild(0).gameObject;
            blackboards[i].GetComponent<BlackBoardPuzzle>().SetIndex(i + 1);
        }
    }

    private void Update()
    {
        if (!isFinish)
        {
            Check();
        }
    }

    public void Check() {
        index = 0;
        for (int i = 0; i < blackboards.Length; i++)
        {
            if (blackboards[i].GetComponent<BlackBoardPuzzle>().GetPuzzle())
            {
                index++;
            }
        }
        if (index == blackboards.Length)
        {
            Finish();
        }
    }
   
    void Finish() {
        isFinish = true;
        // prizeGB.SetActive(true);
        // prizeGB.transform.DOMove(targetPosition.position, 2f);
        // prizeGB.transform.DORotate(Vector3.zero, 2f);
        uimanager.blackboardFinish = true;
        // Destroy(this);
    }

    public bool GetTrigger() {
        return canTrigger;
    }

    public void SetTrigger(bool trigger) {
        this.canTrigger = trigger;
    }

    public GameObject[] GetTriggerList() {
        return blackboards;
    }
}
