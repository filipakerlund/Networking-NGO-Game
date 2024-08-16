using Unity.Netcode;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    [SerializeField] float projectileSpeed = 3.5f;
    [SerializeField] float projectileLifetime = 3f;
    [SerializeField] int projectileDamage = 25;

    private float timer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        timer = 0f;
    }
    
    void Update()
    {
        if (IsServer)
        {
            transform.position += -transform.up * (projectileSpeed * Time.deltaTime);
            timer += Time.deltaTime;

            if (timer > projectileLifetime)
            {
                DestroyProjectileServerRpc();
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsServer)
        {
            if (collision.gameObject.TryGetComponent<HealthSystem>(out HealthSystem healthSystem))
            {
                // If the component is found, deal damage to the object
                healthSystem.ApplyDamage(projectileDamage);
            }
            DestroyProjectileServerRpc();
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void DestroyProjectileServerRpc()
    {
        NetworkObject.Despawn();
    }
}
