using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PuzzlePrize : MonoBehaviour
{
    [SerializeField] float rotateSpeed = 90;
    float timer;
    bool isDone = false;
    Manager uiManager;
    // Update is called once per frame

    void Update()
    {
        this.transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);

        timer += Time.deltaTime;
        if (timer >= 6f && !isDone)
        {
            timer = 0;
            Tween move = this.transform.DOLocalMove(Vector3.zero + new Vector3(1, -1, 1), 1f);
            move.OnComplete(() => DoneMove());
        }
    }

    void DoneMove() {

        FindObjectOfType<Inventories>().AddItem(GetComponent<ItemObject>().item);
        this.gameObject.SetActive(false);
        FindObjectOfType<Manager>().EndPuzzle();
        isDone = true;
    }
}
