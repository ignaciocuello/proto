using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeSpell : Spell {

    [SerializeField]
    private float slowDownDuration;
    [SerializeField]
    private float slowDownFactor;
    [SerializeField]
    private float exponentialGrowthFactor;

    public override GameObject Use()
    {
        GameManager.Instance.TimeManager.SetSlowDownFactor(slowDownFactor, slowDownDuration, exponentialGrowthFactor);
        return null;
    }
}


