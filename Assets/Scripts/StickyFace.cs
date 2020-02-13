using UnityEngine;

public class StickyFace : MonoBehaviour {

    [SerializeField]
    private CubeScript cube;

    /* normal of face in local coordinates */
    [SerializeField]
    private Vector3 localNormal;

    public void OnTriggerEnter(Collider collider)
    {
        //make sure collision is extra-composite not intra-composite
        if (collider.GetComponentInParent<Composite>() != GetComponentInParent<Composite>())
        {
            //collision layer is sticky face only, so safe to assume collider is
            //sticky face
            StickTo(collider.GetComponent<StickyFace>());
        }
    }

    public void StickTo(StickyFace stickyFace)
    {
        if (stickyFace == null || stickyFace.cube.PreventSticky || cube.PreventSticky)
        {
            return;
        }

        //TODO: add stickying to bigger component (order decider)

        Composite root = GetComponentInParent<Composite>();
        Composite stickyFaceRoot = stickyFace.GetComponentInParent<Composite>();

        root.transform.rotation = stickyFaceRoot.transform.rotation;

        //this face's normal in world coordinates
        Vector3 normalWorld = cube.transform.TransformVector(localNormal).normalized;
        //the other face's normal in world coordinates
        Vector3 otherNormalWorld = stickyFace.cube.transform.TransformVector(stickyFace.localNormal).normalized;

        Vector3 cubeUp = cube.transform.TransformVector(new Vector3(0.0f, 1.0f, 0.0f));

        //-otherNormalWorld to have faces against eachother, normals lining up
        root.transform.rotation *= Quaternion.AngleAxis(Vector3.SignedAngle(normalWorld, -otherNormalWorld, cubeUp), cubeUp);

        //target position in world coordinates to place this face (local normals match local point coordinates)
        Vector3 targetPos = stickyFace.cube.transform.TransformPoint(stickyFace.localNormal);
        root.transform.position += (targetPos - cube.transform.position);

        //force create because using normal create we might not be allowed
        //to create a new composite e.g. if the current comp number is 10
        //and the max is 10, however because we destroy two composites in
        //the merging process the resulting number of composites will be
        //9 which is < 10
        Composite composite = CompositePool.Instance.ForceCreateComposite();
        composite.Merge(root, stickyFaceRoot);
        composite.Color = cube.GetComponent<CubeScript>().Color;
    }

}
