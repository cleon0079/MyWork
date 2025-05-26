using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move : MonoBehaviour
{
    [SerializeField] Mesh red;
    [SerializeField] Mesh blue;
    [SerializeField] Mesh green;

    [SerializeField] Material[] redMate = new Material[4];
    [SerializeField] Material[] blueMate = new Material[4];
    [SerializeField] Material[] greenMate = new Material[4];

    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            switch (i)
            {
                case 0:
                    for (int j = 0; j < transform.GetChild(i).childCount; j++)
                    {
                        Transform child = transform.GetChild(i).GetChild(j);
                        child.GetComponent<MeshFilter>().mesh = red;
                        child.GetComponent<MeshRenderer>().materials = redMate;
                        Destroy(child.GetComponent<BoxCollider>());
                        child.gameObject.AddComponent<BoxCollider>();

                        child.localPosition = child.localPosition + new Vector3(-0.35f, 0.24f, 0.2f);
                    }
                    break;

                case 1:
                    for (int j = 0; j < transform.GetChild(i).childCount; j++)
                    {
                        Transform child = transform.GetChild(i).GetChild(j);
                        child.GetComponent<MeshFilter>().mesh = blue;
                        child.GetComponent<MeshRenderer>().materials = blueMate;
                        Destroy(child.GetComponent<BoxCollider>());
                        child.gameObject.AddComponent<BoxCollider>();

                        child.localPosition = child.localPosition + new Vector3(-0.35f, 0.24f, 0.2f);
                    }
                    break;

                case 2:
                    for (int j = 0; j < transform.GetChild(i).childCount; j++)
                    {
                        Transform child = transform.GetChild(i).GetChild(j);
                        child.GetComponent<MeshFilter>().mesh = green;
                        child.GetComponent<MeshRenderer>().materials = greenMate;
                        Destroy(child.GetComponent<BoxCollider>());
                        child.gameObject.AddComponent<BoxCollider>();

                        child.localPosition = child.localPosition + new Vector3(-0.35f, 0.24f, 0.2f);
                    }
                    break;

                default:
                    break;
            }
        }
    }
}
