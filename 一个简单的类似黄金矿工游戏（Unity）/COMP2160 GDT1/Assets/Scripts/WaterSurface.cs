using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSurface : MonoBehaviour
{
    /// <summary>
    /// If the monster hit the water surface but not the player
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        int collisionLayer = collision.gameObject.layer;

        if (collisionLayer == LayerMask.NameToLayer("Monster"))
        {
            Monster monster = collision.gameObject.GetComponent<Monster>();
            monster.IsHit();
        }
    }
}
