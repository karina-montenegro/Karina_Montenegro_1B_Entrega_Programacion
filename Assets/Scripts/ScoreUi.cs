using TMPro;
using Unity.Netcode;
using UnityEngine;
public class ScoreUI : MonoBehaviour
{
    public static ScoreUI Instance { get; private set; }
    public TMP_Text myScoreText;
    public TMP_Text otherScoreText;
    [Header("Colores por rol")]
    [SerializeField] private Color _hostColor = Color.red;
    [SerializeField] private Color _clientColor = Color.blue;
    private PlayerInventory localPlayer;
    private PlayerInventory otherPlayer;
    private void Awake()
    {
        Instance = this;
    }
    // Esta función recibe al jugador dueño de la pantalla local
    public void SetLocalPlayer(PlayerInventory player)
    {
        localPlayer = player;
        if (myScoreText != null && localPlayer != null)
        {
            // Seteamos el valor de tu NetworkVariable 'Score'
            myScoreText.text = localPlayer.Score.Value.ToString();
            myScoreText.color = GetColorForPlayer(localPlayer);
            // Nos suscribimos de manera segura al cambio de valor en red
            localPlayer.Score.OnValueChanged += UpdateMyScoreText;
        }
    }
    // Esta función recibe al jugador rival (réplica de red)
    public void SetOtherPlayer(PlayerInventory player)
    {
        otherPlayer = player;
        if (otherScoreText != null && otherPlayer != null)
        {
            // Seteamos el valor del puntaje del rival
            otherScoreText.text = otherPlayer.Score.Value.ToString();
            otherScoreText.color = GetColorForPlayer(otherPlayer);
            // Nos suscribimos al cambio de valor del rival
            otherPlayer.Score.OnValueChanged += UpdateOtherScoreText;
        }
    }
    private Color GetColorForPlayer(PlayerInventory player)
    {
        // El Host siempre tiene OwnerClientId == 0 en Netcode for GameObjects
        return player.OwnerClientId == 0 ? _hostColor : _clientColor;
    }
    private void UpdateMyScoreText(int oldVal, int newVal)
    {
        if (myScoreText != null)
        {
            myScoreText.text = newVal.ToString();
        }
    }
    private void UpdateOtherScoreText(int oldVal, int newVal)
    {
        if (otherScoreText != null)
        {
            otherScoreText.text = newVal.ToString();
        }
    }
    private void OnDestroy()
    {
        // Limpieza fundamental de eventos usando los mismos métodos que suscribimos
        if (localPlayer != null)
        {
            localPlayer.Score.OnValueChanged -= UpdateMyScoreText;
        }
        if (otherPlayer != null)
        {
            otherPlayer.Score.OnValueChanged -= UpdateOtherScoreText;
        }
        if (Instance == this)
        {
            Instance = null;
        }
    }
}

