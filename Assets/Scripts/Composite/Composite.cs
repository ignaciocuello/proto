using UnityEngine;

public class Composite : MonoBehaviour {

    /* maximum hit points a composite may have */
    [SerializeField]
    private int maxHP;

    [SerializeField]
    private GameObject targetPrefab;

    [SerializeField]
    private Color color;
    public Color Color
    {
        get { return color; }
        set
        {
            color = value;
            //change the color of every cube
            for (int i = 0; i < transform.childCount; i++)
            {
                CubeScript c = transform.GetChild(i).GetComponent<CubeScript>();
                if (c != null)
                {
                    c.Color = color;
                }
            }
        }
    }

    private Target target;

    private int hp;
    public int HP
    {
        get { return hp; }
        set
        {
            int oldHp = hp;

            hp = Mathf.Clamp(value, 0, maxHP);
            //if hp was lost drop a fragment
            if (hp < oldHp)
            {
                Fragment frag = FragmentPool.Instance.CreateFragment(color);
                frag.transform.position = target.transform.position;
            }
            if (hp == 0)
            {
                CompositePool.Instance.DestroyComposite(this);
            }
        }
    }

    public void Merge(Composite c1, Composite c2)
    {
        Rigidbody r1 = c1.GetComponent<Rigidbody>();
        Rigidbody r2 = c2.GetComponent<Rigidbody>();

        //absorb sub-composite rigidbody properties (pick first one arbitarily)
        //NOTE: maybe later average properties of both c1 and c2
        AbsorbRigidbody(r1);

        //reshift rigidbodies
        Destroy(r1);
        Destroy(r2);

        c1.ReshiftCompositeTo(this);
        c2.ReshiftCompositeTo(this);

        CreateTarget();
    }

    public void AbsorbRigidbody(Rigidbody toAbsorb)
    {
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();

        rb.mass = toAbsorb.mass;
        rb.drag = toAbsorb.drag;
        rb.angularDrag = toAbsorb.angularDrag;
        rb.constraints = toAbsorb.constraints;
        rb.collisionDetectionMode = toAbsorb.collisionDetectionMode;
    }

    /* reshift self into newComposite */
    private void ReshiftCompositeTo(Composite newComposite)
    {
        while (transform.childCount > 0)
        {
            GameObject child = transform.GetChild(0).gameObject;
            //if child is cube add it
            if (child.GetComponent<CubeScript>() != null)
            {
                //add cube to new compsite, increase health by one
                child.transform.parent = newComposite.transform;
                newComposite.HP++;
            }
            //else child is target, so destroy it (no need to reshift)
            else
            {
                //set parent to null, so the while loop terminates
                //since Destroy is not instant
                child.transform.parent = null;
                Destroy(child.gameObject);
            }
        }

        //no children for this composite, destroy self
        CompositePool.Instance.DestroyComposite(this);
    }

    /* create target, and set position to average of all cubes */
    public void CreateTarget()
    {
        if (target != null)
        {
            Destroy(target.gameObject);
        }

        GameObject targetObj = Instantiate(targetPrefab);

        //calculate average position
        Vector3 avg = Vector3.zero;
        for (int i = 0; i < transform.childCount; i++)
        {
            avg += transform.GetChild(i).position;
        }
        avg /= transform.childCount;

        //set this as parent here, so the target does not count itself
        //when calculating the average
        targetObj.transform.parent = transform;
        //nudge it up a bit, to prevent clipping issues
        targetObj.transform.position = avg + new Vector3(0.0f, 0.1f, 0.0f);

        target = targetObj.GetComponent<Target>();
    }
}
 