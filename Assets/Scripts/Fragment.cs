using UnityEngine;

public class Fragment : MonoBehaviour {

    [SerializeField]
    private Vector3 angularVelocity;

    private Rigidbody rb;

    private MeshRenderer meshRenderer;
    public MeshRenderer MeshRenderer
    {
        get { return meshRenderer; }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.angularVelocity = angularVelocity;

        meshRenderer = GetComponentInChildren<MeshRenderer>();
    }
}
