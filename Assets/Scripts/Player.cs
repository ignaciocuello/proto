using EZCameraShake;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    [SerializeField]
    private int startingHP;

    [SerializeField]
    private GameObject worldScoreTextPrefab;
    [SerializeField]
    private CameraShakeSettings loseHPCamShakeSettings;

    private int hp;
    public int HP
    {
        get { return hp; }
        set
        {
            int dif = value - hp;
            if (dif < 0)
            {
                loseHPCamShakeSettings.Shake();
            }

            while (dif > 0)
            {
                UI.Instance.HealthUI.AddHeart();
                dif--;
            }
            while (dif < 0)
            {
                UI.Instance.HealthUI.RemoveHeart();
                dif++;
            }
            if (value == 0)
            {
                UI.Instance.HealthUI.Die();
            }

            hp = value;
        }
    }

    private void Awake()
    {
        HP = startingHP;
        GameManager.Instance.Score = 0;
    }

    private void Update () {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Input.GetMouseButtonDown(0))
        {
            //see if mouse clicked on a target
            if (Physics.Raycast(ray, out hit, float.PositiveInfinity, LayerMask.GetMask("Target")))
            {
                Composite comp = hit.collider.GetComponentInParent<Composite>();

                //decrease health and add color to power stack
                //PowerBank.Instance.AddColor(comp.Color);
                comp.HP--;

                GameManager.Instance.Score += GameManager.Instance.HitScore;

                TextMesh textMesh = Instantiate(worldScoreTextPrefab).GetComponent<TextMesh>();
                textMesh.transform.position = hit.collider.transform.position;
                textMesh.text = "+" + GameManager.Instance.HitScore;
                textMesh.color = comp.Color;
            }
        }
        else if (Input.GetMouseButton(1))
        {
            //see if mouse clicked on a fragment
            if (Physics.Raycast(ray, out hit, float.PositiveInfinity, LayerMask.GetMask("Fragment")))
            {
                //check parent for "Fragment" script, since child has collider
                Fragment frag = hit.collider.gameObject.GetComponentInParent<Fragment>();
                PowerQueue.Instance.AddColor(frag.MeshRenderer.material.color);

                FragmentPool.Instance.DestroyFragment(frag);
            }
        }
    }
}
