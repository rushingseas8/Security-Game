using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {
	
	//All of the objects in the world
	public static ArrayList objects;

	public static int size;
	public static int[,] levelArray;

	// Use this for initialization
	void Start () {
		objects = new ArrayList ();

		size = 100;
		levelArray = new int[size, size];
		for (int i = 0; i < size; i++)
			for (int j = 0; j < size; j++)
				levelArray [i, j] = ID.EMPTY;

		//create ("Wall_Vertical", 3, 3);
		createRoom(2, 2, 6, 4, ID.DRYWALL);
		createRoom(9, 4, 3, 3, ID.BRICK);
		createRoom (4, 9, 6, 8, ID.WOOD);
		//createRoom(4, 4, 2, 2, ID.DRYWALL);

		//createRoom (0, 0, 12, 10, ID.BRICK);
		//createRoom (0, 6, 4, 4, ID.DRYWALL);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public static GameObject create(string name) {
		GameObject go = (GameObject)Instantiate (Resources.Load ("Prefabs/" + name));
		objects.Add (go);
		return go;
	}

	public static GameObject create(string name, float x, float y, float z = 0) {
		GameObject go = (GameObject)Instantiate (Resources.Load ("Prefabs/" + name), new Vector3(x, y, z), Quaternion.identity);
		objects.Add (go);
		return go;
	}

	public static GameObject create(string name, float x, float y, float z, Vector3 rot) {
		GameObject go = (GameObject)Instantiate (Resources.Load ("Prefabs/" + name), new Vector3(x, y, z), Quaternion.Euler(rot));
		objects.Add (go);
		return go;
	}

	public static Material loadMaterial(string name) {
		return (Material)Resources.Load ("Materials/" + name);
	}

	public static void destroy(GameObject go) {
		int x = (int)Mathf.Round (go.transform.position.x);
		int y = (int)Mathf.Round (go.transform.position.y);

		if (x >= 0 && x < size && y >= 0 && y < size && levelArray[x, y] == ID.WALL)
			levelArray [x, y] = ID.EMPTY;

		objects.Remove (go);
		Destroy (go);
	}

	public static ArrayList createRoom(int x, int y, int width, int height, int wallID) {
		//Don't even try invalid input
		if (x < 0 || y < 0 || (x + width >= size) || (y + height >= size) || wallID >= ID.walls.Length)
			return new ArrayList();

		//Correct semi-valid inputs
		if (width < 0) 
			x -= (width = -width);

		if (height < 0)
			y -= (height = -height);

		//Check if proposed room overlaps
		/*
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				if (levelArray [x + i, y + j] == ID.WALL) {
					/**
					 * Add this to a list, and essentially split this process into
					 * multiple more to avoid any conflicts! Recursion
					 
				}
			}
		}*/

		for (int i = 0; i < width; i++) //bottom
			levelArray [x + i, y] = ID.WALL;

		for (int i = 0; i < width; i++) //top
			levelArray [x + i, y + height - 1] = ID.WALL;

		for (int i = 1; i < height; i++) //left
			levelArray [x, y + i] = ID.WALL;

		for (int i = 1; i < height; i++) //right
			levelArray [x + width - 1, y + i] = ID.WALL;

		Material mat = loadMaterial (ID.walls [wallID]);

		ArrayList toReturn = new ArrayList();

		//Actually create the wall GameObjects
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				if (levelArray [x + i, y + j] == ID.WALL) {
					GameObject go = createWallAt(x + i, y + j);
					if (go.GetComponent<Renderer> () != null)
						go.GetComponent<Renderer> ().material = mat;
					else
						for (int k = 0; k < go.transform.childCount; k++) {
							go.transform.GetChild (k).GetComponent<Renderer> ().material = mat;
							go.transform.GetChild (k).GetComponent<Renderer> ().material.mainTextureScale = 
								new Vector2 (1f, 0.2f);
						}

					toReturn.Add(go);
				}
			}
		}

		toReturn.Add(create ("light", x + (width / 2), y + (height / 2), -2));
		toReturn.Add(generateFloor (x, y, width, height));

		return toReturn;
	}

	public static GameObject generateFloor(float x, float y, float width, float height) {
		GameObject floor = create ("floor", x - 0.5f + (width / 2f), y - 0.5f + (height / 2f), 0.1f);
		floor.transform.localScale = new Vector3 (width - 1, height - 1, 0.2f);

		return floor;
	}

	/**
	 * Creates a wall based on the surrounding walls.
	 */
	private static GameObject createWallAt(int x, int y) {
		bool north = (y != size) && levelArray [x, y + 1] == ID.WALL;
		bool south = (y != 0) && levelArray [x, y - 1] == ID.WALL;
		bool east = (x != size) && levelArray[x + 1, y] == ID.WALL;
		bool west = (x != 0) && levelArray[x - 1, y] == ID.WALL;

		//Straight, full length
		if (north && south && !east && !west)
			return create ("wall_full", x, y);
		if (!north && !south && east && west)
			return create ("wall_full", x, y, 0, new Vector3(0, 0, 90));

		//Straight, half-length (for doors, windows)
		if (north && !south && !east && !west)
			return create ("wall_half", x, y);
		if (!north && south && !east && !west)
			return create ("wall_half", x, y, 0, new Vector3(0, 0, 180));
		if (!north && !south && east && !west)
			return create ("wall_half", x, y, 0, new Vector3(0, 0, 90));
		if (!north && !south && !east && west)
			return create ("wall_half", x, y, 0, new Vector3(0, 0, 270));

		//Corner
		if (north && !south && east && !west)
			return create ("wall_corner", x, y);
		if (north && !south && !east && west)
			return create ("wall_corner", x, y, 0, new Vector3(0, 0, 90));
		if (!north && south && east && !west)
			return create ("wall_corner", x, y, 0, new Vector3(0, 0, 270));
		if (!north && south && !east && west)
			return create ("wall_corner", x, y, 0, new Vector3(0, 0, 180));

		//T shaped
		if (north && south && east && !west)
			return create ("wall_t", x, y);
		if (!north && south && east && west)
			return create ("wall_t", x, y, 0, new Vector3(0, 0, 270));
		if (north && south && !east && west)
			return create ("wall_t", x, y, 0, new Vector3(0, 0, 180));
		if (north && !south && east && west)
			return create ("wall_t", x, y, 0, new Vector3(0, 0, 90));

		//All 4 sides connected
		if (north && south && east && west)
			return create ("wall_all", x, y);

		//Else, nothing is connected, so return a pillar
		return create("wall_center", x, y);
	}
}
