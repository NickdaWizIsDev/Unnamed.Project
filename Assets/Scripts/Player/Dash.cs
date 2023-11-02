using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Dash : MonoBehaviour
{
    Animator animator;
    Rigidbody2D rb2d;
    PlayerController controller;
    Damageable damageable;

    [SerializeField]
    private bool isDashing = false;
    public bool IsDashing
    {
        get
        {
            return isDashing;
        }
        private set
        {
            isDashing = value;
            animator.SetBool(AnimationStrings.isDashing, value);
            damageable.isInvincible = value;
        }
    }

    public float dashDistance = 3f;
    public float dashDuration = 0.25f;
    public float dashSpeed = 35f;
    public float dashCooldown = 1.5f;
    public int dashCharges = 3;
    private float dashStartTime;
    private float dashTimer;

    public AudioSource audioSource;
    public AudioClip dash;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        controller = GetComponent<PlayerController>();
        damageable = GetComponent<Damageable>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if(dashCharges < 3)
        {
            dashTimer += Time.deltaTime;
        }

        if(dashTimer >= 3f)
        {
            dashCharges++;
            dashTimer = 0f;
        }
    }

    private void Start()
    {
        dashCooldown = 0f;
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.started && !IsDashing && Time.time >= dashStartTime + dashCooldown && dashCharges > 0)
        {
            StartCoroutine(UltraDash(transform.localScale.x));
            dashCharges--;
        }
    }

    private IEnumerator UltraDash(float direction)
    {
        IsDashing = true;
        dashStartTime = Time.time;
        audioSource.PlayOneShot(dash, 0.25f);

        float normalSpeed = controller.runSpeed;
        controller.runSpeed = dashSpeed;

        // Determine the dash direction based on the player's facing direction
        Vector2 dashDirection = controller.IsFacingRight ? Vector2.right : Vector2.left;
        Vector2 dashForce = dashDirection * (dashDistance / dashDuration);

        rb2d.AddForce(dashForce, ForceMode2D.Impulse);
        rb2d.velocity = new Vector2(rb2d.velocity.x, 0);

        yield return new WaitForSeconds(dashDuration);

        controller.runSpeed = normalSpeed;
        IsDashing = false;
    }
}
