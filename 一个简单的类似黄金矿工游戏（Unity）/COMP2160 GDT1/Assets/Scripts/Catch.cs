using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Catch : MonoBehaviour
{
    // Actions
    private PlayerAction actions;
    private InputAction catchAction;

    // Enum State for catch
    private CatchState catchState; 

    // Get the ship script
    private PlayerMovement ship;

    // Speed for the hook
    [Header("Speed for the hook")]
    [SerializeField] private float speed = 3;
    [SerializeField] private float pullBackSpeed = 6;

    // If we have any fish on the hook and a arraylist to hold the fish we have
    private bool isFish = false;
    private ArrayList catchFish = new ArrayList();

    // If we can catch fish or not and the countdown for the catch function
    private bool canCatch = true;
    [Header("Can catch count down")]
    [SerializeField] private float countDown = 0.5f;
    private float time;

    // Bool to check if the player is dead of not
    private bool isDead = false;

    private void Awake()
    {
        actions = new PlayerAction();
        catchAction = actions.Movement.Catch;
        catchAction.performed += OnCatchPerformed;
    }

    private void OnEnable()
    {
        catchAction.Enable();
    }

    private void OnDisable()
    {
        catchAction.Disable();
    }

    /// <summary>
    /// Change state when press the button
    /// </summary>
    private void OnCatchPerformed(InputAction.CallbackContext context)
    {
        if (catchState == CatchState.Nothing || catchState == CatchState.Pulling)
        {
            catchState = CatchState.Catching;
        }
        else if (catchState == CatchState.Catching)
        {
            catchState = CatchState.Pulling;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Set the default state and set the ship to its script
        catchState = CatchState.Nothing;
        ship = this.gameObject.GetComponentInParent<PlayerMovement>();

        // CountDown default time
        time = countDown;
    }

    // Update is called once per frame
    void Update()
    {
        // If the player is not dead then can catch
        if (!isDead)
        {
            CatchingSwitchCase();
            CountDownForCatch();
        }
    }

    /// <summary>
    /// Count down for the catch function
    /// </summary>
    private void CountDownForCatch() 
    {
        time -= Time.deltaTime;
        if (time < 0)
        {
            canCatch = true;
        }
    }

    /// <summary>
    /// Do diffent thing in diffent catch state.
    /// Nothing: If its nothing then the ship can move.
    /// Catching: If its catching, the ship cant move and start catching.
    /// Pulling: If its pulling, the ship cant move and start pulling, When it go back to the ship, then go back to nothing state.
    /// </summary>
    private void CatchingSwitchCase()
    {
        switch (catchState)
        {
            case CatchState.Nothing:
                ship.isCatching = false;
                break;
            case CatchState.Catching:
                Catching();
                ship.isCatching = true;
                break;
            case CatchState.Pulling:
                pullBack();
                if (this.transform.position.y >= ship.transform.position.y)
                {
                    catchState = CatchState.Nothing;
                }
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Move the hook down to start catching
    /// </summary>
    private void Catching() 
    {
        transform.Translate(Vector2.down * speed * Time.deltaTime, Space.Self);
    }

    /// <summary>
    /// Move the hook back up to the ship
    /// </summary>
    private void pullBack() 
    {
        if (this.transform.position.y < ship.transform.position.y)
        {
            transform.Translate(Vector2.up * pullBackSpeed * Time.deltaTime, Space.Self);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Get the layer number
        int collisionLayer = collision.gameObject.layer;

        // If hit a rock/floor or a monster the the hook goes back to the player and we cant catch fish in a wihle
        if (collisionLayer == LayerMask.NameToLayer("Floor") || collisionLayer == LayerMask.NameToLayer("Monster"))
        {
            catchState = CatchState.Pulling;
            time = countDown;
            canCatch = false;

            // If there is any fish on the hook, then release the fish and clear the arraylist
            if (isFish)
            {
                foreach (Fish fish in catchFish)
                {
                    fish.Release();
                }
                catchFish.Clear();
                isFish = false;
            }
        }

        // If the hook hit the fish, then we catch it and keep it on the hook
        if (collisionLayer == LayerMask.NameToLayer("Fish") && canCatch)
        {
            Fish fish = collision.gameObject.GetComponent<Fish>();
            fish.hook = this.gameObject;
            fish.GetCatch();
            isFish = true;
            catchFish.Add(fish);
        }
    }

    /// <summary>
    /// Player is dead then hook cant move
    /// </summary>
    public void IsDead()
    {
        isDead = true;
    }

    /// <summary>
    /// Restart the game then reset all the value to defalut
    /// </summary>
    public void Restart()
    {
        catchState = CatchState.Nothing;
        isDead = false;
        isFish = false;
        catchFish.Clear();
        canCatch = true;
        time = countDown;
    }

    /// <summary>
    /// If we have catch the fish to the ship then clear the arraylist and set the is fish to false
    /// </summary>
    public void ClearFish()     
    {
        catchFish.Clear();
        isFish = false;
    }

    /// <summary>
    /// Count the point we got from the catch
    /// </summary>
    /// <returns>The Sum of all the fish point</returns>
    public int CountFishPoint()
    {
        int current = 0;
        foreach (Fish fish in catchFish)
        {
            current += fish.GetPoint();
        }
        return current;
    }

    /// <summary>
    /// States for the hook
    /// </summary>
    private enum CatchState
    { 
        Nothing,
        Catching,
        Pulling
    }
}
