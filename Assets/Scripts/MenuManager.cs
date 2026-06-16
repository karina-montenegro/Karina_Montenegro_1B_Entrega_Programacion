using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using TMPro;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    [Header("Paneles de la UI")]
    [SerializeField] private GameObject _MainMenu;
    [SerializeField] private GameObject _Join;
    [SerializeField] private GameObject _WaitPlayer;
    [SerializeField] private GameObject _UIGameplay;
    [SerializeField] private GameObject _ResultPanel;

    [Header("Configuracion de Red")]
    [SerializeField] private TMP_InputField _ipInputField;
    [SerializeField] private string _ipPorDefecto = "127.0.0.1";

    [Header("Timer")]
    [SerializeField] private TMP_Text _timerText;

    [Header("Resultado")]
    [SerializeField] private TMP_Text _resultText;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _MainMenu.SetActive(true);
        _Join.SetActive(false);
        _WaitPlayer?.SetActive(false);
        _UIGameplay?.SetActive(false);
        _ResultPanel?.SetActive(false);

        if (_ipInputField != null)
            _ipInputField.text = _ipPorDefecto;
    }

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        _MainMenu.SetActive(false);
        _WaitPlayer?.SetActive(true);
    }

    public void StartClient()
    {
        string ipAddress = _ipInputField != null ? _ipInputField.text : _ipPorDefecto;
        if (string.IsNullOrEmpty(ipAddress) || ipAddress.Length < 5)
            ipAddress = _ipPorDefecto;

        if (NetworkManager.Singleton.NetworkConfig.NetworkTransport is UnityTransport transport)
            transport.ConnectionData.Address = ipAddress;

        NetworkManager.Singleton.StartClient();
        _Join.SetActive(false);
        _WaitPlayer?.SetActive(true);
    }

    public void ShowGameUI()
    {
        _WaitPlayer?.SetActive(false);
        _UIGameplay?.SetActive(true);
    }

    public void UpdateTimer(float seconds)
    {
        if (_timerText == null) return;
        int mins = Mathf.FloorToInt(seconds / 60f);
        int secs = Mathf.FloorToInt(seconds % 60f);
        _timerText.text = $"{mins:00}:{secs:00}";
    }

    public void ShowResult(GameResult result)
    {
        _UIGameplay?.SetActive(false);
        _ResultPanel?.SetActive(true);

        if (_resultText != null)
        {
            _resultText.text = result switch
            {
                GameResult.Win => "¡GANASTE!",
                GameResult.Lose => "PERDISTE",
                GameResult.Draw => "EMPATE",
                _ => ""
            };
        }
    }

    public void BackToMenu()
    {
        NetworkManager.Singleton.Shutdown();
        _ResultPanel?.SetActive(false);
        _UIGameplay?.SetActive(false);
        _MainMenu.SetActive(true);
    }

    public void SalirDelJuego()
    {
        Application.Quit();
    }
}