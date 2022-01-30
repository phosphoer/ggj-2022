using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct PlayerGoalUI
{
  public Text bodyPartLabel;
  public Image bodyPartIcon;
  public RectTransform progressRectTransform;
}

public class PlayerUIHandler : UIPageBase
{
  private ePlayer _assignedPlayer = ePlayer.Invalid;

  public List<PlayerGoalUI> RequiredGoalsUI;
  public PlayerGoalUI BonusGoalUI;

  public Sprite BrainIcon;
  public Sprite OpticalIcon;
  public Sprite MouthIcon;
  public Sprite GutsIcon;
  public Sprite ArmsIcon;
  public Sprite LoinsIcon;
  public Sprite LegsIcon;

  [SerializeField]
  private AnimationCurve _uiThrobCurve = null;
  public float UIThrobStrength = 0.2f;

  public Color UnfullBarColor = new Color(0.2122642f, 0.5551525f, 0.8490566f, 1.0f);
  public Color FullBarColor = new Color(1.0f, 0.9644864f, 0.0f, 1.0f);

  protected override void Awake()
  {
    base.Awake();
    Shown += OnShown;
    Hidden += OnHidden;
  }

  public void AssignPlayer(ePlayer player)
  {
    _assignedPlayer = player;

    Canvas parentCanvas = this.transform.parent.GetComponent<Canvas>();
    parentCanvas.renderMode = RenderMode.ScreenSpaceCamera;
    parentCanvas.worldCamera = CameraManager.Instance.getPlayerCamera(_assignedPlayer);
    parentCanvas.planeDistance = 1.0f;
  }

  private void OnShown()
  {
    if (_assignedPlayer == ePlayer.Invalid)
    {
      Debug.LogError("PlayerUIHandler shown without assigning a player!");
      return;
    }

    ScenarioManager scenarioMgr = ScenarioManager.Instance;
    Scenario scenario = scenarioMgr.GetCurrentScenario();
    PlayerGoals goals = scenario.GetGoalsForPlayer(_assignedPlayer);

    for (int i = 0; i < RequiredGoalsUI.Count; ++i)
    {
      PlayerGoalUI goalUI = RequiredGoalsUI[i];

      if (i < goals.Requirements.Length)
      {
        eBodyPart requiredBodyPart = goals.Requirements[i];
        SetupPlayerGoalUI(goalUI, requiredBodyPart);
      }
      else
      {
        SetupPlayerGoalUI(goalUI, eBodyPart.NONE);
      }
    }

    SetupPlayerGoalUI(BonusGoalUI, goals.Bonus);

    ScenarioManager.ProgressAdjusted += GoalProgressAdjusted;
  }

  private void OnHidden()
  {
    ScenarioManager.ProgressAdjusted -= GoalProgressAdjusted;
  }

  private void GoalProgressAdjusted(ePlayer player, eBodyPart bodyPart, float oldValue, float newValue)
  {
    if (player == _assignedPlayer)
    {
      UpdateGoalProgressMeter(bodyPart, oldValue, newValue);
    }
  }

  void SetupPlayerGoalUI(PlayerGoalUI goalUI, eBodyPart bodyPart)
  {
    if (bodyPart != eBodyPart.NONE)
    {
      goalUI.bodyPartIcon.sprite = GetSpriteForBodyPart(bodyPart);
      goalUI.bodyPartLabel.text = Scenario.GetBodyPartUIText(bodyPart);
    }
    else
    {
      goalUI.bodyPartIcon.enabled = false;
      goalUI.bodyPartLabel.enabled = false;
    }

    goalUI.progressRectTransform.transform.localScale = new Vector3(0, 1, 1);
  }

  void Update()
  {
  }

  public void UpdateGoalProgressMeter(eBodyPart bodyPart, float oldValue, float newValue)
  {
    if (_assignedPlayer == ePlayer.Invalid)
      return;

    ScenarioManager scenarioMgr = ScenarioManager.Instance;
    Scenario scenario = scenarioMgr.GetCurrentScenario();
    PlayerGoals goals = scenario.GetGoalsForPlayer(_assignedPlayer);
    PlayerStats playerStats = scenarioMgr.GetPlayerStats(_assignedPlayer);

    RectTransform rectTransform = null;

    // Update the bonus progress
    if (goals.Bonus == bodyPart && BonusGoalUI.progressRectTransform != null)
    {
      rectTransform= BonusGoalUI.progressRectTransform;
    }
    else
    {
      for (int uiSlotIndex = 0; uiSlotIndex < RequiredGoalsUI.Count; ++uiSlotIndex)
      {
        PlayerGoalUI goalUI = RequiredGoalsUI[uiSlotIndex];

        if (uiSlotIndex < goals.Requirements.Length)
        {
          if (goals.Requirements[uiSlotIndex] == bodyPart)
          {
            rectTransform = goalUI.progressRectTransform;
            break;
          }
        }
      }
    }

    if (rectTransform != null)
    {
      rectTransform.transform.localScale = new Vector3(newValue, 1, 1);

      Image image = rectTransform.gameObject.GetComponent<Image>();
      if (image != null)
      {
        image.color = newValue >= 1.0 ? FullBarColor : UnfullBarColor;
      }

      //if ((oldValue < 1.0f && newValue >= 1.0f) ||
      //    (oldValue >= 1.0f && newValue < 1.0f))
      //{
      //  ThrobUIElement(rectTransform);
      //}
    }
  }

  private Sprite GetSpriteForBodyPart(eBodyPart part)
  {
    switch (part)
    {
      case eBodyPart.NONE:
        return null;
      case eBodyPart.Brain:
        return BrainIcon;
      case eBodyPart.Optical:
        return OpticalIcon;
      case eBodyPart.Mouth:
        return MouthIcon;
      case eBodyPart.Guts:
        return GutsIcon;
      case eBodyPart.Arms:
        return ArmsIcon;
      case eBodyPart.Loins:
        return LoinsIcon;
      case eBodyPart.Legs:
        return LegsIcon;
    }

    return null;
  }

  private void ThrobUIElement(RectTransform rectTransform)
  {
    StartCoroutine(ThrobUIAnimAsync(rectTransform.transform, UIThrobStrength));
  }

  private IEnumerator ThrobUIAnimAsync(Transform uiXform, float throbStrength)
  {
    yield return Tween.CustomTween(1.0f, t =>
    {
        float extraScale = throbStrength * _uiThrobCurve.Evaluate(t);
        uiXform.localScale = new Vector3(uiXform.localScale.x, 1.0f+ extraScale, 1.0f);
    });

    uiXform.localScale = new Vector3(uiXform.localScale.x, 1.0f, 1.0f);
  }
}