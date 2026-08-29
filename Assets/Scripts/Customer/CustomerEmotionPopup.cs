using System.Collections;
using UnityEngine;
public enum CustomerPopupEmotion
{
    Anger,
    Happy,
    Heart,
    Tear,
    ThumbsUp,
    Worry
}
public class CustomerEmotionPopup : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private float duration = 1f;
    [SerializeField] private float moveDistance = 0.5f;
    [SerializeField] private float startScale = 0.6f;
    [SerializeField] private float peakScale = 1.15f;
    [SerializeField] private float endScale = 0.9f;

    [Header("Curve")]
    [SerializeField]
    private AnimationCurve popupCurve =
        AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [SerializeField]
    private AnimationCurve fadeCurve =
        AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        StartCoroutine(PopupRoutine());
    }

    private IEnumerator PopupRoutine()
    {
        Vector3 startPosition = transform.position;
        Vector3 endPosition =
            startPosition + Vector3.up * moveDistance;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / duration);

            // Yukarý doðru hareket
            float moveT = popupCurve.Evaluate(t);

            transform.position = Vector3.Lerp(
                startPosition,
                endPosition,
                moveT
            );

            // Ýlk baþta hýzlý büyü, sonra hafif küçül
            float scale;

            if (t < 0.3f)
            {
                float scaleT = t / 0.3f;

                scale = Mathf.Lerp(
                    startScale,
                    peakScale,
                    popupCurve.Evaluate(scaleT)
                );
            }
            else
            {
                float scaleT = (t - 0.3f) / 0.7f;

                scale = Mathf.Lerp(
                    peakScale,
                    endScale,
                    scaleT
                );
            }

            transform.localScale = Vector3.one * scale;

            // Fade
            if (spriteRenderer != null)
            {
                Color color = spriteRenderer.color;
                color.a = fadeCurve.Evaluate(t);
                spriteRenderer.color = color;
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}