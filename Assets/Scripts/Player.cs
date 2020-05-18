using System.Collections;
using System.Collections.Generic;
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

    private bool isGrounded;
    private bool isJumping;

    [SerializeField]
    private float jump;

    [SerializeField]
    private bool airRunning;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        playerRigidbody = GetComponent<Rigidbody2D>();
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        isGrounded = Grounded();

        if(playerRigidbody.position.y <= -14f){
            Death();
        }

        HandleInput();

        HandleMovement(horizontal);

        HandleLayers();
        
        PlayerChangeDirection(horizontal);

        Reset();
    }

    private void HandleMovement(float horizontal){
        if(playerRigidbody.velocity.y < 0){
            PlayerAnimator.SetBool("land", true);
        }
        if(isGrounded || airRunning){
            playerRigidbody.velocity = new Vector2(horizontal * movementSpeed, playerRigidbody.velocity.y);

            PlayerAnimator.SetFloat("speed", Mathf.Abs(horizontal));
        }

        if(isGrounded && isJumping){
            isGrounded = false;

            playerRigidbody.AddForce(new Vector2(0, jump));
            PlayerAnimator.SetTrigger("jump");
        }
    }

    private void HandleInput(){
        if(Input.GetKeyDown(KeyCode.Space)){
            isJumping = true;
        }
    }

    private void PlayerChangeDirection(float horizontal){
        if(horizontal > 0 && !facingRight || horizontal < 0 && facingRight){
            ChangeDirection();
        }
    }

    private bool Grounded(){
        if(playerRigidbody.velocity.y <= 0){
            foreach (Transform point in groundPoints)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(point.position, radius, isGround);

                for (int i = 0; i < colliders.Length; i++)
                {
                    if(colliders[i].gameObject != gameObject){
                        PlayerAnimator.ResetTrigger("jump");
                        PlayerAnimator.SetBool("land", false);
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private void Reset(){
        isJumping = false;
    }

    private void HandleLayers(){
        if(!isGrounded){
            PlayerAnimator.SetLayerWeight(1, 1);
        }
        else{
            PlayerAnimator.SetLayerWeight(1, 0);
        }
    }

    public override IEnumerator TakeDamage(){
        health -= 1;

        if(!Die){
            PlayerAnimator.SetTrigger("hurt");
        }
        else{
            PlayerAnimator.SetTrigger("die");
            yield return null;
        }
    }

    public override bool Die{
        get {
            return health <= 0;
        }
    }

    public override void Death(){
        playerRigidbody.velocity = Vector2.zero;
        PlayerAnimator.SetTrigger("Idle");
        health = 50;
    }
}
