using TMPro;
using Unity.Netcode;
using UnityEngine;
public class ScoreUI : MonoBehaviour
{
    public static ScoreUI Instance { get; private set; }
    public TMP_Text myScoreText;
    public TMP_Text otherScoreText;
    private PlayerInventory localPlayer;
    private PlayerInventory otherPlayer;
    private void Awake()
    {
        Instance = this;
    }
    // Esta funcion recibe al jugador dueno de la pantalla local
    public void SetLocalPlayer(PlayerInventory player)
    {
        localPlayer = player;
        if (myScoreText != null && localPlayer != null)
        {
            // Seteamos el valor de tu NetworkVariable 'Score'
            myScoreText.text = localPlayer.Score.Value.ToString();
            // Nos suscribimos de manera segura al cambio de valor en red
            localPlayer.Score.OnValueChanged += UpdateMyScoreText;
        }
    }
    // Esta funcion recibe al jugador rival (replica de red)
    public void SetOtherPlayer(PlayerInventory player)
    {
        otherPlayer = player;
        if (otherScoreText != null && otherPlayer != null)
        {
            // Seteamos el valor del puntaje del rival
            otherScoreText.text = otherPlayer.Score.Value.ToString();
            // Nos suscribimos al cambio de valor del rival
            otherPlayer.Score.OnValueChanged += UpdateOtherScoreText;
        }
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
        // Limpieza fundamental de eventos usando los mismos metodos que suscribimos
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
