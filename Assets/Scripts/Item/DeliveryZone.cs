using Unity.Netcode;
using UnityEngine;
public class DeliveryZone : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;
        if (!other.CompareTag("Player")) return;

        PlayerInventory inventory = other.GetComponentInParent<PlayerInventory>();
        if (inventory == null) return;
        if (!inventory.IsCarrying.Value) return;

        inventory.DeliveryItem();
    }
}
