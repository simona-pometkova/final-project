using System.Collections.Generic;
using DungeonGeneration.BinarySpacePartitioning;
using Gameplay.Levels;
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
        [SerializeField] private float randomConversionChance = 0.3f;
        
        private List<PlayerAgent> _playerAgents = new();
        private List<EnemyAgent> _enemyAgents = new();

        private void Awake()
        {
            Agent.OnAgentCollision += HandleCollision;
        }

        private void OnDestroy()
        {
            Agent.OnAgentCollision -= HandleCollision;
        }

        public void SpawnAgents(List<Room> rooms, LevelData level)
        {
            ClearAgents();

            for (int i = 0; i < level.PlayerAgentsCount; i++)
                SpawnAgent(rooms[Random.Range(0, rooms.Count)], playerAgent, playersParent, level.AgentData);

            for (int i = 0; i < level.EnemyAgentsCount; i++)
                SpawnAgent(rooms[Random.Range(0, rooms.Count)], enemyAgent, enemiesParent, level.AgentData);
        }

        private void ClearAgents()
        {
            for (int i = playersParent.childCount - 1; i >= 0; i--)
                Destroy(playersParent.GetChild(i).gameObject);

            for (int i = enemiesParent.childCount - 1; i >= 0; i--)
                Destroy(enemiesParent.GetChild(i).gameObject);
        }

        // TODO more sophisticated placement algorithm 
        private void SpawnAgent(Room room, GameObject prefab, Transform parent, AgentData data)
        {
            Vector2Int spawnTile = room.FloorTiles[Random.Range(0, room.FloorTiles.Count)];
            Vector3 spawnPosition = new Vector3(spawnTile.x, spawnTile.y, 0);
            GameObject go = Instantiate(prefab, spawnPosition, Quaternion.identity, parent);

            Agent agent = go.GetComponent<Agent>();
            data.RandomizeMovementStats(out float moveSpeed,
                                        out float minDirectionChangeTime,
                                        out float maxDirectionChangeTime,
                                        out float idleChance);
            agent.Initialize(moveSpeed, minDirectionChangeTime, maxDirectionChangeTime, idleChance);

            if (agent is EnemyAgent enemy)
            {
                data.RandomizeEnemyStats(out float extinguishTorchChance,
                                         out float lookForTorchChance,
                                         out float torchCheckInterval,
                                         out float searchRadius);
                enemy.InitializeEnemy(extinguishTorchChance, lookForTorchChance, torchCheckInterval, searchRadius);
                _enemyAgents.Add(enemy);
            }
            else
                _playerAgents.Add(agent as PlayerAgent);
        }

        private void HandleCollision(Agent agentA, Agent agentB)
        {
            if (agentA.HasConverted || agentB.HasConverted) return;

            bool forceConversion = CheckSurroundingAdvantage(agentA, agentB, out Agent surrounded);

            if (forceConversion)
            {
                ConvertAgent(surrounded);
                return;
            }

            // TODO - should I have random chance conversion at all?
            if (Random.value < randomConversionChance)
            {
                Agent victim = Random.value < 0.5f ? agentA : agentB;
                ConvertAgent(victim);
            }
        }

        private bool CheckSurroundingAdvantage(Agent agentA, Agent agentB, out Agent surrounded)
        {
            surrounded = null;

            int agentAOpponents = CountNearbyOpponents(agentA);
            int agentBOpponents = CountNearbyOpponents(agentB);

            if (agentAOpponents >= 2 && agentAOpponents >= agentBOpponents + 1)
            {
                surrounded = agentA;
                return true;
            }

            if (agentBOpponents >= 2 && agentBOpponents >= agentAOpponents + 1)
            {
                surrounded = agentB;
                return true;
            }

            return false;
        }

        private int CountNearbyOpponents(Agent agent)
        {
            float radius = 1f;
            Collider2D[] hits = Physics2D.OverlapCircleAll(agent.transform.position, radius);
            int count = 0;

            foreach (var hit in hits)
            {
                var other = hit.GetComponent<Agent>();
                if (!other || other == agent) continue;
             
                bool isOpponent = (agent is PlayerAgent && other is EnemyAgent) ||
                                  (agent is EnemyAgent && other is PlayerAgent);

                if (isOpponent) count++;
            }
            return count;
        }

        private void ConvertAgent(Agent agent)
        {
            if (agent.HasConverted) return;

            agent.SetConverted(true);

            Vector3 position = agent.transform.position;
            Destroy(agent.gameObject);
            
            // TODO When converting, stats need to be randomized as well
            if (agent is PlayerAgent)
                Instantiate(enemyAgent, position, Quaternion.identity, enemiesParent);
            else if (agent is EnemyAgent)
                Instantiate(playerAgent, position, Quaternion.identity, playersParent);
        }
    }
}