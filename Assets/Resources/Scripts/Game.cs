using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour {
	
	//All of the objects in the world
	//public static ArrayList objects;
	private static Storage storage;

	public static int size;
	public static int[,] levelArray;

	// Use this for initialization
	void Start () {
		//objects = new ArrayList ();
		storage = new Storage();

		size = 100;
		levelArray = new int[size, size];
		for (int i = 0; i < size; i++)
			for (int j = 0; j < size; j++)
				levelArray [i, j] = ID.EMPTY;

		//create ("Wall_Vertical", 3, 3);
		//createRoom(2, 2, 6, 4, ID.DRYWALL);
		//createRoom(9, 4, 3, 3, ID.BRICK);
		//createRoom (4, 9, 6, 8, ID.WOOD);
		//createRoom(4, 4, 2, 2, ID.DRYWALL);

		//createRoom (0, 0, 12, 10, ID.BRICK);
		//createRoom (0, 6, 4, 4, ID.DRYWALL);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public static GameObject create(string name) {
		GameObject go = (GameObject)Instantiate (Resources.Load ("Prefabs/" + name));
		storage.add (go);
		return go;
	}

	public static GameObject create(string name, float x, float y, float z = 0) {
		GameObject go = (GameObject)Instantiate (Resources.Load ("Prefabs/" + name), new Vector3(x, y, z), Quaternion.identity);
		storage.add (go);
		return go;
	}

	public static GameObject create(string name, float x, float y, float z, Vector3 rot) {
		GameObject go = (GameObject)Instantiate (Resources.Load ("Prefabs/" + name), new Vector3(x, y, z), Quaternion.Euler(rot));
		storage.add (go);
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

		storage.remove (go);
		Destroy (go);
	}

	public static ArrayList createRoom(int x, int y, int width, int height, int wallID) {
		//Debug.Log ("Create room called. x=" + x + " y=" + y + " w=" + width + " h=" + height + " id=" + wallID);

		//Don't try invalid input
		if (x < 0 || y < 0 || (x + width >= size) || (y + height >= size) || wallID >= ID.walls.Length)
			return new ArrayList();

		//Correct semi-valid inputs
		if (width < 0) 
			x -= (width = -width);

		if (height < 0)
			y -= (height = -height);

		//Check if proposed room overlaps
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				if(storage.containsGrid("wall", x + i, y + j)) {
					/**
					 * Add this to a list, and essentially split this process into
					 * multiple more to avoid any conflicts! Recursion
					 */
					//return new ArrayList ();
				}
			}
		}

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
				//if(storage.containsGrid("wall", x + i, y + j)) {
					GameObject go = createWall(x + i, y + j);
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

		//rescale texture to make it look good
		floor.GetComponent<Renderer> ().material.mainTextureScale = new Vector2((width - 1) / 3, (height - 1) / 3);

		return floor;
	}

	/**
	 * Creates a wall based on the surrounding walls.
	 */
	private static GameObject createWall(int x, int y) {
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

	public static GameObject createWallWithUpdate(int x, int y) {
		return createWall (x, y);
		/*
		for (int i = -1; i <= 1; i++) {
			for (int j = -1; j <= 1; j++) {
				if (isValid (x + i, y + j)) {
					if (storage.contains ("wall", x + i, y + j)) {
						foreach (GameObject go in storage.getAt(x + i, y + j))
							destroy (go);
						createWall (x + i, y + j);
					}
				}
			}
		}*/
	}

	private static bool isValid(int x, int y) {
		return (x >= 0 && y >= 0 && x < size && y < size);
	}

	/**
	 * A helper class that handles internal storage for the game.
	 */
	private class Storage {
		private List<GameObject> storage;

		public Storage() {
			storage = new List<GameObject> ();
		}

		/**
		 * Adds the given GameObject to the storage list. Automatically places it in
		 * sorted order, which is based on position. 
		 * 
		 * Sorting order: least-to-most pattern, done for x -> y -> z in order.
		 * 
		 * To-do: make this method use binary 
		 */
		public void add(GameObject go) {
			//if (storage.Count == 0)
			//	storage.Add (go);

			//Linear search that adds at first match (or greater element)
			//This means for collisions, most recent has a smaller index
			//for (int i = 0; i < storage.Count; i++) {
			//	if (compare (storage [i], go) >= 0) {
			//		storage.Insert (i, go);
			//		return;
			//	}
			//}

			storage.Add(go);
			//Debug.Log ("Adding a gameobject! Size is now " + storage.Count);
		}
			
		public List<GameObject> getAtGrid(float x, float y, float z = 0) {
			return getAtGrid (new Vector3 (x, y, z));
		}

		public List<GameObject> getAtGrid(Vector3 pos) {
			//Debug.Log ("getAtGrid called @x= " + pos.x + " y= " + pos.y + " z= " + pos.z);
			pos.x = Mathf.Round(pos.x);
			pos.y = Mathf.Round(pos.y);
			pos.z = Mathf.Round(pos.z);

			List<GameObject> toReturn = new List<GameObject> ();
			for (int i = 0; i < storage.Count; i++) {
				//Get rounded position of GameObject
				Vector3 gridPos = storage [i].transform.position;
				gridPos.x = Mathf.Round(gridPos.x);
				gridPos.y = Mathf.Round(gridPos.y);
				gridPos.z = Mathf.Round(gridPos.z);

				//Debug.Log ("\tcomparing with x= " + gridPos.x + " y= " + gridPos.y + " z= " + gridPos.z);

				if (compare (gridPos, pos) == 0) {
					toReturn.Add (storage [i]);
				}
			}
			return toReturn;
		}

		public List<GameObject> getAt(float x, float y, float z = 0) {
			return getAt (new Vector3 (x, y, z));
		}

		public List<GameObject> getAt(Vector3 pos) {
			/*
			//First object that matches
				int startIndex = 0;
				while (startIndex < storage.Count && (compare (storage [startIndex].transform.position, pos) != 0))
					++startIndex;

				//No match - return empty list
				if (startIndex == storage.Count)
					return new List<GameObject> ();

				//Last object that matches
				int endIndex = startIndex;
				while (endIndex < storage.Count && (compare (storage [endIndex].transform.position, pos) != 0))
					++endIndex;

				return storage.GetRange (startIndex, endIndex - startIndex);
			*/
			//Debug.Log ("getAt storage size: " + storage.Count);

			List<GameObject> toReturn = new List<GameObject> ();
			for (int i = 0; i < storage.Count; i++) {
				//Debug.Log (storage [i]);
				if (compare (storage [i].transform.position, pos) == 0)
					toReturn.Add (storage [i]);
			}
			return toReturn;
		}

		/**
		 * Returns true if the given position contains an object that has /name/ as
		 * a substring. For example, if a position has something named "wall_left", and
		 * you search for "wall" at that position, then this returns true.
 		 */	
		public bool contains(string name, float x, float y, float z = 0) {
			name = name.ToLower ();
			List<GameObject> list = getAt (x, y, z);
			for (int i = 0; i < list.Count; i++)
				if (list [i].transform.name.Contains(name))
					return true;
			return false;
		}

		public bool containsGrid(string name, float x, float y, float z = 0) {
			name = name.ToLower ();
			List<GameObject> list = getAtGrid (x, y, z);
			Debug.Log ("containsGrid - getAtGrid list size: " + list.Count);
			for (int i = 0; i < list.Count; i++)
				if (list [i].transform.name.Contains(name))
					return true;
			return false;
		}

		public void remove(GameObject go) {
			storage.Remove (go);
			//Debug.Log ("Removing object - size is now " + storage.Count);
		}

		/**
		 * Compares GameObject's positions and returns -1, 0, or 1 based on if
		 * the first is less than, equal to, or greater than the second, respectively.
		 */
		public int compare(GameObject first, GameObject second) {
			if (first == null || second == null)
				return 0;

			if (first == second)
				return 0;

			return compare (first.transform.position, second.transform.position);
		}

		/**
		 * Returns -1, 0, or 1 based on if first is less than, equal to, or
		 * greater than second, respectively.
		 */
		public int compare(Vector3 pos1, Vector3 pos2) {
			if (Mathf.Approximately (pos1.x, pos2.x)) {
				if (Mathf.Approximately (pos1.y, pos2.y)) {
					if (Mathf.Approximately (pos1.z, pos2.z)) {
						return 0;
					} else if (pos1.z < pos2.z) {
						return -1;
					} else {
						return 1;
					}
				} else if (pos1.y < pos2.y) {
					return -1;
				} else {
					return 1;
				}
			} else if (pos1.x < pos2.x) {
				return -1;
			} else {
				return 1;
			}
		}
	}

}
