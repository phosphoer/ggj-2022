using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SidebarUIHandler : UIPageBase
{
  public Image MiniMapBG;
  public Image AngePlayerIcon;
  public Image DevilPlayerIcon;
  public Text SuddenDeathlabel;
  public Image SuddenDeathTimer;
  public Text TimerLabel;
  public Bounds BodyExtents = 
    new Bounds(new Vector3(0,0,0), new Vector3(100, 100, 100));

  protected override void Awake()
  {
    base.Awake();
    Shown += OnShown;
  }

  private void OnShown()
  {
    SuddenDeathlabel.gameObject.SetActive(false);
    UpdateSuddenDeathTimer();
    UpdatePlayerLocation(ePlayer.AngelPlayer, AngePlayerIcon);
    UpdatePlayerLocation(ePlayer.DevilPlayer, DevilPlayerIcon);
  }

  void Update()
  {
    UpdateSuddenDeathTimer();
    UpdatePlayerLocation(ePlayer.AngelPlayer, AngePlayerIcon);
    UpdatePlayerLocation(ePlayer.DevilPlayer, DevilPlayerIcon);
  }

  void UpdateSuddenDeathTimer()
  {
    ScenarioManager scenarioMgr= ScenarioManager.Instance;

    float secondsRemaining= scenarioMgr.ScenarioTimeRemaining;
    SuddenDeathTimer.fillAmount= secondsRemaining / scenarioMgr.TotalScenarioTime;

    TimeSpan timeSpan = TimeSpan.FromSeconds(secondsRemaining);
    TimerLabel.text= string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);

    SuddenDeathlabel.gameObject.SetActive(secondsRemaining <= 0.0f);
  }

  void UpdatePlayerLocation(ePlayer player, Image icon)
  {
    PlayerCharacterController playerController = GameStateManager.Instance.GetPlayer(player);
    if (playerController != null)
    {
      Vector3 playerPosition = playerController.gameObject.transform.position;
      float unitXLocation = (playerPosition.x - BodyExtents.center.x) / BodyExtents.extents.x;
      float unitYLocation = (playerPosition.y - BodyExtents.center.y) / BodyExtents.extents.y;

      RectTransform minimapRectXform = MiniMapBG.gameObject.GetComponent<RectTransform>();
      float pixelX = unitXLocation * (minimapRectXform.rect.width / 2.0f);
      float pixelY = unitYLocation * (minimapRectXform.rect.height / 2.0f);

      RectTransform rectTransform = icon.GetComponent<RectTransform>();
      rectTransform.anchoredPosition = new Vector2(pixelX, pixelY);

      icon.enabled = true;
    }
    else
    {
      icon.enabled = false;
    }
  }
}
