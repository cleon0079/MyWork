using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Manager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI interatText;
    [SerializeField] GameObject middelDot;
    public Camera mainCamera;
    DragObject dragObject;
    Controller player;
    Inventories inventories;
    public bool blackboardFinish = false;
    public bool tablesFinish = false;
    public bool bookShelfFinish = false;
    public bool level2Completed = false;

    public bool lv3DegreeFinish = false;
    public bool lv3PhotoFameFinish = false;
    public bool lv3TableFinish = false;
    public bool level3Completed = false;

    private void Start()
    {
        inventories = FindObjectOfType<Inventories>();
        dragObject = FindObjectOfType<DragObject>();
        player = FindObjectOfType<Controller>();
    }
    void Update()
    {
        CheckLevelCompleted();
        CheckLev3Complete();
    }
    
    public void CanOpenInventory(bool open)
    {
        if (open)
        {
            inventories.BagEnable();
        }
        else
        {
            inventories.BagDisable();
        }
    }
    public void CheckLevelCompleted()   
    {
        if (blackboardFinish && tablesFinish && bookShelfFinish)
        {
            level2Completed = true;
        }

        
    }

    public void CheckLev3Complete()
    {
        Debug.Log("76");
        if (lv3DegreeFinish && lv3PhotoFameFinish && lv3TableFinish)
        {
            Debug.Log("876");
            level3Completed = true;
        }
    }
    public void StartPuzzle()
    {
        dragObject.CanDrag(false);
        player.CanMove(false);

        CanOpenInventory(false);
    }

    public void EndPuzzle() 
    {
        dragObject.CanDrag(true);
        player.CanMove(true);

        CanOpenInventory(true);
    }

    public void ShowDot(bool dot) 
    {
        middelDot.SetActive(dot);
    }

    public void UpdateText(string inText)
    {
        interatText.text = inText;
    }

    public void Inventory(bool inventory) {
        dragObject.CanDrag(inventory);
        player.CanMove(inventory);
    }

    public void CursorMode(bool mode) 
    {
        if (mode)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void StartGame(bool start) {
        if (start)
        {
            Cursor.lockState = CursorLockMode.None;
            dragObject = FindObjectOfType<DragObject>();
            player = FindObjectOfType<Controller>();
            dragObject.CanDrag(false);
            player.CanMove(false);

            
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            dragObject = FindObjectOfType<DragObject>();
            player = FindObjectOfType<Controller>();
            dragObject.CanDrag(true);
            player.CanMove(true);

    
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
