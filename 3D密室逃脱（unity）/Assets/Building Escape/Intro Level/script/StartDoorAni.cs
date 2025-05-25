using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartDoorAni : MonoBehaviour
{
    private Animator animator;
    private bool playerEnter;
    [SerializeField] AudioSource doorOpen;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        playerEnter = false;
    }

    // Update is called once per frame
    void Update()
    {
        PlayAnimation();
    }
    private void PlayAnimation()
    {
        if(playerEnter){
            animator.SetBool("DoorOpen" , true);
            // doorOpen.Play();
        }
        if(!playerEnter){
            animator.SetBool("DoorOpen" , false);
        }
        // if(playerEnter){
        //     animator.SetBool("Enter" , true);
        // }
        // if(!playerEnter){
        //     animator.SetBool("Enter" , false);
        // }
    }

    void OnTriggerEnter (Collider collider){
        
        if(collider.gameObject.layer ==6 ){
            playerEnter = true;
        }
    }

    void OnTriggerExit(Collider collider){
        if(collider.gameObject.layer ==6 ){
            playerEnter = false;
        }
    }
}
