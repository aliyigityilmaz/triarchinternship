using UnityEngine;

public class ItemDrag : MonoBehaviour
{
    private ItemData item;

    private Collider2D itemCollider;

    public ItemData Item => item;

    private void Awake()
    {
        itemCollider = GetComponent<Collider2D>();
    }

    public void Initialize(ItemData itemData)
    {
        item = itemData;

        SpriteRenderer spriteRenderer =
            GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = item.Icon;
        }

        // Sürüklenen item NPC collider'ýný engellemesin.
        if (itemCollider != null)
            itemCollider.enabled = false;
    }

    public void Drop()
    {
        Vector2 dropPosition = transform.position;

        Collider2D[] colliders =
            Physics2D.OverlapPointAll(dropPosition);

        foreach (Collider2D collider in colliders)
        {
            Customer customer =
                collider.GetComponentInParent<Customer>();

            if (customer == null)
                continue;

            customer.GiveItem(item);

            Destroy(gameObject);
            return;
        }

        // NPC üzerinde deðilse item yok olur.
        Destroy(gameObject);
    }
}