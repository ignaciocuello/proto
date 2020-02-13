using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;

    public int HitScore;

    private int score;
    public int Score
    {
        get { return score; }
        set
        {
            score = value;
            UI.Instance.ScoreValue.text = score.ToString();
        }
    }

    private TimeManager timeManager;
    public TimeManager TimeManager
    {
        get { return timeManager; }
    }

    void Awake()
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
        Score = 0;
        timeManager = GetComponent<TimeManager>();

        DontDestroyOnLoad(gameObject);
        GoToMenu();
    }

    public void GoToMenu()
    {
        LoadSceneAsync("Menu");
    }

    public void LoadSceneAsync(string name)
    {
        StartCoroutine(LoadRoot(name));
    }

    IEnumerator LoadRoot(string name)
    {
        AsyncOperation asyncLoadRoot = SceneManager.LoadSceneAsync("Root", LoadSceneMode.Single);
        while (!asyncLoadRoot.isDone)
        {
            yield return null;
        }

        AsyncOperation asyncLoadScene = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);

        while (!asyncLoadScene.isDone)
        {
            yield return null;
        }
    }
}
