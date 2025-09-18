using Gameplay.Agents;
using Gameplay.Items;
using UnityEngine;

namespace Gameplay.Audio
{
    // TODO documentation
    public class SoundsController : MonoBehaviour
    {
        [Header("SFX")] 
        [SerializeField] private AudioClip torchLit;
        [SerializeField] private AudioClip torchExtinguished;
        [SerializeField] private AudioClip playerConverted;
        [SerializeField] private AudioClip enemyConverted;
    
        [Header("Source")] 
        [SerializeField] private AudioSource sfxSource;

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

        private void TorchStateChanged(bool lit)
        {
            if (lit)
                PlaySFX(torchLit);
            else
                PlaySFX(torchExtinguished);
        }

        private void PlayerConverted() => PlaySFX(playerConverted);

        private void EnemyConverted() => PlaySFX(enemyConverted);

        private void PlaySFX(AudioClip clip) => sfxSource.PlayOneShot(clip);
    }  
}

