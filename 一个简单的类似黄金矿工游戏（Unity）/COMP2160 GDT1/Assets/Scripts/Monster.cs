using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [Header("Fish Speed")]
    [SerializeField] private float speed = 3;

    // Monster State
    private MonsterState monsterState = MonsterState.Left;
    private MonsterState defaultState;

    // SpriteRenderer
    private SpriteRenderer sr;

    // To get the World border
    private float sizeOfMonster;
    private float worldLeftPos;
    private float worldRightPos;

    // Check the monster is on the right face
    private bool onLeft = true;

    // Raycast distance
    private float sightDistance = 1000f;
    [Header("Raycast Detail")]
    [SerializeField] private Color sightColor = Color.red;
    private Color defaultColor;
    [SerializeField] private GameObject player;

    [Header("Attack Detail")]
    [SerializeField] private float attackSpeed = 10;
    [SerializeField] private float fallSpeed = 5;
    [SerializeField] private float attackTimer = 3;
    private Vector3 attackPosition;
    private float time;
    private Vector3 defaultPosition;

    // Check if the player is dead
    private bool isDead;
    // The very start position
    private Vector3 startPosition;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sizeOfMonster = sr.size.x / 2;
        defaultColor = sr.color;

        // Get the world positon from the camera
        worldLeftPos = Camera.main.ViewportToWorldPoint(Vector2.zero).x;
        worldRightPos = Camera.main.ViewportToWorldPoint(Vector2.one).x;

        time = attackTimer;
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // If the player is not dead then can move and look at player and attack
        if (!isDead)
        {
            Move();
            GetRightState();
            IsOnSight();
        }
    }

    /// <summary>
    /// If the monster has see the player or not
    /// </summary>
    private void IsOnSight()
    {
        if (monsterState != MonsterState.Attack && monsterState != MonsterState.Fall)
        {
            if (PlayerInSight() || (PlayerInSightWhenLook() && monsterState == MonsterState.Look))
            {
                monsterState = MonsterState.Look;
            }
            else
            {
                Return();
            }
        }

    }

    /// <summary>
    /// Get the right state for the monster to move after looking at player or attack and fall
    /// </summary>
    private void GetRightState()
    {
        if (onLeft)
        {
            defaultState = MonsterState.Left;
        }
        else
        {
            defaultState = MonsterState.Right;
        }
    }

    /// <summary>
    /// Shot a raycast to check if the player is in the the attack range
    /// </summary>
    /// <returns>The player is in sight or not</returns>
    private bool PlayerInSight()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector3.up, sightDistance);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Floor"))
                {
                    return false;
                }
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Shot a raycast that rotate with the monster to check if the player is in sight
    /// </summary>
    /// <returns>The player is in sight or not</returns>
    private bool PlayerInSightWhenLook() 
    {
        Vector3 rayDirection = player.transform.position - transform.position;

        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, rayDirection, sightDistance);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Floor"))
                {
                    return false;
                }
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// If the player is in sight then look at the player and rotate with it
    /// </summary>
    private void LookAtPlayer()
    {
        Vector3 direction = player.transform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(Vector3.forward, direction);
        transform.rotation = rotation;

        sr.color = sightColor;
    }

    /// <summary>
    /// If the monster is no longer looking at the player then back to nomal and move on
    /// </summary>
    private void Return()
    {
        if (monsterState == MonsterState.Look || monsterState == MonsterState.Fall)
        {
            transform.rotation = Quaternion.identity;
            sr.color = defaultColor;
            monsterState = defaultState;
            time = attackTimer;
        }
    }

    /// <summary>
    /// Count down to check if the monster can attack or not
    /// </summary>
    private void CheckForAttack()
    {
        defaultPosition = transform.position;
        time -= Time.deltaTime;
        if (time < 0)
        {
            Vector3 dir = player.transform.position - transform.position;
            Vector3 normalDir = dir.normalized;
            attackPosition = normalDir;
            monsterState = MonsterState.Attack;
            time = attackTimer;
        }
    }

    /// <summary>
    /// Attack the player by its position
    /// </summary>
    /// <param name="attackPosition">The players dir the seceon attack</param>
    private void Attack(Vector3 attackPosition)
    {
        transform.Translate(attackPosition * attackSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Fall when finish the attack
    /// </summary>
    private void Fall()
    {
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
        if (transform.position.y <= defaultPosition.y)
        {
            Return();
        }
    }

    /// <summary>
    /// Got hit by the player or the watersuface
    /// </summary>
    public void IsHit()
    {
        monsterState = MonsterState.Fall;
    }

    /// <summary>
    /// Moving for the monster and switch case for the state
    /// </summary>
    private void Move()
    {
        switch (monsterState)
        {
            case MonsterState.Left:
                transform.Translate(Vector2.left * speed * Time.deltaTime);
                if (transform.position.x < worldLeftPos + sizeOfMonster)
                {
                    monsterState = MonsterState.Right;
                    onLeft = false;
                }
                break;
            case MonsterState.Right:
                transform.Translate(Vector2.right * speed * Time.deltaTime);
                if (transform.position.x > worldRightPos - sizeOfMonster)
                {
                    monsterState = MonsterState.Left;
                    onLeft = true;
                }
                break;
            case MonsterState.Look:
                LookAtPlayer();
                CheckForAttack();
                break;
            case MonsterState.Attack:
                Attack(attackPosition);
                break;
            case MonsterState.Fall:
                Fall();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Tell monster player is dead
    /// </summary>
    public void IsDead()
    {
        isDead = true;
    }

    /// <summary>
    /// Restart then reset all the value to default
    /// </summary>
    public void Restart()
    {
        transform.position = startPosition;
        monsterState = MonsterState.Left;
        sr.color = defaultColor;
        isDead = false;
    }

    private enum MonsterState
    { 
        Left,
        Right,
        Look,
        Attack,
        Fall
    }
}
