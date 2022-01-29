using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScenarioOutroUIHandler : UIPageBase
{
    [SerializeField]
    private Text _titleTextField = null;

    private float _mainResultTimer;
    private float _bonusTimer;
    private string _bonusText;

    public bool IsComplete()
    {
        return _mainResultTimer <= 0 && _bonusTimer <= 0;
    }

    protected override void Awake()
    {
        base.Awake();
        Shown += OnShown;
    }

    private void OnShown()
    {
        if (_titleTextField != null)
        {
            ScenarioManager scenarioMgr= ScenarioManager.Instance;
            Scenario scenario = scenarioMgr.GetCurrentScenario();

            _bonusTimer = 0;
            _bonusText = "";
            _mainResultTimer = scenario.OutroDuration;

            if (scenarioMgr.AngleStats.hasCompletedRequirements() &&
                !scenarioMgr.DevilStats.hasCompletedRequirements())
            {
                _titleTextField.text = scenario.angelGoals.outroText;

                if (scenarioMgr.AngleStats.hasCompletedBonus())
                {
                    _bonusTimer = scenario.OutroDuration;
                    _bonusText = scenario.angelGoals.bonusOutroText;
                }
            }
            else if (!scenarioMgr.AngleStats.hasCompletedRequirements() &&
                     scenarioMgr.DevilStats.hasCompletedRequirements())
            {
                _titleTextField.text = scenario.devilGoals.outroText;

                if (scenarioMgr.DevilStats.hasCompletedBonus())
                {
                    _bonusTimer = scenario.OutroDuration;
                    _bonusText = scenario.devilGoals.bonusOutroText;
                }
            }
            else
            {
                _titleTextField.text = "Stalemate! Restarting the day...";
            }
        }
    }

    void Update()
    {
        if (_mainResultTimer > 0)
        {
            _mainResultTimer -= Time.deltaTime;

            // If _mainResultTimer timer elapses and we have bonus text, show that now
            if (_mainResultTimer <= 0 && _bonusTimer > 0)
            {
                _titleTextField.text = _bonusText;
            }
        }
        else if (_bonusTimer > 0)
        {
            _bonusTimer -= Time.deltaTime;
        }
    }
}