using UnityEngine;
using Utils;

namespace Gameplay.Agents
{
    /// <summary>
    /// A configurable data container for agent behavior and movement statistics.
    /// </summary>
    [CreateAssetMenu(fileName = "Agents Data", menuName = "Game/Agents Data")]
    public class AgentData : ScriptableObject
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeedMin;
        [SerializeField] private float moveSpeedMax;
        [SerializeField] private float minDirectionChangeTime;
        [SerializeField] private float maxDirecionChangeTime;
        [SerializeField] private float idleChanceMin;
        [SerializeField] private float idleChanceMax;

        [Header("Enemy Agents")]
        [SerializeField] private float extinguishTorchChanceMin;
        [SerializeField] private float extinguishTorchChanceMax;
        [SerializeField] private float lookForTorchChanceMin;
        [SerializeField] private float lookForTorchChanceMax;
        [SerializeField] private float torchCheckIntervalMin;
        [SerializeField] private float torchCheckIntervalMax;
        [SerializeField] private float searchRadiusMin;
        [SerializeField] private float searchRadiusMax;

        public void RandomizeMovementStats(out float moveSpeed, 
                                            out float minDirection, 
                                            out float maxDirection, 
                                            out float idleChance)
        {
            moveSpeed = Maths.GetRandomFloat(moveSpeedMin, moveSpeedMax);
            minDirection = minDirectionChangeTime;
            maxDirection = maxDirecionChangeTime;
            idleChance = Maths.GetRandomFloat(idleChanceMin, idleChanceMax);
        }

        public void RandomizeEnemyStats(out float extinguishTorchChance,
                                        out float lookForTorchChance,
                                        out float torchCheckInterval,
                                        out float searchRadius)
        {
            extinguishTorchChance = Maths.GetRandomFloat(extinguishTorchChanceMin, extinguishTorchChanceMax);
            lookForTorchChance = Maths.GetRandomFloat(lookForTorchChanceMin, lookForTorchChanceMax);
            torchCheckInterval = Maths.GetRandomFloat(torchCheckIntervalMin, torchCheckIntervalMax);
            searchRadius = Maths.GetRandomFloat(searchRadiusMin, searchRadiusMax);
        }
    }
}
