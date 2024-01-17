using UnityEngine;

public class SoundAssets : MonoBehaviour
{
    public static SoundAssets Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    [SerializeField] private AudioClip barricading;

    [SerializeField] private AudioClip[] zombieAttack;
    [SerializeField] private AudioClip[] zombieHit;

    public AudioClip Barricading => barricading;
    public AudioClip[] ZombieAttackSounds => zombieAttack;
    public AudioClip[] ZombieHitSounds => zombieHit;
}
