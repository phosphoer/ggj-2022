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


[System.Serializable]
public class Scenario
{
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

    private int _angleScore;
    public int AngleScore => _angleScore;
    private int _devilScore;
    public int DevilScore => _devilScore;

    public Scenario GetCurrentScenario()
    {
        return (_currentScenarioIndex < Scenarios.Count) ? Scenarios[_currentScenarioIndex] : null;
    }

    public void SetupScenario()
    {
        _angleScore = 0;
        _devilScore = 0;
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
