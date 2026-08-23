using UnityEngine;

public class TooltipManager : MonoBehaviour
{
    private TooltipObject currentTooltipObject;

    private void Update()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider == null)
        {
            HideTooltip();
            return;
        }

        TooltipObject tooltipObject =
            hit.collider.GetComponentInParent<TooltipObject>();

        if (tooltipObject == null)
        {
            HideTooltip();
            return;
        }

        if (tooltipObject != currentTooltipObject)
        {
            currentTooltipObject = tooltipObject;

            TooltipUI.Instance.Show(
                tooltipObject.GetTooltipText()
            );
        }
    }

    private void HideTooltip()
    {
        if (currentTooltipObject == null)
            return;

        currentTooltipObject = null;

        TooltipUI.Instance.Hide();
    }
}