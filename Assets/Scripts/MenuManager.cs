using Unity.Netcode;
using Unity.Netcode.Transports.UTP; // Necesario para cambiar la IP por código
using TMPro;
using UnityEngine;
public class MenuManager : MonoBehaviour
{
    [Header("Paneles de la UI")]
    [SerializeField] private GameObject _MainMenu;
    [SerializeField] private GameObject _Join;
    [SerializeField] private GameObject _WaitPlayer;
    [Header("Configuracion de Red")]
    [SerializeField] private TMP_InputField _ipInputField;
    [SerializeField] private string _ipPorDefecto = "127.0.0.1"; //colocar ip en inspector
    private void Start()
    {
        // Al empezar el juego, nos aseguramos de que solo se vea el menú principal
        _MainMenu.SetActive(true);
        _Join.SetActive(false);
        if (_WaitPlayer != null)
        {
            _WaitPlayer.SetActive(false);
        }

        if (_ipInputField != null)
        {
            _ipInputField.text = _ipPorDefecto;
        }
    }
    // Función para el botón SER HOST
    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        // Apagamos todo el menú para que no estorbe en el gameplay
        _MainMenu.SetActive(false);
        if (_WaitPlayer != null)
        {
            _WaitPlayer.SetActive(true);
        }
    }
    // Función para el botón CONECTAR (Cliente)
    public void StartClient()
    {
        // 1. Leemos lo que el usuario escribió en el cuadro de texto
        string ipAddress = _ipInputField.text;
        // Si el usuario no escribió nada, le ponemos la IP local por defecto
        if (string.IsNullOrEmpty(ipAddress) || ipAddress.Length < 5)
        {
            ipAddress = _ipPorDefecto;
        }
        // 2. Le asignamos esa IP al componente de transporte de Netcode
        if (NetworkManager.Singleton.NetworkConfig.NetworkTransport is UnityTransport transport)
        {
            transport.ConnectionData.Address = ipAddress;
        }
        // 3. Conectamos al jugador como Cliente
        NetworkManager.Singleton.StartClient();
        // Apagamos la pantalla de unirse para que pueda jugar
        _Join.SetActive(false);
        if (_WaitPlayer != null)
        {
            _WaitPlayer.SetActive(true);
        }
    }
    // Función para el botón SALIR
    public void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}
