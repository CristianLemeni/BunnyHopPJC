﻿using System.Collections;
using UnityEngine;

public class Enemy : Character
{

    private IEnemyState currentState;

    public GameObject Target { get; set; }
    [SerializeField]
    public float AttackRange;
    public bool IsSliding;
    public bool InAttackRange
    {
        get
        {
            if (Target != null)
            {
                return Vector2.Distance(transform.position, Target.transform.position) <= AttackRange;
            }
            return false;
        }
    }

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

        IsSliding = CharacterAnimator.GetCurrentAnimatorStateInfo(0).IsName("EnemySlide");

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
        //get player script val
        GameObject player = GameObject.Find("Player");
        Player playerScript = player.GetComponent<Player>();
        //Debug.Log(playerScript.isSliding);

        if (collider.tag == "PlayerFeet" && playerScript.isSliding)
        {
            StartCoroutine(TakeDamage());
        }

        currentState.OnTriggerEnter(collider);
    }

    public void Move(float multiplier = 1)
    {
        CharacterAnimator.SetFloat("speed", 1);

        transform.Translate(GetDirection() * (movementSpeed * Time.deltaTime) * multiplier);
    }

    public Vector2 GetDirection()
    {
        return facingRight ? Vector2.right : Vector2.left;
    }

    public override IEnumerator TakeDamage()
    {
        Health -= 1;

        if (IsDead)
        {
            CharacterAnimator.SetTrigger("die");
            yield return null;
            Death();
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

        Destroy(gameObject, 2f);
    }
}
