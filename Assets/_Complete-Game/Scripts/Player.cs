﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;	//Allows us to use UI.
using UnityEngine.SceneManagement;

namespace Completed
{
	//Player inherits from MovingObject, our base class for objects that can move, Enemy also inherits from this.
	public class Player : MonoBehaviour
	{
		public float restartLevelDelay = 1f;		//Delay time in seconds to restart level.
		public int pointsPerFood = 10;				//Number of points to add to player food points when picking up a food object.
		public int pointsPerSoda = 20;				//Number of points to add to player food points when picking up a soda object.
		public int wallDamage = 1;					//How much damage a player does to a wall when chopping it.
		public Text foodText;						//UI Text to display current player food total.
		public AudioClip moveSound1;				//1 of 2 Audio clips to play when player moves.
		public AudioClip moveSound2;				//2 of 2 Audio clips to play when player moves.
		public AudioClip eatSound1;					//1 of 2 Audio clips to play when player collects a food object.
		public AudioClip eatSound2;					//2 of 2 Audio clips to play when player collects a food object.
		public AudioClip drinkSound1;				//1 of 2 Audio clips to play when player collects a soda object.
		public AudioClip drinkSound2;				//2 of 2 Audio clips to play when player collects a soda object.
		public AudioClip gameOverSound;             //Audio clip to play when player dies.

		public float moveSpeed = 5f;
		Rigidbody2D rb;
		Vector2 movement;

		private Animator animator;					//Used to store a reference to the Player's animator component.
		private int food;                           //Used to store player food points total during level.

		private float lastFire;
		public float fireDelay;
		public GameObject bulletPrefab;
		public float bulletSpeed;

#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
        private Vector2 touchOrigin = -Vector2.one;	//Used to store location of screen touch origin for mobile controls.
#endif


		//Start overrides the Start function of MovingObject
		protected void Start ()
		{
			//Get a component reference to the Player's animator component
			animator = GetComponent<Animator>();
			
			//Get the current food point total stored in GameManager.instance between levels.
			food = GameManager.instance.playerFoodPoints;
			
			//Set the foodText to reflect the current player food total.
			foodText.text = "Food: " + food;
			rb = GetComponent<Rigidbody2D>();
		}
		
		
		
		private void Update ()
		{

			//Check if we are running either in the Unity editor or in a standalone build.


			//Get input from the input manager, round it to an integer and store in horizontal to set x axis move direction
			movement.x = Input.GetAxisRaw ("Horizontal");

			//Get input from the input manager, round it to an integer and store in vertical to set y axis move direction
			movement.y = Input.GetAxisRaw ("Vertical");

			//Check if we are running on iOS, Android, Windows Phone 8 or Unity iPhone

			animator.SetFloat("Horizontal", movement.x);
			animator.SetFloat("Vertical", movement.y);
			animator.SetFloat("Speed", movement.sqrMagnitude);


			float shootHorizontal = Input.GetAxis("ShootHorizontal");
			float shootVertical = Input.GetAxis("ShootVertical");
			if ((shootHorizontal != 0 || shootVertical != 0) && Time.time > lastFire + fireDelay)
            {
				Shoot(shootHorizontal, shootVertical); 
				lastFire = Time.time;
			}
		}

		void Shoot(float x, float y)
		{
			GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation) as GameObject;
			//bullet.AddComponent<Rigidbody2D>().gravityScale = 0;
			bullet.GetComponent<Rigidbody2D>().velocity = new Vector3(
				(x < 0) ? Mathf.Floor(x) * bulletSpeed : Mathf.Ceil(x) * bulletSpeed,
				(y < 0) ? Mathf.Floor(y) * bulletSpeed : Mathf.Ceil(y) * bulletSpeed,
				0
			);
		}


		private void FixedUpdate()
        {
			rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }


        //OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
        private void OnTriggerEnter2D (Collider2D other)
		{
			//Check if the tag of the trigger collided with is Exit.
			if(other.tag == "Exit")
			{
				//Invoke the Restart function to start the next level with a delay of restartLevelDelay (default 1 second).
				Invoke ("Restart", restartLevelDelay);
				
				//Disable the player object since level is over.
				enabled = false;
			}
			
			//Check if the tag of the trigger collided with is Food.
			else if(other.tag == "Food")
			{
				//Add pointsPerFood to the players current food total.
				food += pointsPerFood;
				
				//Update foodText to represent current total and notify player that they gained points
				foodText.text = "+" + pointsPerFood + " Food: " + food;
				
				//Call the RandomizeSfx function of SoundManager and pass in two eating sounds to choose between to play the eating sound effect.
				SoundManager.instance.RandomizeSfx (eatSound1, eatSound2);
				
				//Disable the food object the player collided with.
				other.gameObject.SetActive (false);
			}
			
			//Check if the tag of the trigger collided with is Soda.
			else if(other.tag == "Soda")
			{
				//Add pointsPerSoda to players food points total
				food += pointsPerSoda;
				
				//Update foodText to represent current total and notify player that they gained points
				foodText.text = "+" + pointsPerSoda + " Food: " + food;
				
				//Call the RandomizeSfx function of SoundManager and pass in two drinking sounds to choose between to play the drinking sound effect.
				SoundManager.instance.RandomizeSfx (drinkSound1, drinkSound2);
				
				//Disable the soda object the player collided with.
				other.gameObject.SetActive (false);
			}
		}
		
		
		//Restart reloads the scene when called.
		private void Restart ()
		{
			//Load the last scene loaded, in this case Main, the only scene in the game. And we load it in "Single" mode so it replace the existing one
            //and not load all the scene object in the current scene.
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
		}
		
		
		//LoseFood is called when an enemy attacks the player.
		//It takes a parameter loss which specifies how many points to lose.
		public void LoseFood (int loss)
		{
			//Set the trigger for the player animator to transition to the playerHit animation.
			animator.SetTrigger ("playerHit");
			
			//Subtract lost food points from the players total.
			food -= loss;
			
			//Update the food display with the new total.
			foodText.text = "-"+ loss + " Food: " + food;
			
			//Check to see if game has ended.
			CheckIfGameOver ();
		}
		
		
		//CheckIfGameOver checks if the player is out of food points and if so, ends the game.
		private void CheckIfGameOver ()
		{
			//Check if food point total is less than or equal to zero.
			if (food <= 0) 
			{
				//Call the PlaySingle function of SoundManager and pass it the gameOverSound as the audio clip to play.
				SoundManager.instance.PlaySingle (gameOverSound);
				
				//Stop the background music.
				SoundManager.instance.musicSource.Stop();
				
				//Call the GameOver function of GameManager.
				GameManager.instance.GameOver ();
			}
		}
	}
}

