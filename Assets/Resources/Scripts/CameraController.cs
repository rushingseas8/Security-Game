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
