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
  }

  void UpdatePlayerLocation(ePlayer player, Image icon)
  {
    PlayerCharacterController playerController = GameStateManager.Instance.GetPlayer(player);
    if (playerController != null)
    {
      Vector3 playerPosition = playerController.gameObject.transform.position;
      float unitXLocation = (playerPosition.x - BodyExtents.center.x) / BodyExtents.size.x;
      float unitYLocation = (playerPosition.y - BodyExtents.center.y) / BodyExtents.size.y;

      float rectFractionX = (unitXLocation + 1) / 2.0f;
      float rectFractionY = (unitYLocation + 1) / 2.0f;

      Rect minimapRect = MiniMapBG.sprite.rect;
      float pixelX = rectFractionX * (minimapRect.width);
      float pixelY = rectFractionY * (minimapRect.height);

      RectTransform rectTransform = icon.GetComponent<RectTransform>();
      rectTransform.localPosition= new Vector3(pixelX, pixelY);

      icon.enabled = true;
    }
    else
    {
      icon.enabled = false;
    }
  }
}
