using System;
using UnityEngine;

[CreateAssetMenu(
    fileName = "DefaultCustomerItemPool",
    menuName = "Customers/Default Customer Item Pool"
)]
public class DefaultCustomerItemPool : ScriptableObject
{
    [SerializeField] private DefaultCustomerItem[] items;

    public DefaultCustomerItem[] Items => items;

    public DefaultCustomerItem GetRandomItem()
    {
        if (items == null || items.Length == 0)
            return null;

        int totalWeight = 0;

        foreach (DefaultCustomerItem item in items)
        {
            if (item == null || item.Item == null)
                continue;

            if (item.Weight <= 0)
                continue;

            totalWeight += item.Weight;
        }

        if (totalWeight <= 0)
            return null;

        int randomValue = UnityEngine.Random.Range(0, totalWeight);

        foreach (DefaultCustomerItem item in items)
        {
            if (item == null || item.Item == null)
                continue;

            if (item.Weight <= 0)
                continue;

            randomValue -= item.Weight;

            if (randomValue < 0)
                return item;
        }

        return null;
    }
}

[Serializable]
public class DefaultCustomerItem
{
    [Header("Item")]
    [SerializeField] private ItemData item;

    [Header("Customer Request")]
    [SerializeField] private int minimumRequest = 1;
    [SerializeField] private int maximumRequest = 3;

    [Header("Spawn Weight")]
    [SerializeField] private int weight = 1;

    public ItemData Item => item;

    public int Weight => weight;

    public int GetRandomQuantity()
    {
        return UnityEngine.Random.Range(
            minimumRequest,
            maximumRequest + 1
        );
    }
}