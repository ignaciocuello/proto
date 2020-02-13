using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour {

    public static HealthUI Instance;

    [SerializeField]
    private GameObject heartPrefab;
    [SerializeField]
    private float heartGaps;

    [SerializeField]
    private float damageDuration;

    [SerializeField]
    private Image damageSheet;
    private Animator damageSheetAnimator;
    [SerializeField]
    private Animator deathTextAnimator;

    private List<GameObject> hearts;

    public void Awake() {
        hearts = new List<GameObject>();
        damageSheetAnimator = damageSheet.GetComponent<Animator>();
    }

    public void AddHeart()
    {
        GameObject heart = Instantiate(heartPrefab, transform);
        hearts.Add(heart);
        UpdateHeartPositions();
    }

    public void UpdateHeartPositions()
    {
        float heartWidth = heartPrefab.GetComponent<RectTransform>().sizeDelta.x;
        float totalWidth = hearts.Count * heartWidth + heartGaps * (hearts.Count - 1);

        float x = -totalWidth / 2.0f + heartWidth / 2.0f;
        foreach (GameObject heart in hearts)
        {
            heart.GetComponent<RectTransform>().localPosition = new Vector3(x, 0.0f, 0.0f);

            x += heartWidth + heartGaps;
        }
    }

    public void RemoveHeart()
    {
        if (hearts.Count > 0)
        {
            StartCoroutine(FlashDamage());
            
            int last = hearts.Count - 1;

            Destroy(hearts[last]);
            hearts.RemoveAt(last);
            UpdateHeartPositions();
        }
    }

    public void Die()
    {
        damageSheetAnimator.SetBool("TakingDamage", false);
        damageSheetAnimator.SetBool("Dead", true);
        deathTextAnimator.SetBool("Appear", true);
    }

    IEnumerator FlashDamage()
    {
        damageSheetAnimator.SetBool("TakingDamage", true);
        yield return new WaitForSeconds(damageDuration);
        damageSheetAnimator.SetBool("TakingDamage", false);

        Color c = damageSheet.material.color;
        damageSheet.material.color = new Color(c.r, c.g, c.b, 1.0f);
    }
}
