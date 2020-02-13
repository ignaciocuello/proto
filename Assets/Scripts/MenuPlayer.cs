using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPlayer : MonoBehaviour {

    [SerializeField]
    private Vector3 initialCameraPos;

    private void Start()
    {
        Camera.main.GetComponentInParent<CameraAnchor>().transform.position = initialCameraPos;
        UI.Instance.gameObject.SetActive(false);
    }

	private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Input.GetMouseButtonDown(0))
        {
            //see if mouse clicked on target
            if (Physics.Raycast(ray, out hit, float.PositiveInfinity, LayerMask.GetMask("Target")))
            {
                //yes I know this is so bad, I will change it.. maybe
                //survival pressed
                if (hit.collider.transform.position.z > 0.0f)
                {
                    GameManager.Instance.LoadSceneAsync("Main");
                }
                //tutorial pressed
                else
                {
                    GameManager.Instance.LoadSceneAsync("Tutorial");
                }
            }
        }
    }    
}
