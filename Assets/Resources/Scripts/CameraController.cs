using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	private static Camera mainCamera;

	private static float x, y;
	private static float height;

	//Note that the camera is in negative z by default
	private static readonly float MIN_HEIGHT = -2.5f;

	// Use this for initialization
	void Start () {
		mainCamera = Camera.main;

		x = mainCamera.transform.position.x;
		y = mainCamera.transform.position.y;

		height = mainCamera.transform.position.z;
	}
	
	// Update is called once per frame
	void Update () {
		doMove ();
	}

	public static void addX(float amt) {
		x += amt;
	}

	public static void addY(float amt) {
		y += amt;
	}

	public static void addHeight(float amt) {
		height += amt;
		if (height >= MIN_HEIGHT)
			height = MIN_HEIGHT;
	}

	//Add rotation in degrees
	public static void addTilt(float amt) {
		mainCamera.transform.Rotate (new Vector3 (amt, 0, 0));	

		//Because of the way that rotations are stored, we have to write this a bit strangely.
		float angle = mainCamera.transform.eulerAngles.x;
		if (angle > 30 && angle < 330)
			mainCamera.transform.eulerAngles = new Vector3 (330, 0, 0);

		if (angle > 359 || angle < 30)
			mainCamera.transform.eulerAngles = new Vector3 (0, 0, 0);
	}

	public static float getX() {
		return x;
	}

	public static float getY() {
		return y;
	}

	public static float getHeight() {
		return height;
	}

	public static Camera getCamera() {
		return mainCamera;
	}

	private void doMove() {
		mainCamera.transform.position = new Vector3 (x, y, height);
	}
}
