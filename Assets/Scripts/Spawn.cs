using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SizeDistributionElem
{
    /* incusive [Min, Max] */
    public int Min;
    public int Max;

    public int GetValue()
    {
        return Random.Range(Min, Max+1);
    }
}


public class Spawn : MonoBehaviour {

    private static readonly int MIN = 0;
    private static readonly int MAX = 1;

    /* whether cubes are automatically spawned or not, based on batch size, spawn rate, etc. */
    [SerializeField]
    private bool auto;
    public bool Auto
    {
        get { return auto; }
        set { auto = value; }
    }

    [SerializeField]
    private GameObject cubePrefab;

    [SerializeField]
    private Color[] colors;

    [SerializeField]
    private float cubeProb;

    /* how much time to wait in-between waves */
    public float InterWaveWaitTime;

    /* the distribution of crystals of different sizes */
    public List<SizeDistributionElem> SpawnSizeDistribution;

    public float[] RelativeForceRange;
    public float[] BurstDurationRange;
    public float[] BurstRateRange;

    public bool AllowSpawn = true;

    /* whether we're in the spawning period or not */
    private bool spawning;
	
    private void Awake()
    {
        CompositePool.Instance.CompositesChanged.AddListener(OnCompositesChanged);
    }

	private void Update () {
        if (auto && AllowSpawn)
        {
            SpawnWave();
        }
	}

    public List<Vector2> CreateRandom(int size, List<Vector2> free)
    {
        List<Vector2> current = new List<Vector2>();
        List<Vector2> adjacent = GetAdjacent(current, free);

        while (current.Count < size)
        {
            if (adjacent.Count == 0)
            {
                return current;
            }

            Vector2 randAdj = adjacent[Random.Range(0, adjacent.Count)];
            free.Remove(randAdj);

            current.Add(randAdj);

            adjacent = GetAdjacent(current, free);
        }

        return current;
    }

    public List<Vector2> GetAdjacent(List<Vector2> current, List<Vector2> free)
    {
        List<Vector2> adjacent = new List<Vector2>();
        if (current.Count == 0)
        {
            adjacent.AddRange(free);
            return adjacent;
        }

        foreach (Vector2 c in current)
        {
            Vector2 up = c + Vector2.up / transform.localScale.y;
            Vector2 down = c + Vector2.down / transform.localScale.y;
            Vector2 left = c + Vector2.left / transform.localScale.x;
            Vector2 right = c + Vector2.right / transform.localScale.x;

            if (free.Contains(up))
            {
                adjacent.Add(up);
            }
            if (free.Contains(down))
            {
                adjacent.Add(down);
            }
            if (free.Contains(left))
            {
                adjacent.Add(left);
            }
            if (free.Contains(right))
            {
                adjacent.Add(right);
            }
        }

        return adjacent;
    }

    private void SpawnWave()
    {
        //list of all valid grid positions
        List<Vector2> xzPos = GetXZPositions();
        List<List<Vector2>> allStructs = new List<List<Vector2>>();

        for (int size = SpawnSizeDistribution.Count; size > 0; size--)
        {
            int amount = SpawnSizeDistribution[size-1].GetValue();
            while (amount > 0)
            {
                List<Vector2> curr = CreateRandom(size, xzPos);
                if (curr.Count > 0)
                { 
                    allStructs.Add(curr);
                }
                amount--;
            }
        }

        float heightDif = 0.0f;
        foreach (List<Vector2> strc in allStructs)
        {
            foreach (Vector2 pos in strc)
            {
                float angle = Random.Range(-2, 2) * 90.0f;

                float relativeForce = Random.Range(RelativeForceRange[MIN], RelativeForceRange[MAX]);
                float burstDuration = Random.Range(BurstDurationRange[MIN], BurstDurationRange[MAX]);
                float burstDelay = Random.Range(BurstRateRange[MIN], BurstRateRange[MAX]);

                Color color = colors[Random.Range(0, colors.Length)];

                Composite comp = SpawnCube(pos, angle, relativeForce, burstDuration, burstDelay, color);
                comp.transform.position += Vector3.up * heightDif;
            }
            heightDif += 100.0f;
        }

        ForceStick();

        foreach (Composite comp in CompositePool.Instance.Composites)
        {
            foreach (CubeScript cube in comp.GetComponentsInChildren<CubeScript>())
            {
                cube.PreventSticky = true;
            }

            comp.transform.position += comp.transform.position.y * Vector3.down + 0.5f * Vector3.up;
            for (int i = 0; i < comp.transform.childCount; i++)
            {
                comp.transform.GetChild(i).transform.localPosition +=
                    comp.transform.GetChild(i).transform.localPosition.y * Vector3.down;
            }
            comp.CreateTarget();

            //comp.transform.position += comp.transform.GetChild(0).transform.position.y * Vector3.down + 0.5f * Vector3.up;
        }
        //Debug.Break();
        AllowSpawn = false;
    }

    /* must be called after spawn waves */
    public void ForceStick()
    {
        //little hack :smug:
        Physics.autoSimulation = false;
        Physics.Simulate(Time.fixedDeltaTime);
        Physics.autoSimulation = true;

        //remove composites if we've accidentally overflowed, by not combining cubes
        List<Composite> remainder = CompositePool.Instance.GetOverflowComposites();
        while (CompositePool.Instance.Composites.Count > CompositePool.Instance.MaxComps)
        {
            Composite minChild = null;
            foreach (Composite rem in remainder)
            {
                if (minChild == null)
                {
                    minChild = rem;
                }
                else
                {
                    minChild = (rem.transform.childCount < minChild.transform.childCount) ? rem : minChild;
                }
            }

            remainder.Remove(minChild);
            CompositePool.Instance.DestroyComposite(minChild);
        }
    }

    public Composite SpawnCube(
        Vector2 pos, float angle, float relativeForce, float burstDuration, float burstDelay, Color color)
    {
        Composite comp = CompositePool.Instance.ForceCreateComposite();

        if (comp != null)
        {
            CubeScript spawned = null;
            float rand = Random.value;
            if (rand < cubeProb)
            {
                spawned = SpawnCube(relativeForce, burstDuration, burstDelay, color);
            }
            else if (rand < cubeProb + 0.0f)
            {
                //TODO: add probability based spawning
            }


            //center random position
            spawned.transform.position = transform.TransformPoint(new Vector3(pos.x, pos.y, 0.0f)) + 0.5f * Vector3.one;
            spawned.transform.rotation *= Quaternion.AngleAxis(angle, Vector3.up);

            spawned.transform.parent = comp.transform;
            comp.Color = spawned.Color;
            comp.HP = 1;

            comp.AbsorbRigidbody(spawned.GetComponent<Rigidbody>());
            Destroy(spawned.GetComponent<Rigidbody>());

            comp.CreateTarget();
        }

        return comp;
    }

    private CubeScript SpawnCube(float relativeForce, float burstDuration, float burstDelay, Color color)
    {
        GameObject spawned = Instantiate(cubePrefab);

        CubeScript cube = spawned.GetComponent<CubeScript>();

        cube.RelativeForce = new Vector3(0.0f, 0.0f, relativeForce);
        cube.BurstDuration = burstDuration;
        cube.BurstDelay = burstDelay;
        cube.PreventSticky = false;

        cube.Color = color;

        return cube;
    }

    private List<Vector2> GetXZPositions()
    {
        float division = transform.localScale.x;

        List<Vector2> xzPositions = new List<Vector2>();
        for (float x = -division / 2.0f; x < division / 2.0f; x++)
        {
            for (float z = -division / 2.0f; z < division / 2.0f; z++)
            {
                xzPositions.Add(new Vector2(x / division, z / division));
            }
        }

        return xzPositions;
    }

    private CubeScript SpawnCube()
    {
        GameObject spawned = Instantiate(cubePrefab);

        CubeScript cube = spawned.GetComponent<CubeScript>();

        cube.RelativeForce = new Vector3(0.0f, 0.0f, Random.Range(RelativeForceRange[MIN], RelativeForceRange[MAX]));
        cube.BurstDuration = Random.Range(BurstDurationRange[MIN], BurstDurationRange[MAX]);
        cube.BurstDelay = Random.Range(BurstRateRange[MIN], BurstRateRange[MAX]);

        cube.Color = colors[Random.Range(0, colors.Length)];

        return cube;
    }

    public void OnCompositesChanged(List<Composite> composites)
    {
        if (composites.Count == 0)
        {
            StartCoroutine(AllowSpawnCoroutine());
        }
    }

    IEnumerator AllowSpawnCoroutine()
    {
        yield return new WaitForSeconds(InterWaveWaitTime);

        AllowSpawn = true;
    }

    private void OnDestroy()
    {
        CompositePool.Instance.CompositesChanged.RemoveListener(OnCompositesChanged);
    }
}
