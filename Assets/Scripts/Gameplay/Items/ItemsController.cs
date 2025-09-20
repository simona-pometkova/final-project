using System;
using System.Collections.Generic;
using DungeonGeneration.BinarySpacePartitioning;
using Gameplay.Levels;
using UnityEngine;
using Utils;

namespace Gameplay.Items
{
    /// <summary>
    /// Manages the placement, tracking, and state of
    /// interactive items (torches) in each level.
    /// Responsible for pawning a specified number of torches in each room at valid positions,
    /// listening for torch lighting events to detect when all torches are lit
    /// and notifying other systems when torches have been spawned or all lit.
    ///</summary>
    public class ItemsController : MonoBehaviour
    {
        /// <summary>
        /// Triggered when all torches in the current level have been lit.
        /// Used to signal level completion.
        /// </summary>
        public static event Action OnAllTorchesLit;
        public static event Action<List<Torch>> OnTorchesSpawned;

        [Header("Prefabs & Parents")]
        [SerializeField] private GameObject torchPrefab;
        [SerializeField] private Transform itemsParent;

        // A list of all torches currently active in the level
        private readonly List<Torch> _torches = new();
        private const int MaxAttempts = 100;
        
        /// <summary>
        /// Generates torches into the given set of rooms
        /// based on the provided level configuration.
        /// </summary>
        /// <param name="rooms">The list of dungeon rooms where torches may be placed.</param>
        /// <param name="level">The Scriptable Object data for this level.</param>
        public void SpawnItems(List<Room> rooms, LevelData level)
        {
            // Reset
            ClearItems();
            
            // Iterate over each room and attempt to place the required number of torches
            foreach (Room room in rooms)
            {
                // Track tiles already used for torch placement
                List<Vector2Int> placed = new List<Vector2Int>();
                int torchesNeeded = level.TorchesPerRoom;
                int attempts = 0;

                // Continue placing torches until the required count is reached or attempts exceed the limit
                while (placed.Count < torchesNeeded && attempts < MaxAttempts)
                {
                    attempts++;
                    
                    // Randomly select a floor tile in the current room
                    Vector2Int candidate = room.FloorTiles[Maths.GetRandomInt(0, room.FloorTiles.Count)];

                    // Check that the candidate tile is far enough from previously placed torches
                    if (IsFarEnough(candidate, placed, level.MinimumTorchDistance))
                    {
                        placed.Add(candidate);
                        SpawnAt(candidate);
                    }
                }
            }

            // Notify subscribers that all torches have been placed
            OnTorchesSpawned?.Invoke(_torches);
        }
        
        /// <summary>
        /// Instantiates a torch at the specified grid tile and registers it for tracking.
        /// </summary>
        /// <param name="tile">The dungeon grid position where the torch will be placed.</param>
        private void SpawnAt(Vector2Int tile)
        {
            // Convert grid coordinates to world space 
            Vector3 spawnPosition = new Vector3(tile.x, tile.y, 0);
            
            // Create a new torch object    
            GameObject go = Instantiate(torchPrefab, spawnPosition, Quaternion.identity, itemsParent);
            
            // Retrieve the Torch component to track its state
            Torch torch = go.GetComponent<Torch>();

            if (torch)
            {
                torch.OnLit += TrackAllTorches;
                _torches.Add(torch);
            }
        }

        /// <summary>
        /// Checks whether a candidate tile is far enough from all previously placed torches.
        /// </summary>
        /// <param name="candidate">The grid tile being evaluated.</param>
        /// <param name="placed">The list of tiles where torches are already placed.</param>
        /// <param name="minDistance">The minimum allowable distance between torches.</param>
        /// <returns>True if the candidate position is far away enough from all placed torches, otherwise false.</returns>
        private bool IsFarEnough(Vector2Int candidate, List<Vector2Int> placed, float minDistance)
        {
            foreach (var pos in placed)
            {
                // If any existing torch is too close, reject the candidate
                if (Vector2.Distance(candidate, pos) < minDistance)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Removes all currently spawned torches from the scene and clears the list.
        /// </summary>
        private void ClearItems()
        {
            _torches.Clear();

            for (int i = itemsParent.childCount - 1; i >= 0; i--)
                Destroy(itemsParent.GetChild(i).gameObject);
        }
        
        /// <summary>
        /// Tracks torch lighting progress and triggers event when all are lit.
        /// </summary>
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