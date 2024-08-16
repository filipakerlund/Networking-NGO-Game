using System;
using UnityEngine;
using Unity.Netcode;

public class HealthSystem : NetworkBehaviour
{
    private NetworkVariable<int> playerHealth = new NetworkVariable<int>();
    [SerializeField] int baseHealth = 100;

    public static event Action<int> OnHealthChange;

    public static event Action<bool> GameResultEvent;

    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        playerHealth.Value = baseHealth;
        if (!IsServer)
        {
            playerHealth.OnValueChanged += HealthChanged;
        }
    }

    private void HealthChanged(int previousValue, int newValue)
    {
        if (IsOwner)
        {
            // Call event to PlayerInfoUI.
            OnHealthChange?.Invoke(playerHealth.Value);
        }
    }

    public void ApplyDamage(int damage)
    {
        if (IsServer) 
        {
            playerHealth.Value -= damage;

            // Since it can be a host, we need to invoke here as well.
            if (IsOwner) 
            {
                OnHealthChange?.Invoke(playerHealth.Value);
            }

            if (playerHealth.Value <= 0) 
            {
                // Server send loss result to owner.
                LossResultRpc();
                // Server send win result to others.
                WinResultRpc();
            }
        }
    }
    [Rpc(SendTo.Owner)]
    private void LossResultRpc()
    {
        GameResultEvent?.Invoke(false);
    }

    [Rpc(SendTo.NotOwner)]
    private void WinResultRpc()
    { 
        GameResultEvent?.Invoke(true);
    }
}
