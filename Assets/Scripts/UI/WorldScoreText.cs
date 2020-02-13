using UnityEngine;

public class WorldScoreText : MonoBehaviour {

    [SerializeField]
    private float updwardsThrust;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(Vector3.forward * updwardsThrust, ForceMode.Impulse);
    }

	public void DestroyGameObject()
    {
        Destroy(gameObject);
    }
}
