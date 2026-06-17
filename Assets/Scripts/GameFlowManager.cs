using Unity.Netcode;
using UnityEngine;

public class GameFlowManager : NetworkBehaviour
{
    public static GameFlowManager Instance { get; private set; }

    [Header("Configuracion")]
    [SerializeField] private float _gameDuration = 120f;

    public NetworkVariable<bool> GameStarted = new NetworkVariable<bool>(
        false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<float> TimeRemaining = new NetworkVariable<float>(
        0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private bool _gameEnded = false;

    private void Awake()
    {
        Instance = this;
        Debug.Log("[GameFlow] Awake, Instance seteado.");
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        GameStarted.OnValueChanged += OnGameStartedChanged;
        TimeRemaining.OnValueChanged += OnTimeChanged;

        if (GameStarted.Value)
        {
            if (MenuManager.Instance != null)
                MenuManager.Instance.ShowGameUI();
        }

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
            CheckPlayerCount();
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        GameStarted.OnValueChanged -= OnGameStartedChanged;
        TimeRemaining.OnValueChanged -= OnTimeChanged;

        if (IsServer && NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
    }

    private void HandleClientConnected(ulong clientId)
    {
        Debug.Log($"[GameFlow] Cliente conectado: {clientId}");
        CheckPlayerCount();
    }

    private void CheckPlayerCount()
    {
        if (!IsServer || GameStarted.Value) return;

        int count = NetworkManager.Singleton.ConnectedClientsList.Count;
        Debug.Log($"[GameFlow] Chequeando jugadores: {count}");

        if (count >= 2)
        {
            TimeRemaining.Value = _gameDuration;
            GameStarted.Value = true;
            Debug.Log("[GameFlow] GameStarted = true");
        }
    }

    private void Update()
    {
        if (!IsServer || !GameStarted.Value || _gameEnded) return;

        TimeRemaining.Value -= Time.deltaTime;

        if (TimeRemaining.Value <= 0f)
        {
            TimeRemaining.Value = 0f;
            _gameEnded = true;
            EndGame();
        }
    }

    private void EndGame()
    {
        PlayerInventory[] players = FindObjectsByType<PlayerInventory>(FindObjectsSortMode.None);

        int hostScore = 0;
        int clientScore = 0;

        foreach (var p in players)
        {
            if (p.OwnerClientId == 0) hostScore = p.Score.Value;
            else clientScore = p.Score.Value;
        }

        Debug.Log($"[GameFlow] EndGame - Host: {hostScore} Client: {clientScore}");

        ShowResultClientRpc(hostScore, clientScore);
    }

    [ClientRpc]
    private void ShowResultClientRpc(int hostScore, int clientScore)
    {
        bool iAmHost = NetworkManager.Singleton.LocalClientId == 0;

        int myScore = iAmHost ? hostScore : clientScore;
        int theirScore = iAmHost ? clientScore : hostScore;

        GameResult result;
        if (myScore > theirScore) result = GameResult.Win;
        else if (myScore < theirScore) result = GameResult.Lose;
        else result = GameResult.Draw;

        Debug.Log($"[GameFlow] Resultado local: {result} (yo={myScore} rival={theirScore})");

        if (MenuManager.Instance != null)
            MenuManager.Instance.ShowResult(result);
    }

    //avisa a todos los clientes que vuelvan al menu
    [ClientRpc]
    public void BackToMenuClientRpc()
    {
        if (MenuManager.Instance != null)
            MenuManager.Instance.ExecuteBackToMenu();
    }

    private void OnGameStartedChanged(bool oldVal, bool newVal)
    {
        Debug.Log($"[GameFlow] OnGameStartedChanged -> {newVal}");
        if (newVal && MenuManager.Instance != null)
            MenuManager.Instance.ShowGameUI();
    }

    private void OnTimeChanged(float oldVal, float newVal)
    {
        if (MenuManager.Instance != null)
            MenuManager.Instance.UpdateTimer(newVal);
    }
}