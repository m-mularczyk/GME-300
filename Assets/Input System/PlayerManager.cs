using Game.Scripts.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    private PlayerInput_Actions _input;
    [SerializeField] private Player _player;

    void Start()
    {
        _input = new PlayerInput_Actions();
        _input.Player.Movement.Enable();
    }


    void Update()
    {
        _player.CalcutateMovement(_input.Player.Movement.ReadValue<Vector2>());
    }

    public void SetPlayerInVehicle(bool value)
    {
        if (value == true)
        {
            _input.Player.Movement.Disable();
        }
        else
        {
            _input.Player.Movement.Enable();
        }
    }
}
