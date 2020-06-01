using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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



    private float AttackTimer;
    [SerializeField]
    private float AttackCooldown = 2;
    private bool CanAttack = true;


    public bool LevelFinished = false;

    public override bool IsDead
    {
        get
        {
            return Health <= 0;
        }
    }

    [SerializeField]
    public Slider hitPoints;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        playerRigidbody = GetComponent<Rigidbody2D>();
        isSliding = false;
        startPosition = transform.position;
        hitPoints.maxValue = Health;
        hitPoints.value = Health;
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
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyUp(KeyCode.W))
        {
            Jump = true;
            CharacterAnimator.SetTrigger("jump");
        }

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyUp(KeyCode.S))
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
        hitPoints.value = Health;
        if (IsDead)
        {
            CharacterAnimator.SetLayerWeight(1, 0);
            CharacterAnimator.SetTrigger("die");


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
        Health = 10;
        hitPoints.value = Health;

    }
    public int coin = 0;
    public TextMeshProUGUI text;
    public override void OnTriggerEnter2D(Collider2D collider)
    {
        GameObject enemy = GameObject.Find("Enemy");
        Enemy enemyScript = enemy.GetComponent<Enemy>();
        if (collider.tag == "Enemy" && enemyScript.IsSliding)
        {
            StartCoroutine(TakeDamage());
        }

        if (collider.tag == "EndLevel")
        {
            LevelFinished = true;
        }

        if (collider.tag == "Coin")
        {
            coin++;
            text.text = coin.ToString();
            Destroy(collider.gameObject);

        }
    }
    private void Attack()
    {
        AttackTimer += Time.deltaTime;

        if (AttackTimer >= AttackCooldown)
        {
            CanAttack = true;
            AttackTimer = 0;
        }
        if (CanAttack)
        {
            CanAttack = false;
            CharacterAnimator.SetTrigger("slide");
        }
    }

}
