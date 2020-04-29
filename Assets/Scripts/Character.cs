﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    public Animator PlayerAnimator{get; private set;}

    [SerializeField]
    protected float movementSpeed;
    protected bool facingRight;
    // Start is called before the first frame update
    public virtual void Start()
    {
        facingRight = true;
        PlayerAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeDirection(){
        facingRight = !facingRight;
        transform.localScale = new Vector3(transform.localScale.x * -1, 1, 1);
    }
}