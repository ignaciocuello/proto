using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour {

    public static UI Instance;

    public HealthUI HealthUI;

    public Text ScoreValue;
    public Text LevelValue;

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
        //TODO: add functionality later
    }
}
