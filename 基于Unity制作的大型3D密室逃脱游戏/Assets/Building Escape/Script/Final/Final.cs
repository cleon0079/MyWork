using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Final : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        Controller player = FindObjectOfType<Controller>();

        this.transform.position = new Vector3(player.transform.position.x, 0, player.transform.position.z);
    }
}
