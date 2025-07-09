using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class DungeonGenerator : MonoBehaviour {
	public int dungeonRows;
	public int dungeonColumns;
	public int minRoomSize, maxRoomSize;
	public GameObject floorTile;
	public GameObject corridorTile;

	private GameObject[,] _boardPositionsFloor;
	
	void Start () {
		SubDungeon rootSubDungeon = new SubDungeon (new Rect (0, 0, dungeonRows, dungeonColumns));
		Partition (rootSubDungeon);
		rootSubDungeon.CreateRoom ();

		_boardPositionsFloor = new GameObject[dungeonRows, dungeonColumns];
		DrawRooms (rootSubDungeon);
		DrawCorridors (rootSubDungeon);
	}

	public class SubDungeon {
		public SubDungeon Left, Right;
		public Rect Rect;
		public Rect Room = new Rect(-1,-1, 0, 0); // i.e null
		public List<Rect> Corridors = new List<Rect>();


		public SubDungeon(Rect rect) 
		{
			this.Rect = rect;
		}

		public bool IsLeaf() 
		{
			return Left == null && Right == null;
		}

		public bool Split(int minRoomSize, int maxRoomSize) 
		{
			if (!IsLeaf()) 
			{
				return false;
			}

			// choose a vertical or horizontal split depending on the proportions
			// i.e. if too wide split vertically, or too long horizontally, 
			// or if nearly square choose vertical or horizontal at random
			bool splitHorizontally;
			
			if (Rect.width / Rect.height >= 1.25) 
			{
				splitHorizontally = false;
			} 
			else if (Rect.height / Rect.width >= 1.25) 
			{
				splitHorizontally = true;
			} 
			else 
			{
				splitHorizontally = Random.Range (0.0f, 1.0f) > 0.5;
			}

			if (Mathf.Min(Rect.height, Rect.width) / 2 < minRoomSize) 
			{
				return false;
			}

			if (splitHorizontally) 
			{
				// split so that the resulting sub-dungeons widths are not too small
				// (since we are splitting horizontally) 
				int split = Random.Range (minRoomSize, (int)(Rect.width - minRoomSize));

				Left = new SubDungeon (new Rect (Rect.x, Rect.y, Rect.width, split));
				Right = new SubDungeon (
					new Rect (Rect.x, Rect.y + split, Rect.width, Rect.height - split));
			}
			else 
			{
				int split = Random.Range (minRoomSize, (int)(Rect.height - minRoomSize));

				Left = new SubDungeon (new Rect (Rect.x, Rect.y, split, Rect.height));
				Right = new SubDungeon (
					new Rect (Rect.x + split, Rect.y, Rect.width - split, Rect.height));
			}

			return true;
		}

		public void CreateRoom() 
		{
			if (Left != null) 
			{
				Left.CreateRoom ();
			}
			
			if (Right != null) 
			{
				Right.CreateRoom ();
			}
			
			if (Left != null && Right != null) 
			{
				CreateCorridorBetween(Left, Right);
			}
			
			if (IsLeaf()) 
			{
				int roomWidth = (int)Random.Range (Rect.width / 2, Rect.width - 2);
				int roomHeight = (int)Random.Range (Rect.height / 2, Rect.height - 2);
				int roomX = (int)Random.Range (1, Rect.width - roomWidth - 1);
				int roomY = (int)Random.Range (1, Rect.height - roomHeight - 1);

				// room position will be absolute in the board, not relative to the sub-dungeon
				Room = new Rect (Rect.x + roomX, Rect.y + roomY, roomWidth, roomHeight);
			}
		}


		public void CreateCorridorBetween(SubDungeon left, SubDungeon right) 
		{
			Rect leftRoom = left.GetRoom ();
			Rect rightRoom = right.GetRoom ();

			// attach the corridor to a random point in each room
			Vector2 leftPoint = new Vector2 ((int)Random.Range (leftRoom.x + 1, leftRoom.xMax - 1), (int)Random.Range (leftRoom.y + 1, leftRoom.yMax - 1));
			Vector2 rightPoint = new Vector2 ((int)Random.Range (rightRoom.x + 1, rightRoom.xMax - 1), (int)Random.Range (rightRoom.y + 1, rightRoom.yMax - 1));

			// always be sure that left point is on the left to simplify the code
			if (leftPoint.x > rightPoint.x) 
			{
				Vector2 temp = leftPoint;
				leftPoint = rightPoint;
				rightPoint = temp;
			}
				
			int width = (int)(leftPoint.x - rightPoint.x);
			int height = (int)(leftPoint.y - rightPoint.y);

			// if the points are not aligned horizontally
			if (width != 0) 
			{ 
				// choose at random to go horizontal then vertical or the opposite
				if (Random.Range (0, 1) > 2) 
				{
					// add a corridor to the right
					Corridors.Add (new Rect (leftPoint.x, leftPoint.y, Mathf.Abs (width) + 1, 1));

					// if left point is below right point go up
					// otherwise go down
					if (height < 0) 
					{ 
						Corridors.Add (new Rect (rightPoint.x, leftPoint.y, 1, Mathf.Abs (height)));
					} 
					else 
					{
						Corridors.Add (new Rect (rightPoint.x, leftPoint.y, 1, -Mathf.Abs (height)));
					}
				} 
				else 
				{
					// go up or down
					if (height < 0) 
					{
						Corridors.Add (new Rect (leftPoint.x, leftPoint.y, 1, Mathf.Abs (height)));
					} 
					else 
					{
						Corridors.Add (new Rect (leftPoint.x, rightPoint.y, 1, Mathf.Abs (height)));
					}
					
					// then go right
					Corridors.Add (new Rect (leftPoint.x, rightPoint.y, Mathf.Abs (width) + 1, 1));
				}
			} 
			else 
			{
				// if the points are aligned horizontally
				// go up or down depending on the positions
				if (height < 0) 
				{
					Corridors.Add (new Rect ((int)leftPoint.x, (int)leftPoint.y, 1, Mathf.Abs (height)));
				} 
				else 
				{
					Corridors.Add (new Rect ((int)rightPoint.x, (int)rightPoint.y, 1, Mathf.Abs (height)));
				}
			} 
		}

		public Rect GetRoom() 
		{
			if (IsLeaf()) 
			{
				return Room;
			}
			
			if (Left != null) 
			{
				Rect leftRoom = Left.GetRoom ();
				
				if (leftRoom.x != -1) 
				{
					return leftRoom;
				}
			}
			
			if (Right != null) 
			{
				Rect rightRoom = Right.GetRoom ();
				
				if (rightRoom.x != -1) 
				{
					return rightRoom;
				}	
			}

			// workaround non nullable structs
			return new Rect (-1, -1, 0, 0);
		}
	}
	
	public void Partition(SubDungeon subDungeon) 
	{
		if (subDungeon.IsLeaf()) 
		{
			// if the sub-dungeon is too large split it
			if (subDungeon.Rect.width > maxRoomSize 
				|| subDungeon.Rect.height > maxRoomSize 
				|| Random.Range(0.0f,1.0f) > 0.25) 
			{
				if (subDungeon.Split (minRoomSize, maxRoomSize)) 
				{
					Partition(subDungeon.Left);
					Partition(subDungeon.Right);
				}
			}
		}
	}

	public void DrawRooms(SubDungeon subDungeon) 
	{
		if (subDungeon == null) 
		{
			return;
		}
		
		if (subDungeon.IsLeaf()) 
		{
			for (int i = (int)subDungeon.Room.x; i < subDungeon.Room.xMax; i++) 
			{
				for (int j = (int)subDungeon.Room.y; j < subDungeon.Room.yMax; j++) 
				{
					GameObject instance = Instantiate (floorTile, new Vector3 (i, j, 0f), Quaternion.identity) as GameObject;
					instance.transform.SetParent (transform);
					_boardPositionsFloor [i, j] = instance;
				}
			}
		} 
		else 
		{
			DrawRooms (subDungeon.Left);
			DrawRooms (subDungeon.Right);
		}
	}

	void DrawCorridors(SubDungeon subDungeon) 
	{
		if (subDungeon == null) 
		{
			return;
		}

		DrawCorridors (subDungeon.Left);
		DrawCorridors (subDungeon.Right);

		foreach (Rect corridor in subDungeon.Corridors) 
		{
			for (int i = (int)corridor.x; i < corridor.xMax; i++) 
			{
				for (int j = (int)corridor.y; j < corridor.yMax; j++) 
				{
					if (_boardPositionsFloor[i,j] == null) 
					{
						GameObject instance = Instantiate (corridorTile, new Vector3 (i, j, 0f), Quaternion.identity) as GameObject;
						instance.transform.SetParent (transform);
						_boardPositionsFloor [i, j] = instance;
					}
				}
			}
		}
	}

	
}