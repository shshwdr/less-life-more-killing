﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

using System.Collections.Generic;       //Allows us to use Lists. 
using UnityEngine.UI;                   //Allows us to use UI.
                                        //namespace Completed
using Completed;

public class GameManager : MonoBehaviour
{
    public float levelStartDelay = 2f;                      //Time to wait before starting level, in seconds.
    public float turnDelay = 0.1f;                          //Delay between each Player turn.
    public int playerFoodPoints = 100;                      //Starting value for Player food points.
    public static GameManager instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.
    [HideInInspector] public bool playersTurn = true;       //Boolean to check if it's players turn, hidden in inspector but public.


    private Text levelText;                                 //Text to display current level number.
    private GameObject levelImage;                          //Image to block out level as levels are being set up, background for levelText.
    private BoardManager boardScript;                       //Store a reference to our BoardManager which will set up the level.
    public int level = 1;                                   //Current level number, expressed in game as "Day 1".
    private List<Enemy> enemies;                            //List of all Enemy units, used to issue them move commands.
    private bool enemiesMoving;                             //Boolean to check if enemies are moving.
    private bool doingSetup = true;                         //Boolean to check if we're setting up board, prevent Player from moving during setup.

    
    public static int enemyCount;
    public static bool gameStarted = false;
    public static int playerHealth = 3;

    GameObject restartMenu;
    Text restartText;
    public bool wouldDie = false;
    public bool isGameOver = false;

    public int playedTime = 0;
    public Dictionary<int, bool> DiedBefore;
    public bool hasPressedKey = false;
    //Awake is always called before any Start functions
    void Awake()
    {
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);

        //Assign enemies to a new List of Enemy objects.
        enemies = new List<Enemy>();

        //Get a component reference to the attached BoardManager script
        boardScript = GetComponent<BoardManager>();

        //Call the InitGame function to initialize the first level 
        InitGame();
        DiedBefore = new Dictionary<int, bool>();
    }

    //this is called only once, and the paramter tell it to be called only after the scene was loaded
    //(otherwise, our Scene Load callback would be called the very first load, and we don't want that)
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static public void CallbackInitialization()
    {
        //register the callback to be called everytime the scene is loaded
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    //This is called each time a scene is loaded.
    static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        instance.level++;
        instance.InitGame();
    }


    //Initializes the game for each level.
    void InitGame()
    {
        hasPressedKey = false;
           //While doingSetup is true the player can't move, prevent player from moving while title card is up.
           doingSetup = true;
        gameStarted = false;
        enemyCount = 1;
        //Get a reference to our image LevelImage by finding it by name.
        levelImage = GameObject.Find("LevelImage");

        //Get a reference to our text LevelText's text component by finding it by name and calling GetComponent.
        levelText = GameObject.Find("LevelText").GetComponent<Text>();

        //Set the text of levelText to the string "Day" and append the current level number.
        levelText.text = "Level " + level;
        restartMenu = GameObject.Find("Restart");

        restartText = GameObject.Find("RestartText").GetComponent<Text>();

        //Set levelImage to active blocking player's view of the game board during setup.
        levelImage.SetActive(true);
        restartMenu.SetActive(false);

        //Call the HideLevelImage function with a delay in seconds of levelStartDelay.
        Invoke("HideLevelImage", levelStartDelay);

        //Clear any Enemy objects in our List to prepare for next level.
        enemies.Clear();

        //Call the SetupScene function of the BoardManager script, pass it current level number.
        boardScript.StopAllCoroutines();
        boardScript.SetupScene(level);

    }


    //Hides black image used between levels
    void HideLevelImage()
    {
        //Disable the levelImage gameObject.
        levelImage.SetActive(false);

        //Set doingSetup to false allowing player to move again.
        doingSetup = false;
    }

    //Update is called every frame.
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            //wouldDie = !wouldDie;
        }
    }

    //Call this to add the passed in Enemy to the List of Enemy objects.
    public void AddEnemyToList(Enemy script)
    {
        //Add Enemy to List enemies.
        enemies.Add(script);
    }


    //GameOver is called when the player reaches 0 food points
    public void GameOver()
    {
        StopAllCoroutines();
        SoundManager.instance.PlayGameOver();
        //Set levelText to display number of levels passed and game over message
        restartText.text = "After " + level + " days, you died.";

        //Enable black background image gameObject.
        restartMenu.SetActive(true);

        //Disable this GameManager.
        enabled = false;
        Time.timeScale = 0;
        isGameOver = true;
        DiedBefore[level] = true;
    }

}


