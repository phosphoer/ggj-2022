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

  public bool IsAssignedBodyPart(eBodyPart bodyPart)
  {
    return bonusBodyPart == bodyPart || requirementProgress.ContainsKey(bodyPart);
  }

  public float GetProgress(eBodyPart bodyPart)
  {
    if (bodyPart == bonusBodyPart)
    {
      return bonusProgress;
    }
    else if (requirementProgress.ContainsKey(bodyPart))
    {
      return requirementProgress[bodyPart];
    }
    else
    {
      return 0;
    }
  }

  // Delta allowed to be negative for removing progress from an enemy player
  public float AdjustProgress(eBodyPart bodyPart, float delta)
  {
    float resultProgress = 0.0f;

    if (requirementProgress.TryGetValue(bodyPart, out resultProgress))
    {
      resultProgress = Mathf.Clamp01(resultProgress + delta);
      requirementProgress[bodyPart] = resultProgress;
    }

    if (bodyPart == bonusBodyPart)
    {
      resultProgress = Mathf.Clamp01(bonusProgress + delta);
      bonusProgress = resultProgress;
    }

    return resultProgress;
  }

  public int GetCompletedRequirementCount()
  {
    int completedCount = 0;

    foreach (float progress in requirementProgress.Values)
    {
      if (progress >= 1.0)
        completedCount++;
    }

    return completedCount;
  }

  public bool HasCompletedAllRequirements()
  {
    return GetCompletedRequirementCount() >= requirementProgress.Keys.Count;
  }

  public bool HasCompletedBonus()
  {
    return (bonusProgress >= 1.0);
  }
}

[System.Serializable]
public class Scenario
{
  public ScenarioScene scenarioPrefab;

  public PlayerGoals angelGoals = new PlayerGoals();
  public PlayerGoals devilGoals = new PlayerGoals();

  public string Title = "";
  public string IntroText = "";
  public string GameplayText = "";
  public float IntroDuration = 3.0f;
  public float GameplayDuration = 3.0f;
  public float OutroDuration = 3.0f;

  public PlayerGoals GetGoalsForPlayer(ePlayer player)
  {
    switch (player)
    {
      case ePlayer.DevilPlayer:
        return devilGoals;
      case ePlayer.AngelPlayer:
        return angelGoals;
    }

    return null;
  }

  public static string GetBodyPartUIText(eBodyPart part)
  {
    switch (part)
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

  public static event System.Action<eBodyPart, float> PartSlapped;
  public static event System.Action<ePlayer, eBodyPart, float, float> ProgressAdjusted;

  public SoundBank AngelClaimedAudio;
  public SoundBank DevilClaimedAudio;

  public SoundBank AngelWinAudio;
  public SoundBank DevilWinAudio;

  public SoundBank BodyPartStolenAudio;

  public float TotalBodyPartHealth = 20;
  public float TotalScenarioTime = 300; // seconds

  public Transform ScenarioRoot = null;


  private float _scenarioTimeRemaining = 0;
  public float ScenarioTimeRemaining => _scenarioTimeRemaining;
  public bool IsInSuddenDeath => _scenarioTimeRemaining <= 0.0f;

  private ePlayer _scenarioWinner = ePlayer.Invalid;
  public ePlayer LastScenarioWinner => _scenarioWinner;
  public bool IsScenarioCompleted => _scenarioWinner != ePlayer.Invalid;

  private int _currentScenarioIndex = 0;
  public bool HasCompletedAllScenarios => _currentScenarioIndex >= Scenarios.Count;

  private PlayerStats _angleStats = new PlayerStats();
  public PlayerStats AngelStats => _angleStats;

  private PlayerStats _devilStats = new PlayerStats();
  public PlayerStats DevilStats => _devilStats;

  private ScenarioScene _scenarioInstance = null;
  public ScenarioScene CurrentScene => _scenarioInstance;

  private List<ePlayer> _scenarioWinners = new List<ePlayer>();
  public List<ePlayer> ScenarioWinners => _scenarioWinners;

  public void ResetGameStats()
  {
    _scenarioWinners = new List<ePlayer>();
  }

  public Scenario GetCurrentScenario()
  {
    return (_currentScenarioIndex < Scenarios.Count) ? Scenarios[_currentScenarioIndex] : null;
  }

  public PlayerStats GetPlayerStats(ePlayer player)
  {
    switch (player)
    {
      case ePlayer.DevilPlayer:
        return _devilStats;
      case ePlayer.AngelPlayer:
        return _angleStats;
    }

    return null;
  }

  public static ePlayer GetOtherPlayer(ePlayer player)
  {
    switch (player)
    {
      case ePlayer.DevilPlayer:
        return ePlayer.AngelPlayer;
      case ePlayer.AngelPlayer:
        return ePlayer.DevilPlayer;
    }

    return ePlayer.Invalid;
  }

  public void ApplySlapDamage(ePlayer attackingPlayer, eBodyPart bodyPart, float slapStrength)
  {
    float progressDelta = slapStrength / TotalBodyPartHealth;

    PlayerStats playerStats = GetPlayerStats(attackingPlayer);

    ePlayer otherPlayer = GetOtherPlayer(attackingPlayer);
    PlayerStats otherPlayerStats = GetPlayerStats(otherPlayer);

    bool isContested = false;
    if (otherPlayerStats.IsAssignedBodyPart(bodyPart))
    {
      float oldProgress = otherPlayerStats.GetProgress(bodyPart);
      float newProgress = otherPlayerStats.AdjustProgress(bodyPart, -progressDelta);

      ProgressAdjusted?.Invoke(otherPlayer, bodyPart, oldProgress, newProgress);

      if (oldProgress >= 1.0 && newProgress <= 1.0)
      {
        if (BodyPartStolenAudio != null)
        {
          AudioManager.Instance.PlaySound(BodyPartStolenAudio);
        }
      }

      isContested = newProgress > 0;
    }

    if (!isContested && playerStats.IsAssignedBodyPart(bodyPart))
    {
      float oldProgress = playerStats.GetProgress(bodyPart);
      float newProgress = playerStats.AdjustProgress(bodyPart, progressDelta);

      ProgressAdjusted?.Invoke(attackingPlayer, bodyPart, oldProgress, newProgress);

      if (oldProgress < 1.0 && newProgress >= 1.0)
      {
        if (otherPlayer == ePlayer.AngelPlayer)
          AudioManager.Instance.PlaySound(DevilClaimedAudio);
        else if (otherPlayer == ePlayer.DevilPlayer)
          AudioManager.Instance.PlaySound(AngelClaimedAudio);
      }
    }

    PartSlapped?.Invoke(bodyPart, slapStrength);
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

    _scenarioTimeRemaining = TotalScenarioTime;
    _scenarioWinner = ePlayer.Invalid;
  }

  public void TeardownScenario()
  {
    if (_scenarioInstance != null)
    {
      CameraManager.Instance.ScenarioCameraStack.PopCurrentController();
      Destroy(_scenarioInstance.gameObject);
      _scenarioInstance = null;
    }
  }

  private void SpawnScenarioPrefab(Scenario scenario)
  {
    if (scenario.scenarioPrefab != null)
    {
      _scenarioInstance = Instantiate(scenario.scenarioPrefab, ScenarioRoot);
      _scenarioInstance.transform.SetIdentityTransformLocal();
      _scenarioInstance.CurrentState = ScenarioScene.ScenarioState.InGame;

      // Get the offset of the dummy camera in the scenario
      Camera dummyCamera = _scenarioInstance.GetComponentInChildren<Camera>();
      dummyCamera.enabled = false;
    }
  }

  [ContextMenu("Devil Win")]
  private void DebugDevilWin()
  {
    _scenarioWinner = ePlayer.DevilPlayer;
    _scenarioTimeRemaining = 0;
  }

  [ContextMenu("Angel Win")]
  private void DebugAngelWin()
  {
    _scenarioWinner = ePlayer.AngelPlayer;
    _scenarioTimeRemaining = 0;
  }

  public void UpdateScenario()
  {
    if (_scenarioWinner != ePlayer.Invalid)
      return;

    _scenarioTimeRemaining = Mathf.Max(_scenarioTimeRemaining - Time.deltaTime, 0.0f);

    if (IsInSuddenDeath)
    {
      int angelScore = _angleStats.GetCompletedRequirementCount();
      if (_angleStats.HasCompletedBonus())
      {
        angelScore += 1;
      }

      int devilScore = _devilStats.GetCompletedRequirementCount();
      if (_devilStats.HasCompletedBonus())
      {
        devilScore += 1;
      }

      if (angelScore > devilScore)
      {
        _scenarioWinner = ePlayer.AngelPlayer;
      }
      else if (devilScore > angelScore)
      {
        _scenarioWinner = ePlayer.DevilPlayer;
      }
    }
    else
    {
      if (_angleStats.HasCompletedAllRequirements())
      {
        _scenarioWinner = ePlayer.AngelPlayer;
      }
      else if (_devilStats.HasCompletedAllRequirements())
      {
        _scenarioWinner = ePlayer.DevilPlayer;
      }
    }

    // See if a player just won this scenario
    if (_scenarioWinner != ePlayer.Invalid)
    {
      switch (_scenarioWinner)
      {
        case ePlayer.AngelPlayer:
          if (AngelWinAudio != null)
          {
            AudioManager.Instance.PlaySound(AngelWinAudio);
          }
          break;
        case ePlayer.DevilPlayer:
          if (DevilWinAudio != null)
          {
            AudioManager.Instance.PlaySound(DevilWinAudio);
          }
          break;
      }

      _scenarioWinners.Add(_scenarioWinner);
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
