using Unity.Netcode;
using UnityEngine;
public class CollectibleItem : NetworkBehaviour
{
    [SerializeField] private int _pointValue = 1;
    private bool _collected = false;
    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;
        if (_collected) return;
        if (!other.CompareTag("Player")) return;

        PlayerInventory inventory = other.GetComponentInParent<PlayerInventory>();
        if (inventory == null) return;
        if (inventory.IsCarrying.Value) return;

        _collected = true;
        inventory.PickUpItem(_pointValue);
        GetComponent<NetworkObject>().Despawn();
    }
}
