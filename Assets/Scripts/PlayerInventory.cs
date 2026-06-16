using Unity.Netcode;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
public class PlayerInventory : NetworkBehaviour
{
    public NetworkVariable<bool> IsCarrying = new NetworkVariable<bool>(false);
    public NetworkVariable<int> CarriedValue = new NetworkVariable<int>(0);
    public NetworkVariable<int> Score = new NetworkVariable<int>(0);
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (ScoreUI.Instance != null)
        {
            RegisterWithUI();
        }
        else
        {
            StartCoroutine(WaitForUIAndRegister());
        }
    }
    private System.Collections.IEnumerator WaitForUIAndRegister()
    {
        while (ScoreUI.Instance == null)
            yield return null;

        RegisterWithUI();
    }
    private void RegisterWithUI()
    {
        if (IsOwner)
            ScoreUI.Instance.SetLocalPlayer(this);
        else
            ScoreUI.Instance.SetOtherPlayer(this);
    }
    public void PickUpItem(int pointValue)
    {
        if (!IsServer) return;
        if (IsCarrying.Value) return;
        IsCarrying.Value = true;
        CarriedValue.Value = pointValue;
    }
    public void DeliveryItem()
    {
        if (!IsServer) return;
        if (!IsCarrying.Value) return;
        Score.Value += CarriedValue.Value;
        IsCarrying.Value = false;
        CarriedValue.Value = 0;
    }
}