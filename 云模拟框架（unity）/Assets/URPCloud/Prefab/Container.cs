using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{
    public Vector3 containerSize = new Vector3(500, 50, 500);

    public Color gizmoColor = Color.cyan;

    public CloudRenderPipeline.CloudType cloudType;

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;

        Gizmos.DrawWireCube(transform.position, containerSize);
    }
}
