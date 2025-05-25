using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotoPuzzleItem : MonoBehaviour
{
    private float power;
    Rigidbody rigidbody;


    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        power = 4f;
    }

    private void Update()
    {
        if (this.transform.localPosition.z != 0)
        {
            this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, 0);
        }
    }

    public void Push(Vector3 direction) {
        rigidbody.AddForce(transform.TransformDirection(direction) * power, ForceMode.Impulse);
    }
}
