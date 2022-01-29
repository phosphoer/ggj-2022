using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScenarioIntroUIHandler : UIPageBase
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
            Scenario scenario= ScenarioManager.Instance.GetCurrentScenario();

            _titleTextField.text = scenario.IntroText;
            _timer = scenario.IntroDuration;
        }
    }

    void Update()
    {
        _timer -= Time.deltaTime;
    }
}