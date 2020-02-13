using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class CompositesChangedEvent : UnityEvent<List<Composite>>
{
}

public class CompositePool : MonoBehaviour {

    public static CompositePool Instance;

    /* maximum components allowed on the screen */
    public int MaxComps;
    [SerializeField]
    private GameObject compositePrefab;

    private List<Composite> composites;
    public List<Composite> Composites
    {
        get { return composites; }
    }

    public CompositesChangedEvent CompositesChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Initialize();

            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Initialize()
    {
        composites = new List<Composite>();
    }

	public Composite CreateComposite()
    {
        if (composites.Count < MaxComps)
        {
            return ForceCreateComposite();
        }

        return null;
    }

    /* Same as CreateComposite, but there is no check to see if maximum Count
    * is exceeded */
    public Composite ForceCreateComposite()
    {
        Composite comp = Instantiate(compositePrefab).GetComponent<Composite>();
        composites.Add(comp);
        CompositesChanged.Invoke(composites);

        return comp;
    }

    public void DestroyComposite(Composite composite)
    {
        composites.Remove(composite);
        CompositesChanged.Invoke(composites);

        Destroy(composite.gameObject);
    }

    public List<Composite> GetOverflowComposites()
    {
        if (composites.Count > MaxComps)
        {
            return composites.GetRange(MaxComps, composites.Count - MaxComps);
        }

        return new List<Composite>();
    }

    public void Clear()
    {
        bool changed = false;
        while (composites.Count > 0)
        {
            DestroyComposite(composites[0]);
            changed = true;
        }

        if (changed)
        {
            CompositesChanged.Invoke(composites);
        }
    }
}
