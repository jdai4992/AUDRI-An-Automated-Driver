/* The logger script is used to handle:
 * - Logging simulator data to a file (on command and via a timer),
 * - Logging player data to a file (on command and via a timer).
*/

using UnityEngine;
using System.Collections;
using System.IO;

public class Logger : MonoBehaviour {
	// Declarations	of: bottom of screen for despawning, top of screen for initalising and lane x co-ordinates.
	private const double bottomOfScreen = -5.7;
	private const double topOfScreen = 5.7;
	private const float leftLaneX = -5.69f;
	private const float middleLaneX = -5.09f;
	private const float rightLaneX = -4.46f;
	
	private double lowestLeftLaneEnemy, lowestMiddleLaneEnemy, lowestRightLaneEnemy;
	private int movetoLane, currentLane;
	private GameObject[] enemies;
	private Vector3 enemy_position;

	private bool logging = false;
	private const float waitTime = 0.1f;

	// Update is just used for the automatic logging.
	private void Update() {
		if (!logging) {
			StartCoroutine(timeLogging());
		}
	}

	// System for automatic logging where by each waitTime interval it logs that the current state of each object is "staying".
	// This is used so that the logs don't just contain the left and right moves and do also recognise when to stay in each lane when it's safe.
	private IEnumerator timeLogging() {
		logging = true;
		Log("player", "stay");
		Log("simulator", "stay");
		yield return new WaitForSeconds (waitTime);
		logging = false;
	}

	// The log function is the main system for logging the situation of both the player and the simulator object to their txt files.
	public void Log(string logObject, string logMove) {
		// If it is logging for the simulator then get the current lane of the simulator.
		if (logObject == "simulator") {
			GameObject simulator = GameObject.Find("Simulator");
			SimulatorMovement simulatorScript = simulator.GetComponent<SimulatorMovement>();
			currentLane = simulatorScript.currentLane;
		// Otherwise if it's logging for the player then get the current lane of the player.
		} else if (logObject == "player"){
			GameObject player = GameObject.Find("Player");
			PlayerMovement playerScript = player.GetComponent<PlayerMovement>();
			currentLane = playerScript.currentLane;
		// If neither simulator or player is entered then log an error message and return to stop unneccesary calculation.
		} else {
			Debug.LogError("Logged object must be player or simulator, not " + logObject);
			return;
		}

		// Firstly set the lowest enemy of each lane to be the top of the screen so that when it reads the enemy y positions later it will be...
		// ...replaced by an actual enemy y position (unless there isn't a single enemy in said lane).
		lowestLeftLaneEnemy = topOfScreen;
		lowestMiddleLaneEnemy = topOfScreen;
		lowestRightLaneEnemy = topOfScreen;

		// Switch statement that matches with the logged move and saves what the new lane would be after the move is completed.
		switch (logMove) {
		case "left" : 	
			movetoLane = currentLane -1;
			break;
		case "right" : movetoLane = currentLane +1;
			break;
		case "stay" : movetoLane = currentLane;
			break;
		}

		// Statement which loops through every enemy, checks which lane they are in and that they are above the bottom of the screen...
		// ...and checks to see if they are the lowest enemy in said lane (if they are then they become the lowest enemy).
		enemies = GameObject.FindGameObjectsWithTag("Enemy");
		foreach (GameObject enemy in enemies) {
			enemy_position = enemy.transform.position;
			if 	(enemy_position.x == leftLaneX){
				if ((enemy_position.y < lowestLeftLaneEnemy) && (enemy_position.y>bottomOfScreen)){
					lowestLeftLaneEnemy = enemy_position.y;
				}
			}
			if 	(enemy_position.x == middleLaneX){
				if ((enemy_position.y < lowestMiddleLaneEnemy) && (enemy_position.y>bottomOfScreen)){
					lowestMiddleLaneEnemy = enemy_position.y;
				}
			}
			if 	(enemy_position.x == rightLaneX){
				if ((enemy_position.y < lowestRightLaneEnemy) && (enemy_position.y>bottomOfScreen)){
					lowestRightLaneEnemy = enemy_position.y;
				}
			}
		}

		// If the object is the simulator then write to the simulator file, otherwise write it to the player file (slightly move efficient as the sim...
		// ...is probably going to make more moves and so called more often than the player so therefore it's put first).
		if (logObject == "simulator") {
			TextWriter f = new StreamWriter ("simulatorData.txt", true);
			f.WriteLine((currentLane + 2) + "," + lowestLeftLaneEnemy + "," + lowestMiddleLaneEnemy + "," + lowestRightLaneEnemy + "," + (movetoLane + 2));
			f.Close ();
		} else {
			TextWriter f = new StreamWriter ("playerData.txt", true);
			f.WriteLine((currentLane + 2) + "," + lowestLeftLaneEnemy + "," + lowestMiddleLaneEnemy + "," + lowestRightLaneEnemy + "," + (movetoLane + 2));
			f.Close ();
		}
	}
}