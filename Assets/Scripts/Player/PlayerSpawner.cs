using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [Header("Puntos de Spawn")]
    [Tooltip("Arrastrá aquí 2 GameObjects vacíos ubicados donde querés que aparezca cada jugador")]
    [SerializeField] private Transform[] _spawnPoints;

    // Lleva la cuenta de cuántos jugadores ya se conectaron, para asignar el spawn point correspondiente
    private int _siguienteIndiceSpawn = 0;

    private void Start()
    {
        StartCoroutine(SuscribirseCuandoExistaNetworkManager());
    }

    private IEnumerator SuscribirseCuandoExistaNetworkManager()
    {
        // Esperamos hasta que el NetworkManager.Singleton esté listo,
        // por si este script se inicializa antes de que Netcode termine de configurarse
        while (NetworkManager.Singleton == null)
            yield return null;

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        // Esta lógica solo debe ejecutarse en el servidor/host, que es quien tiene autoridad sobre las posiciones
        if (!NetworkManager.Singleton.IsServer) return;

        if (_spawnPoints == null || _spawnPoints.Length == 0)
            return;

        // Buscamos el NetworkObject del jugador que se acaba de conectar
        if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out NetworkClient client))
            return;

        NetworkObject playerObject = client.PlayerObject;
        if (playerObject == null)
            return;

        // Elegimos el spawn point según el orden de conexión, sin pasarnos del array
        int indice = _siguienteIndiceSpawn % _spawnPoints.Length;
        Transform spawnPoint = _spawnPoints[indice];

        playerObject.transform.position = spawnPoint.position;
        playerObject.transform.rotation = spawnPoint.rotation;

        _siguienteIndiceSpawn++;
    }
}