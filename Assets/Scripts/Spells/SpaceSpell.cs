using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceSpell : Spell {

    [SerializeField]
    private GameObject spaceSpellObjPrefab;

    public override GameObject Use()
    {
        return spaceSpellObjPrefab;
    }
}
