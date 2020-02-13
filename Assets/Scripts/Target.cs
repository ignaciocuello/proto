using UnityEngine;

public class Target : MonoBehaviour {

    private const int MIN = 0;
    private const int MAX = 0;

    /* speed range [a, b] considererd when altering the size of the target a speed <= a will result
     * in a size equal to the lower bound of size range, likewise a speed >= b will result in the
     * upper bound of size range */
    [SerializeField]
    private float[] speedRange;
    [SerializeField]
    private float[] sizeRange;

    private Vector3 defaultScale;
    private CameraAnchor camAnchor;

    private void Awake()
    {
        defaultScale = transform.localScale;
        camAnchor = Camera.main.GetComponentInParent<CameraAnchor>();
    }

	private void Update()
    {
        float factor;
        Vector3 scale = camAnchor.GetScale(defaultScale, out factor);
        if (factor != 1.0f || defaultScale != Vector3.one)
        {
            transform.localScale = scale;
        }
    }
}
