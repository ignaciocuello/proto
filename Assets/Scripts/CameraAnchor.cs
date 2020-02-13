using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAnchor : MonoBehaviour {

    [SerializeField]
    private Vector3 defaultPos;

    [SerializeField]
    private Wall wall;
    public Wall Wall
    {
        get { return wall; }
        set
        {
            wall = value;
            if (wall != null)
            {
                transform.position = defaultPos;

                defaultY = transform.position.y;
                defaultWallScale = wall.transform.localScale;
            }
        }
    }

    private float defaultY;
    private Vector3 defaultWallScale;

    private void Awake()
    {
        if (wall != null)
        {
            defaultY = transform.position.y;
            defaultWallScale = wall.transform.localScale;
        }
    }

	private void Update()
    {
        if (wall != null)
        {
            float factor;
            Vector3 scale = GetScale(defaultWallScale, out factor);
            //scale x-z axes of wall so it fits the screen
            if (factor != 1.0f || wall.transform.localScale != Vector3.one)
            {
                wall.transform.localScale = scale;
            }
        }
    }

    public Vector3 GetScale(Vector3 defaultScale, out float factor)
    {
        factor = transform.position.y / defaultY;
        if (defaultY == 0.0f)
        {
            factor = 1.0f;
        }

        return Vector3.Scale(new Vector3(factor, 1.0f, factor), defaultScale);
    }
}
