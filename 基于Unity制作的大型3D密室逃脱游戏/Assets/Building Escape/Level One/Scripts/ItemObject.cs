using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public Item item;
    public int ID;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PickUp()
    {
        //gameObject.SetActive(false);
        GetComponent<Collider>().enabled = false;
        GetComponent<Renderer>().enabled = false;
    }

}
