using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviveWallSpellObj : MonoBehaviour {

    public float Duration;

    private void Awake()
    {
        StartCoroutine(PlayEffect());
    }

    private void ReviveWalls(bool revive)
    {
        GameObject[] wallObjs = GameObject.FindGameObjectsWithTag("Wall");
        foreach (GameObject wallObj in wallObjs)
        {
            if (revive)
            {
                wallObj.GetComponent<Wall>().ReviveDeadBricks();
            }
            else
            {
                wallObj.GetComponent<Wall>().RekillDeadBricks();
            }
        }
    }

	IEnumerator PlayEffect()
    {
        ReviveWalls(true);
        yield return new WaitForSeconds(Duration);
        ReviveWalls(false);
    }
}
