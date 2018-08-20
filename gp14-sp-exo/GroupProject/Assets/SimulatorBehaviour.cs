/* The simulator behaviour script is used to handle:
 * - WEKA AI management.
 * - Bespoke AI management.
*/

using UnityEngine;
using System.Collections;

public class SimulatorBehaviour : MonoBehaviour {
	// Declarations for various WEKA variables.
	private SimulatorMovement simulatorObject;
	private weka.classifiers.trees.J48 j48Classifier;
	private weka.core.Instances insts;
	private float accumulator;
	private float totalTime;
	private const float REFRESH_RATE = 0.0f;

	// Locations for the x positions of the lanes and the y positions of the top and bottom of the screen.
	private const float leftLaneX = -5.69f;
	private const float middleLaneX = -5.09f;
	private const float rightLaneX = -4.46f;	
	private const float bottomOfScreen = -5.7f;
	private const float topOfScreen = 5.3f;

	// The dangerZone is used in the bespokeAI to determine below what y value is an enemy car considered dangerous (should move out the way of).
	private static float dangerZone = -2.9f;

	// Bool to ensure the classifier is only initialised once.
	bool classified = false;

	private void Awake() {
		simulatorObject = GetComponent<SimulatorMovement>();
		accumulator = 0.0f;
		totalTime = 0.0f;
	}
	
	// Decides which algorithm to run, only run either WekaAI or bespokeAI, running them at the same time will not work.
	public void Update() {
		GameObject player = GameObject.Find("Player");
		PlayerBehaviour playerBehaviourScript = player.GetComponent<PlayerBehaviour>();
		if (playerBehaviourScript.fileChosen && !classified) {
			initialiseClassifier ();
			classified = true;
		}
		if (playerBehaviourScript.fileChosen) {
			WekaAI ();
		}
		//bespokeAI ();
	}

	private void WekaAI () {

		accumulator += Time.deltaTime;
		totalTime += accumulator;
		if (accumulator >= REFRESH_RATE){
			//Algoirthm will refresh based on the constant
			accumulator = 0;
			float[] positions = getPositions();
			int currentLane = (int)positions[3] + 2;
			double result = getAlgorithmResult(positions);
			int newLane = ((int)result);

			// Handles the movement for the decisions from WEKA.
            switch (newLane) {
				case 0:
					if(currentLane == 1) {
						simulatorObject.NewMove("left");
					} else {
						simulatorObject.NewMove("stay");
					}
                    break;
				case 1:
					if (currentLane == 2) {
                        simulatorObject.NewMove("left");
					} else if (currentLane == 0) {
                        simulatorObject.NewMove("right");
					} else {
						simulatorObject.NewMove("stay");
					}
                    break;
				case 2:
					if (currentLane == 1) {
                        simulatorObject.NewMove("right");
					} else if (currentLane == 3) {
                        simulatorObject.NewMove("left");
					} else{
						simulatorObject.NewMove("stay");
					}
                    break;
				case 3:
					if (currentLane == 2) {
                        simulatorObject.NewMove("right");
					} else if (currentLane == 4) {
                        simulatorObject.NewMove("left");
					} else {
						simulatorObject.NewMove("stay");
					}
                    break;
				case 4:
					if(currentLane == 3) {
                  	 	simulatorObject.NewMove("right");
					} else {
						simulatorObject.NewMove("stay");
					}
					break;
                default:
                    Debug.Log("Unclassified Data");
                    break;
            }
		}
	}

	// Returns an array of {currentLane, lowestLeftLaneEnemy, lowestMiddleLaneEnemy, lowestRightLaneEnemy}.
	private float[] getPositions() {
		GameObject simulator = GameObject.Find("Simulator");
		SimulatorMovement simulatorScript = simulator.GetComponent<SimulatorMovement>();
		int currentLane = simulatorScript.currentLane;

		// Sets the initial lowest enemy as the top of the screen so that it will almost certainly be replaced by an actual enemy value unless...
		// ...no enemy exists in said lane.
		float lowestLeftLaneEnemy = 5.3f;
		float lowestMiddleLaneEnemy = 5.3f;
		float lowestRightLaneEnemy = 5.3f;

		// For every enemy on the screen is searches through them, checks their lane and then checks to see if they are the lowest (but not lower than...
		// ...the bottom of the screen).
		GameObject[] gos = GameObject.FindGameObjectsWithTag("Enemy");
		foreach (GameObject go in gos) {
			Vector3 enemy_position = go.transform.position;
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
		float[] positions = new float[]{lowestLeftLaneEnemy, lowestMiddleLaneEnemy, lowestRightLaneEnemy, currentLane};
		return positions;
	}
		//returns the classification result as the lane number
	private double getAlgorithmResult(float[] attributes) {
		//sets up the current environment as a new state 'Instance'
		//labels as the same type as the training data, sets up the attribute values
		//leaves the classifier blank
		weka.core.Instance currentState = new weka.core.DenseInstance(6);
		currentState.setDataset(insts);
		currentState.setValue(0,(attributes[3]+2));
		currentState.setValue(1,attributes[0]);
		currentState.setValue(2,attributes[1]);
		currentState.setValue(3,attributes[2]);
		currentState.setValue(4,totalTime);
		currentState.setClassMissing();
		//Debug.Log (currentState.ToString());
		double result = j48Classifier.classifyInstance(currentState);
		return result;
		}


	private void initialiseClassifier() {
		// Loads the file to grab the filename.
		GameObject player = GameObject.Find("Player");
		PlayerBehaviour playerBehaviourScript = player.GetComponent<PlayerBehaviour>();
		//loads training and test data, builds new classifier and trains it
		insts = new weka.core.Instances(new java.io.FileReader(playerBehaviourScript.stringToEdit));
		insts.setClassIndex(4);
		j48Classifier = new weka.classifiers.trees.J48();
		//Optionals methods currently testing for improvements
		//j48Classifier.setUnpruned (true);
		//j48Classifier.setUseLaplace (true);
		j48Classifier.buildClassifier(insts);
		//NO CURRENT TEST SET READY TO USE
		//weka.core.Instances testData = new weka.core.Instances(new java.io.FileReader("TestSet.arff"));
		//testData.setClassIndex(5);
		//weka.classifiers.Evaluation eval = new weka.classifiers.Evaluation(insts);
		//eval.evaluateModel (j48Classifier,testData);
		Debug.Log (j48Classifier.toSummaryString());
	}

	private void bespokeAI() {
		float[] positions = getPositions ();
		float lowestLeftLaneEnemy = positions [0];
		float lowestMiddleLaneEnemy = positions [1];
		float lowestRightLaneEnemy = positions [2];
		float currentLane = positions [3];
		
		// Lane prioritisation.
		if (currentLane == -2) {
			// Check if lane -1 is free (want to move right, into left lane).
			if (lowestLeftLaneEnemy > dangerZone) {
				// Able to move right.
				simulatorObject.NewMove ("right");
			}
		} else if (currentLane == -1) {
			// Don't need to prioritise, in the best lane position (don't want to move).
		} else if (currentLane == 0) {
			// Check if lane -1 is free (want to move left, into left lane).
			if (lowestLeftLaneEnemy > dangerZone) {
				// Able to move left.
				simulatorObject.NewMove ("left");
			}
		} else if (currentLane == 1) {
			// Check if lane 0 is free (want to move left, into middle lane).
			if (lowestMiddleLaneEnemy > dangerZone) {
				// Able to move left.
				simulatorObject.NewMove ("left");
			}
		} else if (currentLane == 2) {
			// Check if lane 1 is free (want to move left, into right lane).
			if (lowestRightLaneEnemy > dangerZone) {
				// Able to move left.
				simulatorObject.NewMove ("left");
			}
		}
		
		// Lane analysis.
		
		// If you're in the left lane.
		// Are you in danger?  If so, can you move right?  So check right lane.  If safe then move right, otherwise have to move left.
		if (currentLane == -1) {
			if (lowestLeftLaneEnemy <= dangerZone) {
				if (lowestMiddleLaneEnemy <= dangerZone) {
					simulatorObject.NewMove ("left");
				} else {
					simulatorObject.NewMove ("right");
				}
			}
		}
		
		// If you're in the middle lane.
		// Can you move left?  So check left lane.
		// If safe then move left, otherwise check right, if right is free then move right.
		// If neither is free go to the lane with the best option (one with the highest enemy).
		else if (currentLane == 0) {
			if (lowestMiddleLaneEnemy <= dangerZone) {
				if (lowestLeftLaneEnemy <= dangerZone) {
					if (lowestRightLaneEnemy <= dangerZone) {
						simulatorObject.NewMove ("left");
					} else {
						simulatorObject.NewMove ("right");
					}
				} else {
					simulatorObject.NewMove ("left");
				}
			}
		}
		
		// If you're in the right lane.
		// Are you in danger?  If so, can you move left?  So check left lane.  If safe then move left, otherwise have to move right.
		else if (currentLane == 1) {
			if (lowestRightLaneEnemy <= dangerZone) {
				if (lowestMiddleLaneEnemy <= dangerZone) {
					simulatorObject.NewMove ("right");
				} else {
					simulatorObject.NewMove ("left");
				}
			}
		}
	}
}
