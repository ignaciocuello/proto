using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Tutorial
{
    public string Title;
    [TextArea]
    public string Body;
    public string Skip;

    public GameObject ArrowPrefab;
    public Vector2 ArrowPoint;
}

public class TutorialDirector : MonoBehaviour {

    private const int MAX_WALL_HITS = 2;

    [SerializeField]
    private List<GameObject> wallPrefabs;
    [SerializeField]
    private List<GameObject> spawnPrefabs;

    [SerializeField]
    public List<Tutorial> tutorials;
    private List<Action> tutorialActions;

    [SerializeField]
    private GameObject tutorialPanelPrefab;
    [SerializeField]
    private GameObject arrowPrefab;

    /* how much to wait after you're hurt before restarting the tutorial
     * section */
    [SerializeField]
    private float hurtWaitTime;

    [SerializeField]
    private Player player;
    private Wall wall;
    private Spawn spawn;

    private int tutorialIndex;
    public int TutorialIndex
    {
        get { return tutorialIndex; }
        set
        {
            tutorialIndex = Mathf.Clamp(value, 0, tutorials.Count - 1);
        }
    }

    private TutorialPanel currentTutorialPanel;
    private Arrow currentArrow;

    private int wallHits;

    private void Awake()
    {
        InitTutorialActions();

        SpawnTutorial();
    }

    private void InitTutorialActions()
    {
        tutorialActions = new List<Action>
        {
            ActivateSpawn,
            BreakThrough,
            ActivateSpawn,
            ActivateSpawn,
            ActivateSpawn,
            ActivateSpawn,
            ActivateSpawn,
            GoToMenu,
        };
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            TutorialIndex--;
            SpawnTutorial();
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            TutorialIndex++;
            SpawnTutorial();
        }
    }

    public void SpawnTutorial()
    {
        if (currentTutorialPanel != null)
        {
            currentTutorialPanel.MakeDisappear();
        }
        if (currentArrow != null)
        {
            currentArrow.MakeDisappear();
        }

        currentTutorialPanel = Instantiate(tutorialPanelPrefab).GetComponent<TutorialPanel>();

        Tutorial current = tutorials[tutorialIndex];
        currentTutorialPanel.Title.text = current.Title;
        currentTutorialPanel.Body.text = current.Body;
        if (current.Skip != "")
        {
            currentTutorialPanel.Skip.text = current.Skip;
        }

        RectTransform rectTransform = currentTutorialPanel.GetComponent<RectTransform>();
        rectTransform.SetParent(UI.Instance.gameObject.GetComponent<RectTransform>());
        rectTransform.localPosition = tutorialPanelPrefab.transform.localPosition;//Vector3.zero;

        if (current.ArrowPrefab != null)
        {
            currentArrow = Instantiate(current.ArrowPrefab).GetComponent<Arrow>();
            RectTransform arrowRect = currentArrow.GetComponent<RectTransform>();
            arrowRect.SetParent(UI.Instance.gameObject.GetComponent<RectTransform>());
            arrowRect.localPosition = current.ArrowPoint;
        }
        else
        {
            currentArrow = null;
        }

        tutorialActions[tutorialIndex].Invoke();
    }

    public void ActivateSpawn()
    {
        AdvanceStage();

        wall = Instantiate(wallPrefabs[tutorialIndex]).GetComponent<Wall>();
        wall.gameObject.SetActive(true);

        spawn = Instantiate(spawnPrefabs[tutorialIndex]).GetComponent<Spawn>();
        spawn.gameObject.SetActive(true);

        wall.OnCubeEnter.AddListener((c) =>
        {
            wallHits++;
            if (wallHits >= MAX_WALL_HITS)
            {
                ActivateSpawn();
            }
        });
        //TODO add so only like 4 things spawned
        spawn.Auto = true;
    }

    public void BreakThrough()
    {
        AdvanceStage();

        wall = Instantiate(wallPrefabs[tutorialIndex]).GetComponent<Wall>();
        wall.gameObject.SetActive(true);
        wall.OnBreakthrough.AddListener(c => StartCoroutine(OnBreakthrough()) );

        spawn = Instantiate(spawnPrefabs[tutorialIndex].GetComponent<Spawn>());
        spawn.gameObject.SetActive(true);
        spawn.Auto = false;
        spawn.SpawnCube(Vector2.zero, angle: -180.0f, relativeForce: 150.0f, burstDuration: spawn.BurstDurationRange[1],
            burstDelay: spawn.BurstRateRange[1], color: Color.blue);

        //destroy bottom row
        int start = wall.NumBricksWidth + wall.NumBricksHeight - 1;
        int end = start + wall.NumBricksWidth - 1;
        for (int i = start; i < end; i++)
        {
           wall.DestroyCubeAt(i);
        }
    }

    private IEnumerator OnBreakthrough()
    {
        yield return new WaitForSecondsRealtime(hurtWaitTime);
        if (tutorialActions[tutorialIndex] == BreakThrough)
        {
            AdvanceStage();
            player.HP++;
            BreakThrough();
        }
    }

    private void AdvanceStage()
    {
        CompositePool.Instance.Clear();
        FragmentPool.Instance.Clear();

        if (wall != null)
        {
            Camera.main.GetComponentInParent<CameraAnchor>().Wall = null;
            Destroy(wall.gameObject);
        }
        if (spawn != null)
        {
            Destroy(spawn.gameObject);
        }
        wallHits = 0;
    }

    public void GoToMenu()
    {
        GameManager.Instance.LoadSceneAsync("Menu");
    }
}
