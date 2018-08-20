/* The enemy spawning script is used to handle:
 * - Creating the enemy car prefabs in a random lane.
 * - Getting all the difficulty settings from the sliders.
 * - Handling the spawn rate and its increment.
*/

using UnityEngine;
using System.Collections;

public class EnemySpawning : MonoBehaviour {
	public GameObject Enemy;

	// Started is used to just get the slider variables once when the game has started.
	// Spawning is used to ensure there's only one SpawningScript coroutine.
	private bool started = false;
	private bool spawning = false;

	// Declarations for the sliders in the main menu which are used to set the difficulty.
	private float maximumDifficulty;
	private float difficultyIncrement;
	private float currentDifficulty;

	// Locations for where to spawn the enemies for each lane, with an append which is added to to make them appear in the simulator lanes.
	public static Vector3 rightPlayerSpawn = new Vector3(-4.46f,6,-1);
	public static Vector3 middlePlayerSpawn = new Vector3(-5.09f,6,-1);
	public static Vector3 leftPlayerSpawn = new Vector3(-5.69f,6,-1);
	public static Vector3 simulatorSpawnAppend = new Vector3(9.93f,0,0);

	// The number of despawns is recorded so the player/simulator can access this for the overtake statistics.
	public int numberOfDespawns = 0;

	// Update is used to continually spawn enemies.
	private void Update () {
		if (!spawning) {
			StartCoroutine(SpawningScript());
		}
	}

	// The spawning script handles the spawning logic but not the creation of the prefabs themselves.
	private IEnumerator SpawningScript () {
		// Spawning is set to true so no other coroutines are started, it then waits for the difficulty amount.
		spawning = true;
		yield return new WaitForSeconds(currentDifficulty);

		// Here is gets the player component and checks if they have quit the menu, and therefore set the difficulty on the sliders.
		// It then sets the max, current and increment for the difficulties using the slider amounts.
		GameObject player = GameObject.Find("Player");
		PlayerBehaviour playerBehaviour = player.GetComponent<PlayerBehaviour>();
		if (!playerBehaviour.mainMenu && !started) {
			maximumDifficulty = playerBehaviour.maximumDifficulty;
			difficultyIncrement = playerBehaviour.difficultyIncrement;
			currentDifficulty = playerBehaviour.startingDifficulty;
			started = true;
		}

		// Checks that the game isn't paused or in a menu state, if not then it spawns an enemy and increases the difficulty by the increment amount.
		bool paused = playerBehaviour.paused;
		bool mainMenu = playerBehaviour.mainMenu;
		if (!paused && !mainMenu) {
			Spawn();
			if (currentDifficulty > maximumDifficulty) {
				currentDifficulty = currentDifficulty - difficultyIncrement;
			}
		}
		spawning = false;
	}

	// The spawn script handles the actual creation of the enemy prefab.
	// It first chooses a random lane and then spawns an enemy in that lane (the identity function is just used so no rotation is added to the prefab).
	private void Spawn() {
		int spawnLane = Random.Range (-1, 2);
		Enemy.tag = "Enemy";

		if (spawnLane == 0) {
			Instantiate (Enemy, leftPlayerSpawn, Quaternion.identity);
			Instantiate (Enemy, leftPlayerSpawn+simulatorSpawnAppend, Quaternion.identity);
		} else if (spawnLane == 1) {
			Instantiate (Enemy, middlePlayerSpawn, Quaternion.identity);
			Instantiate (Enemy, middlePlayerSpawn+simulatorSpawnAppend, Quaternion.identity);
		} else {
			Instantiate (Enemy, rightPlayerSpawn, Quaternion.identity);
			Instantiate (Enemy, rightPlayerSpawn+simulatorSpawnAppend, Quaternion.identity);
		}
	}
}