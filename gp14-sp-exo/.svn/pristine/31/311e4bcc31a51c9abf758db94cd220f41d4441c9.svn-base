/* The background spawning script is used to handle:
 * - Creating the background prefabs.
*/

using UnityEngine;
using System.Collections;

public class BackgroundSpawning : MonoBehaviour {
	public GameObject Background;

	// SpawnTime is used to say how often (in seconds) a background object should spawn. The spawn location is simply the top of the screen.
	private static float spawnTime = 1;
	private static Vector3 backgroundSpawnLocation = new Vector3(0,10,0);

	// Although start is just called once, the while true treats it like an Update function (without needing to coroutine it).
	private IEnumerator Start () {
		while(true) {
			yield return new WaitForSeconds(spawnTime);

			// Finds out if the game is currently paused.
			GameObject player = GameObject.Find("Player");
			PlayerBehaviour playerScript = player.GetComponent<PlayerBehaviour>();
			bool paused = playerScript.paused;

			// If it's not paused then spawn more background prefabs, one per time period.
			if (!paused) {
				Spawn();
			}
		}
	}

	public void Spawn(){
		Instantiate (Background, backgroundSpawnLocation, Quaternion.identity);
	}
}