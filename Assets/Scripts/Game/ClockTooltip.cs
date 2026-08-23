using UnityEngine;

public class ClockTooltip : TooltipObject
{
    public override string GetTooltipText()
    {
        if (GameTimeManager.Instance == null)
            return "09:00";

        return GameTimeManager.Instance.GetFormattedTime();
    }
}