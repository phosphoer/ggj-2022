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
    public PlayerGoals goalsPlayer1= new PlayerGoals();
    public PlayerGoals goalsPlayer2= new PlayerGoals();

    public string IntroText = "";
    public float IntroDuration = 3.0f;
    public float OutroDuration= 3.0f;

    public PlayerGoals GetGoalsForPlayer(ePlayer player)
    {
        switch(player)
        {
            case ePlayer.LeftPlayer:
                return goalsPlayer1;
            case ePlayer.RightPlayer:
                return goalsPlayer2;
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
    public SoundBank LeftPlayerWinAudio;
    public SoundBank RightPlayerWinAudio;

    private int _currentScenarioIndex = 0;
    public bool HasCompletedAllScenarios => _currentScenarioIndex >= Scenarios.Count;

    private int _scorePlayer1;
    public int ScorePlayer1 => _scorePlayer1;
    private int _scorePlayer2;
    public int ScorePlayer2 => _scorePlayer2;

    public Scenario GetCurrentScenario()
    {
        return (_currentScenarioIndex < Scenarios.Count) ? Scenarios[_currentScenarioIndex] : null;
    }

    public void SetupScenario()
    {
        _scorePlayer1 = 0;
        _scorePlayer2 = 0;
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
