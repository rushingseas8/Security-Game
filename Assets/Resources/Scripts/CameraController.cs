using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	private static Camera mainCamera;

	private static float x, y;
	private static float height;

	private static Vector2 anchor;

	//This is the equivalent of a null value for Vector2s
	private static readonly Vector2 NO_POINT = new Vector2(0, 0);

	private static readonly float PANNING_SCALE = 0.1f;
	private static readonly float SCROLLING_SCALE = 3f;

	//Note that the camera is in negative z by default
	private static readonly float MIN_HEIGHT = -2f;

	// Use this for initialization
	void Start () {
		mainCamera = Camera.main;

		x = mainCamera.transform.position.x;
		y = mainCamera.transform.position.y;

		height = mainCamera.transform.position.z;

		anchor = NO_POINT;
	}
	
	// Update is called once per frame
	void Update () {
		//WASD to pan around
		float xMovement = -height * PANNING_SCALE * Input.GetAxis("Horizontal");
		float yMovement = -height * PANNING_SCALE * Input.GetAxis ("Vertical");

		x += xMovement;
		y += yMovement;

		//LMB also can pan around
		if (Input.GetMouseButton (0)) {
			//We need to calculate the distance from the camera to get world coords
			Vector3 clickedPoint = Input.mousePosition;
			clickedPoint.z = -height;
			clickedPoint = mainCamera.ScreenToWorldPoint (clickedPoint);

			//Set initial point, if needed
			if (anchor == NO_POINT)
				anchor = clickedPoint;
			//Move the delta between the init and current point
			else {
				x += anchor.x - clickedPoint.x;
				y += anchor.y - clickedPoint.y;
			}
		} else
			anchor = NO_POINT; //clear anchor

		//Handle scrolling
		height += Input.GetAxis ("Mouse ScrollWheel");
		if (height >= MIN_HEIGHT)
			height = MIN_HEIGHT;

		doMove ();
	}

	private void doMove() {
		mainCamera.transform.position = new Vector3 (x, y, height);
	}
}
