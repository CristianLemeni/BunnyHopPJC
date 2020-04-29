using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D playerRigidbody;
    private Animator playerAnimator;

    [SerializeField]
    private float movementSpeed;

    private bool facingRight;

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
    void Start()
    {
        facingRight = true;
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        isGrounded = Grounded();

        HandleInput();

        HandleMovement(horizontal);

        HandleLayers();
        
        ChangeDirection(horizontal);

        Reset();
    }

    private void HandleMovement(float horizontal){
        if(playerRigidbody.velocity.y < 0){
            playerAnimator.SetBool("land", true);
        }
        if(isGrounded || airRunning){
            playerRigidbody.velocity = new Vector2(horizontal * movementSpeed, playerRigidbody.velocity.y);

            playerAnimator.SetFloat("speed", Mathf.Abs(horizontal));
        }

        if(isGrounded && isJumping){
            isGrounded = false;

            playerRigidbody.AddForce(new Vector2(0, jump));
            playerAnimator.SetTrigger("jump");
        }
    }

    private void HandleInput(){
        if(Input.GetKeyDown(KeyCode.Space)){
            isJumping = true;
        }
    }

    private void ChangeDirection(float horizontal){
        if(horizontal > 0 && !facingRight || horizontal < 0 && facingRight){
            facingRight = !facingRight;
            Vector3 playerScale = transform.localScale;

            playerScale.x *= -1;

            transform.localScale = playerScale;
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
                        playerAnimator.ResetTrigger("jump");
                        playerAnimator.SetBool("land", false);
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
            playerAnimator.SetLayerWeight(1, 1);
        }
        else{
            playerAnimator.SetLayerWeight(1, 0);
        }
    }
}
