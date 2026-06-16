using Unity.Netcode;
using UnityEngine;
public class WaitPanel : MonoBehaviour
{
    [SerializeField] private GameObject _waitPanel;
    private bool _subscribed = false;

    private void Update()
    {
        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsListening) return;

        if (!_subscribed && GameFlowManager.Instance != null)
        {
            _subscribed = true;
            Debug.Log("[WaitPanel] Suscrito a GameStarted. Valor actual: " + GameFlowManager.Instance.GameStarted.Value);
            GameFlowManager.Instance.GameStarted.OnValueChanged += OnGameStartedChanged;
            UpdatePanel(GameFlowManager.Instance.GameStarted.Value);
        }
    }

    private void OnGameStartedChanged(bool oldValue, bool newValue)
    {
        Debug.Log($"[WaitPanel] OnGameStartedChanged: {oldValue} -> {newValue}");
        UpdatePanel(newValue);
    }

    private void UpdatePanel(bool gameStarted)
    {
        Debug.Log($"[WaitPanel] UpdatePanel llamado con gameStarted={gameStarted}. _waitPanel null? {_waitPanel == null}");
        if (_waitPanel != null)
        {
            _waitPanel.SetActive(!gameStarted);
            Debug.Log("[WaitPanel] Panel ahora activeSelf=" + _waitPanel.activeSelf);
        }
    }

    private void OnDestroy()
    {
        if (GameFlowManager.Instance != null)
        {
            GameFlowManager.Instance.GameStarted.OnValueChanged -= OnGameStartedChanged;
        }
    }
}
