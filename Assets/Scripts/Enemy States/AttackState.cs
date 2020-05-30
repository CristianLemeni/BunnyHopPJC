using UnityEngine;

public class AttackState : IEnemyState
{
    private Enemy enemy;

    private float AttackTimer;
    private float AttackCooldown = 3;
    private bool CanAttack = true;



    public void Enter(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public void Execute()
    {
        Attack();
        enemy.Move();
        if (enemy.Target == null)
        {
            enemy.ChangeState(new IdleState());
        }
    }



    public void Exit()
    {

    }

    public void OnTriggerEnter(Collider2D other)
    {

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
            enemy.CharacterAnimator.SetTrigger("slide");
        }
    }
}
