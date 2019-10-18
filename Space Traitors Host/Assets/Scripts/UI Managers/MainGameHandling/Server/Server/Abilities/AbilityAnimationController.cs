using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using TMPro;

public class AbilityAnimationController : MonoBehaviour
{
    public GameObject abilityAnimationPanel;
    public VideoPlayer animationPlayer;
    public TextMeshProUGUI abilityActivatedText;

    public VideoClip secretPathsClip;
    public VideoClip powerBoostClip;
    public VideoClip encouragingSongClip;
    public VideoClip muddleSensorsClip;
    public VideoClip sensorScanClip;
    public VideoClip codeInspectionClip;
    public VideoClip superchargeClip;

    public void InitAbilityAnimations()
    {
        abilityAnimationPanel.SetActive(false);
        animationPlayer.loopPointReached += EndAnimation;
        abilityActivatedText.text = "";
    }

    private void EndAnimation(VideoPlayer videoPlayer)
    {
        abilityAnimationPanel.SetActive(false);
    }

    public void PlayAnimation(Ability ability, string playerName, string targetName)
    {
        abilityAnimationPanel.SetActive(true);
        animationPlayer.clip = null;
        string activatedString;
        if (targetName == "")
        {
            activatedString = string.Format("{0} has activated {1}", playerName, ability.AbilityName);
        }
        else
        {
            activatedString = string.Format("{0} has activated {1} on {2}", playerName, ability.AbilityName, targetName);
        }
        abilityActivatedText.text = activatedString;

        VideoClip videoClip = null;

        switch (ability.abilityType)
        {
            case (Ability.AbilityTypes.Secret_Paths):
                videoClip = secretPathsClip;
                break;
            case (Ability.AbilityTypes.Power_Boost):
                videoClip = powerBoostClip;
                break;
            case (Ability.AbilityTypes.Encouraging_Song):
                videoClip = encouragingSongClip;
                break;
            case (Ability.AbilityTypes.Muddle_Sensors):
                videoClip = muddleSensorsClip;
                break;
            case (Ability.AbilityTypes.Sensor_Scan):
                videoClip = sensorScanClip;
                break;
            case (Ability.AbilityTypes.Code_Inspection):
                videoClip = codeInspectionClip;
                break;
            case (Ability.AbilityTypes.Supercharge):
                videoClip = superchargeClip;
                break;
            default:
                Debug.Log("Not a valid ability type");
                break;
        }

        animationPlayer.clip = videoClip;

        animationPlayer.Play();
    }
}
