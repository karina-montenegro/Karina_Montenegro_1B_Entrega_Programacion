using Unity.Netcode;
using UnityEngine;

public class CollectibleItem : NetworkBehaviour
{
    [SerializeField] private int _pointValue = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        if (other.CompareTag("Player"))
        {
            // Acá sumás los puntos al jugador. ajustalo a tu script de puntaje real:
            // other.GetComponent<PlayerScore>().AddPoints(_pointValue);

            GetComponent<NetworkObject>().Despawn();
        }
    }
}