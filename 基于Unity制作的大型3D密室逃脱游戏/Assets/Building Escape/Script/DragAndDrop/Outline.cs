using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outline : MonoBehaviour
{
    DragObject dragObject;
    Renderer renderer;
    bool isInRay = false;
    bool hasAdded = false;

    private void Start()
    {
        dragObject = FindObjectOfType<DragObject>();
        renderer = GetComponent<Renderer>();
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, dragObject.rayDistance, dragObject.draggableLayer))
        {
            if (hit.collider.gameObject == this.gameObject)
            {
                isInRay = true;
            }

        }
                    else
            {
                isInRay = false;
            }

        if (isInRay)
        {
            if (!hasAdded)
            {
                AddOuline();
            }
        }
        else
        {
            if (hasAdded)
            {
                RemoveOutline();
            }
        }

        if (!FindObjectOfType<DragObject>().isDragging)
        {
            if (GetComponent<ConfigurableJoint>() != null)
            {
                Destroy(this.GetComponent<ConfigurableJoint>());
                FindObjectOfType<DragObject>().configurableJoint = null;
            }
        }
    }

    private void AddOuline()
    {

        Material[] newMaterials = new Material[renderer.materials.Length + 1];
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                newMaterials[i] = renderer.materials[i];
            }

            newMaterials[renderer.materials.Length] = dragObject.outline;
            renderer.materials = newMaterials;
        hasAdded = true;
    }

    private void RemoveOutline()
    { 
        Material[] materials = renderer.materials;
        Material[] newMaterials = new Material[materials.Length - 1];
            for (int i = 0; i < materials.Length; i++)
            {
                if (i != materials.Length - 1)
                {
                    newMaterials[i] = materials[i];
                }
            }
            renderer.materials = newMaterials;
        hasAdded = false;
    }
}
