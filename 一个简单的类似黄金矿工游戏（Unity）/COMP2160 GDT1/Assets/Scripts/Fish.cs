using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    [HideInInspector] public GameObject hook;

    // Speed for the fish
    private float speed;
    // Muliter for the fish to see howmany point it shound *
    private int multier;

    [Header("Up and down distance and speed")]
    [SerializeField] private float upDownDistance = 0.5f;
    [SerializeField] private float upDownSpeed = 1;
    private Vector3 fristPosion;

    private FishFace fishFace = FishFace.Left;
    private FishFace defaltFace;
    private FishUpDownState fishUpDownState = FishUpDownState.Up;

    private SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {
        // Get the position when the fish spawn
        fristPosion = transform.position;

        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // Move the fish
        FishUpDown();
        FishMove();
        // Destroy the fish if its out the border
        DestroyFish();
    }

    /// <summary>
    /// If the fish is out of the border then destroy the fish
    /// </summary>
    private void DestroyFish()
    {
        float borderRight = Camera.main.ViewportToWorldPoint(Vector2.one).x + sr.size.x / 2;
        float borderLeft = Camera.main.ViewportToWorldPoint(Vector2.zero).x - sr.size.x / 2;
        switch (fishFace)
        {
            case FishFace.Left:
                if (transform.position.x > borderRight)
                {
                    Destroy(this);
                }
                break;
            case FishFace.Right:
                if (transform.position.x < borderLeft)
                {
                    Destroy(this);
                }
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Move the fish up and down when the fish is between the distance
    /// </summary>
    private void FishUpDown()
    {
        
        if (fishFace != FishFace.Catch)
        {
            switch (fishUpDownState)
            {
                case FishUpDownState.Up:
                    transform.Translate(Vector2.up * upDownSpeed * Time.deltaTime);
                    break;
                case FishUpDownState.Down:
                    transform.Translate(Vector2.down * upDownSpeed * Time.deltaTime);
                    break;
                default:
                    break;
            }
            if (transform.position.y > fristPosion.y + upDownDistance)
            {
                fishUpDownState = FishUpDownState.Down;
            }
            else if (transform.position.y < fristPosion.y - upDownDistance)
            {
                fishUpDownState = FishUpDownState.Up;
            }
        }
    }

    /// <summary>
    /// Move the fish depend on the spawn space of the fish, if got catch then follow the hook
    /// </summary>
    private void FishMove()
    {
        
        switch (fishFace)
        {
            case FishFace.Left:
                transform.Translate(Vector2.right * speed * Time.deltaTime);
                break;
            case FishFace.Right:
                transform.Translate(Vector2.left * speed * Time.deltaTime);
                break;
            case FishFace.Catch:
                transform.position = hook.transform.position;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// If the fish is on the right side then give the fish its state
    /// </summary>
    public void RightFish() 
    {
        fishFace = FishFace.Right;
        defaltFace = FishFace.Right;
    }

    /// <summary>
    /// If the fish is on the left side then give the fish its state
    /// </summary>
    public void LeftFish()
    {
        fishFace = FishFace.Left;
        defaltFace = FishFace.Left;
    }

    /// <summary>
    /// If the fish get catch the give the fish its state
    /// </summary>
    public void GetCatch()
    {
        fishFace = FishFace.Catch;
    }

    /// <summary>
    /// If the fish releace then put the state back to its defalt state and keep swimming
    /// And set the y position at the postion right now for up and down move
    /// </summary>
    public void Release()
    {
        fishFace = defaltFace;
        fristPosion = transform.position;
    }


    /// <summary>
    /// Set the speed and the muiltipler for the fish
    /// </summary>
    /// <param name="speed">Speed of the fish</param>
    /// <param name="multi">Muiltipler of the fish</param>
    public void SetFish(float speed, int multi)
    {
        this.speed = speed;
        this.multier = multi;
    }


    /// <summary>
    /// Get the point for this one fish
    /// </summary>
    /// <returns>The point for this fish</returns>
    public int GetPoint()
    {
        return Mathf.RoundToInt(speed) * multier;
    }

    private enum FishFace
    { 
        Left,
        Right,
        Catch
    }

    private enum FishUpDownState
    { 
        Up,
        Down
    }
}
