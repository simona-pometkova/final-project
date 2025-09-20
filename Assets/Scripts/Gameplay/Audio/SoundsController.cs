using Gameplay.Agents;
using Gameplay.Items;
using UnityEngine;

namespace Gameplay.Audio
{
    /// <summary>
    /// Listens for gameplay events and
    /// plays relevant sound effects.
    /// </summary>
    public class SoundsController : MonoBehaviour
    {
        [Header("SFX")] 
        [SerializeField] private AudioClip torchLit;
        [SerializeField] private AudioClip torchExtinguished;
        [SerializeField] private AudioClip playerConverted;
        [SerializeField] private AudioClip enemyConverted;
    
        [Header("Source")] 
        [SerializeField] private AudioSource sfxSource;

        /// <summary>
        /// Subscribes to gameplay events.
        /// </summary>
        private void OnEnable()
        {
            Torch.OnStateChanged += TorchStateChanged;
            AgentsController.OnPlayerConverted += PlayerConverted;
            AgentsController.OnEnemyConverted += EnemyConverted;
        }

        private void OnDisable()
        {
            Torch.OnStateChanged -= TorchStateChanged;
            AgentsController.OnPlayerConverted -= PlayerConverted;
            AgentsController.OnEnemyConverted -= EnemyConverted;
        }

        /// <summary>
        /// Called when a torch's state has been changed.
        /// Plays the relevant sound effect depending on the state.
        /// </summary>
        /// <param name="lit">The torch's lit state.</param>
        private void TorchStateChanged(bool lit)
        {
            if (lit)
                PlaySFX(torchLit);
            else
                PlaySFX(torchExtinguished);
        }

        /// <summary>
        /// Called when a player has been converted to an enemy.
        /// Plays the relevant sound effect.
        /// </summary>
        private void PlayerConverted() => PlaySFX(playerConverted);

        /// <summary>
        /// Called when an enemy has been converted to a player.
        /// Plays the relevant sound effect.
        /// </summary>
        private void EnemyConverted() => PlaySFX(enemyConverted);

        /// <summary>
        /// Plays the provided sound effect on the audio source.
        /// </summary>
        /// <param name="clip">The sound to play.</param>
        private void PlaySFX(AudioClip clip) => sfxSource.PlayOneShot(clip);
    }  
}

