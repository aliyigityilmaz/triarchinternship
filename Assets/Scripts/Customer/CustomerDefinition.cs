using UnityEngine;

[CreateAssetMenu(
    fileName = "CustomerDefinition",
    menuName = "Customers/Customer Definition"
)]
public class CustomerDefinition : ScriptableObject
{
    [Header("Customer")]
    [SerializeField] private string customerID;
    [SerializeField] private CustomerType customerType;

    [Header("Prefab")]
    [SerializeField] private Customer prefab;

    [Header("Spawn")]
    [Min(0f)]
    [SerializeField] private float spawnWeight = 1f;

    public string CustomerID => customerID;
    public CustomerType CustomerType => customerType;
    public Customer Prefab => prefab;
    public float SpawnWeight => spawnWeight;
}