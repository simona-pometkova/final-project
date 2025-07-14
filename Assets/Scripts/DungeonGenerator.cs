using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [SerializeField] private int dungeonRows = 50;
    [SerializeField] private int dungeonColumns = 50;
    [SerializeField] private int minRoomSize = 10; 
    [SerializeField] private int maxRoomSize = 20;
    
    [SerializeField] private GameObject floorTilePrefab;
    [SerializeField] private GameObject corridorTilePrefab;

    private GameObject[,] _dungeon;

    /// <summary>
    /// Main entry point of the program - the flow of the BSP algorithm.
    /// </summary>
    private void Start()
    {
        // Create the main room (root node in the BSP tree) that takes up the whole size of the dungeon.
        SubDungeon rootSubDungeon = new SubDungeon(new Rect(0, 0, dungeonRows, dungeonColumns));
        
        // Recursively partition the root node.
        Partition(rootSubDungeon);
        
        // Create rooms for every subspace.
        rootSubDungeon.CreateRoom();

        // Create a dungeon GameObject.
        _dungeon = new GameObject[dungeonRows, dungeonColumns];
        
        // Draw rooms and corridors.
        DrawRooms(rootSubDungeon);
        DrawCorridors(rootSubDungeon);
    }

    private void Partition(SubDungeon subDungeon)
    {
        if (!subDungeon.IsLeaf()) return;
        
        // if the sub-dungeon is too large split it
        if (subDungeon.Rect.width > maxRoomSize
            || subDungeon.Rect.height > maxRoomSize
            || Random.Range(0.0f, 1.0f) > 0.25)
        {
            if (subDungeon.Split(minRoomSize, maxRoomSize))
            {
                Partition(subDungeon.LeftChild);
                Partition(subDungeon.RightChild);
            }
        }
    }

    private void DrawRooms(SubDungeon subDungeon)
    {
        if (subDungeon == null) return;

        if (subDungeon.IsLeaf())
        {
            for (int i = (int)subDungeon.Room.x; i < subDungeon.Room.xMax; i++)
            {
                for (int j = (int)subDungeon.Room.y; j < subDungeon.Room.yMax; j++)
                {
                    GameObject instance =
                        Instantiate(floorTilePrefab, new Vector3(i, j, 0f), Quaternion.identity) as GameObject;
                    instance.transform.SetParent(transform);
                    
                    _dungeon[i, j] = instance;
                }
            }
        }
        else
        {
            DrawRooms(subDungeon.LeftChild);
            DrawRooms(subDungeon.RightChild);
        }
    }

    private void DrawCorridors(SubDungeon subDungeon)
    {
        if (subDungeon == null) return;

        DrawCorridors(subDungeon.LeftChild);
        DrawCorridors(subDungeon.RightChild);

        foreach (Rect corridor in subDungeon.Corridors)
        {
            for (int i = (int)corridor.x; i < corridor.xMax; i++)
            {
                for (int j = (int)corridor.y; j < corridor.yMax; j++)
                {
                    if (_dungeon[i, j] == null)
                    {
                        GameObject instance =
                            Instantiate(corridorTilePrefab, new Vector3(i, j, 0f), Quaternion.identity) as GameObject;
                        instance.transform.SetParent(transform);
                        _dungeon[i, j] = instance;
                    }
                }
            }
        }
    }
}