using System.Collections;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    public Animator CharacterAnimator{get; private set;}

    [SerializeField]
    protected float movementSpeed;

    [SerializeField]
    protected int Health;

    protected bool facingRight;

    public abstract bool IsDead {get;}

    public bool TakeDamege { get; set; }



    // Start is called before the first frame update
    public virtual void Start()
    {
        facingRight = true;
        CharacterAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeDirection(){
        facingRight = !facingRight;
        transform.localScale = new Vector3(transform.localScale.x * -1, 1, 1);
    }

    public virtual void OnTriggerEnter2D(Collider2D collider)
    {
    }

    public abstract IEnumerator TakeDamage();

    public abstract void Death();

    
}
