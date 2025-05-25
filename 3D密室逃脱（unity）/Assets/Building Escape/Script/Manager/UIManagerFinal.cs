using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;


public class UIManagerFinal : MonoBehaviour
{
    // Singleton
    // static private UIManager instance;
    // static public UIManager Instance 
    // {
    //     get 
    //     {
    //         if (instance == null) 
    //         {
    //             Debug.LogError("There is not UIManager in the scene.");
    //         }            
    //         return instance;
    //     }
    // }
    // Manager manager;
    [SerializeField] private GameObject startPanel;
    // [SerializeField] private GameObject controlPanel;
    [SerializeField] private TMP_Text startText;
    [SerializeField] private Button startButton;
    [SerializeField] private Button exitButton;
    private GameInput inputs;
    private InputAction exitKey;

    // PaperPuzzlePlayerIn paperPuzzlePlayerIn;
    // Inventories inventories;
    void Awake() 
    {

        // if (instance != null)
        // {
        //     // there is already a UIManager in the scene, destory this one
        //     Destroy(gameObject);
        // }
        // else
        // {
        //     instance = this;
        //     DontDestroyOnLoad(gameObject);
        // }
       inputs = new GameInput();
       exitKey = inputs.UiAction.ESC;
       exitKey.started += SetMenuPanelOn;
    //    exitKey.canceled += SetStartpanelOff();
    }
    // private void OnEnable()
    // {
    //     exitKey.Enable(); 
    // }
    // private void OnDisable()
    // {
    //     exitKey.Disable();
    // }

    // Start is called before the first frame update
    void Start()
    {
        exitKey.Enable();
        // manager = FindObjectOfType<Manager>();
        // manager.StartGame(true);
        // startPanel.SetActive(true);
        // controlPanel.SetActive(false);
        startButton.onClick.AddListener(SetStartpanelOff);
        exitButton.onClick.AddListener(ExitGame);
        // paperPuzzlePlayerIn = FindAnyObjectByType<PaperPuzzlePlayerIn>();
        // inventories = FindAnyObjectByType<Inventories>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void SetStartpanelOff()
    {   
        Debug.Log("sadasdas");
        startPanel.SetActive(false);
        // exitKey.Enable();
        Time.timeScale = 1f;
        
        // controlPanel.SetActive(true);
        // manager.StartGame(false);
    }

    void SetMenuPanelOn(InputAction.CallbackContext context)
    {
        Time.timeScale = 0f;
        startText.text = "Resume"; 
        if(startPanel.activeSelf){
            SetStartpanelOff();
            Cursor.lockState = CursorLockMode.Locked;
        }else{
            Cursor.lockState = CursorLockMode.None;
            startPanel.SetActive(true);
            // controlPanel.SetActive(false);
            // manager.StartGame(true);
        }
        
    }
    public void EnableEscKey(bool isExit){
        if(isExit){
            exitKey.Enable(); 
        }else{
            exitKey.Disable();  
        }
        
    }
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
