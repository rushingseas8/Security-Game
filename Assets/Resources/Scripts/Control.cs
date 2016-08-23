﻿using UnityEngine;
using System.Collections;

public class Control : MonoBehaviour {

	private static Vector2 anchor;

	//This is the equivalent of a null value for Vector2s
	private static readonly Vector2 NO_POINT = new Vector2(0, 0);

	private static readonly float PANNING_SCALE = 0.1f;
	private static readonly float SCROLLING_SCALE = 1.2f;

	//What will we create when we left click?
	public static int creationType;

	private static ArrayList ghost;
	private static int lastXSize, lastYSize;

	// Use this for initialization
	void Start () {
		anchor = NO_POINT;

		creationType = 0;

		ghost = new ArrayList ();
		lastXSize = -1;
		lastYSize = -1;
	}
	
	// Update is called once per frame
	void Update () {
		//WASD to pan around
		float xMovement = -CameraController.getHeight() * PANNING_SCALE * Input.GetAxis("Horizontal");
		float yMovement = -CameraController.getHeight() * PANNING_SCALE * Input.GetAxis ("Vertical");

		CameraController.addX(xMovement);
		CameraController.addY(yMovement);

		//RMB also can pan around
		if (Input.GetMouseButton (1)) {
			//We need to calculate the distance from the camera to get world coords
			Vector3 clickedPoint = Input.mousePosition;
			clickedPoint.z = -CameraController.getHeight();
			clickedPoint = CameraController.getCamera().ScreenToWorldPoint (clickedPoint);

			//Set initial point, if needed
			if (anchor == NO_POINT)
				anchor = clickedPoint;
			//Move the delta between the init and current point
			else {
				CameraController.addX(anchor.x - clickedPoint.x);
				CameraController.addY(anchor.y - clickedPoint.y);
			}
		} else {
			//anchor = NO_POINT; //clear anchor
		}

		if (Input.GetMouseButton (0)) {
			Vector3 clickedPoint = Input.mousePosition;
			clickedPoint.z = -CameraController.getHeight ();
			clickedPoint = CameraController.getCamera ().ScreenToWorldPoint (clickedPoint);

			//Round to nearest integer
			clickedPoint.x = Mathf.Round (clickedPoint.x);
			clickedPoint.y = Mathf.Round (clickedPoint.y);

			//Set initial point, if needed. Round to nearest integer.
			if (anchor == NO_POINT) 
				anchor = clickedPoint;
			else {
				int xSize = (int)Mathf.Abs (anchor.x - clickedPoint.x);
				int ySize = (int)Mathf.Abs (anchor.y - clickedPoint.y);

				if (lastXSize == -1)
					lastXSize = xSize;
				if (lastYSize == -1)
					lastYSize = ySize;

				switch (creationType) {
				case 0:
					if (xSize != lastXSize || ySize != lastYSize) {
						//Debug.Log ("Size updated. Selection size: " + xSize + "x" + ySize);
						if (xSize >= 2 && ySize >= 2) {
							lastXSize = xSize;
							lastYSize = ySize;

							//Clearing code
							foreach (GameObject g in ghost)
								Game.destroy (g);
							ghost.Clear ();

							//Create a room
							ghost = Game.createRoom ((int)anchor.x, (int)anchor.y, xSize, -ySize, ID.BRICK);
						}
					}
					break;
				case 1:
					//Game.
					break;
				}
			}
		} else {
			//anchor = NO_POINT;

			//foreach (GameObject g in ghost)
			//	Game.destroy (g);
			ghost.Clear ();
		}

		if (!Input.GetMouseButton (0) && !Input.GetMouseButton (1))
			anchor = NO_POINT;

		//Handle scrolling
		CameraController.addHeight(SCROLLING_SCALE * Input.GetAxis ("Mouse ScrollWheel"));
		
	}
}
