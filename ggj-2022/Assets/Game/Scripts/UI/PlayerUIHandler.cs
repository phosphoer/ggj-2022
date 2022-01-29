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

    protected override void Awake()
    {
        base.Awake();
        Shown += OnShown;
    }

    public void AssignPlayer(ePlayer player)
    {
        _assignedPlayer = player;
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

        for (int i= 0; i < RequiredGoalsUI.Count; ++i)
        {
            PlayerGoalUI goalUI= RequiredGoalsUI[i];
            
            if (i < goals.Requirements.Length)
            {
                eBodyPart requiredBodyPart= goals.Requirements[i];
                SetupPlayerGoalUI(goalUI, requiredBodyPart);
            }
            else
            {
                SetupPlayerGoalUI(goalUI, eBodyPart.NONE);
            }
        }

        SetupPlayerGoalUI(BonusGoalUI, goals.Bonus);
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
}