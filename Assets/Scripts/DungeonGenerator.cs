using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [SerializeField] private int dungeonRows;
    [SerializeField] private int dungeonColumns;
    [SerializeField] private int minRoomSize; 
    [SerializeField] private int maxRoomSize;
    
    [SerializeField] private GameObject floorTilePrefab;
    [SerializeField] private GameObject corridorTilePrefab;

    private GameObject[,] _boardPositionsFloor;

    private void Start()
    {
        SubDungeon rootSubDungeon = new SubDungeon(new Rect(0, 0, dungeonRows, dungeonColumns));
        Partition(rootSubDungeon);
        rootSubDungeon.CreateRoom();

        _boardPositionsFloor = new GameObject[dungeonRows, dungeonColumns];
        DrawRooms(rootSubDungeon);
        DrawCorridors(rootSubDungeon);
    }

    private void Partition(SubDungeon subDungeon)
    {
        if (subDungeon.IsLeaf())
        {
            // if the sub-dungeon is too large split it
            if (subDungeon.Rect.width > maxRoomSize
                || subDungeon.Rect.height > maxRoomSize
                || Random.Range(0.0f, 1.0f) > 0.25)
            {
                if (subDungeon.Split(minRoomSize, maxRoomSize))
                {
                    Partition(subDungeon.Left);
                    Partition(subDungeon.Right);
                }
            }
        }
    }

    private void DrawRooms(SubDungeon subDungeon)
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
                    GameObject instance =
                        Instantiate(floorTilePrefab, new Vector3(i, j, 0f), Quaternion.identity) as GameObject;
                    instance.transform.SetParent(transform);
                    _boardPositionsFloor[i, j] = instance;
                }
            }
        }
        else
        {
            DrawRooms(subDungeon.Left);
            DrawRooms(subDungeon.Right);
        }
    }

    void DrawCorridors(SubDungeon subDungeon)
    {
        if (subDungeon == null)
        {
            return;
        }

        DrawCorridors(subDungeon.Left);
        DrawCorridors(subDungeon.Right);

        foreach (Rect corridor in subDungeon.Corridors)
        {
            for (int i = (int)corridor.x; i < corridor.xMax; i++)
            {
                for (int j = (int)corridor.y; j < corridor.yMax; j++)
                {
                    if (_boardPositionsFloor[i, j] == null)
                    {
                        GameObject instance =
                            Instantiate(corridorTilePrefab, new Vector3(i, j, 0f), Quaternion.identity) as GameObject;
                        instance.transform.SetParent(transform);
                        _boardPositionsFloor[i, j] = instance;
                    }
                }
            }
        }
    }
}