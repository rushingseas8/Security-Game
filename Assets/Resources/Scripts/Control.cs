using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Control : MonoBehaviour {

	private static Vector2 anchor;

	//This is the equivalent of a null value for Vector2s
	private static readonly Vector2 NO_POINT = new Vector2(-9999, -9999);

	private const float PANNING_SCALE = 0.1f;
	private const float SCROLLING_SCALE = 1.2f;
	private const float ROTATION_SCALE = 3f;

	//What will we create when we left click?
	public static int creationType;

	/*
	 * True if we should have the selection cursor on the center of the tile-
	 * this is actually 0.5 up and right of the whole number, but is useful for
	 * placing down furniture and such.
	 */
	public static bool centerOnTile;

	private static List<GameObject> ghost;
	private static int lastXSize, lastYSize;

	private static GameObject selection;

	// Use this for initialization
	void Start () {
		anchor = NO_POINT;

		creationType = 0;

		ghost = new List<GameObject>();
		lastXSize = 0;
		lastYSize = 0;
	}
	
	// Update is called once per frame
	void Update () {
		//WASD to pan around
		float xMovement = -CameraController.getHeight() * PANNING_SCALE * Input.GetAxis("Horizontal");
		float yMovement = -CameraController.getHeight() * PANNING_SCALE * Input.GetAxis ("Vertical");

		CameraController.addX(xMovement);
		CameraController.addY(yMovement);

		Vector3 clickedPoint = Input.mousePosition;
		clickedPoint.z = -CameraController.getHeight ();
		clickedPoint = CameraController.getCamera ().ScreenToWorldPoint (clickedPoint);

		//QE to rotate
		if (Input.GetKey (KeyCode.Q)) {
			CameraController.addTilt (ROTATION_SCALE);
		}
		if (Input.GetKey (KeyCode.E)) {
			CameraController.addTilt (-ROTATION_SCALE);
		}

		//RMB also can pan around
		if (Input.GetMouseButton (1)) {
			//Set initial point, if needed
			if (anchor == NO_POINT)
				anchor = clickedPoint;
			//Move the delta between the init and current point
			else {
				CameraController.addX(anchor.x - clickedPoint.x);
				CameraController.addY(anchor.y - clickedPoint.y);
			}
		}

		if (Input.GetMouseButton (0)) {
			//Round to nearest integer
			clickedPoint.x = Mathf.Round (clickedPoint.x);
			clickedPoint.y = Mathf.Round (clickedPoint.y);

			//Set initial point, if needed. Round to nearest integer.
			if (anchor == NO_POINT) {
				Debug.Log ("Anchor: " + clickedPoint);
				anchor = clickedPoint;
			} else {
				Debug.Log ("Clicked point: " + clickedPoint);
				int xSize = (int)(clickedPoint.x - anchor.x);
				int ySize = (int)(clickedPoint.y - anchor.y);

				Debug.Log ("Selection size: " + xSize + "x" + ySize);

				switch (creationType) {
				case 0:
					if (xSize != lastXSize || ySize != lastYSize) {
						Debug.Log ("Size updated. Selection size: " + xSize + "x" + ySize);
						//if (xSize >= 2 && ySize >= 2) {
							lastXSize = xSize;
							lastYSize = ySize;

							//Clearing code
							foreach (GameObject g in ghost)
								if(g.tag == "Ghost")
									Game.destroy (g);
							ghost.Clear ();

							//Create a room
							ghost = Game.createRoomGhost ((int)anchor.x, (int)anchor.y, xSize, ySize);
						//}
					}
					break;
				case 1:
					//Game.
					break;
				}
			}
		} else {
			//Finalize the placement of the objects in the room, if any
			if (ghost.Count > 0) {
				//Remove all ghost objects
				foreach (GameObject go in ghost)
					if(go.tag == "Ghost")
						Game.destroy (go);
				ghost.Clear ();

				//*Attempt* placement of the same objects
				Game.createRoom ((int)anchor.x, (int)anchor.y, lastXSize, lastYSize, ID.BRICK);
			}
		}

		if (!Input.GetMouseButton (0) && !Input.GetMouseButton (1))
			anchor = NO_POINT;

		//TEMP - test of furniture vs. room placement
		float offset = 0;
		if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift))
		//if(centerOnTile)
			offset = 0.5f;
		
		if (selection == null) {
			selection = Game.create ("selection", Mathf.Round (clickedPoint.x) + offset, Mathf.Round (clickedPoint.y) + offset);
		} else {
			Game.destroy (selection);
			selection = Game.create ("selection", Mathf.Round(clickedPoint.x) + offset, Mathf.Round(clickedPoint.y) + offset);
		}

		//Handle scrolling
		CameraController.addHeight(SCROLLING_SCALE * Input.GetAxis ("Mouse ScrollWheel"));
		
	}
}
