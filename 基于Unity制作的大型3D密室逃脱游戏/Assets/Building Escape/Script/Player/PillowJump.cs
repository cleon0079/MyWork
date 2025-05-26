using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillowJump : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    private Transform playerPosition;

    private Vector3 dist;
    // Start is called before the first frame update
    void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerPosition = player.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*    private void OnTriggerEnter(Collider other)
        {
            Rigidbody otherRigidbody = other.gameObject.GetComponent<Rigidbody>();
            dist = other.transform.position - transform.position;
            Vector3 direction = dist.normalized;
            otherRigidbody.AddForce(direction * speed);
        }*/
    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody otherRigidbody = collision.gameObject.GetComponent<Rigidbody>();
        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.point.y > transform.position.y)
            {

                Vector3 bounceDirection = Vector3.up;

                otherRigidbody.AddForce(bounceDirection * speed, ForceMode.Impulse);

            }
        }

    }
}
