using System;
using System.Collections.Generic;
using DungeonGeneration.BinarySpacePartitioning;
using Gameplay.Levels;
using UnityEngine;

namespace Gameplay.Items
{
    public class ItemsController : MonoBehaviour
    {
        public static event Action OnAllTorchesLit;

        [Header("Prefabs & Parents")]
        [SerializeField] private GameObject torchPrefab;
        [SerializeField] private Transform itemsParent;

        private List<Torch> _torches = new();

        public void SpawnItems(List<Room> rooms, LevelData level)
        {
            ClearItems();

            foreach (Room room in rooms)
                for (int i = 0; i < level.TorchesPerRoom; i++)
                    SpawnTorch(room, torchPrefab, itemsParent);
        }

        private void ClearItems()
        {
            for (int i = itemsParent.childCount - 1; i >= 0; i--)
                Destroy(itemsParent.GetChild(i).gameObject);
        }
        
        // TODO a more sophisticated placement algorithm - place torches where it makes sense (i.e. close to walls)
        private void SpawnTorch(Room room, GameObject prefab, Transform parent)
        {
            Vector2Int spawnTile = room.FloorTiles[UnityEngine.Random.Range(0, room.FloorTiles.Count)];
            Vector3 spawnPosition = new Vector3(spawnTile.x, spawnTile.y, 0);

            GameObject go = Instantiate(prefab, spawnPosition, Quaternion.identity, parent);
            Torch torch = go.GetComponent<Torch>();

            if (torch)
            {
                torch.OnLit += TrackAllTorches;
                _torches.Add(torch);
            }
        }

        private void TrackAllTorches()
        {
            if (_torches.TrueForAll(torch => torch.IsTorchLit))
            {
                _torches.ForEach(torch => torch.OnLit -= TrackAllTorches);
                OnAllTorchesLit?.Invoke();
            }
        }
    }
}