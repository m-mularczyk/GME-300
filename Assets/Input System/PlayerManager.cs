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

        //_input.Player.Movement.performed += Movement_performed;
    }

    private void Movement_performed(InputAction.CallbackContext context)
    {
        Debug.Log("Player moving ");
    }

    void Update()
    {
        _player.CalcutateMovement(_input.Player.Movement.ReadValue<Vector2>());
    }
}
