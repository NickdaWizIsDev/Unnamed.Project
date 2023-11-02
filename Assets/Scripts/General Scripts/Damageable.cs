using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    [SerializeField]
    private float maxHealth = 100f;
    public float MaxHealth
    {
        get
        {
            return maxHealth;
        }
        set
        {
            maxHealth = value;
        }
    }

    [SerializeField]
    private float health = 100f;
    public float Health
    {
        get
        {
            return health;
        }
        set
        {
            health = value;

            if (health <= 0)
            {
                IsAlive = false;
            }
        }
    }

    [SerializeField]
    private bool isAlive = true;
    public bool IsAlive
    {
        get
        {
            return isAlive;
        }
        set
        {
            isAlive = value;
            animator.SetBool(AnimationStrings.isAlive, value);
            Debug.Log("IsAlive was set to " + value);

            if (!isAlive && !gameObject.CompareTag("Player"))
            {
                // Play death audio clip
                if (deathAudioSource == null)
                {
                    GameObject audioObject = new("Death Audio");

                    AudioSource audioSource = audioObject.AddComponent<AudioSource>();

                    audioSource.PlayOneShot(deathClip, 0.3f);
                    Destroy(audioObject, 0.4f);
                }
                Destroy(gameObject, 0.5f);
            }
        }
    }

    public AudioSource audioSource;
    public AudioSource deathAudioSource;
    public AudioClip dmgClip;
    public AudioClip deathClip;

    public bool isInvincible;
    public bool IsHit
    {
        get
        {
            return animator.GetBool(AnimationStrings.isHit);
        }
        private set
        {
            animator.SetBool(AnimationStrings.isHit, value);

            // Play hit audio clip
            if (audioSource != null && dmgClip != null && IsHit)
            {
                audioSource.PlayOneShot(dmgClip, 0.5f);
            }
        }
    }
    public float iFrames = 0.5f;
    private float timeSinceHit = 0;
    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        Health = maxHealth;
    }

    private void Update()
    {
        if (isInvincible)
        {
            if (timeSinceHit > iFrames)
            {
                isInvincible = false;
                timeSinceHit = 0;
                IsHit = false;
            }
            timeSinceHit += Time.deltaTime;
        }
    }

    public bool Hit(float damage)
    {
        if (IsAlive && !isInvincible)
        {
            Health -= damage;
            isInvincible = true;

            Debug.Log("Dealt " + damage + " damage to " + gameObject.name);

            IsHit = true;

            return true;
        }
        else if(IsAlive && iFrames == 0)
        {
            Health -= damage;

            Debug.Log("Dealt " + damage + " damage to " + gameObject.name);

            return true;
        }
        else
        {
            return false;
        }
    }
}
