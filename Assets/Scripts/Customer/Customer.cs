using UnityEngine;

public class Customer : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private CustomerVisual visual;

    [Header("Request UI")]
    [SerializeField] private CustomerRequestUI requestUI;

    private CustomerSpawnPoint currentSpawnPoint;
    private CustomerManager customerManager;

    private CustomerType customerType;

    private ItemData requestedItem;
    private int requestedQuantity;

    private bool isLeaving;

    public CustomerType Type => customerType;

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
        if (isLeaving)
            return;

        isLeaving = true;

        customerManager.CustomerLeaving(this);
    }

    public void ForceLeave()
    {
        if (isLeaving)
            return;

        isLeaving = true;

        customerManager.CustomerLeaving(this);
    }
}