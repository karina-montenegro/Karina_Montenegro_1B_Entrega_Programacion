using Unity.Netcode;
using UnityEngine;

public class WaitPanel : MonoBehaviour
{
    [SerializeField] private GameObject _waitPanel;
    private bool _subscribed = false;

    private void Update()
    {
        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsListening)
        {
            // Si se desconecta, reseteamos para poder suscribirse de nuevo
            if (_subscribed)
            {
                _subscribed = false;
                // Ocultamos el panel cuando se cierra la sesion
                if (_waitPanel != null)
                    _waitPanel.SetActive(false);
            }
            return;
        }

        if (!_subscribed && GameFlowManager.Instance != null)
        {
            _subscribed = true;
            GameFlowManager.Instance.GameStarted.OnValueChanged += OnGameStartedChanged;
            UpdatePanel(GameFlowManager.Instance.GameStarted.Value);
        }
    }

    private void OnGameStartedChanged(bool oldValue, bool newValue)
    {
        UpdatePanel(newValue);
    }

    private void UpdatePanel(bool gameStarted)
    {
        if (_waitPanel != null)
            _waitPanel.SetActive(!gameStarted);
    }

    private void OnDestroy()
    {
        if (GameFlowManager.Instance != null)
            GameFlowManager.Instance.GameStarted.OnValueChanged -= OnGameStartedChanged;
    }
}