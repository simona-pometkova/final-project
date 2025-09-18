using System;
using System.Collections.Generic;
using DungeonGeneration.BinarySpacePartitioning;
using Gameplay.Levels;
using UnityEngine;

namespace Gameplay.Items
{
    // TODO document
    public class ItemsController : MonoBehaviour
    {
        public static event Action OnAllTorchesLit;
        public static event Action<List<Torch>> OnTorchesSpawned;

        [Header("Prefabs & Parents")]
        [SerializeField] private GameObject torchPrefab;
        [SerializeField] private Transform itemsParent;

        private List<Torch> _torches = new();

        public void SpawnItems(List<Room> rooms, LevelData level)
        {
            ClearItems();
            
            foreach (Room room in rooms)
            {
                List<Vector2Int> placed = new List<Vector2Int>();
                int torchesNeeded = level.TorchesPerRoom;
                int attempts = 0;
                // extract?
                const int maxAttempts = 100;

                while (placed.Count < torchesNeeded && attempts < maxAttempts)
                {
                    attempts++;
                    Vector2Int candidate = room.FloorTiles[UnityEngine.Random.Range(0, room.FloorTiles.Count)];

                    if (IsFarEnough(candidate, placed, level.MinimumTorchDistance))
                    {
                        placed.Add(candidate);
                        SpawnAt(candidate);
                    }
                }
            }

            OnTorchesSpawned?.Invoke(_torches);
        }
        
        private void SpawnAt(Vector2Int tile)
        {
            Vector3 spawnPosition = new Vector3(tile.x, tile.y, 0);
            GameObject go = Instantiate(torchPrefab, spawnPosition, Quaternion.identity, itemsParent);
            Torch torch = go.GetComponent<Torch>();

            if (torch)
            {
                torch.OnLit += TrackAllTorches;
                _torches.Add(torch);
            }
        }

        private bool IsFarEnough(Vector2Int candidate, List<Vector2Int> placed, float minDistance)
        {
            foreach (var pos in placed)
            {
                if (Vector2.Distance(candidate, pos) < minDistance)
                    return false;
            }
            return true;
        }

        private void ClearItems()
        {
            _torches.Clear();

            for (int i = itemsParent.childCount - 1; i >= 0; i--)
                Destroy(itemsParent.GetChild(i).gameObject);
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