using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class OnBreakthroughEvent : UnityEvent<Composite>
{
}

public class Wall : MonoBehaviour {

    public OnCubeEnterEvent OnCubeEnter;
    public OnBreakthroughEvent OnBreakthrough;

    [SerializeField]
    private Player player;

    [SerializeField]
    private GameObject brickPrefab;

    [SerializeField]
    private List<Brick> bricks;
    
    [SerializeField]
    private int numBricksWidth;
    public int NumBricksWidth
    {
        get { return numBricksWidth; }
    }

    [SerializeField]
    private int numBricksHeight;
    public int NumBricksHeight
    {
        get { return numBricksHeight; }
    }

    /* the total length of the cubes when placed adjacent to eachother horizontally */
    private float DerivedWidth;
    /* the total length of the cubes when placed adjacent to eachother vertically */
    private float DerivedHeight;

    /* how much to increase each dimension of the box collider from the original 
    derived width/height */
    [SerializeField]
    private float ColliderWidthIncrease;
    [SerializeField]
    private float ColliderHeightIncrease;

    private BoxCollider boxCollider;
    public bool Invincible
    {
        get { return bricks[0].Invincible; }
        set
        {
            foreach (Brick brick in bricks)
            {
                if (brick != null)
                {
                    brick.Invincible = value;
                }
            }
        }
    }

    private void Awake()
    {
        bricks = new List<Brick>();
        
        CreateBricks();
        CreateBoxCollider();

        Camera.main.GetComponentInParent<CameraAnchor>().Wall = this;
    }

    private void CreateBricks()
    {
        DerivedWidth = brickPrefab.transform.localScale.x * numBricksWidth;
        DerivedHeight = brickPrefab.transform.localScale.x * numBricksHeight;

        if (numBricksHeight == 0)
        {
            return;
        }

        //top row from left to right
        GenerateBrickLine(
            new Vector3(-DerivedWidth / 2.0f + brickPrefab.transform.localScale.x/2.0f, 0.0f,
                        DerivedHeight / 2.0f - brickPrefab.transform.localScale.y/2.0f), Vector3.right, numBricksWidth);
        if (numBricksHeight > 1)
        {
            //right column from top to bottom
            GenerateBrickLine(bricks[bricks.Count - 1].transform.localPosition, Vector3.back, numBricksHeight, 1);
            //bottom row from right to left
            GenerateBrickLine(bricks[bricks.Count - 1].transform.localPosition, Vector3.left, numBricksWidth, 1);
            //left column from bottom to top
            GenerateBrickLine(bricks[bricks.Count - 1].transform.localPosition, Vector3.forward, numBricksHeight - 1, 1);
            //numBricksHeight-1 used because that brick is already placed in top row
        }
    }

    private void CreateBoxCollider()
    {
        boxCollider = GetComponent<BoxCollider>();
        boxCollider.size = new Vector3(
            DerivedWidth + ColliderWidthIncrease, boxCollider.size.y, DerivedHeight + ColliderHeightIncrease);
    }

    private void GenerateBrickLine(Vector3 origin, Vector3 direction, int numGenerations)
    {
        GenerateBrickLine(origin, direction, numGenerations, offset:0);
    }

    private void GenerateBrickLine(Vector3 origin, Vector3 direction, int numGenerations, int offset)
    {
        for (int i = offset; i < numGenerations; i++)
        {
            Brick brick = Instantiate(brickPrefab, transform).GetComponent<Brick>();
            brick.transform.localPosition =
                origin + i * Vector3.Scale(brick.transform.localScale, direction);
            brick.OnCubeEnter.AddListener(OnCubeEnterCallback);

            bricks.Add(brick);
        }
    }

    public void OnCubeEnterCallback(Composite comp)
    {
        OnCubeEnter.Invoke(comp);
    }

    public void DestroyCubeAt(int index)
    {
        Destroy(bricks[index].gameObject);
        //bricks.RemoveAt(index);
    }

    public void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Target"))
        {
            Composite comp = collider.GetComponentInParent<Composite>();
            OnBreakthrough.Invoke(comp);
            player.HP--;

            CompositePool.Instance.DestroyComposite(comp);

            
        }
    }

    public void ReviveDeadBricks()
    {
        foreach (Brick brick in bricks)
        {
            if (brick.HasDied)
            {
                brick.gameObject.SetActive(true);
            }
        }
    }

    public void RekillDeadBricks()
    {
        foreach (Brick brick in bricks)
        {
            if (brick.HasDied)
            {
                brick.gameObject.SetActive(false);
            }
        }
    }
}
