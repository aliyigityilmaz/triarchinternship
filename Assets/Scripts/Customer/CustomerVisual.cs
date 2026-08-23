using UnityEngine;

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

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        SetEmotion(CustomerEmotion.Waiting);
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

    public CustomerEmotion GetCurrentEmotion()
    {
        return currentEmotion;
    }
}