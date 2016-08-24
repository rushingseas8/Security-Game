using UnityEngine;
using System.Collections;

public class UIController : MonoBehaviour {

	public static int buttonSelection = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void buttonClicked(int value) {
		Debug.Log ("Clicked button " + value);
		buttonSelection = value;
	}
}
