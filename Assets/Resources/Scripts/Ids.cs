using UnityEngine;
using System.Collections;

/**
 * A standard-violating class that provides easy, global access to 
 * key variables in the game.
 */
public class id {
	//What is in a tile in the world?
	public const int EMPTY = 0;
	public const int WALL = 1;

	//Types of walls - string array
	public static string[] walls = {
		"drywall", "wood", "brick"
	};

	//Types of walls - ids of above
	public const int DRYWALL = 0;
	public const int WOOD = 1;
	public const int BRICK = 2;
}
