using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScenarioOutroUIHandler : UIPageBase
{
    [SerializeField]
    private Text _titleTextField = null;

    public float _timer;

    public bool IsComplete()
    {
        return _timer <= 0;
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

            if (scenarioMgr.ScorePlayer1 > scenarioMgr.ScorePlayer2)
            {
                _titleTextField.text = scenario.goalsPlayer1.outroText;
            }
            else if (scenarioMgr.ScorePlayer2 > scenarioMgr.ScorePlayer1)
            {
                _titleTextField.text = scenario.goalsPlayer2.outroText;
            }
            else
            {
                _titleTextField.text = "Stalemate! Restarting the day...";
            }

            _timer = scenario.OutroDuration;
        }
    }

    void Update()
    {
        _timer -= Time.deltaTime;
    }
}