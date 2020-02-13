using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerQueue : MonoBehaviour {

    public static PowerQueue Instance;

    [SerializeField]
    private Spell[] spells;

    /* assumed to be in order red, green and blue */
    [SerializeField]
    private Image[] toggles;

    [SerializeField]
    private Color flushedColor;
    private List<Color> colors;

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
        foreach (Image img in toggles)
        {
            img.material = new Material(img.material)
            {
                color = flushedColor
            };
        }

        colors = new List<Color>();
    }

    //adds a color at the start of the toggles (from left to right on UI)
    public void AddColor(Color color)
    {
        //add color at beginning
        colors.Insert(0, color);
        //make sure there are only n colors, where n is the number of toggles
        if (colors.Count > toggles.Length)
        {
            colors.RemoveRange(toggles.Length, colors.Count - toggles.Length);
        }

        int i = 0;
        foreach (Color clr in colors)
        {
            toggles[i].material.color = clr;
            i++;
        }
    }

    public void Update()
    {
        if (Input.GetButton("Jump"))
        {
            foreach (Spell spell in spells)
            {
                if (spell.Signature.SequenceEquals(colors))
                {
                    GameObject spellObj = spell.Use();
                    if (spellObj != null)
                    {
                        Instantiate(spellObj);
                    }

                    Flush();
                    break;
                }
            }
        }
    }

    public void Flush()
    {
        colors.Clear();

        foreach (Image toggle in toggles)
        {
            toggle.material.color = flushedColor;
        }
    }
}

public static class ListExtension
{
    public static bool SequenceEquals<T>(this List<T> list, List<T> other)
    {
        if (list.Count != other.Count)
        {
            return false;
        }

        for (int i = 0; i < list.Count; i++)
        {
            if (!list[i].Equals(other[i]))
            {
                return false;
            }
        }

        return true;
    }
}