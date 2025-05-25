using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotoPuzzle : MonoBehaviour
{
    private Collider[] colliders;
    private Collider player;

    private bool isInArea = false;
    private bool isDone = false;
    private int puzzleNum = 0;

    Manager uimanager;
    // Start is called before the first frame update
    void Start()
    {
        uimanager = uimanager = FindObjectOfType<Manager>();

        player = FindObjectOfType<Controller>().gameObject.GetComponent<Collider>();

        puzzleNum = this.transform.childCount - 5;
        colliders = new Collider[this.transform.childCount];

        for (int i = 0; i < this.transform.childCount; i++)
        {
            colliders[i] = this.transform.GetChild(i).GetComponent<Collider>();
        }

        for (int i = 0; i < this.transform.childCount; i++)
        {
            if (i < puzzleNum)
            {
                for (int j = i + 1; j < puzzleNum; j++)
                {
                    Physics.IgnoreCollision(colliders[i], colliders[j]);
                }
            }
            else
            {
                Physics.IgnoreCollision(colliders[i], player);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isInArea && !isDone)
        {
            for (int i = 0; i < puzzleNum; i++)
            {
                for (int j = i + 1; j < puzzleNum; j++)
                {
                    Transform child1 = this.transform.GetChild(i);
                    Transform child2 = this.transform.GetChild(j);

                    Rigidbody rb1 = child1.GetComponent<Rigidbody>();
                    Rigidbody rb2 = child2.GetComponent<Rigidbody>();
                    if (rb1.velocity.magnitude < 0.01f && rb2.velocity.magnitude < 0.01f)
                    {
                        float distanceX = child1.localPosition.x - child2.localPosition.x;

                        if (Mathf.Abs(distanceX) < 4)
                        {
                            if (child1.localPosition.x >= 0 && child2.localPosition.x >= 0)
                            {
                                Vector3 pushDirection = (child1.localPosition - child2.localPosition).normalized;
                                pushDirection.z = 0;
                                pushDirection.y = 0;

                                child1.GetComponent<PhotoPuzzleItem>().Push(pushDirection);
                                child2.GetComponent<PhotoPuzzleItem>().Push(-pushDirection);
                            }
                            else if (child1.localPosition.x <= 0 && child2.localPosition.x <= 0)
                            {
                                Vector3 pushDirection = (child1.localPosition - child2.localPosition).normalized;
                                pushDirection.z = 0;
                                pushDirection.y = 0;

                                child1.GetComponent<PhotoPuzzleItem>().Push(pushDirection);
                                child2.GetComponent<PhotoPuzzleItem>().Push(-pushDirection);
                            }
                            else if (child1.localPosition.x >= 0 && child2.localPosition.x <= 0)
                            {
                                Vector3 pushDirection = (child1.localPosition - child2.localPosition).normalized;
                                pushDirection.z = 0;
                                pushDirection.y = 0;

                                child1.GetComponent<PhotoPuzzleItem>().Push(pushDirection);
                                child2.GetComponent<PhotoPuzzleItem>().Push(-pushDirection);
                            }
                            else
                            {
                                Vector3 pushDirection = (child1.localPosition - child2.localPosition).normalized;
                                pushDirection.z = 0;
                                pushDirection.y = 0;

                                child1.GetComponent<PhotoPuzzleItem>().Push(pushDirection);
                                child2.GetComponent<PhotoPuzzleItem>().Push(-pushDirection);
                            }
                            return;
                        }
                    }
                }
            }

            isDone = true;
            for (int i = 0; i < puzzleNum - 1; i++)
            {
                if (this.transform.GetChild(i).transform.localPosition.x >= this.transform.GetChild(i + 1).transform.localPosition.x - 4)
                {
                    isDone = false;
                    break;
                }
            }

        }

        if (isDone)
        {
            Debug.Log(1);
            Physics.IgnoreCollision(colliders[this.transform.childCount - 1], player);
            Physics.IgnoreCollision(colliders[this.transform.childCount - 2], player);
            Physics.IgnoreCollision(colliders[this.transform.childCount - 3], player);

            Debug.Log("lv3PhotoFameFinish");
            uimanager.lv3PhotoFameFinish=true;
            // uimanager.CheckLevelCompleted();
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            isInArea = true;
            Physics.IgnoreCollision(colliders[this.transform.childCount - 1], player, false);
            Physics.IgnoreCollision(colliders[this.transform.childCount - 2], player, false);
            Physics.IgnoreCollision(colliders[this.transform.childCount - 3], player, false);
        }
    }
}
