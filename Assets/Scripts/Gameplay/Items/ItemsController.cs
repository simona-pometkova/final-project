using System.Collections.Generic;
using DungeonGeneration.BinarySpacePartitioning;
using UnityEngine;

namespace Gameplay.Items
{
    public class ItemsController : MonoBehaviour
    {
        [Header("Prefabs & Parents")]
        [SerializeField] private GameObject torchPrefab;
        [SerializeField] private GameObject applePrefab;
        [SerializeField] private Transform itemsParent;

        // TODO use level settings to determine how many items to spawn
        public void SpawnItems(List<Room> rooms)
        {
            // hard-coded for now
            foreach (Room room in rooms)
                SpawnItem(room, torchPrefab, itemsParent);
        }
        
        // TODO a more sophisticated placement algorithm - place torches where it makes sense (i.e. close to walls)
        private void SpawnItem(Room room, GameObject prefab, Transform parent)
        {
            Vector2Int spawnTile = room.FloorTiles[Random.Range(0, room.FloorTiles.Count)];
            Vector3 spawnPosition = new Vector3(spawnTile.x, spawnTile.y, 0);
            Instantiate(prefab, spawnPosition, Quaternion.identity, parent);
        }
    }
}