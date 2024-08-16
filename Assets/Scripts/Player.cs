using Unity.Netcode;
using UnityEngine;


public class Player : NetworkBehaviour
{

    [SerializeField] InputReader inputReader;
    [SerializeField] GameObject objectToSpawn;

    [SerializeField] float moveSpeed = 1f;

    [SerializeField] float projectileCD = 1f;

    // Rotation slerp is currently not in the game.
    //[SerializeField] float rotationSpeed = 10f;

    [SerializeField] GameObject muzzle;

    private NetworkVariable<Vector2> moveInput = new NetworkVariable<Vector2>();

    private NetworkVariable<float> aimRotation = new NetworkVariable<float>();

    private float projectileCDTimer = 1f;

    private Vector3 mouseWorldPosition = Vector3.zero;

    Camera cam;

    void Start()
    {
        if (inputReader != null && IsLocalPlayer)
        {
            inputReader.MoveEvent += OnMove;
            inputReader.ShootEvent += OnFire;
            inputReader.AimEvent += OnAim;
        }
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsLocalPlayer) 
        {
            cam = Camera.main;
        }
    }

    private void OnFire()
    {
        FireRPC();
    }

    private void OnMove(Vector2 input)
    {
        MoveRPC(input);
    }

    private void OnAim(Vector2 input)
    {
        if (IsLocalPlayer) // Only the local players values and screen
        {
            Vector3 mousePosition = input;
            mousePosition.z = 0;
            mouseWorldPosition = cam.ScreenToWorldPoint(mousePosition);

            Vector3 aimDirection = (mouseWorldPosition - transform.position);
            aimDirection.Normalize();

            // Use tangent of y and x to get the rotaion angle (in degrees),
            // added 90 degrees to the angle to match the players default position.
            float rotAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg + 90f;

            AimRPC(rotAngle);
        }
    }

    private void Update()
    {
        if (IsServer)
        {
            if (projectileCDTimer < projectileCD)
            {
                projectileCDTimer += Time.deltaTime;
            }

            transform.position += (Vector3)(moveInput.Value * moveSpeed * Time.deltaTime);

            // Apply the rotation to the player (on the z axis since 2d).
            transform.rotation = Quaternion.Euler(0, 0, aimRotation.Value);
        }
    }

    [Rpc(SendTo.Server)]
    private void FireRPC()
    {
        if (projectileCDTimer >= projectileCD)
        {
            projectileCDTimer = 0;
            // Get the network object component of what we wish to spawn
            NetworkObject ob = Instantiate(objectToSpawn).GetComponent<NetworkObject>();

            // Set position based on the muzzle.
            ob.gameObject.transform.position = this.muzzle.transform.position;

            // Set direction of the projectile after the rotation on the character.
            // Testing if this could work, could we create it not being on the server?
            ob.gameObject.transform.rotation = this.transform.rotation;

            // Then spawn the object across the network.
            ob.Spawn();
        }
    }

    [Rpc(SendTo.Server)]
    private void MoveRPC(Vector2 data)
    {
        moveInput.Value = data;
    }

    [Rpc(SendTo.Server)]
    private void AimRPC(float data)
    {
        aimRotation.Value = data;
    }
}

