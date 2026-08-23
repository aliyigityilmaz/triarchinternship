using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    public static CustomerManager Instance { get; private set; }

    [Header("Customer Database")]
    [SerializeField] private CustomerDatabase customerDatabase;

    [Header("Default Customer Items")]
    [SerializeField] private DefaultCustomerItemPool defaultCustomerItemPool;

    [Header("Spawn Points")]
    [SerializeField] private CustomerSpawnPoint[] spawnPoints;

    [Header("Spawn Timing")]
    [SerializeField] private float minimumSpawnInterval = 10f;
    [SerializeField] private float maximumSpawnInterval = 25f;

    [Header("Group Size")]
    [SerializeField] private int minimumCustomersPerSpawn = 1;
    [SerializeField] private int maximumCustomersPerSpawn = 2;

    [Header("Group Spawn Chance")]
    [Range(0f, 1f)]
    [SerializeField] private float multipleCustomerChance = 0.35f;

    [Header("Customer Type Chances")]
    [Min(0f)]
    [SerializeField] private float defaultCustomerWeight = 1f;

    [Min(0f)]
    [SerializeField] private float orderCustomerWeight = 0f;

    [Min(0f)]
    [SerializeField] private float specialRequestCustomerWeight = 0f;

    private readonly List<Customer> activeCustomers = new();

    private float spawnTimer;
    private bool spawning;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        if (GameTimeManager.Instance == null)
            return;

        GameTimeManager.Instance.OnDayStarted += OnDayStarted;
        GameTimeManager.Instance.OnDayFinished += OnDayFinished;
    }

    private void OnDestroy()
    {
        if (GameTimeManager.Instance == null)
            return;

        GameTimeManager.Instance.OnDayStarted -= OnDayStarted;
        GameTimeManager.Instance.OnDayFinished -= OnDayFinished;
    }

    private void OnDayStarted()
    {
        spawning = true;
        SetNextSpawnTime();
    }

    private void OnDayFinished()
    {
        spawning = false;
    }

    private void Update()
    {
        if (!spawning)
            return;

        if (GameTimeManager.Instance == null)
            return;

        if (!GameTimeManager.Instance.IsDayActive)
            return;

        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f)
        {
            TrySpawnCustomers();
            SetNextSpawnTime();
        }
    }

    private void SetNextSpawnTime()
    {
        spawnTimer = Random.Range(
            minimumSpawnInterval,
            maximumSpawnInterval
        );
    }

    private void TrySpawnCustomers()
    {
        List<CustomerSpawnPoint> freePoints =
            GetFreeSpawnPoints();

        if (freePoints.Count == 0)
            return;

        int amountToSpawn = GetCustomerAmount();

        amountToSpawn = Mathf.Min(
            amountToSpawn,
            freePoints.Count
        );

        Shuffle(freePoints);

        for (int i = 0; i < amountToSpawn; i++)
        {
            SpawnCustomer(freePoints[i]);
        }
    }

    private int GetCustomerAmount()
    {
        if (maximumCustomersPerSpawn <= 1)
            return 1;

        if (Random.value <= multipleCustomerChance)
        {
            return Random.Range(
                minimumCustomersPerSpawn,
                maximumCustomersPerSpawn + 1
            );
        }

        return 1;
    }

    private void SpawnCustomer(
        CustomerSpawnPoint spawnPoint)
    {
        if (spawnPoint.IsOccupied)
            return;

        CustomerType type = GetRandomCustomerType();

        CustomerDefinition definition =
            GetRandomCustomerDefinition(type);

        if (definition == null)
        {
            Debug.LogWarning(
                $"No customer definition for {type}"
            );

            return;
        }

        Customer customer = Instantiate(
            definition.Prefab,
            spawnPoint.transform.position,
            Quaternion.identity
        );

        spawnPoint.SetOccupied(true);

        customer.Initialize(
            this,
            spawnPoint,
            type
        );

        activeCustomers.Add(customer);

        SetupCustomer(customer);
    }

    private void SetupCustomer(Customer customer)
    {
        switch (customer.Type)
        {
            case CustomerType.Default:
                SetupDefaultCustomer(customer);
                break;

            case CustomerType.Order:
                SetupOrderCustomer(customer);
                break;

            case CustomerType.SpecialRequest:
                SetupSpecialRequestCustomer(customer);
                break;
        }
    }

    private void SetupDefaultCustomer(Customer customer)
    {
        if (defaultCustomerItemPool == null)
        {
            Debug.LogWarning(
                "Default Customer Item Pool is missing."
            );

            return;
        }

        DefaultCustomerItem item =
            defaultCustomerItemPool.GetRandomItem();

        if (item == null)
            return;

        int quantity =
            item.GetRandomQuantity();

        customer.SetDefaultRequest(
            item.Item,
            quantity
        );
    }

    private void SetupOrderCustomer(Customer customer)
    {
        // Daha sonra yapýlacak.
    }

    private void SetupSpecialRequestCustomer(Customer customer)
    {
        // Daha sonra yapýlacak.
    }

    private CustomerType GetRandomCustomerType()
    {
        float totalWeight =
            defaultCustomerWeight +
            orderCustomerWeight +
            specialRequestCustomerWeight;

        if (totalWeight <= 0f)
            return CustomerType.Default;

        float randomValue = Random.value * totalWeight;

        if (randomValue < defaultCustomerWeight)
            return CustomerType.Default;

        randomValue -= defaultCustomerWeight;

        if (randomValue < orderCustomerWeight)
            return CustomerType.Order;

        return CustomerType.SpecialRequest;
    }

    private CustomerDefinition GetRandomCustomerDefinition(
        CustomerType type)
    {
        if (customerDatabase == null)
            return null;

        List<CustomerDefinition> candidates =
            customerDatabase.GetCustomersByType(type);

        if (candidates.Count == 0)
            return null;

        float totalWeight = 0f;

        foreach (CustomerDefinition definition in candidates)
        {
            if (definition.SpawnWeight > 0f)
                totalWeight += definition.SpawnWeight;
        }

        if (totalWeight <= 0f)
            return candidates[
                Random.Range(0, candidates.Count)
            ];

        float randomValue = Random.value * totalWeight;

        foreach (CustomerDefinition definition in candidates)
        {
            if (definition.SpawnWeight <= 0f)
                continue;

            randomValue -= definition.SpawnWeight;

            if (randomValue <= 0f)
                return definition;
        }

        return candidates[candidates.Count - 1];
    }

    private List<CustomerSpawnPoint> GetFreeSpawnPoints()
    {
        List<CustomerSpawnPoint> freePoints = new();

        foreach (CustomerSpawnPoint point in spawnPoints)
        {
            if (!point.IsOccupied)
                freePoints.Add(point);
        }

        return freePoints;
    }

    public void PayDefaultCustomer(Customer customer)
    {
        if (customer == null)
            return;

        Debug.Log(
            $"Default customer bought " +
            $"{customer.RequestedQuantity}x " +
            $"{customer.RequestedItem.ItemName}"
        );

        // MoneyManager buraya baðlanacak.
    }

    public void CustomerLeaving(Customer customer)
    {
        if (customer == null)
            return;

        if (customer.transform == null)
            return;

        CustomerSpawnPoint point = null;

        foreach (CustomerSpawnPoint spawnPoint in spawnPoints)
        {
            if (Vector2.Distance(
                spawnPoint.transform.position,
                customer.transform.position
            ) < 0.1f)
            {
                point = spawnPoint;
                break;
            }
        }

        if (point != null)
            point.SetOccupied(false);

        activeCustomers.Remove(customer);

        Destroy(customer.gameObject);
    }

    private void Shuffle(
        List<CustomerSpawnPoint> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex =
                Random.Range(i, list.Count);

            CustomerSpawnPoint temp = list[i];

            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}