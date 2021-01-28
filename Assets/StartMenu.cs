using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {

        Time.timeScale = 1;
        GameManager.instance.isGameOver = false;
        Completed.SoundManager.instance.PlayMainGame();
        var gm = GameManager.instance;
        if (gm)
        {
            gm.level = 0;
            GameManager.playerHealth = 3;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
            GameManager.instance.playedTime += 1;
        }
    }
}
