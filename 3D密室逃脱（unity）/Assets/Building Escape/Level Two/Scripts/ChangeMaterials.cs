using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMaterials : MonoBehaviour
{   
    [SerializeField] private AudioClip finishSound;
    private AudioSource audioSource;
    BlackBoardPuzzleCheck blackBroad;
    GameObject[] blackBroads;
    [SerializeField] private Material finishMaterial;
    bool isMaterialDone = false;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        blackBroad = FindObjectOfType<BlackBoardPuzzleCheck>();
        blackBroads = GameObject.FindGameObjectsWithTag("BlackBoard");
    }

    // Update is called once per frame
    void Update()
    {

        ChangeMaterial();
        
    }
    void ChangeMaterial()
    {
        if(blackBroad.isFinish && !isMaterialDone)
        {
            foreach(GameObject obj in blackBroads)
            {
                Renderer renderer = obj.GetComponent<Renderer>();
                renderer.material = finishMaterial;
            }
            if(!audioSource.isPlaying){
                audioSource.PlayOneShot(finishSound);
            }
                
            isMaterialDone = true;
        }

    }
}
