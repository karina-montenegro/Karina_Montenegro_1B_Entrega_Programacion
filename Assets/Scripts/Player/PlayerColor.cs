using Unity.Netcode;
using UnityEngine;

public class PlayerColor : NetworkBehaviour
{
    [SerializeField] private Renderer _renderer;
    [SerializeField] private Material[] _colors;

    public override void OnNetworkSpawn()
    {
        // Solo el host calcula su color y le avisa al Server
        if (IsOwner)
        {
            int index = IsHost ? 0 : 1;
            SolicitarColorAlServerRpc(index);
        }
    }

    // El cliente le pide permiso al Servidor
    [Rpc(SendTo.Server)]
    private void SolicitarColorAlServerRpc(int colorIndex)
    {
        // El servidor recibe la orden y la replica a todos los jugadores
        EnviarColorATodosRpc(colorIndex);
    }

    // se ejecuta en las pantallas de todos los jugadores
    [Rpc(SendTo.Everyone)]
    private void EnviarColorATodosRpc(int colorIndex)
    {
        if (_renderer != null && _colors.Length > 0)
        {
            _renderer.material = _colors[colorIndex];
        }
    }
}