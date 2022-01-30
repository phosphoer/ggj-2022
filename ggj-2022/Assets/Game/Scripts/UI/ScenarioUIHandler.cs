using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScenarioUIHandler : UIPageBase
{
    protected override void Awake()
    {
        base.Awake();
        Shown += OnShown;
    }

    private void OnShown()
    {
        Canvas parentCanvas = this.transform.parent.GetComponent<Canvas>();
        parentCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        parentCanvas.worldCamera = CameraManager.Instance.ScenarioCamera;
        parentCanvas.planeDistance = 1.0f;
    }

    void Update()
    {
    }
}
