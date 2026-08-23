using UnityEngine;

public class TooltipObject : MonoBehaviour
{
    [SerializeField] private string tooltipText;

    public virtual string GetTooltipText()
    {
        return tooltipText;
    }
}