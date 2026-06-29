using Game.Scripts.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private PlayerInputActions _input;
    [SerializeField] private Player _player;

    void Start()
    {
        if( _player == null)
        {
            Debug.LogError("Player is NULL");
        }

        _input = new PlayerInputActions();
        _input.Player.Enable();
    }

    void Update()
    {
        _player.CalcutateMovement(_input.Player.Movement.ReadValue<Vector2>());
    }
}
