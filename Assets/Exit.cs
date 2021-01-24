using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour
{
    public bool isOpened;
    SpriteRenderer render;
    public Sprite openedDoor;
    public Sprite closedDoor;
    Collider2D collider;
    // Start is called before the first frame update
    void Start()
    {
        render = GetComponent<SpriteRenderer>();
        render.sprite = closedDoor;
        collider = GetComponent<Collider2D>();
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
    }
}
