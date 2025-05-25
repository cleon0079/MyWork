using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // Actions
    private PlayerAction actions;
    private InputAction moveAction;

    // SpriteRenderer
    private SpriteRenderer sr;

    // Speed of the Ship
    [Header("Speed for the Ship")]
    [SerializeField] private float speed = 3;

    // To get the World border
    private float sizeOfShip;
    private float worldLeftPos;
    private float worldRightPos;

    // Check if it is catching
    [HideInInspector] public bool isCatching = false;

    // Get the hook
    private Catch hook;

    // The Game Manager
    [Header("Game Manager")]
    [SerializeField] private PointManager pointManager;

    // Player Dead detail
    private bool isDead;
    [Header("Player Dead Detail")]
    [SerializeField] private float rotationSpeed = 90;
    [SerializeField] private float sinkSpeed = 2;
    private Vector3 startPosition;

    private void Awake()
    {
        actions = new PlayerAction();
        moveAction = actions.Movement.Move;
    }

    private void OnEnable()
    {
        moveAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
    }


    // Start is called before the first frame update
    void Start()
    {
        // Get the component SpriteRenderer
        sr = GetComponent<SpriteRenderer>();
        hook = GetComponentInChildren<Catch>();

        // Get the world positon from the camera
        worldLeftPos = Camera.main.ViewportToWorldPoint(Vector2.zero).x;
        worldRightPos = Camera.main.ViewportToWorldPoint(Vector2.one).x;

        // Get the half size of the ship 
        sizeOfShip = sr.size.x/2;

        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            // If the ship is in the screen and also not catching then can move
            if (isInScreen(worldLeftPos, worldRightPos, sizeOfShip) && !isCatching)
            {
                move();
            }

            // If the ship is out of the border then move it back
            offLeftScreen(worldLeftPos, sizeOfShip);
            offRightScreen(worldRightPos, sizeOfShip);
        }
        else
        {
            // If the player Dead then sink the ship
            Sink();
        }
    }

    /// <summary>
    /// Return true if the ship is in the screen, if not then return false
    /// </summary>
    /// <param name="left">Left border fo the camera</param>
    /// <param name="right">Right border fo the camera</param>
    /// <param name="size">Haft size of the ship</param>
    /// <returns></returns>
    private bool isInScreen(float left, float right, float size) 
    {
        float currentPosition = transform.position.x;
        if (left + size <= currentPosition && right - size>= currentPosition)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// If the Ship is off the left Screen then move it back to the screen
    /// </summary>
    /// <param name="left">Left border fo the camera</param>
    /// <param name="size">Haft size of the ship</param>
    private void offLeftScreen(float left, float size) 
    {
        Vector3 currentPosition = transform.position;
        if (currentPosition.x < left + size)
        {
            transform.position = new Vector3(left + size, currentPosition.y, currentPosition.z);
        }
    }

    /// <summary>
    /// If the ship is off the right screen then move it back to the screen
    /// </summary>
    /// <param name="right">Right border of the camera</param>
    /// <param name="size">Haf size of the ship</param>
    private void offRightScreen(float right, float size)
    {
        Vector3 currentPosition = transform.position;
        if (currentPosition.x > right - size)
        {
            transform.position = new Vector3(right - size, currentPosition.y, currentPosition.z);
        }
    }

    /// <summary>
    /// Move the ship code
    /// </summary>
    private void move() 
    {
        float acceleration = moveAction.ReadValue<Vector2>().x;
        transform.Translate(Vector2.right * speed * acceleration * Time.deltaTime, Space.Self);
    }


    /// <summary>
    /// If the player dead then keep the hook with the player and tell hook player is dead
    /// </summary>
    public void IsDead()
    {
        isDead = true;
        hook.transform.position = transform.position;
        hook.IsDead();
    }


    /// <summary>
    /// Sink when player is dead, rotate the ship and go below the screen
    /// </summary>
    private void Sink()
    {
        if (transform.position.y >= Camera.main.ViewportToWorldPoint(Vector2.zero).y - sr.size.y)
        {
            float rotationAmount = rotationSpeed * Time.deltaTime;
            transform.Rotate(Vector3.forward, rotationAmount, Space.Self);

            float sinkAmount = sinkSpeed * Time.deltaTime;
            transform.Translate(Vector3.down * sinkAmount, Space.World);

        }
    }


    /// <summary>
    /// If the player press the restart button then set all the value to default
    /// </summary>
    public void Restart()
    {
        isDead = false;
        transform.rotation = Quaternion.identity;
        transform.position = startPosition;
        hook.transform.position = transform.position;
        hook.Restart();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        int collisionLayer = collision.gameObject.layer;
        if (collisionLayer == LayerMask.NameToLayer("Fish"))
        {
            // Destroy the fish gameobject if it is a fish we catch and clear the hook

            pointManager.IncreaseScore(hook.CountFishPoint());
            hook.ClearFish();
            Destroy(collision.gameObject);
        }

        if (collisionLayer == LayerMask.NameToLayer("Monster"))
        {
            // Lose 1 hp if the monster hit the player
            Monster monster = collision.gameObject.GetComponent<Monster>();
            pointManager.playerHealth--;
            monster.IsHit();
        }
    }
}
