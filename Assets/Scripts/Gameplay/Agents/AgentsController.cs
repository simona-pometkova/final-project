using System.Collections.Generic;
using DungeonGeneration.BinarySpacePartitioning;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay.Agents
{
    // TODO
    public class AgentsController : MonoBehaviour
    {
        [Header("Prefabs & Parents")]
        [SerializeField] private GameObject playerAgent;
        [SerializeField] private GameObject enemyAgent;
        [SerializeField] private Transform playersParent;
        [SerializeField] private Transform enemiesParent;
        
        private List<PlayerAgent> _playerAgents;
        private List<EnemyAgent> _enemyAgents;
        
        // TODO use level settings to determine how many agents to place
        public void SpawnAgents(List<Room> rooms)
        {
            // hard-coded for now
            for (int i = 0; i < 3; i++)
            {
                SpawnAgent(rooms[Random.Range(0, rooms.Count)], playerAgent, playersParent);
                SpawnAgent(rooms[Random.Range(0, rooms.Count)], enemyAgent, enemiesParent);
            }
        }

        // TODO more sophisticated placement algorithm 
        private void SpawnAgent(Room room, GameObject prefab, Transform parent)
        {
            Vector2Int spawnTile = room.FloorTiles[Random.Range(0, room.FloorTiles.Count)];
            Vector3 spawnPosition = new Vector3(spawnTile.x, spawnTile.y, 0);
            Instantiate(prefab, spawnPosition, Quaternion.identity, parent);
        }
        
        public void ConvertAgent() {}
    }
}