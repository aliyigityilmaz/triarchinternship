using UnityEngine;
using System.Collections;
public class CustomerVisual : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite waitingSprite;
    [SerializeField] private Sprite talkingSprite;
    [SerializeField] private Sprite happySprite;
    [SerializeField] private Sprite sadSprite;

    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    private CustomerEmotion currentEmotion;

    [Header("Feedback Animation")]
    [SerializeField] private float bounceHeight = 0.25f;
    [SerializeField] private float bounceDuration = 0.25f;
    [SerializeField] private float shakeStrength = 0.15f;
    [SerializeField] private float shakeDuration = 0.25f;
    [SerializeField] private int shakeVibrato = 6;

    private Coroutine feedbackRoutine;
    private Vector3 basePosition;
    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        SetEmotion(CustomerEmotion.Waiting);
    }

    public void ResetBasePosition()
    {
        basePosition = transform.localPosition;
    }
    public void SetEmotion(CustomerEmotion emotion)
    {
        currentEmotion = emotion;

        switch (emotion)
        {
            case CustomerEmotion.Waiting:
                spriteRenderer.sprite = waitingSprite;
                break;

            case CustomerEmotion.Talking:
                spriteRenderer.sprite = talkingSprite;
                break;

            case CustomerEmotion.Happy:
                spriteRenderer.sprite = happySprite;
                break;

            case CustomerEmotion.Sad:
                spriteRenderer.sprite = sadSprite;
                break;
        }
    }

    public void PlayCorrectFeedback()
    {
        if (feedbackRoutine != null) StopCoroutine(feedbackRoutine);
        feedbackRoutine = StartCoroutine(BounceRoutine());
    }

    public void PlayWrongFeedback()
    {
        if (feedbackRoutine != null) StopCoroutine(feedbackRoutine);
        feedbackRoutine = StartCoroutine(ShakeRoutine());
    }

    private IEnumerator BounceRoutine()
    {
        float elapsed = 0f;

        while (elapsed < bounceDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / bounceDuration);

            // 0 -> 1 -> 0 (yukarý çýk, geri in)
            float height = Mathf.Sin(t * Mathf.PI) * bounceHeight;

            transform.localPosition = basePosition + Vector3.up * height;

            yield return null;
        }

        transform.localPosition = basePosition;
    }

    private IEnumerator ShakeRoutine()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / shakeDuration);

            // Sönümlenen (damping) yatay titreme
            float damper = 1f - t;
            float offsetX = Mathf.Sin(t * shakeVibrato * Mathf.PI * 2f)
                * shakeStrength
                * damper;

            transform.localPosition = basePosition + Vector3.right * offsetX;

            yield return null;
        }

        transform.localPosition = basePosition;
    }

    public CustomerEmotion GetCurrentEmotion()
    {
        return currentEmotion;
    }

    public void SetAlpha(float alpha)
    {
        if (spriteRenderer == null) return;
        Color c = spriteRenderer.color;
        c.a = alpha;
        spriteRenderer.color = c;
    }
}