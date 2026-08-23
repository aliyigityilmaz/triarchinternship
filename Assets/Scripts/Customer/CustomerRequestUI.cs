using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomerRequestUI : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text quantityText;

    public void SetRequest(
        ItemData item,
        int quantity)
    {
        if (item == null)
            return;

        itemIcon.sprite = item.Icon;
        quantityText.text = $"{quantity}";
    }

    public void SetQuantity(int quantity)
    {
        quantityText.text = $"{quantity}";
    }
}