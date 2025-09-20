using System;
using System.Collections.Generic;
using DungeonGeneration.BinarySpacePartitioning;
using Gameplay.Levels;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Gameplay.Agents
{
    /// <summary>
    /// A class that is responsible for generating
    /// and keeping track of agents in the dungeon,
    /// and handling conversion logic.
    /// </summary>
    public class AgentsController : MonoBehaviour
    {
        public static event Action<List<PlayerAgent>, List<EnemyAgent>> OnAgentsSpawned;
        public static event Action<int, int> OnAgentConverted;
        public static event Action OnPlayerConverted;
        public static event Action OnEnemyConverted;
        
        // Conversion settings
        private const float RandomConversionChance = 0.3f;
        private const float ConversionRadius = 1f;

        [Header("Prefabs & Parents")]
        [SerializeField] private GameObject playerAgent;
        [SerializeField] private GameObject enemyAgent;
        [SerializeField] private Transform playersParent;
        [SerializeField] private Transform enemiesParent;

        private readonly List<PlayerAgent> _playerAgents = new();
        private readonly List<EnemyAgent> _enemyAgents = new();
        private AgentData _agentData;

        private void Awake()
        {
            Agent.OnAgentCollision += HandleCollision;
        }

        private void OnDestroy()
        {
            Agent.OnAgentCollision -= HandleCollision;
        }

        /// <summary>
        /// Generates agents into the given set of rooms
        /// based on the provided level configuration.
        /// </summary>
        /// <param name="rooms">The list of rooms where agents will be placed</param>
        /// <param name="level">The Scriptable Object data for this level.</param>
        public void SpawnAgents(List<Room> rooms, LevelData level)
        {
            // Reset 
            ClearAgents();
            _agentData = level.AgentData;

            // Generate player agents in randomly selected rooms
            for (int i = 0; i < level.PlayerAgentsCount; i++)
                SpawnAgent(rooms[Maths.GetRandomInt(0, rooms.Count)], playerAgent, playersParent);

            // Generate enemy agents in randomly selected rooms
            for (int i = 0; i < level.EnemyAgentsCount; i++)
                SpawnAgent(rooms[Maths.GetRandomInt(0, rooms.Count)], enemyAgent, enemiesParent);

            OnAgentsSpawned?.Invoke(_playerAgents, _enemyAgents);
        }

        /// <summary>
        /// Resets the agents after completing a level.
        /// </summary>
        private void ClearAgents()
        {
            _playerAgents.Clear();
            _enemyAgents.Clear();

            for (int i = playersParent.childCount - 1; i >= 0; i--)
                Destroy(playersParent.GetChild(i).gameObject);

            for (int i = enemiesParent.childCount - 1; i >= 0; i--)
                Destroy(enemiesParent.GetChild(i).gameObject);
        }

        /// <summary>
        /// Generate an agent at a random tile in a room.
        /// </summary>
        /// <param name="room">The room to select a floor time from.</param>
        /// <param name="prefab">The GameObject to instantiate.</param>
        /// <param name="parent">The parent of the GameObject.</param>
        private void SpawnAgent(Room room, GameObject prefab, Transform parent)
        {
            Vector2Int spawnTile = room.FloorTiles[Maths.GetRandomInt(0, room.FloorTiles.Count)];
            Vector3 spawnPosition = new Vector3(spawnTile.x, spawnTile.y, 0);
            SpawnAgentAtPosition(spawnPosition, prefab, parent);
        }

        /// <summary>
        /// Handles the instantiation logic,
        /// initializes the agent and adds it
        /// to the respective list.
        /// </summary>
        /// <param name="position">The position to place the agent at.</param>
        /// <param name="prefab">The GameObject to instantiate.</param>
        /// <param name="parent">The parent of the GameObject.</param>
        private void SpawnAgentAtPosition(Vector3 position, GameObject prefab, Transform parent)
        {
            GameObject go = Instantiate(prefab, position, Quaternion.identity, parent);

            Agent agent = go.GetComponent<Agent>();
            agent.Initialize(_agentData);  

            if (agent is EnemyAgent enemy)
                _enemyAgents.Add(enemy);
            else
                _playerAgents.Add(agent as PlayerAgent);
        }

        /// <summary>
        /// Called on collision trigger event between two opponent agents.
        /// Determines whether an agent should be converted
        /// and if so - which one.
        /// </summary>
        /// <param name="agentA">The first agent involved in the collision.</param>
        /// <param name="agentB">The second agent involved in the collision.</param>
        private void HandleCollision(Agent agentA, Agent agentB)
        {
            if (agentA.HasConverted || agentB.HasConverted) return;

            // If one of the agents is surrounded by opponents, it will be converted
            bool forceConversion = CheckSurroundingAdvantage(agentA, agentB, out Agent surrounded);

            if (forceConversion)
            {
                ConvertAgent(surrounded);
                return;
            }

            // If there's no opponent advantage, determine random conversion
            if (Random.value < RandomConversionChance)
            {
                // A coin-flip to determine which of the two agents will be converted
                Agent victim = Random.value < 0.5f ? agentA : agentB;
                ConvertAgent(victim);
            }
        }

        /// <summary>
        /// Determines whether either of two colliding agents is surrounded by
        /// a significantly larger number of nearby opponents.
        /// </summary>
        /// <param name="agentA">The first agent involved in the collision.</param>
        /// <param name="agentB">The second agent involved in the collision.</param>
        /// <param name="surrounded">Outputs the agent that is considered surrounded if an advantage is found, or null.</param>
        /// <returns>True if one of the agents is surrounded and should be converted. Otherwise, false.</returns>
        private bool CheckSurroundingAdvantage(Agent agentA, Agent agentB, out Agent surrounded)
        {
            surrounded = null;

            int agentAOpponents = CountNearbyOpponents(agentA);
            int agentBOpponents = CountNearbyOpponents(agentB);

            // First agent is surrounded if it has at least two nearby opponents
            // and at least one more opponent than Agent B
            if (agentAOpponents >= 2 && agentAOpponents >= agentBOpponents + 1)
            {
                surrounded = agentA;
                return true;
            }

            // Same check for second agent
            if (agentBOpponents >= 2 && agentBOpponents >= agentAOpponents + 1)
            {
                surrounded = agentB;
                return true;
            }

            // Neither agent is surrounded
            return false;
        }

        /// <summary>
        /// Counts the number of opposing agents 
        /// around the specified agent.
        /// </summary>
        /// <param name="agent">The agent for whom nearby opponents will be counted.</param>
        /// <returns>The total number of agents of the opposite type.</returns>
        private int CountNearbyOpponents(Agent agent)
        {
            // Get all colliders around the agent
            Collider2D[] hits = Physics2D.OverlapCircleAll(agent.transform.position, ConversionRadius);
            int count = 0;

            foreach (var hit in hits)
            {
                var other = hit.GetComponent<Agent>();
                // Skip if no agent exists around the agent or if the collider is the same agent
                if (!other || other == agent) continue;
             
                bool isOpponent = (agent is PlayerAgent && other is EnemyAgent) ||
                                  (agent is EnemyAgent && other is PlayerAgent);

                if (isOpponent) count++;
            }
            return count;
        }

        /// <summary>
        /// Converts the specified agent to the opposite team,
        /// removes the original agent from the game, and spawns a replacement
        /// agent of the new type at the same position. Notifies
        /// subscribers of the conversion.
        /// </summary>
        /// <param name="agent">The agent to convert.</param>
        private void ConvertAgent(Agent agent)
        {
            // Prevent double conversions
            if (agent.HasConverted) return;

            // Change agent state
            agent.SetConverted(true);

            // Store the agent's position before destroying it
            Vector3 position = agent.transform.position;
            Destroy(agent.gameObject);

            // Check the agent type and convert to the opposite team
            if (agent is PlayerAgent)
            {
                // Remove from player list and spawn an enemy in the same position
                _playerAgents.Remove(agent as PlayerAgent);
                SpawnAgentAtPosition(position, enemyAgent, enemiesParent);
                
                OnPlayerConverted?.Invoke();
            }
            else if (agent is EnemyAgent)
            {
                // Remove from enemy list and spawn a player in the same position
                _enemyAgents.Remove(agent as EnemyAgent);
                SpawnAgentAtPosition(position, playerAgent, playersParent);
                
                OnEnemyConverted?.Invoke();
            }

            OnAgentConverted?.Invoke(_playerAgents.Count, _enemyAgents.Count);
        }
    }
}