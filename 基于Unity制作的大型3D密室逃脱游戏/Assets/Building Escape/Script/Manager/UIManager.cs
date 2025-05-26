 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;


public class UIManager : MonoBehaviour
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
    Manager manager;
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject controlPanel;
    [SerializeField] private TMP_Text startText;
    [SerializeField] private Button startButton;
    [SerializeField] private Button exitButton;
    private GameInput inputs;
    private InputAction exitKey;
    private InputAction tabKey;
    PaperPuzzlePlayerIn paperPuzzlePlayerIn;
    Inventories inventories;
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
       tabKey = inputs.UiAction.Tab;
       tabKey.started += SetControlpanelOff;
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
        manager = FindObjectOfType<Manager>();
        manager.StartGame(true);
        startPanel.SetActive(true);
        controlPanel.SetActive(false);
        startButton.onClick.AddListener(SetStartpanelOff);
        exitButton.onClick.AddListener(manager.ExitGame);
        paperPuzzlePlayerIn = FindAnyObjectByType<PaperPuzzlePlayerIn>();
        inventories = FindAnyObjectByType<Inventories>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void SetStartpanelOff()
    {   
        // exitKey.Enable();
        Time.timeScale = 1f;
        startPanel.SetActive(false);
        controlPanel.SetActive(true);
        manager.StartGame(false);
        tabKey.Enable();

        manager.CanOpenInventory(true);
    }
    void SetControlpanelOff(InputAction.CallbackContext context){
        
        if(controlPanel.activeSelf){
            controlPanel.SetActive(false);
        }else{
            controlPanel.SetActive(true);
        }
    }
    void SetMenuPanelOn(InputAction.CallbackContext context)
    {
        Time.timeScale = 0f;
        startText.text = "Resume"; 
        if(startPanel.activeSelf){
            SetStartpanelOff();
        }else{
            manager.CanOpenInventory(false);
            startPanel.SetActive(true);
            controlPanel.SetActive(false);
            manager.StartGame(true);
        }
        
    }
    public void EnableEscKey(bool isExit){
        if(isExit){
            exitKey.Enable(); 
        }else{
            exitKey.Disable();  
        }
        
    }

}
