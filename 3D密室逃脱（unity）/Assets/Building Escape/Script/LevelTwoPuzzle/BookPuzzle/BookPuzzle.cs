using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BookPuzzle : MonoBehaviour
{
    [SerializeField] private AudioClip cabinetSound;
    [SerializeField] private AudioClip bookSound;
    private AudioSource audioSource;

    Manager uiManager;
    [SerializeField] string showedText = "Press F to interact";
    [SerializeField] GameObject target;

    private GameInput input;
    private InputAction interatAction;
    private InputAction escAction;
    private InputAction mousePosition;

    [SerializeField] LayerMask bookLayer;

    [SerializeField] List<GameObject> levelObjects;

    bool isPuzzling = false;
    bool isBookCliked = false;  
    bool isDragging = false;
    float startMousePosition;

    Transform book;

    Animator animator;
    [SerializeField] GameObject prize;
    bool isDone = false;
    private UIManager uIManager2;



    private void Awake()
    {
        input = new GameInput();
        interatAction = input.Player.Interart;
        escAction = input.Player.Esc;
        mousePosition = input.Player.MousePosition;

        interatAction.started += OnStart;
        audioSource = GetComponent<AudioSource>();
    }

    void OnStart(InputAction.CallbackContext context)
    {
        uiManager.StartPuzzle();

        uiManager.mainCamera.transform.SetParent(target.transform);
        uiManager.mainCamera.transform.DOLocalMove(Vector3.zero, 1f);
        uiManager.mainCamera.transform.DOLocalRotate(Vector3.zero, 1f);

        uiManager.ShowDot(false);
        uiManager.UpdateText("");

        uiManager.CursorMode(true);
        interatAction.started -= OnStart;
        interatAction.started += OnClick;
        interatAction.canceled += OnNoClick;
        escAction.started += OnEsc;

        mousePosition.Enable();
        escAction.Enable();
        
        isPuzzling = true;

    }

    void OnClick(InputAction.CallbackContext context)
    {
        Ray ray = uiManager.mainCamera.ScreenPointToRay(mousePosition.ReadValue<Vector2>());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10, bookLayer))
        {
            isDragging = true;
            book = hit.transform;
            startMousePosition = mousePosition.ReadValue<Vector2>().x;

            if (hit.transform.CompareTag("Book") && bookSound != null && audioSource != null)
            {
                float volume = 0.09f;
                audioSource.PlayOneShot(bookSound, volume);
                isBookCliked = true;
            }
        }
    }

    void OnNoClick(InputAction.CallbackContext context)
    {
        isDragging = false;

        if (audioSource != null && !audioSource.isPlaying && bookSound != null && isBookCliked) 
        {
            audioSource.PlayOneShot(bookSound, 0.09f);
            isBookCliked = false;
        }
        else if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
            isBookCliked = false;
        }

        CheckPuzzle();
    }

    void OnEsc(InputAction.CallbackContext context)
    {
        EscPuzzle();
    }

    private void Start()
    {
        uiManager = FindObjectOfType<Manager>();
        animator = GetComponentInParent<Animator>();
        uIManager2 = FindAnyObjectByType<UIManager>();

    }

    private void FixedUpdate()
    {
        if (isDragging)
        {
            float currentMousePosition = mousePosition.ReadValue<Vector2>().x;
            float deltaMousePosition = currentMousePosition - startMousePosition;

            float moveAmount = deltaMousePosition * Time.fixedDeltaTime * .05f;

            if (book.localPosition.x <= -1.8f && moveAmount < 0)
            {
                moveAmount = 0;
            }
            else if (book.localPosition.x >= 1.8f && moveAmount > 0)
            {
                moveAmount = 0;
            }
            book.localPosition += new Vector3(moveAmount, 0, 0);

            startMousePosition = currentMousePosition;
        }
    }

    void CheckPuzzle()
    {
        int i = 0;
        for (int k = 0; k < levelObjects.Count; k++)
        {
            if (levelObjects[k].transform.GetChild(i).localPosition.x < levelObjects[k].transform.GetChild(i + 1).localPosition.x)
            {
                if (levelObjects[k].transform.GetChild(i + 1).localPosition.x < levelObjects[k].transform.GetChild(i + 2).localPosition.x)
                {
                    if (k == levelObjects.Count - 1)
                    {
                        PuzzleCompleted();
                    }
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }
    }

    void EscPuzzle()
    {
        isDragging = false;

        uiManager.EndPuzzle();

        uiManager.mainCamera.transform.SetParent(FindObjectOfType<Controller>().transform);
        uiManager.mainCamera.transform.DOLocalMove(new Vector3(0, 1, 0), 1f);
        uiManager.mainCamera.transform.DOLocalRotate(Vector3.zero, 1f);

        uiManager.ShowDot(true);
        uiManager.CursorMode(false);

        interatAction.started -= OnClick;
        interatAction.canceled -= OnNoClick;
        escAction.started -= OnEsc;

        interatAction.started += OnStart;
        uiManager.UpdateText(showedText);

        interatAction.Disable();
        escAction.Disable();
        mousePosition.Disable();

        isPuzzling = false;

        
    }

    private void PuzzleCompleted()
    {
        isDone = true;

        EscPuzzle();
        interatAction.started -= OnStart;

        animator.SetBool("BookShelfOpen", true);
        uiManager.UpdateText("");

        if (cabinetSound != null && audioSource != null)
        {
            float volume = 0.3f;
            audioSource.PlayOneShot(cabinetSound, volume);
        }
        prize.SetActive(true);
        prize.transform.parent = FindObjectOfType<Controller>().transform.GetChild(0).transform;
        prize.transform.DOLocalMove(Vector3.zero + new Vector3(0, 0, 1), 1f);

        prize.transform.DOLocalRotate(Vector3.zero + new Vector3(180, 0, 0), 1f);
        uiManager.bookShelfFinish = true;
        uIManager2.EnableEscKey(true);

        uiManager.StartPuzzle();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") && !isDone)
        {
            uIManager2.EnableEscKey(false);
            if (!isPuzzling)
            {
                Debug.Log("IF player is inside");
                uiManager.UpdateText(showedText);
            }
            interatAction.Enable();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") && !isDone)
        {
            
            uIManager2.EnableEscKey(true);
            uiManager.UpdateText("");
            interatAction.Disable();
        }
    }
}
