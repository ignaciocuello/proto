using System.Collections.Generic;
using UnityEngine;

public abstract class Spell : ScriptableObject {

    public List<Color> Signature;

    public abstract GameObject Use();
}
