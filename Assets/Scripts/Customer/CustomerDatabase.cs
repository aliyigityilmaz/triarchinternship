using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "CustomerDatabase",
    menuName = "Customers/Customer Database"
)]
public class CustomerDatabase : ScriptableObject
{
    [SerializeField] private List<CustomerDefinition> customers = new();

    public List<CustomerDefinition> Customers => customers;

    public List<CustomerDefinition> GetCustomersByType(CustomerType type)
    {
        List<CustomerDefinition> result = new();

        foreach (CustomerDefinition customer in customers)
        {
            if (customer == null)
                continue;

            if (customer.CustomerType == type)
                result.Add(customer);
        }

        return result;
    }
}