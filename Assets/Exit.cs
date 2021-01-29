using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Exit : MonoBehaviour
{
    public bool isOpened;
    SpriteRenderer render;
    public Sprite openedDoor;
    public Sprite closedDoor;
    Collider2D collider;
    AudioSource audioSource;
    public AudioClip doorOpen;
    // Start is called before the first frame update
    void Start()
    {
        render = GetComponent<SpriteRenderer>();
        render.sprite = closedDoor;
        collider = GetComponent<Collider2D>();
        audioSource = GetComponent<AudioSource>();
        GetComponentInChildren<Light2D>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        //move this to a delegate..
        if (GameManager.enemyCount <= 0 &&!isOpened)
        {
            OpenDoor();
        }
    }

    void OpenDoor()
    {
        isOpened = true;
        render.sprite = openedDoor;
        collider.isTrigger = true;
        audioSource.PlayOneShot(doorOpen);
        GetComponentInChildren<Light2D>().enabled = true;
    }
}
