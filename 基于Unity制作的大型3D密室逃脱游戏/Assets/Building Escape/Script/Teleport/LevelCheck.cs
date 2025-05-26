using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class LevelCheck : MonoBehaviour
{
    Manager uiManager;
    [SerializeField] private GameObject door;
    
    [SerializeField] private GameObject finalDoor;
    

    // private bool lv3open =false;
    // private bool Finalopen =false;
    // Start is called before the first frame update
    private enum DoorState
    {
        allDoorClose,
        levelTwo,
        levelThree,
    }
    private DoorState doorState;
    void Start()
    {
        doorState= DoorState.allDoorClose;

        // finalDoor.GetComponent<MeshRenderer>().enabled=false;
        // finalDoor.gameObject.GetComponent<BoxCollider>().isTrigger=false;
        uiManager = FindObjectOfType<Manager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(uiManager.level2Completed)
        {
           doorState = DoorState.levelTwo;

        }else if(uiManager.level3Completed)
        {
            doorState = DoorState.levelThree;

        }else{
            doorState = DoorState.allDoorClose;
        }
        switch(doorState)
        {
            case DoorState.levelTwo:
                door.gameObject.SetActive(true);
            break;

            case DoorState.levelThree:
                finalDoor.gameObject.SetActive(true);
            break;

            case DoorState.allDoorClose:
                door.gameObject.SetActive(false);
                finalDoor.gameObject.SetActive(false);
            break;
        }
    }
}
