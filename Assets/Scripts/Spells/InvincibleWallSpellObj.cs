using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvincibleWallSpellObj : MonoBehaviour {

    public float Duration;

    private void Awake()
    {
        StartCoroutine(PlayEffect());
    }

    private void SetWallsInvincible(bool invincible)
    {
        GameObject[] wallObjs = GameObject.FindGameObjectsWithTag("Wall");
        foreach (GameObject wallObj in wallObjs)
        {
            wallObj.GetComponent<Wall>().Invincible = invincible;
        }
    }

    IEnumerator PlayEffect()
    {
        SetWallsInvincible(true);
        yield return new WaitForSeconds(Duration);
        SetWallsInvincible(false);
    }
}
