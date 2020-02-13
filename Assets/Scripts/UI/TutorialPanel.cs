using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPanel : MonoBehaviour {

    [SerializeField]
    private Text title;
    public Text Title
    {
        get { return title; }
    }

    [SerializeField]
    private Text body;
    public Text Body
    {
        get { return body; }
    }

    [SerializeField]
    private Text skip;
    public Text Skip
    {
        get { return skip; }
    }

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
