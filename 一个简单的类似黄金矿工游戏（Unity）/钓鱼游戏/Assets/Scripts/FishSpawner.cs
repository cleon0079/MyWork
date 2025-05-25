using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    [Header("Where the spawn is")]
    [SerializeField] private SpawnSpace spawnspace = SpawnSpace.Right;

    [Header("Put the fish here")]
    [SerializeField] private Fish[] fish;

    [Header("Top and bottom game object")]
    [SerializeField] private GameObject top;
    [SerializeField] private GameObject bottom;

    [Header("Max and min spawn time")]
    [Range(6, 10)] [SerializeField] private float maxSpawnTime;
    [Range(3, 5)] [SerializeField] private float minSpawnTime;
    private float time;

    [Header("Fish Speed")]
    [Range(5, 7)] [SerializeField] private float maxFishSpeed;
    [Range(3, 5)] [SerializeField] private float minFishSpeed;

    // Check if the player is dead
    private bool isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        // Set the spawn time for the frist fish
        time = Random.Range(minSpawnTime, maxSpawnTime);
    }

    // Update is called once per frame
    void Update()
    {
        // If the player is not dead then start spawn the fish
        if (!isDead)
        {
            CountDown();
        }
    }

    /// <summary>
    /// Destroy all the fish if the player is dead
    /// </summary>
    public void IsDead()
    {
        isDead = true;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform childTransform = transform.GetChild(i);
            GameObject childGameObject = childTransform.gameObject;
            Destroy(childGameObject);
        }
    }

    /// <summary>
    /// Restart to spawn fish
    /// </summary>
    public void Restart()
    {
        isDead = false;
    }

    /// <summary>
    /// CountDown to spawn fish
    /// </summary>
    private void CountDown() 
    {
        time -= Time.deltaTime;
        if (time <= 0)
        {
            Spawn();
            time = Random.Range(minSpawnTime, maxSpawnTime);
        }
    }

    /// <summary>
    /// Spawn the fish
    /// </summary>
    private void Spawn()
    {
        // Spawn random fish from the list
        int randomFish = Random.Range(0, fish.Length);
        Fish newFish = Instantiate(fish[randomFish]);

        // Give the fish a position, where between top gameobject and bottom gameobject
        newFish.transform.parent = this.transform;
        newFish.transform.position = new Vector3(this.transform.position.x, 
            Random.Range(bottom.transform.position.y, top.transform.position.y), 
            this.transform.position.z);

        // Give the fish a random speed
        newFish.SetFish(Random.Range(minFishSpeed, maxFishSpeed), randomFish + 1);

        // Flip the fish if it is on the wrong way and give the fish witch way they are
        SpriteRenderer newFishsr = newFish.GetComponent<SpriteRenderer>();
        if (spawnspace == SpawnSpace.Right)
        {
            newFishsr.flipX = true;
            newFish.RightFish();
        }
        else
        {
            newFishsr.flipX = false;
            newFish.LeftFish();
        }
    }

    private enum SpawnSpace
    { 
        Left,
        Right
    }
}
