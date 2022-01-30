using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eBodyPart
{
    NONE,
    Brain,
    Optical,
    Mouth,
    Guts,
    Arms,
    Loins,
    Legs
}

[System.Serializable]
public class PlayerGoals
{
    public eBodyPart[] Requirements = new eBodyPart[3] { eBodyPart.NONE, eBodyPart.NONE, eBodyPart.NONE };
    public eBodyPart Bonus = eBodyPart.NONE;
    public string outroText = "";
    public string bonusOutroText = "";
}

public class PlayerStats
{
    public Dictionary<eBodyPart, float> requirementProgress;
    public eBodyPart bonusBodyPart;
    public float bonusProgress;

    public PlayerStats()
    {
        requirementProgress = new Dictionary<eBodyPart, float>();
        bonusBodyPart = eBodyPart.NONE;
        bonusProgress = 0.0f;
    }

    public PlayerStats(PlayerGoals goals)
    {
        requirementProgress = new Dictionary<eBodyPart, float>();
        foreach (eBodyPart bodyPart in goals.Requirements)
        {
            if (bodyPart != eBodyPart.NONE)
            {
                requirementProgress.Add(bodyPart, 0.0f);
            }
        }

        bonusBodyPart = goals.Bonus;
        bonusProgress = 0.0f;
    }

    // Delta allowed to be negative for removing progress from an enemy player
    public void adjustProgress(eBodyPart bodyPart, float delta)
    {
        float currentProgress = 0.0f;
        if (requirementProgress.TryGetValue(bodyPart, out currentProgress))
        {
            requirementProgress[bodyPart] = Mathf.Clamp01(currentProgress + delta);
        }

        if (bodyPart == bonusBodyPart)
        {
            bonusProgress = Mathf.Clamp01(bonusProgress + delta);
        }
    }

    public bool hasCompletedRequirements()
    {
        foreach (float progress in requirementProgress.Values)
        {
            if (progress < 1.0)
                return false;
        }

        return true;
    }

    public bool hasCompletedBonus()
    {
        return (bonusProgress >= 1.0);
    }
}

[System.Serializable]
public class Scenario
{
    public GameObject scenarioPrefab;

    public PlayerGoals angelGoals= new PlayerGoals();
    public PlayerGoals devilGoals= new PlayerGoals();

    public string Title = "";
    public string IntroText = "";
    public float IntroDuration = 3.0f;
    public float OutroDuration= 3.0f;

    public PlayerGoals GetGoalsForPlayer(ePlayer player)
    {
        switch(player)
        {
            case ePlayer.LeftPlayer:
                return devilGoals;
            case ePlayer.RightPlayer:
                return angelGoals;
        }

        return null;
    }

    public static string GetBodyPartUIText(eBodyPart part)
    {
        switch(part)
        {
            case eBodyPart.NONE:
                return "NONE";
            case eBodyPart.Brain:
                return "Brain";
            case eBodyPart.Optical:
                return "Optical";
            case eBodyPart.Mouth:
                return "Mouth";
            case eBodyPart.Guts:
                return "Guts";
            case eBodyPart.Arms:
                return "Arms";
            case eBodyPart.Loins:
                return "Loins";
            case eBodyPart.Legs:
                return "Legs";
        }

        return "";
    }
}

public class ScenarioManager : Singleton<ScenarioManager>
{
    public List<Scenario> Scenarios;

    public SoundBank IntroAudio;
    public SoundBank LoopAudio;
    public SoundBank AngelWinAudio;
    public SoundBank DevilWinAudio;

    private int _currentScenarioIndex = 0;
    public bool HasCompletedAllScenarios => _currentScenarioIndex >= Scenarios.Count;

    private PlayerStats _angleStats = new PlayerStats();
    public PlayerStats AngleStats => _angleStats;

    private PlayerStats _devilStats = new PlayerStats();
    public PlayerStats DevilStats => _devilStats;

    private GameObject _scenarioInstance = null;

    public Scenario GetCurrentScenario()
    {
        return (_currentScenarioIndex < Scenarios.Count) ? Scenarios[_currentScenarioIndex] : null;
    }

    public void SetupScenario()
    {
        Scenario scenario = GetCurrentScenario();

        if (scenario != null)
        {
            _angleStats = new PlayerStats(scenario.angelGoals);
            _devilStats = new PlayerStats(scenario.devilGoals);

            SpawnScenarioPrefab(scenario);
        }
        else
        {
            _angleStats = new PlayerStats();
            _devilStats = new PlayerStats();
        }
    }
    public void TeardownScenario()
    {
        if (_scenarioInstance != null)
        {
            Destroy(_scenarioInstance);
            _scenarioInstance = null;
        }
    }

    private void SpawnScenarioPrefab(Scenario scenario)
    {
        if (scenario.scenarioPrefab != null)
        {
            // Get the location of the scenario camera up in the sky
            Vector3 scenarioCamPos = CameraManager.Instance.ScenarioCamera.transform.position;

            // Create the scenario prefab
            _scenarioInstance = Instantiate(scenario.scenarioPrefab);

            // Get the offset of the dummy camera in the scenario
            Camera dummyCamera= _scenarioInstance.GetComponentInChildren<Camera>();
            Vector3 offsetPos= dummyCamera.transform.localPosition;

            Destroy(dummyCamera.gameObject);

            _scenarioInstance.transform.position = scenarioCamPos - offsetPos;
        }
    }


    public void AdvanceScenario()
    {
        _currentScenarioIndex++;
    }

    private void Awake()
    {
        Instance = this;
    }
}
