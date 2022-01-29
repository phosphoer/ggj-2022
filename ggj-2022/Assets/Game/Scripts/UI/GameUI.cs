using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : Singleton<GameUI>
{
    public WorldAttachedUI WorldAttachedUIPlayer1;
    public WorldAttachedUI WorldAttachedUIPlayer2;
    public MainMenuUIHandler MainMenuUI;
    public SettingsUIHandler SettingsUI;
    public ScenarioIntroUIHandler ScenarioIntroUI;
    public ScenarioUIHandler ScenarioUI;
    public ScenarioOutroUIHandler ScenarioOutroUI;
    public EndGameUIHandler EndGameUI;

    private void Awake()
    {
        Instance = this;
    }
}
