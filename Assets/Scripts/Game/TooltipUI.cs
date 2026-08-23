using TMPro;
using UnityEngine;

public class TooltipUI : MonoBehaviour
{
    public static TooltipUI Instance { get; private set; }

    [Header("References")]
    [SerializeField] private GameObject tooltipObject;
    [SerializeField] private TMP_Text tooltipText;

    [Header("Position")]
    [SerializeField] private float offset = 20f;

    private RectTransform tooltipRect;
    private RectTransform canvasRect;
    private Canvas canvas;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        tooltipRect = tooltipObject.GetComponent<RectTransform>();

        canvas = GetComponentInParent<Canvas>();
        canvasRect = canvas.GetComponent<RectTransform>();

        Hide();
    }

    private void Update()
    {
        if (!tooltipObject.activeSelf)
            return;

        UpdatePosition();
    }

    private void UpdatePosition()
    {
        Vector2 localMousePosition;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            Input.mousePosition,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : canvas.worldCamera,
            out localMousePosition
        );

        Vector2 canvasSize = canvasRect.rect.size;

        bool mouseOnLeft = localMousePosition.x < 0f;
        bool mouseOnBottom = localMousePosition.y < 0f;

        float tooltipWidth = tooltipRect.rect.width;
        float tooltipHeight = tooltipRect.rect.height;

        Vector2 position;

        // SOL ÜST
        if (mouseOnLeft && !mouseOnBottom)
        {
            tooltipRect.pivot = new Vector2(0f, 1f);

            position = localMousePosition +
                       new Vector2(offset, -offset);
        }

        // SAÐ ÜST
        else if (!mouseOnLeft && !mouseOnBottom)
        {
            tooltipRect.pivot = new Vector2(1f, 1f);

            position = localMousePosition +
                       new Vector2(-offset, -offset);
        }

        // SOL ALT
        else if (mouseOnLeft && mouseOnBottom)
        {
            tooltipRect.pivot = new Vector2(0f, 0f);

            position = localMousePosition +
                       new Vector2(offset, offset);
        }

        // SAÐ ALT
        else
        {
            tooltipRect.pivot = new Vector2(1f, 0f);

            position = localMousePosition +
                       new Vector2(-offset, offset);
        }

        tooltipRect.localPosition = position;
    }

    public void Show(string text)
    {
        tooltipText.text = text;

        tooltipObject.SetActive(true);

        UpdatePosition();
    }

    public void Hide()
    {
        tooltipObject.SetActive(false);
    }
}