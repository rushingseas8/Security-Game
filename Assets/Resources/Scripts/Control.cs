using UnityEngine;
using System.Collections;

public class Control : MonoBehaviour {

	private static Vector2 anchor;

	//This is the equivalent of a null value for Vector2s
	private static readonly Vector2 NO_POINT = new Vector2(0, 0);

	private static readonly float PANNING_SCALE = 0.1f;
	private static readonly float SCROLLING_SCALE = 1.2f;

	private static ArrayList ghost;
	private static int lastXSize, lastYSize;

	// Use this for initialization
	void Start () {
		anchor = NO_POINT;

		lastXSize = -1;
		lastYSize = -1;
		ghost = new ArrayList ();
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
			if (anchor == NO_POINT) {
				Debug.Log ("Setting anchor");
				anchor = clickedPoint;
			} else {
				int xSize = (int)Mathf.Abs (anchor.x - clickedPoint.x);
				int ySize = (int)Mathf.Abs (anchor.y - clickedPoint.y);

				Debug.Log (xSize + "x" + ySize);

				if (xSize == lastXSize && ySize == lastYSize) {
					//goto EXIT_LMB;
				}

				if (lastXSize == -1)
					lastXSize = xSize;
				if (lastYSize == -1)
					lastYSize = ySize;
				
				if (xSize >= 2 && ySize >= 2) {
					Debug.Log ("Creating a room!");

					//clear
					foreach (GameObject g in ghost)
						Game.destroy (g);
					ghost.Clear ();

					ghost = Game.createRoom ((int)anchor.x, (int)anchor.y, xSize, -ySize, ID.DRYWALL);
				}
			}
		} else {
			//anchor = NO_POINT;

			//foreach (GameObject g in ghost)
			//	Game.destroy (g);
			ghost.Clear ();
		}

		//EXIT_LMB:;

		if (!Input.GetMouseButton (0) && !Input.GetMouseButton (1))
			anchor = NO_POINT;

		//Handle scrolling
		CameraController.addHeight(SCROLLING_SCALE * Input.GetAxis ("Mouse ScrollWheel"));
		
	}
}
