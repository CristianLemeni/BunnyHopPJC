using System.Collections;
using UnityEngine;

public class Enemy : Character
{

    private IEnemyState currentState;

    public GameObject Target { get; set; }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        ChangeState(new IdleState());
    }

    // Update is called once per frame
    void Update()
    {
        if (IsDead) return;

        if (TakeDamege) { LookAt(); return; }

        currentState.Execute();
        LookAt();

    }

    private void LookAt()
    {
        if (Target != null)
        {
            float xDirection = Target.transform.position.x - transform.position.x;

            if (xDirection < 0 && facingRight || xDirection > 0 && !facingRight)
            {
                ChangeDirection();
            }
        }
    }

    public void ChangeState(IEnemyState newState)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }

        currentState = newState;
        currentState.Enter(this);
    }

    public override void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Player" )
        {
            StartCoroutine(TakeDamage());
        }
      
        currentState.OnTriggerEnter(collider);
    }

    public void Move()
    {
        CharacterAnimator.SetFloat("speed", 1);

        transform.Translate(GetDirection() * (movementSpeed * Time.deltaTime));
    }

    public Vector2 GetDirection()
    {
        return facingRight ? Vector2.right : Vector2.left;
    }

    public override IEnumerator TakeDamage()
    {
        Health  -= 1;

        if (IsDead)
        {
            CharacterAnimator.SetTrigger("die");
            yield return null;
        }
        else
            CharacterAnimator.SetTrigger("hurt");

    }

    public override bool IsDead
    {
        get
        {
            return Health <= 0;
        }
    }

    public override void Death()
    {
        Destroy(gameObject);
    }
}
