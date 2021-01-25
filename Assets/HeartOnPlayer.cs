using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartOnPlayer : MonoBehaviour
{
    Animator animator;
    AudioSource audio;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();
    }
    public void Shake()
    {
        animator.SetTrigger("shake");
        audio.Play();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
