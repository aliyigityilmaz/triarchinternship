using UnityEngine;

public class CustomerSpawnPoint : MonoBehaviour
{
    public bool IsOccupied { get; private set; }

    public void SetOccupied(bool occupied)
    {
        IsOccupied = occupied;
    }
}