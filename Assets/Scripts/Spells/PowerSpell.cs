using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class PowerSpell : Spell {

    [SerializeField]
    private GameObject powerSpellObjPrefab;

    public override GameObject Use()
    {
        //need toRemove List to prevent concurrent modification error
        List<Composite> toRemove = new List<Composite>();
        foreach (Composite comp in CompositePool.Instance.Composites)
        {
            if (comp.Color != Color.red)
            {
                toRemove.Add(comp);
            }
        }

        foreach (Composite comp in toRemove)
        {
            CompositePool.Instance.DestroyComposite(comp);
        }

        return powerSpellObjPrefab;
    } 
}
