/* The background movement script is used to handle:
 * - Moving the enemy car down (using a different speed depending on the lane).
 * - Despawning the enemy car when it reaches the bottom of the screen (and logging the despawn for statistics).
*/

using UnityEngine;
public class EnemyBehaviour : MonoBehaviour {

	// The y value of the bottom of the screen (out of camera view).
	private static float screenBottom = -5.2f;

	// The "falling speed" of the enemy cars.
	private Vector2 speed;

	// The spawn locations for each of the lanes with the append which is added to spawn cars in the simulator lanes.
	public static Vector3 rightPlayerSpawn = new Vector3(-4.46f,6,-1);
	public static Vector3 middlePlayerSpawn = new Vector3(-5.09f,6,-1);
	public static Vector3 leftPlayerSpawn = new Vector3(-5.69f,6,-1);
	public static Vector3 simulatorSpawnAppend = new Vector3(9.93f,0,0);

	// Finding the lane which the car is in and then setting the speed variable to be different depending on said lane.
	// The left lane has the fastest cars as they would logically be moving slower compared to your car speed, with right being the slowest (fastest drivers).
	private void Awake () {
		if (GetComponent<Rigidbody2D>().position.x == leftPlayerSpawn.x || GetComponent<Rigidbody2D>().position.x == (leftPlayerSpawn.x + simulatorSpawnAppend.x)) {
			speed = new Vector2 (0,-0.14f);
		} else if (GetComponent<Rigidbody2D>().position.x == middlePlayerSpawn.x || GetComponent<Rigidbody2D>().position.x == (middlePlayerSpawn.x + simulatorSpawnAppend.x)) {
			speed = new Vector2 (0,-0.12f);
		} else if (GetComponent<Rigidbody2D>().position.x == rightPlayerSpawn.x || GetComponent<Rigidbody2D>().position.x == (rightPlayerSpawn.x + simulatorSpawnAppend.x)) {
			speed = new Vector2 (0,-0.1f);
		} else {
			Debug.LogError("Enemy spawning in incorrect lane");
		}
	}

	// Checks to see if the player is in either paused or in a menu, in which case don't spawn an enemy.
	private void Update () {
		GameObject player = GameObject.Find("Player");
		PlayerBehaviour playerScript = player.GetComponent<PlayerBehaviour>();
		bool paused = playerScript.paused;
		bool mainMenu = playerScript.mainMenu;

		if (!paused && !mainMenu) {
			transform.Translate(speed, Space.World);
			// Checks if the car is out of the screen bounds, then de-spawns if it is.
			if (transform.position.y < screenBottom) {
				GameObject gameManager = GameObject.Find("GameManager");
				EnemySpawning enemySpawning = gameManager.GetComponent<EnemySpawning>();
				enemySpawning.numberOfDespawns = enemySpawning.numberOfDespawns + 1;
				Destroy (gameObject);
			}
		}
	}
}