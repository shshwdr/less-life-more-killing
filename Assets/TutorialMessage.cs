using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMessage : MonoBehaviour
{
    public GameObject beforeKill;
    public GameObject afterKill;
    // Start is called before the first frame update
    void Start()
    {
        beforeKill.SetActive(true);
        afterKill.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //move this to a delegate..
        if (GameManager.enemyCount <= 0)
        {

            beforeKill.SetActive(false);
            afterKill.SetActive(true);
        }
    }
}
