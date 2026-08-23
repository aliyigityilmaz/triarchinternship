using UnityEngine;

[CreateAssetMenu(
    fileName = "Item",
    menuName = "Items/Item"
)]
public class ItemData : ScriptableObject
{
    [Header("Item")]
    [SerializeField] private string itemName;
    [SerializeField] private Sprite icon;

    public string ItemName => itemName;
    public Sprite Icon => icon;
}