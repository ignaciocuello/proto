using System.Collections.Generic;
using UnityEngine;

public class Director : MonoBehaviour {

    private const int MAX_STAGE = 3;

    [SerializeField]
    private int level;

    [SerializeField]
    private int stage;
    public int Stage
    {
        get { return stage; }
        set
        {
            stage = value;
            if (stage == MAX_STAGE + 1)
            {
                stage = 1;
                level++;
            }

            UpdateAttributesBasedOnLevel();
        }
    }

    [SerializeField]
    private Spawn spawn;

    //THINGS TO ALTER BY LEVEL
    //SPAWN SIZE
    //CUBE FORCE 
    //CUBE BURST DURATION
    //CUBE BURST RANGE
    //BATCH SIZE
    //BAR SIZE
    //(CUBE TYPES??)
    //COMPOSITE CAP
    //SPAWN RATE

    //HOW TO MEASURE LEVEL PROGRESSION?

    private int scoreForNextLevel;

    public void OnValidate()
    {
        if (spawn != null && CompositePool.Instance != null)
        {
            Stage = stage;
        }
    }

	// Use this for initialization
	void Start () {
        Stage = stage;
	}

    void Update()
    {
        if (GameManager.Instance.Score >= scoreForNextLevel)
        {
            Stage++;
        }
    }

    public void UpdateAttributesBasedOnLevel()
    {
        spawn.InterWaveWaitTime = CalculateInterWaveWaitTime();
        spawn.SpawnSizeDistribution = CalculateSpawnSizeDistribution();
        spawn.transform.localScale = CalculateLevelScale();
        spawn.RelativeForceRange = CalculateLevelRelativeForceRange();
        spawn.BurstDurationRange = CalculateLevelBurstDurationRange();
        spawn.BurstRateRange = CalculateLevelBurstRateRange();
        
        CompositePool.Instance.MaxComps = CalculateLevelMaxComps();

        scoreForNextLevel += CalcualteLevelScoreForNextStage();

        UI.Instance.LevelValue.text = level + "." + stage;
        //spawn.
    }

    public float CalculateInterWaveWaitTime()
    {
        return 1.0f;
    }

    public List<SizeDistributionElem> CalculateSpawnSizeDistribution()
    {
        List<SizeDistributionElem> distr = new List<SizeDistributionElem>();
        for (int i = 0; i < 10; i++)
        {
            if (i == 0)
            {
                distr.Add(new SizeDistributionElem() { Min = 1, Max = level });
            }
            else if (i == stage)
            {
                distr.Add(new SizeDistributionElem() { Min = 0, Max = 1 });
            }
            else if (i == level)
            {
                distr.Add(new SizeDistributionElem() { Min = 0, Max = 1 });
            }
            else
            {
                distr.Add(new SizeDistributionElem() { Min = 0, Max = 0 });
            }
        }

        return distr;
    }

    public int CalculateLevelBatchSize()
    {
        return level;
    }

    public Vector3 CalculateLevelScale()
    {
        int size = 9;

        return Vector3.one * size;
    }

    private float[] CalculateLevelRelativeForceRange()
    {
        return new float[]
        {
            50.0f + (level-1) * 5.0f + (stage-1) * 2.5f, 100.0f + (level-1) * 1.0f * (stage-1) * 5.0f
        };
    }

    private float[] CalculateLevelBurstDurationRange()
    {
        return new float[]
        {
            0.5f + (level-1) * 0.05f + (stage-1) * 0.025f, 1.0f + (level-1) * 0.01f * (stage-1) * 0.05f
        };
    }

    private float[] CalculateLevelBurstRateRange()
    {
        return new float[]
        {
            0.5f + (level-1) * 0.05f + (stage-1) * 0.025f, 1.0f + (level-1) * 0.01f * (stage-1) * 0.05f
        };
    }

    public int CalculateLevelMaxComps()
    {
        return level + Mathf.Min((stage - 1), 1);
    }

    public float CalcualteLevelSpawnRate()
    {
        return stage != 3 ? 1.0f : 0.5f;
    }

    public int CalcualteLevelScoreForNextStage()
    {
        return 100 * level + 100 * stage;
    }
}
