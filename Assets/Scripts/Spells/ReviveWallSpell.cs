using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviveWallSpell : Spell {

    [SerializeField]
    private GameObject reviveWallSpellObj;

    public override GameObject Use()
    {
        return reviveWallSpellObj;
    }
}
