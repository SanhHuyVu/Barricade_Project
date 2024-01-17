using System.Collections;
using UnityEngine;

public class Enemy : StatsManager
{
    [SerializeField] private Animator animator;
    [SerializeField] private CapsuleCollider zombieCollider;
    [SerializeField] private Transform[] waypoints;

    [SerializeField] private AudioSource audioSource;

    public Animator Animator => animator;
    public Transform[] Waypoints => waypoints;

    private void Start()
    {
        for (int i = 0; i < attributes.Length; i++)
        {
            attributes[i].value.Init(this);
        }
        // SetUpStats();
        UpdateStats(true);
    }

    public override void UpdateStats(bool init = false)
    {
        base.UpdateStats(init);
        int currentDay = TimeController.Instance.CurrentDay;
        // Attack += (currentDay + Random.Range(0, 2)) * Random.Range(2, 4);
        Attack += (currentDay + Random.Range(0, 2));
        Defense += currentDay * 2 + Random.Range(0, 5);
        MaxHP += (currentDay + Random.Range(0, 5)) * 3;
        testAttack = Attack;
        testDefense = Defense;
        testMaxHP = MaxHP;
        zombieCollider.enabled = true;
    }

    public override bool TakeDamage(int damage, bool hpLost = false)
    {
        bool die = base.TakeDamage(damage);
        if (die)
        {
            animator.SetTrigger(ConstVariables.ZOMBIE_DIE);
            animator.SetBool(ConstVariables.ZOMBIE_ISCHASING, false);
            animator.SetBool(ConstVariables.ZOMBIE_ISATTACKING, false);
            StartCoroutine(HideBody(3f));
            Player.Instance.ScoreTracker.KillCount++;
        }
        else
        {
            animator.SetTrigger(ConstVariables.ZOMBIE_GETHIT);
            animator.SetBool(ConstVariables.ZOMBIE_ISATTACKING, false);
        }

        if (audioSource.isPlaying) audioSource.Stop();
        audioSource.PlayOneShot(SoundAssets.Instance.ZombieHitSounds[Random.Range(0, SoundAssets.Instance.ZombieHitSounds.Length)]);

        return die;
    }

    private IEnumerator HideBody(float timer)
    {
        zombieCollider.enabled = false;
        yield return new WaitForSeconds(timer);
        gameObject.SetActive(false);
    }

    public void SetWaypoints(Transform[] newWaypoints)
    {
        waypoints = newWaypoints;
    }
}
