using System.Collections.Generic;
using DungeonGeneration;
using DungeonGeneration.BinarySpacePartitioning;
using UnityEngine;
using Utils;

namespace Gameplay
{
    /// <summary>
    /// Handles visualisation of dungeon layout into Unity scene.
    /// </summary>
    public class DungeonRenderer : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private List<GameObject> floorTilePrefabs;
        [SerializeField] private List<GameObject> wallTilePrefabs;

        [Header("Transforms")] 
        [SerializeField] private Transform roomsParent;
        [SerializeField] private Transform corridorsParent;
        [SerializeField] private Transform wallsParent;

        private GameObject[,] _dungeonGameObject;

        /// <summary>
        /// Draws the dungeon into the Unity Scene by instantiating
        /// appropriate tile GameObjects (1 - floor, 0 - wall) on each coordinate.
        /// </summary>
        /// <param name="dungeon">The dungeon data to use.</param>
        public void DrawDungeon(DungeonData dungeon)
        {
            ClearDungeon();

            _dungeonGameObject = new GameObject[dungeon.Width, dungeon.Height];

            // First draw all room tiles
            foreach (Room room in dungeon.Rooms)
            {
                // Local room grid to world dungeon grid
                room.TranslateToGlobalGrid(dungeon.Grid);
                GameObject roomGameObject = new GameObject("Room");
                roomGameObject.transform.SetParent(roomsParent);

                // Create a GameObject for each floor tile of the room
                foreach (Vector2Int tile in room.FloorTiles)
                    CreateGameObject(ChooseRandom(floorTilePrefabs), tile.x, tile.y, roomGameObject.transform);
            }

            // Iterate over dungeon and fill out 
            // corridors and walls on the remaining empty coordinates
            for (int x = 0; x < dungeon.Width; x++)
                for (int y = 0; y < dungeon.Height; y++)
                    if (dungeon.Grid[x, y] == 1 && _dungeonGameObject[x, y] == null)
                        CreateGameObject(ChooseRandom(floorTilePrefabs), x, y, corridorsParent);
                    else if (dungeon.Grid[x, y] == 0)
                        CreateGameObject(ChooseRandom(wallTilePrefabs), x, y, wallsParent);
        }

        /// <summary>
        /// Clears the dungeon by destroying existing game objects.
        /// Used for level progression and generating a new dungeon
        /// for each level.
        /// </summary>
        private void ClearDungeon()
        {
            for (int i = roomsParent.childCount - 1; i >= 0; i--)
                Destroy(roomsParent.GetChild(i).gameObject);

            for (int i = corridorsParent.childCount - 1; i >= 0; i--)
                Destroy(corridorsParent.GetChild(i).gameObject);

            for (int i = wallsParent.childCount - 1; i >= 0; i--)
                Destroy(wallsParent.GetChild(i).gameObject);

            _dungeonGameObject = null;
        }

        /// <summary>
        /// Instantiates a GameObject and updates
        /// the main dungeon GameObject.
        /// </summary>
        /// <param name="prefab">The prefab to use for instantiation.</param>
        /// <param name="positionX">The x-position to instantiate the GameObject on.</param>
        /// <param name="positionY">The y-position to instantiate the GameObject on.</param>
        /// <param name="parent">The parent of the GameObject.</param>
        private void CreateGameObject(GameObject prefab, int positionX, int positionY, Transform parent)
        {
            Vector3 position = new Vector3(positionX, positionY, 0);
            GameObject go = Instantiate(prefab, position, Quaternion.identity, parent);
            _dungeonGameObject[positionX, positionY] = go;
        }
        
        /// <summary>
        /// Choose a random prefab from list and return it.
        /// </summary>
        /// <param name="prefabs">The list of prefabs to choose from.</param>
        /// <returns>A floor tile GameObject.</returns>
        private GameObject ChooseRandom(List<GameObject> prefabs) => prefabs[Maths.GetRandomInt(0, prefabs.Count)];
        
    }
}