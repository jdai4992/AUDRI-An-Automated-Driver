/* The background movement script is used to handle:
 * - Moving the background element downwards.
 * - Despawning it when it reaches the bottom of the screen.
*/

using UnityEngine;

public class BackgroundMovement : MonoBehaviour {

	// Variables for what is considered the screen bottom (for despawning) and the movement speed (negative y value, no x value as it just moves down).
	private static int screenBottom = -14;
	private static Vector2 speed = new Vector2 (0,-0.2f);

	// Checks if the game is paused, in which case it shouldn't move, if it's not then move the object down.
	private void Update () {
		GameObject player = GameObject.Find("Player");
		PlayerBehaviour playerScript = player.GetComponent<PlayerBehaviour>();
		bool paused = playerScript.paused;
		
		if (!paused) {
			transform.Translate(speed, Space.World);
			// Checks if the background is out of the screen bounds, then de-spawns if it is.
			if (transform.position.y < screenBottom) {
				Destroy (gameObject);
			}
		}
	}
}