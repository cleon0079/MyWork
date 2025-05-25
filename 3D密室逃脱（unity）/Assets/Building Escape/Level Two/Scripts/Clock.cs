using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : MonoBehaviour
{
    [SerializeField] private AudioClip clockSound;
    private AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            audioSource.Stop();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            float volume = 0.09f;
            audioSource.volume = volume;
            audioSource.loop = true;
            audioSource.clip = clockSound;
            audioSource.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
