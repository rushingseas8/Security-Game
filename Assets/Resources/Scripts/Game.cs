using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour {
	
	//All of the objects in the world
	//public static ArrayList objects;
	private static Storage storage;

	//public static int size;
	//public static int[,] levelArray;

	// Use this for initialization
	void Start () {
		//objects = new ArrayList ();
		storage = new Storage();

		//size = 100;
		//levelArray = new int[size, size];
		//for (int i = 0; i < size; i++)
		//	for (int j = 0; j < size; j++)
		//		levelArray [i, j] = ID.EMPTY;

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
		//int x = (int)Mathf.Round (go.transform.position.x);
		//int y = (int)Mathf.Round (go.transform.position.y);

		storage.remove (go);
		Destroy (go);

		//if (x >= 0 && x < size && y >= 0 && y < size && levelArray[x, y] == ID.WALL)
		//	levelArray [x, y] = ID.EMPTY;
	}

	/**
	 * Attempts to create a room with the given parameters.
	 * Will overwrite any wall objects below it.
	 */
	public static List<GameObject> createRoom(int x, int y, int width, int height, int wallID) {
		//Debug.Log ("Create room called. x=" + x + " y=" + y + " w=" + width + " h=" + height + " id=" + wallID);

		//Don't try invalid input
		//if (x < 0 || y < 0 || (x + width >= size) || (y + height >= size) || wallID >= ID.walls.Length)
		//	return new ArrayList();

		if (wallID >= ID.walls.Length)
			return new List<GameObject>();

		//Correct semi-valid inputs
		if (width < 0) 
			x -= (width = -width);

		if (height < 0)
			y -= (height = -height);

		bool[,] prev = new bool[width, height];

		//Check if proposed room overlaps anything
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				if (storage.containsGrid ("wall", x + i, y + j)) {
					prev [i, j] = true;
					/**
					 * Add this to a list, and essentially split this process into
					 * multiple more to avoid any conflicts! Recursion
					 */
				} else
					prev [i, j] = false;
			}
		}

		List<GameObject> toReturn = new List<GameObject>();

		Material mat = loadMaterial (ID.walls [wallID]);

		for (int i = 0; i < width; i++) {//bottom
			if (!storage.containsGrid ("wall", x + i, y)) {
				//Create wall - make sure to update surrounding
				//GameObject go = createWallWithUpdate (x + i, y, mat);


				//Assign material
				/*
				if (go.GetComponent<Renderer> () != null)
					go.GetComponent<Renderer> ().material = mat;
				else
					for (int k = 0; k < go.transform.childCount; k++) {
						go.transform.GetChild (k).GetComponent<Renderer> ().material = mat;
						go.transform.GetChild (k).GetComponent<Renderer> ().material.mainTextureScale = 
							new Vector2 (1f, 0.2f);
					}*/

				//toReturn.Add(go);
				//toReturn.AddRange(createWallWithUpdate(x + i, y, mat));

				createWallWithUpdate (x + i, y, mat);
			}
			//levelArray [x + i, y] = ID.WALL;
		}


		for (int i = 0; i < width; i++) //top
			if(!storage.containsGrid("wall", x + i, y + height - 1))
				createWallWithUpdate(x + i, y + height - 1, mat);

		for (int i = 1; i < height; i++) //left
			if(!storage.containsGrid("wall", x, y + i))
				createWallWithUpdate(x, y + i, mat);

		for (int i = 1; i < height; i++) //right
			if(!storage.containsGrid("wall", x + width - 1, y + i))
				createWallWithUpdate(x + width - 1, y + i, mat);


		toReturn = storage.getBetweenGrid ("wall", x, y, x + width, y + height);

		//Exclude previous items
		//for (int i = 0; i < toReturn.Count; i++) {
		//	int xVal = (int)toReturn [i].transform.position.x - x;
		//	int yVal = (int)toReturn [i].transform.position.y - y;
		//	if (prev [xVal, yVal])
		//		toReturn.RemoveAt (i);
		//}


		//Remove all elements that aren't "ghost" ones
		//for (int i = 0; i < toReturn.Count; i++)
		//	if (toReturn [i].tag != "Ghost")
		//		toReturn.RemoveAt (i);

		//Actually create the wall GameObjects
		/*
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				//if (levelArray [x + i, y + j] == ID.WALL) {
				if(storage.containsGrid("wall", x + i, y + j)) {
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
		}*/

		toReturn.Add(create ("light", x + (width / 2), y + (height / 2), -2));
		toReturn.Add(generateFloor (x, y, width, height));

		return toReturn;
	}

	public static List<GameObject> createRoomGhost(int x, int y, int width, int height) {
		List<GameObject> toReturn = new List<GameObject>();

		//Correct semi-valid inputs
		if (width < 0) 
			x -= (width = -width);

		if (height < 0)
			y -= (height = -height);
		
		GameObject temp;
		for (int i = 0; i < width; i++) { //bottom
			if (!storage.containsGrid ("wall", x + i, y)) {
				temp = createWallWithUpdate (x + i, y, null);
				temp.tag = "Ghost";
			}
		}

		for (int i = 0; i < width; i++) //top
			if (!storage.containsGrid ("wall", x + i, y + height - 1)) {
				temp = createWallWithUpdate (x + i, y + height - 1, null);
				temp.tag = "Ghost";
			}

		for (int i = 1; i < height; i++) //left
			if (!storage.containsGrid ("wall", x, y + i)) {
				temp = createWallWithUpdate (x, y + i, null);
				temp.tag = "Ghost";
			}

		for (int i = 1; i < height; i++) //right
			if (!storage.containsGrid ("wall", x + width - 1, y + i)) {
				temp = createWallWithUpdate (x + width - 1, y + i, null);
				temp.tag = "Ghost";
			}


		toReturn = storage.getBetweenGrid ("wall", x, y, x + width, y + height);

		//Remove all elements that aren't "ghost" ones
		for (int i = 0; i < toReturn.Count; i++)
			if (toReturn [i].tag != "Ghost")
				toReturn.RemoveAt (i);

		temp = create ("light", x + (width / 2), y + (height / 2), -2);
		temp.tag = "Ghost";
		toReturn.Add(temp);

		temp = generateFloor (x, y, width, height);
		temp.tag = "Ghost";
		toReturn.Add(temp);

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
	private static GameObject createWall(int x, int y, Material mat) {
		//bool north = (y != size) && levelArray [x, y + 1] == ID.WALL;
		//bool south = (y != 0) && levelArray [x, y - 1] == ID.WALL;
		//bool east = (x != size) && levelArray[x + 1, y] == ID.WALL;
		//bool west = (x != 0) && levelArray[x - 1, y] == ID.WALL;

		bool north = storage.containsGrid ("wall", x, y + 1);
		bool south = storage.containsGrid ("wall", x, y - 1);
		bool east = storage.containsGrid ("wall", x + 1, y);
		bool west = storage.containsGrid ("wall", x - 1, y);

		GameObject newWall = null;

		//Straight, full length
		if (north && south && !east && !west)
			newWall = create ("wall_full", x, y);
		else if (!north && !south && east && west)
			newWall = create ("wall_full", x, y, 0, new Vector3(0, 0, 90));

		//Straight, half-length (for doors, windows)
		else if (north && !south && !east && !west)
			newWall = create ("wall_half", x, y);
		else if (!north && south && !east && !west)
			newWall = create ("wall_half", x, y, 0, new Vector3(0, 0, 180));
		else if (!north && !south && east && !west)
			newWall = create ("wall_half", x, y, 0, new Vector3(0, 0, 270));
		else if (!north && !south && !east && west)
			newWall = create ("wall_half", x, y, 0, new Vector3(0, 0, 90));

		//Corner
		else if (north && !south && east && !west)
			newWall = create ("wall_corner", x, y);
		else if (north && !south && !east && west)
			newWall = create ("wall_corner", x, y, 0, new Vector3(0, 0, 90));
		else if (!north && south && east && !west)
			newWall = create ("wall_corner", x, y, 0, new Vector3(0, 0, 270));
		else if (!north && south && !east && west)
			newWall = create ("wall_corner", x, y, 0, new Vector3(0, 0, 180));

		//T shaped
		else if (north && south && east && !west)
			newWall = create ("wall_t", x, y);
		else if (!north && south && east && west)
			newWall = create ("wall_t", x, y, 0, new Vector3(0, 0, 270));
		else if (north && south && !east && west)
			newWall = create ("wall_t", x, y, 0, new Vector3(0, 0, 180));
		else if (north && !south && east && west)
			newWall = create ("wall_t", x, y, 0, new Vector3(0, 0, 90));

		//All 4 sides connected
		else if (north && south && east && west)
			newWall = create ("wall_all", x, y);

		//Else, nothing is connected, so return a pillar
		else
			newWall = create("wall_center", x, y);

		//Assign texture to the newly created wall
		for (int i = 0; i < newWall.transform.childCount; i++) {
			newWall.transform.GetChild (i).GetComponent<Renderer> ().material = mat;
			newWall.transform.GetChild (i).GetComponent<Renderer> ().material.mainTextureScale = 
				new Vector2 (1f, 0.2f);
		}

		return newWall;
	}

	public static GameObject createWallWithUpdate(int x, int y, Material mat) {
		//ArrayList toReturn = new ArrayList ();
		GameObject wall = createWall (x, y, mat);
		//toReturn.Add (wall);
		for (int i = -1; i <= 1; i++) {
			for (int j = -1; j <= 1; j++) {
				if (i == 0 && j == 0)
					continue;

				updateWall (x + i, y + j, mat);
			}
		}
		return wall;
	}

	//returns: the update GO, or null if nothing to update
	public static GameObject updateWall(int x, int y, Material mat) {
		if (storage.contains ("wall", x, y)) {
			List<GameObject> list = storage.getAtGrid (x, y);

			//For any walls found at a given position, delete them
			string oldTag = "Untagged";
			Material oldMat = null;
			for (int i = 0; i < list.Count; i++) {
				if (list [i].name.Contains ("wall")) {
					oldTag = list [i].tag;
					oldMat = list [i].transform.GetChild (0).GetComponent<Renderer> ().material;
					destroy (list [i]);
				}
			}

			//recreate
			GameObject go = createWall (x, y, mat);
			go.tag = oldTag;
			for (int i = 0; i < go.transform.childCount; i++) {
				go.transform.GetChild (i).GetComponent<Renderer> ().material = mat == null ? oldMat : mat;
				go.transform.GetChild (i).GetComponent<Renderer> ().material.mainTextureScale = 
				new Vector2 (1f, 0.2f);
			}
		}
		return null;
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

		public List<GameObject> getBetweenGrid(string name, float xStart, float yStart, float xEnd, float yEnd) {
			List<GameObject> list = new List<GameObject> ();
			for (int i = (int)xStart; i <= (int)xEnd; i++) {
				for (int j = (int)yStart; j <= (int)yEnd; j++) {
					List<GameObject> smallList = getAtGrid (i, j);
					for (int k = 0; k < smallList.Count; k++)
						if (smallList [k].transform.name.Contains (name))
							list.Add (smallList [k]);
				}
			}
			Debug.Log ("Between (" + xStart + ", " + yStart + ") and (" + xEnd + ", " + yEnd + 
				" found " + list.Count + " objects with name containing \"" + name + "\".");
			return list;
		}
			
		public List<GameObject> getAtGrid(float x, float y, float z = 0) {
			return getAtGrid (new Vector3 (x, y, z));
		}

		public List<GameObject> getAtGrid(Vector3 pos) {
			//Debug.Log ("getAtGrid called @x= " + pos.x + " y= " + pos.y + " z= " + pos.z);
			pos.x = (int)(pos.x);
			pos.y = (int)(pos.y);
			pos.z = (int)(pos.z);

			List<GameObject> toReturn = new List<GameObject> ();
			for (int i = 0; i < storage.Count; i++) {
				//Get rounded position of GameObject
				Vector3 gridPos = storage [i].transform.position;
				gridPos.x = (int)(gridPos.x);
				gridPos.y = (int)(gridPos.y);
				gridPos.z = (int)(gridPos.z);

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
			//Debug.Log ("containsGrid - getAtGrid list size: " + list.Count);
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
