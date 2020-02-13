using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvincibleWallSpell : Spell {

    [SerializeField]
    private GameObject invincibleWallSpellObjPrefab;

    public override GameObject Use()
    {
        return invincibleWallSpellObjPrefab;
    }
}
