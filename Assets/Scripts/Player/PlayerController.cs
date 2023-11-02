using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Canvas gameOver;
    public AudioSource music;
    private Rigidbody2D rb2d;
    private TouchingDirections touching;
    private Animator animator;
    private Damageable damageable;
    private Dash dash;

    public AudioSource audioSource;
    public AudioClip walk1;
    public AudioClip walk2;
    public AudioClip land;

    Vector2 moveInput;
    public float runSpeed = 7.5f;

    public float CurrentMoveSpeed
    {
        get
        {
            if (CanMove && !touching.IsOnWall)
            {
                if (IsMoving)
                {
                    return runSpeed;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }
        set
        {

        }
    }
    private bool isMoving;
    public bool IsMoving
    {
        get
        {
            return isMoving;
        }

        private set
        {
            isMoving = value;
            animator.SetBool(AnimationStrings.isMoving, value);
        }
    }
    public bool CanMove
    {
        get
        {
            return animator.GetBool(AnimationStrings.canMove);
        }
    }

    public float jumpImpulse = 7.5f;
    public float fallGravityScale = 7f;
    private int jumpCount;

    private bool isFacingRight = true;
    public bool IsFacingRight
    {
        get
        {
            return isFacingRight;
        }
        private set
        {
            if (isFacingRight != value)
            {
                transform.localScale *= new Vector2(-1, 1);
            }

            isFacingRight = value;
        }
    }

    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        touching = GetComponent<TouchingDirections>();
        animator = GetComponent<Animator>();
        damageable = GetComponent<Damageable>();
        dash = GetComponent<Dash>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        animator.SetFloat(AnimationStrings.yVelocity, rb2d.velocity.y);

        if (rb2d.velocity.y < 0)
        {
            rb2d.gravityScale = fallGravityScale;
        }
        else if (dash.IsDashing)
        {
            rb2d.gravityScale = 0;
        }
        else
        {
            rb2d.gravityScale = 4f;
        }

        if (touching.IsOnWall && rb2d.velocity.y < 0)
        {
            rb2d.gravityScale = 1f;
        }

        if (!damageable.IsAlive)
        {
            Time.timeScale = 0f;
            gameOver.gameObject.SetActive(true);
            music.Stop();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("MusicPlayer"))
        {
            music.PlayDelayed(0.5f);
            Destroy(collision.gameObject);
        }
    }

    private void FixedUpdate()
    {
        rb2d.velocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb2d.velocity.y);

        if (damageable.IsHit)
        {
            animator.SetTrigger(AnimationStrings.isHit);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        IsMoving = moveInput != Vector2.zero;

        SetFacingDirection(moveInput);
    }

    private void SetFacingDirection(Vector2 moveInput)
    {
        if (moveInput.x > 0 && !IsFacingRight)
        {
            IsFacingRight = true;
        }
        else if (moveInput.x < 0 && IsFacingRight)
        {
            IsFacingRight = false;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (touching.IsGrounded)
        {
            jumpCount = 0;
            Debug.Log("Jumps reset!");
        }

        if (context.started && touching.IsGrounded)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpImpulse);
            animator.SetTrigger(AnimationStrings.jump);
        }
        else if (context.started && jumpCount == 0 && touching.IsOnWall)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpImpulse);
            jumpCount++;
        }
        else if (jumpCount < 3 && context.started && touching.IsOnWall)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpImpulse);
            jumpCount++;
        }
        else if (context.canceled)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, rb2d.velocity.y * 0.5f);
            rb2d.gravityScale = fallGravityScale;
        }
    }

    public void PlayWalkOne()
    {
        audioSource.PlayOneShot(walk1, 0.45f);
    }

    public void PlayWalkTwo()
    {
        audioSource.PlayOneShot(walk2, 0.45f);
    }

    public void Land()
    {
        audioSource.PlayOneShot(land, 0.15f);
    }
}
