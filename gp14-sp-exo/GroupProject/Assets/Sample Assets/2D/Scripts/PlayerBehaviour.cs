/* The player behaviour script is used to handle:
 * - Key stroke detection (detecting if any 'significant' keys have been pressed).
 * - Handling the pause and main menus.
*/

using UnityEngine;

public class PlayerBehaviour : MonoBehaviour {
	private PlayerMovement character;
	private BackgroundSpawning spawningScript;

	// Declarations of the keys for various moves, allows us to change keys later if necessary (possible gamepad/wheel support).
	private static string leftKey = "left";
	private static string rightKey = "right";
	private static string quitKey = "escape";
	private static string pauseKey = "p";

	// String for which AI version to use and bool to show it's submitted.
	public string stringToEdit = "file.txt";
	public bool fileChosen = false;

	// Declarations for the start states of various booleans. By default the main menu is true so that the main menu UI appears on loadup.
	public bool paused = false;
	public bool mainMenu = true;
	public bool optionsMenu = false;

	// Declarations to be used in the sliders in the main menu UI.
	public float startingDifficulty;
	public float difficultyIncrement;
	public float maximumDifficulty;

	// Awake function ran once upon running. This function is used to grab the background spawning script (used later on) and the movement script.
	private void Awake() {
		GameObject gameManager = GameObject.Find("GameManager");
		spawningScript = gameManager.GetComponent<BackgroundSpawning>();
		character = GetComponent<PlayerMovement>();
	}

	private void Update () {
		// If the game is not paused and not in the main menu then it is allowed to take in key presses.
		if (!paused && !mainMenu) {
			if (Input.GetKeyDown(leftKey)) {
				character.NewMove(leftKey);
			} else if (Input.GetKeyDown(rightKey)) {
				character.NewMove(rightKey);
			}
		// If the user presses the quit key as defined above, quit.  If the user presses the pause key then it will toggle the pause.
		// Note that the pause key also spawns an extra background this is because backgrounds are spawned on a timer that continues even when...
		// ...the game is paused, thus it can become out of sync and produce gaps. This spawning solves the issue.
		} if(Input.GetKeyDown(quitKey)){
				Application.Quit();
		} if (Input.GetKeyDown (pauseKey)) {
			if(!mainMenu){
				paused = !paused;
				spawningScript.Spawn();
			}
		}
	}

	// OnGUI is updated every frame and is used to handle the GUI elements.
	private void OnGUI() {
		// If the game has been paused and not already in the main menu then it will open the pause menu (two buttons for new game and quit).
		// Note that the numbers are just the sizes of the rectangle: left, top, width, height.
		if (paused && !mainMenu) {
			GUI.BeginGroup(new Rect(((Screen.width/2) - (200/2)),((Screen.height/2) - (170/2)),200,170));
			GUI.Box (new Rect(25,0,150,30),"PAUSED");
			if(GUI.Button (new Rect(50,60,100,40),"New Game")){
				Application.LoadLevel("GameScene");
			}
			if(GUI.Button (new Rect(50,100,100,40),"Quit")){
				Application.Quit();
			}
			GUI.EndGroup ();
		}

		// If the main menu has been activated and you're not already in the options menu, it will open the main menu.
		if(mainMenu && !optionsMenu ){
			// Starts the group which contains all the GUI elements for the menu.
			GUI.BeginGroup(new Rect(((Screen.width/2) - (200/2)),((Screen.height/4) - (170/2)),200,500));
			GUI.Box (new Rect(0,0,200,50),"AUDRI \n- AN AUTOMATED\n DRIVING SIMULATOR -\n"+ "Main Menu");
			if(GUI.Button (new Rect(50,60,100,20),"New Game")){
				// If the user presses the new game button close the mainMenu.
				// These two if statements are used to default the difficulties if the player doesn't touch the sliders at all and just starts the game...
				// ...in which case the variables usually default to 0, which for our situation wouldn't work - so we define our own defaults.
				if (startingDifficulty == 0) {
					startingDifficulty = 0.9f;
				}
				if (maximumDifficulty == 0) {
					maximumDifficulty = 0.3f;
				}
				mainMenu = false;
			}
			// If the quit button is pressed then quit.
			if(GUI.Button (new Rect(50,85,100,20),"Quit")){
				Application.Quit();
			}

			// Text box containing the play instructions:
			GUI.Box (new Rect(0,115,200,220),"How to play\n"+
			         "+--------------------+\n"+
					 "There are three main lanes and\n"+
			         "two emergency off-road lanes.\n"+
			         "Use the left and right arrows to\n"+
			         "avoid the enemy cars.\n"+
					 "+--------------------------------------------+\n"+
					 " The two\n " +
			         "off-road lanes lose points. The\n"+
			         "left most lane gains the most\n points.\n"+
					 "+--------------------------------------------+\n"+
					 "Pressing 'p' on the keyboard \n "+
					 "pauses and resumes the game.");

			// The sliders which are used to define spawn script variables, the last two values are their associated min and max values.
			GUI.Box (new Rect(0,345,200,20),"Starting difficulty:");
			startingDifficulty = GUI.HorizontalSlider((new Rect(0,365,200,20)), startingDifficulty, 0.9f, 0.3f);
			GUI.Box (new Rect(0,380,200,20),"Difficulty increment:");
			difficultyIncrement = GUI.HorizontalSlider((new Rect(0,400,200,20)), difficultyIncrement, 0f, 0.005f);
			GUI.Box (new Rect(0,415,200,20),"Maximum difficulty:");
			maximumDifficulty = GUI.HorizontalSlider((new Rect(0,435,200,20)), maximumDifficulty, 0.9f, 0.3f);
			GUI.Box (new Rect(0,450,200,20),"AI filename and path:");
			stringToEdit = GUI.TextField(new Rect(0,475,175,20), stringToEdit, 25);
			if (GUI.Button(new Rect(180, 475, 20, 20), ""))
			{
				fileChosen = true;
			}
			GUI.EndGroup ();
		}
	}
}