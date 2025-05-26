using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperPuzzleBlock : MonoBehaviour
{
    [SerializeField] private Collider[] colliders;
    private Collider player;

    private void Start()
    {
        player = FindObjectOfType<Controller>().gameObject.GetComponent<Collider>();
        colliders = new Collider[this.transform.childCount];

        for (int i = 0; i < this.transform.childCount; i++)
        {
            colliders[i] = this.transform.GetChild(i).GetComponent<Collider>();

            Physics.IgnoreCollision(colliders[i], player);
        }
    }
}
