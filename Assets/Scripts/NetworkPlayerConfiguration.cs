using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkPlayerConfig : NetworkBehaviour
{
    [SerializeField] private PlayerInput _playerInput;
    private void Awake()
    {
        //PlayerInput
        _playerInput.enabled = false;
    }

    public override void OnNetworkSpawn()
    {
        _playerInput.enabled = IsOwner;
    }

    public override void OnNetworkDespawn()
    {
        _playerInput.enabled = false;
    }
}