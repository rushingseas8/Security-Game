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
				levelArray [i, j] = id.EMPTY;

		//create ("Wall_Vertical", 3, 3);
		createRoom(2, 2, 6, 4, id.DRYWALL);
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

	public static void createRoom(int x, int y, int width, int height, int wallID) {
		for (int i = 0; i < width; i++) //bottom
			levelArray [x + i, y] = id.WALL;

		for (int i = 0; i < width; i++) //top
			levelArray [x + i, y + height - 1] = id.WALL;

		for (int i = 1; i < height; i++) //left
			levelArray [x, y + i] = id.WALL;

		for (int i = 1; i < height; i++) //right
			levelArray [x + width - 1, y + i] = id.WALL;

		//Actually create the wall GameObjects
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				if (levelArray [x + i, y + j] == id.WALL) {
					GameObject go = create (getWallType (x + i, y + j), x + i, y + j);
					if (go.GetComponent<Renderer> () != null)
						go.GetComponent<Renderer> ().material = loadMaterial("drywall");
					
					else 
						for(int k = 0; k < go.transform.childCount; k++)
							go.transform.GetChild(k).GetComponent<Renderer>().material = loadMaterial("drywall");
				}
			}
		}

		create ("light", x + (width / 2), y + (height / 2), -2);

		generateFloor (x, y, width, height);
	}

	public static void generateWalls(int wallID) {
	}

	public static void generateFloor(float x, float y, float width, float height) {
		GameObject floor = create ("floor", (x + width + 1) / 2f, (y + height + 1) / 2f, 0.1f);
		floor.transform.localScale = new Vector3 (width - 1, height - 1, 0.2f);
	}

	/**
	 * Return the wall type (prefab name) based on surrounding walls,
	 * given a position.
	 */
	private static string getWallType(int x, int y) {
		bool north = (y != size) && levelArray [x, y + 1] == id.WALL;
		bool south = (y != 0) && levelArray [x, y - 1] == id.WALL;
		bool east = (x != size) && levelArray[x + 1, y] == id.WALL;
		bool west = (x != 0) && levelArray[x - 1, y] == id.WALL;

		if (north && south && !east && !west)
			return "wall_vertical";
		if (!north && !south && east && west)
			return "wall_horizontal";

		if (north && !south && !east && !west)
			return "wall_north";
		if (!north && south && !east && !west)
			return "wall_south";
		if (!north && !south && east && !west)
			return "wall_east";
		if (!north && !south && !east && west)
			return "wall_west";
		
		if (north && !south && east && !west)
			return "wall_northeast";
		if (north && !south && !east && west)
			return "wall_northwest";		
		if (!north && south && east && !west)
			return "wall_southeast";
		if (!north && south && !east && west)
			return "wall_southwest";

		return "wall_center";
	}
}
