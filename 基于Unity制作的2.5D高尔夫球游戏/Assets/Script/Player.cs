using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private float Speed = 3f;
    [SerializeField] private float turningRate = 30f;
    private float walkSpeed;
    private float stop = 0;

    [SerializeField] private Transform ball;
    [SerializeField] private Rigidbody ballRigid;
    [SerializeField] private float grabRadius = 0.2f;
    [SerializeField] private float offset = 0.8f;
    [SerializeField] private Vector3 offsetVector = new Vector3(0, 1, 0);
    private bool isGrab = false;

    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float timeBetweenPoints = 1f;
    [SerializeField] private int linePoints = 10;

    [SerializeField] private float maxKickHeight = 0.8f;
    [SerializeField] private float maxKickWidth = 0.3f;
    [SerializeField] private float maxKickNegativeWidth = -0.3f;
    private int kickOffset = 100;
    private Vector3 initialImpulseDir;
    private Vector3 impulseDir;

    [SerializeField] private float impulseForce = 1f;


    [SerializeField] private float boostRate = 2f;
    private float boost = 1;

    private PlayerActions actions;
    private InputAction movementAction;
    private InputAction turboAction;
    private InputAction grabAction;
    private Animator animator;

    private GameManager gameManager;

    private void Awake()
    {
        actions = new PlayerActions();
        movementAction = actions.Move.Movment;
        movementAction.performed += OnWalk;
        movementAction.canceled += EndWalk;
        turboAction = actions.Move.Turbo;
        turboAction.performed += OnTurbo;
        turboAction.canceled += EndTurbo;
        grabAction = actions.Move.Grab;
        grabAction.performed += OnGrab;
        grabAction.canceled += EndGrab;
        grabAction.started += StartGrab;
    }

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        walkSpeed = Speed;
        animator = GetComponent<Animator>();
        lineRenderer.positionCount = linePoints;
        lineRenderer.enabled = false;
    }

    private void OnEnable()
    {
        movementAction.Enable();
        turboAction.Enable();
        grabAction.Enable();
    }

    private void OnDisable()
    {
        movementAction.Disable();
    }

    public void Goal()
    {
        animator.SetBool("IsGoal", true);
    }

    public void Reset()
    {
        animator.SetBool("IsGoal", false);
    }

    void StartGrab(InputAction.CallbackContext context)
    {
        impulseDir = (ballRigid.position - transform.position).normalized;
        initialImpulseDir = impulseDir;
    }
    void OnGrab(InputAction.CallbackContext context)
    {
        float distance = Vector3.Distance(ball.position, transform.position);
        if (distance <= grabRadius)
        {
            isGrab = true;
            animator.SetBool("IsGrab", true);
        }
        walkSpeed = stop;
    }

    void EndGrab(InputAction.CallbackContext context)
    {
        animator.SetBool("IsGrab", false);


        if (isGrab)
        {
            ballRigid.isKinematic = false;
            ballRigid.AddForce(impulseDir * impulseForce, ForceMode.Impulse);
            gameManager.ParAdd();
        }

        isGrab = false;
        walkSpeed = Speed;
        impulseDir = Vector3.zero;
        initialImpulseDir = Vector3.zero;
    }

    void OnWalk(InputAction.CallbackContext context)
    {
        animator.SetBool("IsWalk", true);
    }

    void EndWalk(InputAction.CallbackContext context)
    {
        animator.SetBool("IsWalk", false);
    }

    void OnTurbo(InputAction.CallbackContext context)
    {
        boost = boostRate;
        animator.SetBool("IsRun", true);
    }

    void EndTurbo(InputAction.CallbackContext context)
    {
        boost = 1;
        animator.SetBool("IsRun", false);
    }

    void Update()
    {


        if (isGrab)
        {
            Vector3 rotation = Quaternion.LookRotation(ball.position - transform.position).eulerAngles;
            rotation.x = 0f;
            transform.rotation = Quaternion.Euler(rotation);

            ballRigid.isKinematic = isGrab;
            ball.position = transform.position + transform.forward * offset + offsetVector;
            MoveBallTrajectory();

        }
        else if (!isGrab)
        {
            ballRigid.isKinematic = isGrab;
            PlayerMovement();
            lineRenderer.enabled = false;
        }

    }

    private void DisplayTrajectory()
    {
        lineRenderer.enabled = true;

        Vector3 startPosition = this.transform.position;
        Vector3 startVelocity = (impulseDir).normalized * impulseForce;

        for (int i = 0; i < linePoints; i++)
        {
            float time = i * timeBetweenPoints;
            Vector3 point = CalculateTrajectoryPoint(startPosition, startVelocity, time);
            lineRenderer.SetPosition(i, point);
        }
    }

    private Vector3 CalculateTrajectoryPoint(Vector3 start, Vector3 velocity, float time)
    {
        return start + velocity * time + 0.5f * Physics.gravity * time * time;
    }

    private void MoveBallTrajectory()
    {
        float acceleration = movementAction.ReadValue<Vector2>().y;
        float Xacceleration = movementAction.ReadValue<Vector2>().x;
        float offSet = 0.5f;

        if (impulseDir.y <= maxKickHeight)
        {
            impulseDir.y += acceleration/kickOffset;
        } else
        {
            impulseDir.y = 0.8f;
        }

        float impulseCheck = impulseDir.x - initialImpulseDir.x;
        float max = initialImpulseDir.x + maxKickWidth;
        float min = initialImpulseDir.x + maxKickNegativeWidth;

        if (max < 1 && min > 0)
        {
            if (impulseDir.x <= max && impulseDir.x >= min)
            {

                impulseDir.x = (Quaternion.Euler(0, Xacceleration, 0) * impulseDir).x;
            }
            else if (impulseDir.x > max && impulseDir.x <= max + offSet)
            {
                Xacceleration = Mathf.Clamp(Xacceleration, -1, 0);
                impulseDir.x = (Quaternion.Euler(0, Xacceleration, 0) * impulseDir).x;
            }
            else if (impulseDir.x < min && impulseDir.x >= min - offSet)
            {
                Xacceleration = Mathf.Clamp(Xacceleration, 0, 1);
                impulseDir.x = (Quaternion.Euler(0, Xacceleration, 0) * impulseDir).x;
            }
        }



        lineRenderer.enabled = false;
        DisplayTrajectory();
    }

    private void PlayerMovement()
    {
        float acceleration = movementAction.ReadValue<Vector2>().y;
        float Xacceleration = movementAction.ReadValue<Vector2>().x;
        transform.Translate((Vector3.forward / 2 + Vector3.right / 2) * walkSpeed * boost * acceleration * Time.deltaTime, Space.World);
        transform.Translate(-(Vector3.forward / 2 + Vector3.left / 2) * walkSpeed * boost * Xacceleration * Time.deltaTime, Space.World);
        if (acceleration > 0)
        {
            Quaternion toRotation = Quaternion.LookRotation((Vector3.forward / 2 + Vector3.right / 2), Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, turningRate * Time.deltaTime);
        }
        else if (acceleration < 0)
        {
            Quaternion toRotation = Quaternion.LookRotation(-(Vector3.forward / 2 + Vector3.right / 2), Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, turningRate * Time.deltaTime);
        }

        if (Xacceleration < 0)
        {
            Quaternion toRotation = Quaternion.LookRotation((Vector3.forward / 2 + Vector3.left / 2), Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, turningRate * Time.deltaTime);
        }
        else if (Xacceleration > 0)
        {
            Quaternion toRotation = Quaternion.LookRotation(-(Vector3.forward / 2 + Vector3.left / 2), Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, turningRate * Time.deltaTime);
        }
    }
}