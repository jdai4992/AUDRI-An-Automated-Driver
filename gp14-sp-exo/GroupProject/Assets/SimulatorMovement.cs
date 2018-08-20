/* The simulator movement script is used to handle:
 * - Points management for the simulator.
 * - Moving the simulator sprite/rigidbody.
 * - Printing out statistics if either itself or the player crashes.
*/

using UnityEngine;
using System.Collections;

public class SimulatorMovement : MonoBehaviour {
	// Initalises the currentLane as the left lane (as this is where the sprite starts) and points as zero.
	public int currentLane = -1;
	public int points = 0;

	// The moves amounts, only the x value exists as the simulator isn't allowed to move up or down.
	private static Vector2 moveLeft = new Vector2 (-0.595f,0);
	private static Vector2 moveRight = new Vector2 (0.595f,0);

	// Both variables are used in coroutines to stop multiple running at once, see later code.
	private bool moving = false;
	private bool isRunning = false;

	private string pointsString = "";
	private string pointsText = "";

	// Starts of uncollided.
	public bool collision = false;

	//Statistic data variables
	private int numberOfOvertakes;
	private int numberOfMoves = 0;
	private int timeInLeft = 0;
	private int timeOfRun = 0;
	private int timeInOffroad = 0;
	private int timeInOthers = 0;

	private void Update () {
		if (isRunning == false) {
			StartCoroutine (Waiting ());
		}
		// This will check if there hasn't been a collision, if not then it will also check to see if the player has collided yet.
		if(!collision){
			GameObject player = GameObject.Find("Player");
			PlayerMovement playermovementScript = player.GetComponent<PlayerMovement>();
			collision = playermovementScript.collision;

			// Also takes the opportunity to take the numOfOvertakes and timeOfRun from the player script to use as stats for the sim (should be the same).
			numberOfOvertakes = playermovementScript.numberOfOvertakes;
			timeOfRun = playermovementScript.timeOfRun;
		}
	}

	// Waiting is used in exactly the same way as the player point management. It first checks it's in the game (not in a menu, collided or paused)...
	// ...then it checks which lane it is in and updates the points and associated GUI elements.
	private IEnumerator Waiting() {
		isRunning = true;
		yield return new WaitForSeconds(1);
		
		GameObject player = GameObject.Find("Player");
		PlayerBehaviour playerScript = player.GetComponent<PlayerBehaviour>();
		bool paused = playerScript.paused;
		bool mainMenu = playerScript.mainMenu;

		if (!paused && !collision && !mainMenu) {
			if (currentLane == -2 || currentLane == 2) {
				points = points - 2;
				timeInOffroad++;
				pointsString = "Offroad -2";
				pointsText = points + " (-2)";
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
		}

		if (!paused && !collision && !mainMenu) {
			GUIText points1GUI = GameObject.Find("SimulatorPoints1").GetComponent<GUIText>();
			GUIText points2GUI = GameObject.Find("SimulatorPoints2").GetComponent<GUIText>();
			GUIText points3GUI = GameObject.Find("SimulatorPoints3").GetComponent<GUIText>();
			GUIText points4GUI = GameObject.Find("SimulatorPoints4").GetComponent<GUIText>();

			points1GUI.text = pointsText;
			points4GUI.text = points3GUI.text;
			points3GUI.text = points2GUI.text;
			points2GUI.text = pointsString;
		}
		isRunning = false;
	}

	// NewMove handles moving the rigidbody/sprite of the sim.  Checks that it's not already moving as the simulator is very quick at making moves and...
	// ...can in theory without the !moving check do multiple moves at the same time (which would break the system).
	public void NewMove(string move) {
		GameObject gameManager = GameObject.Find("GameManager");
		Logger logger = gameManager.GetComponent<Logger>();
		if (!moving) {
			moving = true;
			if (move == "left") {
				if (currentLane == -1 || currentLane == 0 || currentLane == 1 || currentLane == 2) {
					logger.Log("simulator", move);
					currentLane = currentLane - 1;
					GetComponent<Rigidbody2D>().position = (GetComponent<Rigidbody2D>().position + (moveLeft));
				}
			} else if (move == "right") {
				if (currentLane == -2 || currentLane == -1 || currentLane == 0 || currentLane == 1) {
					logger.Log("simulator", move);
					currentLane = currentLane + 1;
					GetComponent<Rigidbody2D>().position = (GetComponent<Rigidbody2D>().position + (moveRight));
				}
			} else if (move == "stay") {
				logger.Log("simulator", move);
			}
			moving = false;
		}
	}

	private void OnCollisionEnter2D(Collision2D coll) {
		collision = true;
	}

	// OnGUI is continually run but only produces the statistics once the collision has been detected (either player or simulator).
	void OnGUI()
	{
		if (collision) {
			GUI.Box (new Rect(800,100,195,170),"GAME OVER \n\n Your Score: " + points
			         + "\nLength of run: " + timeOfRun + " seconds"
			         + "\nNumber of moves: " + numberOfMoves
			         + "\nNumber of Overtaken cars: " + numberOfOvertakes
			         + "\nTime in Left Lane: " + timeInLeft
			         + "\nTime in Other Lanes: " + timeInOthers
			         + "\nTime in Off-Road Lanes: " + timeInOffroad);
		}
	}
}