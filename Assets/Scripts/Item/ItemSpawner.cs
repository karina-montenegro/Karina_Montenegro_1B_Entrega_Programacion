using Unity.Netcode;
using UnityEngine;

public class ItemSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject[] _itemPrefabs; // ahora es un array
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private float _spawnInterval = 20f;
    [SerializeField] private LayerMask _blockingLayers;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        InvokeRepeating(nameof(SpawnItems), 0f, _spawnInterval);
    }

    private void SpawnItems()
    {
        foreach (Transform point in _spawnPoints)
        {
            if (point == null) continue;

            // Pasamos _blockingLayers al final para que IGNORE el suelo
            Collider[] colliders = Physics.OverlapSphere(point.position, 0.5f, _blockingLayers);

            if (colliders.Length == 0)
            {
                int randomIndex = Random.Range(0, _itemPrefabs.Length);
                GameObject item = Instantiate(_itemPrefabs[randomIndex], point.position, Quaternion.identity);
                item.GetComponent<NetworkObject>().Spawn();
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (_spawnPoints == null) return;

        Gizmos.color = Color.cyan;
        foreach (Transform point in _spawnPoints)
        {
            if (point != null)
                Gizmos.DrawWireSphere(point.position, 0.5f);
        }
    }
}