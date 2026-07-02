using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Scripts.UI;


namespace Game.Scripts.LiveObjects
{
    public class InteractableZone : MonoBehaviour
    {
        private enum ZoneType
        {
            Collectable,
            Action,
            HoldAction
        }

        /* // Not necessary with the new Input System
        private enum KeyState
        {
            Press,
            PressHold
        }
        */

        [SerializeField]
        private ZoneType _zoneType;
        [SerializeField]
        private int _zoneID;
        [SerializeField]
        private int _requiredID;
        [SerializeField]
        [Tooltip("Press the (---) Key to .....")]
        private string _displayMessage;
        [SerializeField]
        private GameObject[] _zoneItems;
        private bool _inZone = false;
        private bool _itemsCollected = false;
        private bool _actionPerformed = false;
        [SerializeField]
        private Sprite _inventoryIcon;
        /* // Not necessary with the new Input System
        [SerializeField]
        private KeyCode _zoneKeyInput;
        */
        /* // Not necessary with the new Input System
        [SerializeField]
        private KeyState _keyState;
        */
        [SerializeField]
        private GameObject _marker;

        //private bool _inHoldState = false;

        private static int _currentZoneID = 0;
        public static int CurrentZoneID
        { 
            get 
            { 
               return _currentZoneID; 
            }
            set
            {
                _currentZoneID = value; 
                         
            }
        }

        public static event Action<InteractableZone> onZoneInteractionComplete;
        public static event Action<int> onHoldStarted;
        public static event Action<int> onHoldEnded;

        private PlayerInput _playerInput;

        private void Awake()
        {
            _playerInput = FindFirstObjectByType<PlayerInput>();
            if (_playerInput == null)
            {
                Debug.LogError("PlayerInput is NULL!");
            }

        }

        private void OnEnable()
        {
            InteractableZone.onZoneInteractionComplete += SetMarker;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && _currentZoneID > _requiredID)
            {
                _playerInput.SetZoneInRange(this);
                //Debug.Log("Interactable zone is now: " + this);


                switch (_zoneType)
                { 
                    case ZoneType.Collectable:
                        if (_itemsCollected == false)
                        {
                            _inZone = true;
                            if (_displayMessage != null)
                            {
                                string message = $"Press the {_playerInput.GetInteractionKey().ToString()} key to {_displayMessage}.";
                                //string message = $"Press the {_zoneKeyInput.ToString()} key to {_displayMessage}.";
                                UIManager.Instance.DisplayInteractableZoneMessage(true, message);
                            }
                            else
                                UIManager.Instance.DisplayInteractableZoneMessage(true, $"Press the {_playerInput.GetInteractionKey().ToString()} key to collect");
                                //UIManager.Instance.DisplayInteractableZoneMessage(true, $"Press the {_zoneKeyInput.ToString()} key to collect");
                        }
                        break;

                    case ZoneType.Action:
                        if (_actionPerformed == false)
                        {
                            _inZone = true;
                            if (_displayMessage != null)
                            {
                                string message = $"Press the {_playerInput.GetInteractionKey().ToString()} key to {_displayMessage}.";
                                //string message = $"Press the {_zoneKeyInput.ToString()} key to {_displayMessage}.";
                                UIManager.Instance.DisplayInteractableZoneMessage(true, message);
                            }
                            else
                                UIManager.Instance.DisplayInteractableZoneMessage(true, $"Press the {_playerInput.GetInteractionKey().ToString()} key to perform action");
                                //UIManager.Instance.DisplayInteractableZoneMessage(true, $"Press the {_zoneKeyInput.ToString()} key to perform action");
                        }
                        break;

                    case ZoneType.HoldAction:
                        _inZone = true;
                        if (_displayMessage != null)
                        {
                            string message = $"Press the {_playerInput.GetInteractionKey().ToString()} key to {_displayMessage}.";
                            //string message = $"Press the {_zoneKeyInput.ToString()} key to {_displayMessage}.";
                            UIManager.Instance.DisplayInteractableZoneMessage(true, message);
                        }
                        else
                            UIManager.Instance.DisplayInteractableZoneMessage(true, $"Hold the {_playerInput.GetInteractionKey().ToString()} key to perform action");
                            //UIManager.Instance.DisplayInteractableZoneMessage(true, $"Hold the {_zoneKeyInput.ToString()} key to perform action");
                        break;
                }
            }
        }

        private void Update()
        {
            /* // Whole section moved to the Interact() method
            if (_inZone == true)
            {

                if (Input.GetKeyDown(_zoneKeyInput) && _keyState != KeyState.PressHold)
                {
                    //press
                    switch (_zoneType)
                    {
                        case ZoneType.Collectable:
                            if (_itemsCollected == false)
                            {
                                CollectItems();
                                _itemsCollected = true;
                                UIManager.Instance.DisplayInteractableZoneMessage(false);
                            }
                            break;

                        case ZoneType.Action:
                            if (_actionPerformed == false)
                            {
                                PerformAction();
                                _actionPerformed = true;
                                UIManager.Instance.DisplayInteractableZoneMessage(false);
                            }
                            break;
                    }
                }
                else if (Input.GetKey(_zoneKeyInput) && _keyState == KeyState.PressHold && _inHoldState == false)
                {
                    _inHoldState = true;

                   

                    switch (_zoneType)
                    {                      
                        case ZoneType.HoldAction:
                            PerformHoldAction();
                            break;           
                    }
                }

                if (Input.GetKeyUp(_zoneKeyInput) && _keyState == KeyState.PressHold)
                {
                    _inHoldState = false;
                    onHoldEnded?.Invoke(_zoneID);
                }
  
            }
            */
        }
       
        private void CollectItems()
        {
            foreach (var item in _zoneItems)
            {
                item.SetActive(false);
            }

            UIManager.Instance.UpdateInventoryDisplay(_inventoryIcon);

            CompleteTask(_zoneID);

            onZoneInteractionComplete?.Invoke(this);

        }

        private void PerformAction()
        {
            foreach (var item in _zoneItems)
            {
                item.SetActive(true);
            }

            if (_inventoryIcon != null)
                UIManager.Instance.UpdateInventoryDisplay(_inventoryIcon);

            onZoneInteractionComplete?.Invoke(this);
        }

        private void PerformHoldAction()
        {
            UIManager.Instance.DisplayInteractableZoneMessage(false);
            onHoldStarted?.Invoke(_zoneID);
        }

        public GameObject[] GetItems()
        {
            return _zoneItems;
        }

        public int GetZoneID()
        {
            return _zoneID;
        }

        public void CompleteTask(int zoneID)
        {
            if (zoneID == _zoneID)
            {
                _currentZoneID++;
                onZoneInteractionComplete?.Invoke(this);
            }
        }

        public void ResetAction(int zoneID)
        {
            if (zoneID == _zoneID)
                _actionPerformed = false;
        }

        public void SetMarker(InteractableZone zone)
        {
            if (_zoneID == _currentZoneID)
                _marker.SetActive(true);
            else
                _marker.SetActive(false);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _inZone = false;
                UIManager.Instance.DisplayInteractableZoneMessage(false);

                var input = FindFirstObjectByType<PlayerInput>();
                input.SetZoneInRange(null);
            }
        }

        private void OnDisable()
        {
            InteractableZone.onZoneInteractionComplete -= SetMarker;
        }

        public void StartHold()
        {
            //Debug.Log("Hold started");
            if (!_inZone || _zoneType != ZoneType.HoldAction) return;
            
            //_inHoldState = true;
            PerformHoldAction();
        }
        public void EndHold()
        {
            //Debug.Log("Hold ended");
            if (!_inZone || _zoneType != ZoneType.HoldAction) return;

            //_inHoldState = false;
            onHoldEnded?.Invoke(_zoneID);

        }

        
        public void Interact()
        {
            if (!_inZone) return;

            switch (_zoneType)
            {
                case ZoneType.Collectable:
                    if (!_itemsCollected)
                    {
                        CollectItems();
                        _itemsCollected = true;
                        UIManager.Instance.DisplayInteractableZoneMessage(false);
                    }
                    break;

                case ZoneType.Action:
                    if (!_actionPerformed)
                    {
                        PerformAction();
                        _actionPerformed = true;
                        UIManager.Instance.DisplayInteractableZoneMessage(false);
                    }
                    break;

                case ZoneType.HoldAction:
                    //PerformHoldAction();
                    //Debug.Log("HoldAction zone requires hold input, not tap");
                    break;
            }

            Laptop laptop = GetLaptop();
            if (laptop != null)
            {
                laptop.NextSurveillanceCamera();
            }
        }

        public void ExitZoneActivity()
        {
            GetLaptop()?.ExitSurveillanceCameras();
        }

        private Laptop GetLaptop()
        {
            foreach (var item in _zoneItems)
            {
                if (item == null) continue;

                var laptop = item.GetComponent<Laptop>();
                if (laptop != null)
                    return laptop;
            }
            return null;
        }
    }
}


