using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class DegreePuzzle : MonoBehaviour
{
    Animator puzzleAni;
    [SerializeField] string showedText = "Press F to interact";
    private GameInput input;
    private InputAction interatAction;
    private InputAction mousePosition;
    float startMousePosition;
    Manager uiManager;
    [SerializeField] LayerMask degreeLayer;
    Transform book;
    private UIManager uIManager2;
    bool isAnimationPlayed = false;
    public bool canInterat = false;
    public bool isPlayerExit =false;
    bool isAnimationCompleted = false;
    void Awake()
    {
        input = new GameInput();
        interatAction = input.Player.Interart;
        interatAction.started += PlayAnimation;
        
    }
    void PlayAnimation(InputAction.CallbackContext context)
    {
        isAnimationPlayed = true;
        puzzleAni.SetTrigger("Interact");
        uiManager.UpdateText("");
    }
    void OnClick(InputAction.CallbackContext context)
    {
        Ray ray = uiManager.mainCamera.ScreenPointToRay(mousePosition.ReadValue<Vector2>());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10, degreeLayer))
        {
            book = hit.transform;
            startMousePosition = mousePosition.ReadValue<Vector2>().x;
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        uiManager = FindObjectOfType<Manager>();
        puzzleAni = GetComponent<Animator>();
        uIManager2 = FindObjectOfType<UIManager>();
        if (uiManager == null)
        {
            Debug.LogError("Manager not found in the scene.");
        }
        if (uIManager2 == null)
        {
            Debug.LogError("UIManager not found in the scene.");
        }
        if(puzzleAni == null)
        {
            Debug.LogError("puzzleAni not found in the scene.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckAnimation();
    }
    private void CheckAnimation()
    {
        if (isAnimationCompleted) return;

        AnimatorStateInfo stateInfo = puzzleAni.GetCurrentAnimatorStateInfo(0); 
        if (stateInfo.IsName("Fly pos") && stateInfo.normalizedTime >= 1.0f)
        {
            puzzleAni.enabled = false; 
            canInterat = true;
            isAnimationCompleted = true;
            foreach (Transform child in transform)
            {
                if (child.GetComponent<Rigidbody>() == null)
                    {
                        Rigidbody rb = child.gameObject.AddComponent<Rigidbody>();
                    }   
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            interatAction.Enable();
            if (uiManager == null || uIManager2 == null||interatAction == null)
            {
                Debug.LogError("uiManager or uIManager2 are null?");
                return;
            }
            if (!isAnimationPlayed)
            {
                uiManager.UpdateText(showedText);
            }
            uIManager2.EnableEscKey(false);
        }
        
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            interatAction.Disable();
            uIManager2.EnableEscKey(true);
            isPlayerExit = true;
            uiManager.UpdateText("");
            
        }
    }
}
