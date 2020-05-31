using System.Collections;
using UnityEngine;

public class Player : Character
{
    private Rigidbody2D playerRigidbody;

    [SerializeField]
    private Transform[] groundPoints;

    [SerializeField]
    private float radius;

    [SerializeField]
    private LayerMask isGround;

    private bool IsGrounded;
    private bool Jump;


    public bool isSliding;

    [SerializeField]
    private float JumpForce;

    [SerializeField]
    private bool airRunning;

    private Vector3 startPosition;

    public override bool IsDead
    {
        get
        {
            return Health <= 0;
        }
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        playerRigidbody = GetComponent<Rigidbody2D>();
        isSliding = false;
        startPosition = transform.position;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (TakeDamege)
        {
            playerRigidbody.velocity = Vector2.zero;
            return;
        }
        float horizontal = Input.GetAxis("Horizontal");

        IsGrounded = Grounded();

        if (playerRigidbody.position.y <= -14f) { Death(); }


        HandleLayers();
        PlayerChangeDirection(horizontal);
        HandleInput();
        HandleMovement(horizontal);




        Reset();
        isSliding = CharacterAnimator.GetCurrentAnimatorStateInfo(0).IsName("Slide");

    }

    private void HandleMovement(float horizontal)
    {
        if (playerRigidbody.velocity.y < 0)
        {
            CharacterAnimator.SetBool("land", true);
        }

        if (!isSliding && (IsGrounded || airRunning))
        {
            playerRigidbody.velocity = new Vector2(horizontal * movementSpeed, playerRigidbody.velocity.y);
        }

        if (!isSliding && IsGrounded && Jump)
        {
            IsGrounded = false;
            playerRigidbody.AddForce(new Vector2(0, JumpForce));

        }

        CharacterAnimator.SetFloat("speed", Mathf.Abs(horizontal));
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            Jump = true;
            CharacterAnimator.SetTrigger("jump");
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            CharacterAnimator.SetTrigger("slide");
            isSliding = true;
        }

    }

    private void PlayerChangeDirection(float horizontal)
    {
        if (horizontal > 0 && !facingRight || horizontal < 0 && facingRight)
        {
            ChangeDirection();
        }
    }

    private bool Grounded()
    {
        if (playerRigidbody.velocity.y <= 0)
        {
            foreach (Transform point in groundPoints)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(point.position, radius, isGround);

                for (int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i].gameObject != gameObject)
                    {
                        CharacterAnimator.ResetTrigger("jump");
                        CharacterAnimator.SetBool("land", false);
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private void Reset()
    {
        Jump = false;
        isSliding = false;

    }

    private void HandleLayers()
    {
        if (!IsGrounded)
        {
            CharacterAnimator.SetLayerWeight(1, 1);
        }
        else
        {
            CharacterAnimator.SetLayerWeight(1, 0);
        }
    }

    public override IEnumerator TakeDamage()
    {
        Health -= 1;

        if (IsDead)
        {
            CharacterAnimator.SetLayerWeight(1, 0);
            CharacterAnimator.SetTrigger("die");

            Health = 10;
            Invoke("Death", 1);

            yield return null;
        }
        else
            CharacterAnimator.SetTrigger("hurt");
    }


    public override void Death()
    {
        playerRigidbody.velocity = Vector2.zero;
        transform.position = startPosition;

    }

    public override void OnTriggerEnter2D(Collider2D collider)
    {
        
        if (collider.tag == "Enemy")
        {
            StartCoroutine(TakeDamage());
        }
    }

}
