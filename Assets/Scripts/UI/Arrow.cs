using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour {

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    //to be used as an animation event
    public void DestroyGameObject()
    {
        Destroy(gameObject);
    }

    //call this, instead of Destroy
    public void MakeDisappear()
    {
        animator.SetTrigger("Disappeared");
    }
}
