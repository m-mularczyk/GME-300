using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Game.Scripts.UI;
using UnityEngine.Rendering.Universal;

namespace Game.Scripts.LiveObjects
{
    public class Drone : MonoBehaviour
    {
        private enum Tilt
        {
            NoTilt, Forward, Back, Left, Right
        }

        [SerializeField]
        private Rigidbody _rigidbody;
        [SerializeField]
        private float _speed = 5f;
        private bool _inFlightMode = false;
        [SerializeField]
        private Animator _propAnim;
        [SerializeField]
        private CinemachineVirtualCamera _droneCam;
        [SerializeField]
        private InteractableZone _interactableZone;
        

        public static event Action OnEnterFlightMode;
        public static event Action onExitFlightmode;

        private PlayerInput_Actions _input;
        private PlayerManager _playerManager;

        private void OnEnable()
        {
            InteractableZone.onZoneInteractionComplete += EnterFlightMode;
        }

        private void Start()
        {
            _input = new PlayerInput_Actions();
            _input.Drone.Enable();
            _input.Player.Escape.Enable();

            _playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
            if (_playerManager == null)
            {
                Debug.LogError("PlayerManager is NULL");
            }
        }

        private void EnterFlightMode(InteractableZone zone)
        {
            if (_inFlightMode != true && zone.GetZoneID() == 4) // drone Scene
            {
                _propAnim.SetTrigger("StartProps");
                _droneCam.Priority = 11;
                _inFlightMode = true;
                _playerManager.SetPlayerInVehicle(_inFlightMode);
                OnEnterFlightMode?.Invoke();
                UIManager.Instance.DroneView(true);
                _interactableZone.CompleteTask(4);
            }
        }

        private void ExitFlightMode()
        {            
            _droneCam.Priority = 9;
            _inFlightMode = false;
            _playerManager.SetPlayerInVehicle(_inFlightMode);
            UIManager.Instance.DroneView(false);            
        }

        private void Update()
        {
            if (_inFlightMode)
            {
                CalculateTilt();
                CalculateMovementUpdate();

                //if (Input.GetKeyDown(KeyCode.Escape))
                if (_input.Player.Escape.WasPressedThisFrame())
                {
                    _inFlightMode = false;
                    onExitFlightmode?.Invoke();
                    ExitFlightMode();
                }
            }
        }

        private void FixedUpdate()
        {
            _rigidbody.AddForce(transform.up * (9.81f), ForceMode.Acceleration);
            if (_inFlightMode)
                CalculateMovementFixedUpdate();
        }

        private void CalculateMovementUpdate()
        {
            /*
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                var tempRot = transform.localRotation.eulerAngles;
                tempRot.y -= _speed / 3;
                transform.localRotation = Quaternion.Euler(tempRot);
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                var tempRot = transform.localRotation.eulerAngles;
                tempRot.y += _speed / 3;
                transform.localRotation = Quaternion.Euler(tempRot);
            }
            */

            var tempRot = transform.localRotation.eulerAngles;
            tempRot.y += _input.Drone.Rotation.ReadValue<float>() * _speed / 3;
            transform.localRotation = Quaternion.Euler(tempRot);

        }

        private void CalculateMovementFixedUpdate()
        {
            /*
            if (Input.GetKey(KeyCode.Space))
            {
                _rigidbody.AddForce(transform.up * _speed, ForceMode.Acceleration);
            }
            if (Input.GetKey(KeyCode.V))
            {
                _rigidbody.AddForce(-transform.up * _speed, ForceMode.Acceleration);
            }
            */

            _rigidbody.AddForce(transform.up * _input.Drone.Thrust.ReadValue<float>() * _speed, ForceMode.Acceleration);

        }

        private void CalculateTilt()
        {

            /*
            if (Input.GetKey(KeyCode.A)) 
                transform.rotation = Quaternion.Euler(00, transform.localRotation.eulerAngles.y, 30);
            else if (Input.GetKey(KeyCode.D))
                transform.rotation = Quaternion.Euler(0, transform.localRotation.eulerAngles.y, -30);
            else if (Input.GetKey(KeyCode.W))
                transform.rotation = Quaternion.Euler(30, transform.localRotation.eulerAngles.y, 0);
            else if (Input.GetKey(KeyCode.S))
                transform.rotation = Quaternion.Euler(-30, transform.localRotation.eulerAngles.y, 0);
            else 
                transform.rotation = Quaternion.Euler(0, transform.localRotation.eulerAngles.y, 0);
            */

            transform.rotation = Quaternion.Euler(_input.Drone.Movement.ReadValue<Vector2>().y * 30f, transform.localRotation.eulerAngles.y, _input.Drone.Movement.ReadValue<Vector2>().x * (-30f));
        }

        private void OnDisable()
        {
            InteractableZone.onZoneInteractionComplete -= EnterFlightMode;
        }
    }
}
