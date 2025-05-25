using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class DragObject : MonoBehaviour
{

    [SerializeField] private AudioClip tableSound;
    private AudioSource audioSource;

    public float rayDistance = 10f;
    public LayerMask draggableLayer;
    [SerializeField] float minHeight = 0.5f;
    float positionSpring = 200f;
    float positionDamper = 20f;
    float rotationSpring = 200f;
    float rotationDamper = 20f;
    float linearLimit = 0.5f;
    float angularLimit = 45f;
    [SerializeField] float minDistance = 1f;
    [SerializeField] float maxDistance = 10f;
    [SerializeField] float scrollSpeed = 1f;
    [SerializeField] float rotationSpeed = 100f;

    private Rigidbody draggedRigidbody;
    public ConfigurableJoint configurableJoint;
    private Transform grabTransform;
    public bool isDragging = false;
    private bool isRotating = false;
    private bool canRotate = false;
    private float currentDistance;

    private GameInput input;
    private InputAction interatAction;
    private InputAction scrollAction;
    private InputAction rotateAction;
    private InputAction deltaAction;
    private bool isRightClicking = false;

    private Controller player;

    [SerializeField] public Material outline;

    private void Awake()
    {
        input = new GameInput();
        interatAction = input.Player.Interart;
        scrollAction = input.Player.Scroll;
        rotateAction = input.Player.Rotate;
        deltaAction = input.Player.Look;

        interatAction.started += OnDrag;
        scrollAction.performed += OnScroll;
        rotateAction.started += OnRightClick;

        player = FindObjectOfType<Controller>();

        grabTransform = new GameObject("Grab Point").transform;
        grabTransform.gameObject.AddComponent<Rigidbody>().isKinematic = true;

        audioSource = GetComponent<AudioSource>();
    }

    private void DragEnable()
    {
        interatAction.Enable();
    }

    private void DragDisable()
    {
        interatAction.Disable();
    }

    void OnRightClick(InputAction.CallbackContext context)
    {
        if (canRotate)
        {
            isRightClicking = !isRightClicking;
        }
        if (isRightClicking)
        {
            player.CanMove(false);
            interatAction.Disable();
        }
        else
        {
            player.CanMove(true);
            scrollAction.Disable();
            rotateAction.Disable();
            deltaAction.Disable();
            interatAction.Enable();
            if (draggedRigidbody!=null)
            {
                if (draggedRigidbody.gameObject.GetComponent<ConfigurableJoint>() == null)
                {
                    AddConfigurableJoint();
                }
            }
            
        }
    }

    private void OnDrag(InputAction.CallbackContext context)
    {
        if (isDragging)
        {
            StopDrag();
        }
        else
        {
            TryStartDrag();
        }
    }

    void OnScroll(InputAction.CallbackContext context)
    {
        float scroll = context.ReadValue<Vector2>().y;
        AdjustGrabDistance(scroll);
    }

    void Update()
    {

        if (isDragging && draggedRigidbody != null)
        {
            DragTheObject();

            if (isRightClicking)
            {
                Vector2 mouseDelta = deltaAction.ReadValue<Vector2>();
                RotateObject(mouseDelta);
            }
        }
    }

    public void CanDrag(bool drag)
    {
        if (drag)
        {
            DragEnable();
        }
        else
        {
            DragDisable();
        }
    }

    void TryStartDrag()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance, draggableLayer))
        {
            if (hit.transform.CompareTag("BlackBoard"))
            {
                if (hit.transform.GetComponent<BlackBoardItem>().GetPuzzleIn())
                {
                    hit.transform.parent = null;
                    hit.transform.GetComponent<BlackBoardItem>().SetPuzzleIn(false);
                    hit.transform.DOKill();
                    GameObject[] blackboards = FindObjectOfType<BlackBoardPuzzleCheck>().GetTriggerList();
                    FindObjectOfType<BlackBoardPuzzleCheck>().SetTrigger(false);
                    for (int i = 0; i < blackboards.Length; i++)
                    {
                        blackboards[i].GetComponent<BlackBoardPuzzle>().SetCount(true);
                    }
                }
            }
            if (hit.transform.CompareTag("BrokenDegree"))
            {
                if (hit.transform.GetComponent<DegreeItem>().GetPuzzleIn())
                {
                    hit.transform.parent = null;
                    hit.transform.GetComponent<DegreeItem>().SetPuzzleIn(false);
                    hit.transform.DOKill();
                    GameObject[] blackboards = FindObjectOfType<Degreepuzzlechecker>().GetTriggerList();
                    FindObjectOfType<Degreepuzzlechecker>().SetTrigger(false);
                    for (int i = 0; i < blackboards.Length; i++)
                    {
                        blackboards[i].GetComponent<DegreePuzzle2>().SetCount(true);
                    }
                }
            }

            if (hit.rigidbody == null)
            {
                hit.transform.GetComponent<MeshCollider>().convex = true;
                hit.transform.gameObject.AddComponent<Rigidbody>();
                hit.rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
            }

            draggedRigidbody = hit.rigidbody;

            if (draggedRigidbody != null)
            {
                grabTransform.position = hit.point;
                grabTransform.parent = null;

                currentDistance = Vector3.Distance(grabTransform.position, Camera.main.transform.position);

                AddConfigurableJoint();

                isDragging = true;
                canRotate = false;
            }

            if ((hit.transform.CompareTag("Table")||hit.transform.CompareTag("Chair") )&& tableSound != null)
            {
                audioSource.clip = tableSound;
                float volume = 0.05f;
                audioSource.volume = volume;
                audioSource.Play();
                audioSource.loop = true;
            }
        }
    }

    void DragTheObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        Vector3 targetPoint;
        RaycastHit hit;
        int layerMask = ~LayerMask.GetMask("LevelThree");
        if (Physics.Raycast(ray, out hit, rayDistance, layerMask))
        {
            if (((1 << hit.collider.gameObject.layer) & draggableLayer) != 0)
            {
                return;
            }
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.origin + ray.direction * currentDistance;
        }

        targetPoint.y = Mathf.Max(targetPoint.y, minHeight);

        grabTransform.position = targetPoint;

        if (Vector3.Distance(grabTransform.position, draggedRigidbody.position) < draggedRigidbody.gameObject.GetComponent<Collider>().bounds.extents.magnitude)
        {
            canRotate = true;
            if (!isRightClicking)
            {
                scrollAction.Enable();
                rotateAction.Enable();
                deltaAction.Enable();
            }
        }
    }

    public void StopDrag()
    {
        if (draggedRigidbody != null)
        {
            isDragging = false;
            isRightClicking = false;
            Destroy(draggedRigidbody.gameObject.GetComponent<ConfigurableJoint>());

            configurableJoint = null;
            draggedRigidbody.useGravity = true;
            draggedRigidbody = null;

            canRotate = false;
            
            player.CanMove(true);

            if (audioSource.isPlaying && audioSource.clip == tableSound)
            {
                audioSource.Stop();
                audioSource.loop = false;
            }
        }
    }

    void AdjustGrabDistance(float scroll)
    {
        if (isDragging && draggedRigidbody != null)
        {
            Vector3 direction = (grabTransform.position - Camera.main.transform.position).normalized;
            currentDistance = Mathf.Clamp(currentDistance + scroll * scrollSpeed, minDistance, maxDistance);

            grabTransform.position = Camera.main.transform.position + direction * currentDistance;
        }
    }

    void RotateObject(Vector2 mouseDelta)
    {
        if (draggedRigidbody == null) return;

        Vector3 rotationAxis = Camera.main.transform.up * -mouseDelta.x + Camera.main.transform.right * mouseDelta.y;

        if (rotationAxis != Vector3.zero)
        {
            if (!isRotating)
            {
                RemoveConfigurableJoint();
                isRotating = true;
            }

            draggedRigidbody.transform.RotateAround(draggedRigidbody.position, rotationAxis, rotationSpeed * Time.deltaTime);
        }
        else if (isRotating)
        {
            isRotating = false;
            if (isDragging)
            {
                AddConfigurableJoint();
            }
        }
    }

    void AddConfigurableJoint()
    {
        if (draggedRigidbody == null) return;

        configurableJoint = draggedRigidbody.gameObject.AddComponent<ConfigurableJoint>();
        configurableJoint.connectedBody = grabTransform.GetComponent<Rigidbody>();

        SoftJointLimit linearSoftLimit = new SoftJointLimit { limit = linearLimit };
        configurableJoint.linearLimit = linearSoftLimit;

        SoftJointLimitSpring angularLimitSpring = new SoftJointLimitSpring
        {
            spring = rotationSpring,
            damper = rotationDamper
        };
        configurableJoint.angularXLimitSpring = angularLimitSpring;
        configurableJoint.angularYZLimitSpring = angularLimitSpring;

        SoftJointLimit highAngularXLimit = new SoftJointLimit { limit = angularLimit };
        SoftJointLimit lowAngularXLimit = new SoftJointLimit { limit = -angularLimit };
        SoftJointLimit angularYZLimit = new SoftJointLimit { limit = angularLimit };
        configurableJoint.highAngularXLimit = highAngularXLimit;
        configurableJoint.lowAngularXLimit = lowAngularXLimit;
        configurableJoint.angularYLimit = angularYZLimit;
        configurableJoint.angularZLimit = angularYZLimit;

        JointDrive positionDrive = new JointDrive
        {
            positionSpring = positionSpring,
            positionDamper = positionDamper,
            maximumForce = float.MaxValue
        };
        configurableJoint.xDrive = positionDrive;
        configurableJoint.yDrive = positionDrive;
        configurableJoint.zDrive = positionDrive;

        JointDrive rotationDrive = new JointDrive
        {
            positionSpring = rotationSpring,
            positionDamper = rotationDamper,
            maximumForce = float.MaxValue
        };
        configurableJoint.angularXDrive = rotationDrive;
        configurableJoint.angularYZDrive = rotationDrive;

        draggedRigidbody.useGravity = false;
    }

    void RemoveConfigurableJoint()
    {
        if (draggedRigidbody == null) return;
        Destroy(configurableJoint);
    }
}