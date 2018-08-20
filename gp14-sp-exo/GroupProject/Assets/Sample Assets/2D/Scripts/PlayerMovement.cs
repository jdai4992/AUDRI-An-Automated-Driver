/* The player movement script is used to handle:
 * - Points management (works out the players points and sets the GUI text elements),
 * - Lane management (keeps track of which lane the player is in and ensures they cannot 'brake lane'/escape the lanes),
 * - Object movement (moving the rigidbody/sprite left and right),
 * - Detecting collisions (detect collision with other car objects),
 * - Displaying statistics (listing the associated statistics once a collision has been detected).
*/

// UnityEngine is used as a base (needed), System.Collections is needed to use IEnumerators (the wait funcationality).
using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {
	// Current lane is set as the left lane (-1) as default to coincide with the sprite), points as 0 and collisison as false (as the car hasn't collided yet).
	public int currentLane = -1;
	public int points = 0;
	public bool collision = false;

	// The move values are the x distances used to move between lanes, y distances are ignored as we don't want the sprite to move up or down.
	private static Vector2 moveLeft = new Vector2 (-0.595f,0);
	private static Vector2 moveRight = new Vector2 (0.595f,0);

	// The isRunning variable is used to ensure only one coroutine is running at any given moment, see later code.
	private bool isRunning = false;
	
	private string pointsString = "";
	private string pointsText = "";

	// All of these variables are used for the statistics once a collision has occured. numOfOvertakes and timeOfRun are both used in sim (so need to be public).
	public int numberOfOvertakes;
	private int numberOfMoves = 0;
	public int timeOfRun = 0;
	private int timeInLeft = 0;
	private int timeInOffroad = 0;
	private int timeInOthers = 0;

	// Update function is called every frame and is used to start the points handling coroutine, the isRunning is used to stop simultaneous coroutines.
	private void Update () {
		if (isRunning == false) {
			StartCoroutine (pointsHandling ());
		}
	}

	// IEnumerator is used to allow the WaitForSeconds feature, isRunning is set to true at the start and false at the end.
	private IEnumerator pointsHandling() {
		isRunning = true;
		yield return new WaitForSeconds(1);

		// This section is used to ascertain if the game is currently paused in in the menu, thus shouldn't record point changes.
		GameObject player = GameObject.Find("Player");
		PlayerBehaviour playerScript = player.GetComponent<PlayerBehaviour>();
		bool paused = playerScript.paused;
		bool mainMenu = playerScript.mainMenu;

		// Providing it's not paused, in menu or collided the timers for statistics increase (times seconds).  Points also update based on the lane.
		if (!paused && !collision && !mainMenu) {
			timeOfRun++;
			if (currentLane == -2 || currentLane == 2) {
				points = points - 2;
				pointsString = "Offroad -2";
				pointsText = points + " (-2)";
				timeInOffroad++;
			} else if(currentLane == -1) {
				points = points + 2;
				pointsString = "Left Lane +2";
				pointsText = points + " (+2)";
				timeInLeft++;
			} else if(currentLane == 0) {
				points = points + 1;
				pointsString = "Middle Lane +1";
				pointsText = points + " (+1)";
				timeInOthers++;
			} else {
				pointsString = "Right Lane +0";
				pointsText = points + " (+0)";
				timeInOthers++;
			}

			// The GUI objects are found and updated with the the new points.
			GUIText points1GUI = GameObject.Find("PlayerPoints1").GetComponent<GUIText>();
			GUIText points2GUI = GameObject.Find("PlayerPoints2").GetComponent<GUIText>();
			GUIText points3GUI = GameObject.Find("PlayerPoints3").GetComponent<GUIText>();
			GUIText points4GUI = GameObject.Find("PlayerPoints4").GetComponent<GUIText>();
			points1GUI.text = pointsText;
			points4GUI.text = points3GUI.text;
			points3GUI.text = points2GUI.text;
			points2GUI.text = pointsString;
		}
		isRunning = false;
	}

	// The NewMove function is used to handle moving the rigidbody/sprite of the player as well as logging its movement and updating the lane position.
	public void NewMove(string move) {
		// Checks that it's not already collided, for efficiency the pause/menu check is done in the Behaviour script and collision check in this script.
		if (!collision) {
			// This is used to find the GameManager object and the Logger script within it (needed to use the logger.Log function).
			GameObject gameManager = GameObject.Find("GameManager");
			Logger logger = gameManager.GetComponent<Logger>();

			// If the move is left and it's not in the leftmost lane already it can; change the lane, move the player, and then log the move (and +1 to moves).
			if (move == "left") {
				if (currentLane == -1 || currentLane == 0 || currentLane == 1 || currentLane == 2) {
					currentLane = currentLane - 1;
					GetComponent<Rigidbody2D>().position = (GetComponent<Rigidbody2D>().position + (moveLeft));
					logger.Log("player", "left");
					numberOfMoves++;
				}
			// If the move is right and it's not in the rightmost lane already it can; change the lane, move the player, and then log the move (and +1 to moves).
			} else if (move == "right") {
				if (currentLane == -2 || currentLane == -1 || currentLane == 0 || currentLane == 1) {
					currentLane = currentLane + 1;
					GetComponent<Rigidbody2D>().position = (GetComponent<Rigidbody2D>().position + (moveRight));
					logger.Log("player", "right");
					numberOfMoves++;
				}
			}
		}
	}

	// This function is called when a collision with another object is detected.
	private void OnCollisionEnter2D(Collision2D coll) {
		// Checks if it's already collided then calculates the number of overtakes by using the amount of enemies spawned halved (to ignore simulator enemies).
		if (!collision) {
			GameObject gameManager = GameObject.Find("GameManager");
			EnemySpawning enemySpawning = gameManager.GetComponent<EnemySpawning>();
			numberOfOvertakes = enemySpawning.numberOfDespawns / 2;
			collision = true;
		}
	}

	// The function is continually called but only performs the script if a collision has been detected.
	void OnGUI() {
		if (collision) {
			// Draws the statistics page once a collision is detected as well as two buttons for restarting and quiting.
			GUI.Box (new Rect(Screen.width/4 - 200,100,195,170),"GAME OVER \n\n Your Score: " + points
			         + "\nLength of run: " + timeOfRun + " seconds"
			         + "\nNumber of moves: " + numberOfMoves
			         + "\nNumber of Overtaken cars: " + numberOfOvertakes
			         + "\nTime in Left Lane: " + timeInLeft
			         + "\nTime in Other Lanes: " + timeInOthers
			         + "\nTime in Off-Road Lanes: " + timeInOffroad);
			if(GUI.Button (new Rect(Screen.width/4 - 200,290,90,30),"New Game")){
				Application.LoadLevel("GameScene");
			}
			if(GUI.Button (new Rect(Screen.width/4 - 100,290,90,30),"Quit")){
				Application.Quit();
			}
		}
	}
}
