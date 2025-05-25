using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    [Header("Top and bottom game object")]
    // The line begain gameobject
    [SerializeField] private GameObject begainObject;
    // The line end gameobject
    [SerializeField] private GameObject endObject;


    private LineRenderer line;
    private SpriteRenderer sr;

    // Size of the hook
    private float sizeY;

    // Start is called before the first frame update
    void Start()
    {
        line = GetComponent<LineRenderer>();
        sr = endObject.GetComponent<SpriteRenderer>();
        // Haft of the size by y for the hook
        sizeY = sr.size.y/2;
    }

    // Update is called once per frame
    void Update()
    {
        LinkLine();
    }

    /// <summary>
    /// Draw a line from the ship to the hook's end part
    /// </summary>
    private void LinkLine() 
    {
        line.SetPosition(0, begainObject.transform.position);
        line.SetPosition(1,new Vector3(endObject.transform.position.x, endObject.transform.position.y + sizeY, endObject.transform.position.z));
    }
}
