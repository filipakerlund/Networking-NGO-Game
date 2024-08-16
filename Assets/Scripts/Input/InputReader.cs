using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputReader", menuName = "Game/Input Reader")]
public class InputReader : ScriptableObject, GameInput.IGameplayActions
{
    private GameInput gameInput;
    public event UnityAction<Vector2> MoveEvent = delegate { };
    public event UnityAction ShootEvent = delegate { };
    public event UnityAction SendEvent = delegate { };
    public event UnityAction<Vector2> AimEvent = delegate { };

    private bool isInputEnabled = true;

    public void OnMove(InputAction.CallbackContext context)
    {
        if (isInputEnabled)
        {
            MoveEvent.Invoke(context.ReadValue<Vector2>());
        }
    }

    public void OnSend(InputAction.CallbackContext context)
    {
        if (context.performed) { SendEvent.Invoke(); }
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (isInputEnabled)
        {
            if (context.performed) { ShootEvent.Invoke(); }
        }
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        AimEvent.Invoke(context.ReadValue<Vector2>()); 
    }

    private void OnEnable()
    {
        if (gameInput == null)
        {
            gameInput = new GameInput();
            gameInput.Gameplay.SetCallbacks(this);
            gameInput.Gameplay.Enable();
        }
    }

    public void ToggleInput(bool isEnabled)
    {
        isInputEnabled = isEnabled;
    }
}
