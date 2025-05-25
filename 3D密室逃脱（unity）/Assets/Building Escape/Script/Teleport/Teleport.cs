using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Teleport : MonoBehaviour
{

    [SerializeField] Level level;
    [SerializeField] Transform[] levels = new Transform[3];
    [SerializeField] GameObject[] postProcessing = new GameObject[2];

   
    [SerializeField] Animator animator;

    private void Start()
    {
        for (int i = 0; i < postProcessing.Length; i++)
        {
            postProcessing[i] = GameObject.Find("PostProcessing").transform.GetChild(i).gameObject;
        }
        postProcessing[1].SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            animator.SetBool("FadeIn", true);
            animator.SetBool("FadeOut", false);
            

            StartCoroutine(WaitForComplete(other));
            /*
            switch (level)
            {
                case Level.LevelOne:
                    other.transform.position = levels[0].transform.position;
                    other.transform.rotation = levels[0].transform.rotation;
                    postProcessing[0].SetActive(false);
                    postProcessing[1].SetActive(true);
                    break;
                case Level.LevelTwo:
                    other.transform.position = levels[1].transform.position;
                    postProcessing[0].SetActive(false);
                    postProcessing[1].SetActive(true);
                    break;
                case Level.LevelThree:
                    other.transform.position = levels[2].transform.position;
                    other.transform.rotation = Quaternion.identity;
                    postProcessing[0].SetActive(true);
                    postProcessing[1].SetActive(false);
                    break;
                default:
                    break;
            
            }
            */


        }
    }


        private IEnumerator WaitForComplete(Collider other)
    {
        yield return new WaitForSeconds(1);

        switch (level)
        {
            case Level.LevelOne:
                other.transform.position = levels[0].transform.position;
                other.transform.rotation = levels[0].transform.rotation;
                postProcessing[0].SetActive(false);
                postProcessing[1].SetActive(true);

                break;
            case Level.LevelTwo:
                other.transform.position = levels[1].transform.position;
                postProcessing[0].SetActive(false);
                postProcessing[1].SetActive(true);
                break;
            case Level.LevelThree:
                other.transform.position = levels[2].transform.position;
                other.transform.rotation = Quaternion.identity;
                postProcessing[0].SetActive(true);
                postProcessing[1].SetActive(false);
  
                break;
            case Level.Final:

                SceneManager.LoadScene(1);
                break;
            default:
                break;

        }
        animator.SetBool("FadeIn", false);
        animator.SetBool("FadeOut", true);

    }

    public enum Level
    { 
        LevelOne,
        LevelTwo,
        LevelThree,
        Final
    }
}
