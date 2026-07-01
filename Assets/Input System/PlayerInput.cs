using Game.Scripts.LiveObjects;
using Game.Scripts.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private PlayerInputActions _input;
    [SerializeField] private Player _player;
    private InteractableZone _zoneInRange;

    private float _pressTime;

    void Start()
    {
        if( _player == null)
        {
            Debug.LogError("Player is NULL");
        }

        _input = new PlayerInputActions();
        _input.Player.Enable();
        _input.Player.Interaction.started += Interaction_started;
        _input.Player.Interaction.canceled += Interaction_canceled;
    }

    private void Interaction_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        // On Hold Start
        _pressTime = Time.time;

        _zoneInRange?.StartHold();
    }

    private void Interaction_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        // On Hold End
        if (_zoneInRange == null) return;

        float heldTime = Time.time - _pressTime;

        if (heldTime < 0.25f)
        {
            _zoneInRange.Interact();
            _zoneInRange.EndHold();
        }
        else
        {
            _zoneInRange.EndHold();
        }

    }

    void Update()
    {
        _player.CalcutateMovement(_input.Player.Movement.ReadValue<Vector2>());
    }

    public void SetZoneInRange(InteractableZone zone)
    {
        _zoneInRange = zone;
    }

    public string GetInteractionKey()
    {
        return _input.Player.Interaction.GetBindingDisplayString();
    }

}
