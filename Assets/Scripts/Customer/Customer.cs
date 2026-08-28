using UnityEngine;
using System.Collections;

public class Customer : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private CustomerVisual visual;

    [Header("Request UI")]
    [SerializeField] private CustomerRequestUI requestUI;


    [Header("Spawn / Leave Animation")]
    [SerializeField] private float spawnMoveDistance = 2f;
    [SerializeField] private float spawnDuration = 0.35f;
    [SerializeField] private float leaveMoveDistance = 2f;
    [SerializeField] private float leaveDuration = 0.35f;
    [SerializeField] private AnimationCurve moveCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private Coroutine animationRoutine;

    private CustomerSpawnPoint currentSpawnPoint;
    private CustomerManager customerManager;

    private CustomerType customerType;

    private ItemData requestedItem;
    private int requestedQuantity;

    private bool isLeaving;

    public CustomerType Type => customerType;
    public CustomerSpawnPoint CurrentSpawnPoint => currentSpawnPoint;
    public ItemData RequestedItem => requestedItem;
    public int RequestedQuantity => requestedQuantity;

    public void Initialize(
    CustomerManager manager,
    CustomerSpawnPoint spawnPoint,
    CustomerType type)
    {
        customerManager = manager;
        currentSpawnPoint = spawnPoint;
        customerType = type;
        isLeaving = false;

        visual.SetEmotion(CustomerEmotion.Waiting);

        Vector3 targetPos = spawnPoint.transform.position;
        float side = Random.value < 0.5f ? -1f : 1f;
        Vector3 startPos = targetPos + Vector3.right * side * spawnMoveDistance;

        transform.position = startPos;
        visual.SetAlpha(0f);

        if (animationRoutine != null) StopCoroutine(animationRoutine);
        animationRoutine = StartCoroutine(
            MoveAndFade(startPos, targetPos, 0f, 1f, spawnDuration)
        );
    }

    public void SetDefaultRequest(
        ItemData item,
        int quantity)
    {
        requestedItem = item;
        requestedQuantity = quantity;

        if (requestUI != null)
        {
            requestUI.SetRequest(
                requestedItem,
                requestedQuantity
            );
        }
    }

    public void StartTalking()
    {
        if (isLeaving)
            return;

        visual.SetEmotion(CustomerEmotion.Talking);
    }

    public void GiveItem(ItemData item)
    {
        if (customerType != CustomerType.Default)
            return;

        if (isLeaving)
            return;

        if (item == null)
            return;

        // WRONG ITEM
        if (item != requestedItem)
        {
            WrongItem();
            return;
        }

        // CORRECT ITEM
        requestedQuantity--;

        if (requestUI != null)
        {
            requestUI.SetQuantity(requestedQuantity);
        }

        // Still needs more
        if (requestedQuantity > 0)
        {
            visual.SetEmotion(CustomerEmotion.Happy);
            return;
        }

        // Finished
        CompleteRequest();
    }

    private void WrongItem()
    {
        visual.SetEmotion(CustomerEmotion.Sad);

        Invoke(nameof(Leave), 0.5f);
    }

    private void CompleteRequest()
    {
        visual.SetEmotion(CustomerEmotion.Happy);

        if (customerManager != null)
        {
            customerManager.PayDefaultCustomer(this);
        }

        Invoke(nameof(Leave), 1f);
    }

    private void Leave()
    {
        if (isLeaving) return;
        isLeaving = true;

        if (animationRoutine != null) StopCoroutine(animationRoutine);
        animationRoutine = StartCoroutine(LeaveRoutine());
    }

    public void ForceLeave()
    {
        if (isLeaving) return;
        isLeaving = true;

        if (animationRoutine != null) StopCoroutine(animationRoutine);
        animationRoutine = StartCoroutine(LeaveRoutine());
    }

    private IEnumerator LeaveRoutine()
    {
        Vector3 startPos = transform.position;
        float side = Random.value < 0.5f ? -1f : 1f;
        Vector3 targetPos = startPos + Vector3.right * side * leaveMoveDistance;

        yield return MoveAndFade(startPos, targetPos, 1f, 0f, leaveDuration);

        if (customerManager != null)
            customerManager.CustomerLeaving(this);
    }

    private IEnumerator MoveAndFade(
        Vector3 from,
        Vector3 to,
        float fromAlpha,
        float toAlpha,
        float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float eased = moveCurve.Evaluate(t);

            transform.position = Vector3.Lerp(from, to, eased);
            visual.SetAlpha(Mathf.Lerp(fromAlpha, toAlpha, eased));

            yield return null;
        }

        transform.position = to;
        visual.SetAlpha(toAlpha);
    }
}