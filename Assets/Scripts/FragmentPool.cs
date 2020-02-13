using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentPool : MonoBehaviour {

    public static FragmentPool Instance;

    /* maximum fragments allowed in the pool */
    [SerializeField]
    private int maxFrags;
    [SerializeField]
    private GameObject fragmentPrefab;

    private Dictionary<Color, List<Fragment>> fragments;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Initialize()
    {
        fragments = new Dictionary<Color, List<Fragment>>
        {
            { Color.red, new List<Fragment>() },
            { Color.green, new List<Fragment>() },
            { Color.blue, new List<Fragment>() }
        };
    }

    public Fragment CreateFragment(Color color)
    {
        Fragment frag = Instantiate(fragmentPrefab).GetComponent<Fragment>();
        frag.MeshRenderer.material = new Material(frag.MeshRenderer.material)
        {
            color = color
        };

        List<Fragment> colorFragList = fragments[color];
        colorFragList.Add(frag);

        if (colorFragList.Count > maxFrags)
        {
            int dif = colorFragList.Count - maxFrags;
            for (int i = 0; i < dif; i++)
            {
                DestroyFragment(colorFragList[0]);
            }
        }

        return frag;
    }

    public void DestroyFragment(Fragment frag)
    {
        fragments[frag.MeshRenderer.material.color].Remove(frag);
        Destroy(frag.gameObject);
    }

    public void Clear()
    {
        foreach (List<Fragment> frags in fragments.Values)
        {
            while (frags.Count > 0)
            {
                DestroyFragment(frags[0]);
            }
        }
    }
}
