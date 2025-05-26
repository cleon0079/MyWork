using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float yIndex = 8.55f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.layer == 6)
        {
            player.transform.position = new Vector3(0,yIndex,player.position.z);
            collider.transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }
}
