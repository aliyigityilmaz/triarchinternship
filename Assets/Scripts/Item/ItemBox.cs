using UnityEngine;

public class ItemBox : TooltipObject
{
    [Header("Item")]
    [SerializeField] private ItemData item;

    [Header("Drag Item")]
    [SerializeField] private GameObject itemDragPrefab;

    private GameObject currentDraggedItem;
    private bool isDragging;

    public ItemData Item => item;

    private void OnMouseDown()
    {
        if (item == null)
            return;

        if (itemDragPrefab == null)
        {
            Debug.LogWarning(
                $"No Item Drag Prefab assigned to {gameObject.name}"
            );

            return;
        }

        StartDragging();
    }

    private void OnMouseDrag()
    {
        if (!isDragging || currentDraggedItem == null)
            return;

        UpdateDraggedItemPosition();
    }

    private void OnMouseUp()
    {
        if (!isDragging)
            return;

        StopDragging();
    }

    private void StartDragging()
    {
        isDragging = true;

        Vector3 mousePosition =
            GetMouseWorldPosition();

        currentDraggedItem = Instantiate(
            itemDragPrefab,
            mousePosition,
            Quaternion.identity
        );

        ItemDrag itemDrag =
            currentDraggedItem.GetComponent<ItemDrag>();

        if (itemDrag != null)
        {
            itemDrag.Initialize(item);
        }
    }

    private void UpdateDraggedItemPosition()
    {
        currentDraggedItem.transform.position =
            GetMouseWorldPosition();
    }

    private void StopDragging()
    {
        isDragging = false;

        if (currentDraggedItem == null)
            return;

        ItemDrag itemDrag =
            currentDraggedItem.GetComponent<ItemDrag>();

        if (itemDrag != null)
        {
            itemDrag.Drop();
        }
        else
        {
            Destroy(currentDraggedItem);
        }

        currentDraggedItem = null;
    }

    private Vector3 GetMouseWorldPosition()
    {
        if (Camera.main == null)
            return transform.position;

        Vector3 mousePosition =
            Camera.main.ScreenToWorldPoint(
                Input.mousePosition
            );

        mousePosition.z = 0f;

        return mousePosition;
    }

    public override string GetTooltipText()
    {
        if (item == null)
            return string.Empty;

        return item.ItemName;
    }
}