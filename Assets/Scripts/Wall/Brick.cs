using EZCameraShake;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class OnCubeEnterEvent : UnityEvent<Composite>
{
}

public class Brick : MonoBehaviour {

    public OnCubeEnterEvent OnCubeEnter;

    [SerializeField]
    private Material waterMaterial;
    [SerializeField]
    private Material normalMat;
    [SerializeField]
    private Material rainbowMat;

    [SerializeField]
    private CameraShakeSettings destroyCamShakeSettings;

    public Wall wall;

    private bool invincible;
    public bool Invincible
    {
        get { return invincible; }
        set
        {
            invincible = value;
            if (hasDied)
            {
                GetComponent<MeshRenderer>().material = invincible ? rainbowMat : waterMaterial;
            }
            else
            {
                GetComponent<MeshRenderer>().material = invincible ? rainbowMat : normalMat;
            } 
        }
    }

    private bool hasDied;
    public bool HasDied
    {
        get { return hasDied; }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Cube"))
        {
            Composite comp = collision.collider.GetComponentInParent<Composite>();

            //check if HP > 0, this so if the cube has only one HP, but touches
            //multiple bricks at once, only one will be destroyed
            if (comp.HP > 0)
            {
                //shake the cam, for the fans
                destroyCamShakeSettings.Shake();

                if (!Invincible)
                {
                    gameObject.SetActive(false);

                    if (!hasDied) {
                        hasDied = true;
                        GetComponent<MeshRenderer>().material = waterMaterial;
                    }
                }

                comp.HP--;
                OnCubeEnter.Invoke(comp);
            }
        }        
    }
}
