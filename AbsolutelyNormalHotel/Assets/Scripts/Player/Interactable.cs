// Created 05.07.2023 by Krista Plagemann//
// An interactable teleportation point. //


using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

public class Interactable : MonoBehaviour
{
    [SerializeField] private InteractableType InteractableType;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Material _hoverMat;
    [SerializeField] private Material _selectMat;
    private Material _baseMat;

    [SerializeField, Tooltip("If we should send a haptic feedback when starting select.")]
    private bool _SendStartSelectImpulse = false;
    [SerializeField, Tooltip("If we should send a haptic feedback when releasing select.")]
    private bool _SendEndSelectImpulse = true;

    [SerializeField, Tooltip("Index for possessable objects or wire parts.")]
    private int _IndexOfObject;

    public UnityEvent OnStartHover;
    public UnityEvent OnEndHover;
    public UnityEvent OnStartSelect;
    public UnityEvent OnEndSelect;


    [Header("Possessable object")]

    [SerializeField] private Transform _ToTeleportToObject;
    private TeleportPlayer _teleportPlayer;


    private bool _hovering = false;

    // Wire object

    private InputDevice _inputHand;
    private bool _sendHapticContinuous = false;

    private void Awake()
    {
        if (InteractableType == InteractableType.Teleportpoint)
        {
            if(_ToTeleportToObject == null)
                _ToTeleportToObject = transform;
            _teleportPlayer = FindObjectOfType<TeleportPlayer>();
        }

        if (_meshRenderer == null)
            _meshRenderer = GetComponent<MeshRenderer>();
        _baseMat = _meshRenderer.material;
    }

    private void LateUpdate()
    {
        if(_sendHapticContinuous)
            if(_inputHand != null)
                _inputHand.SendHapticImpulse(0, 0.4f, 0.1f);
    }

    // When start hovering over the object
    public void StartHover()
    {
        _hovering = true;
        OnStartHover?.Invoke();

        switch (InteractableType)
        {
            case InteractableType.Teleportpoint:
                if(_meshRenderer!=null)
                    _meshRenderer.material = _hoverMat;
                break;
            case InteractableType.Grabbable:
                if (_meshRenderer != null) _meshRenderer.material = _hoverMat;
                break;
        }
    }

    // Start selecting the object
    public void StartSelect(InputDevice selectingHand)
    {
        
        switch (InteractableType)
        {
            case InteractableType.Teleportpoint:
                if (_meshRenderer != null) _meshRenderer.material = _selectMat;

                // Sends haptic feedback on select
                if (_SendStartSelectImpulse)
                    selectingHand.SendHapticImpulse(0, 0.4f, 0.1f);
                break;

            case InteractableType.Grabbable:
                _inputHand = selectingHand;
                if (_SendStartSelectImpulse)
                    _sendHapticContinuous = true;
                if (_meshRenderer != null) _meshRenderer.material = _selectMat;
                break;
        }
        OnStartSelect?.Invoke();
    }

    // Stops selecting the object
    public void EndSelect(InputDevice selectingHand)
    {
        switch (InteractableType)
        {
            case InteractableType.Teleportpoint:
                if (_meshRenderer != null) _meshRenderer.material = _baseMat;
                // Only teleport if still hovering over object
                if (!_hovering)
                    return;
                _teleportPlayer.TeleportTo(_ToTeleportToObject);
                break;

            case InteractableType.Grabbable:
                if (_SendStartSelectImpulse)
                    _sendHapticContinuous = false; // turns off the haptic impulse
                if (_meshRenderer != null) _meshRenderer.material = _baseMat;
                break;
        }
        // Sends haptic feedback on select
        if (_SendEndSelectImpulse)
            selectingHand.SendHapticImpulse(0, 0.4f, 0.1f);
        OnEndSelect?.Invoke();
    }

    // A copy of EndSelect that doesn't include haptical to call in special cases.
    public void EndSelect()
    {
        switch (InteractableType)
        {
            case InteractableType.Teleportpoint:
                if (_meshRenderer != null) _meshRenderer.material = _baseMat;
                // Only teleport if still hovering over object
                if (!_hovering)
                    return;
                _teleportPlayer.TeleportTo(_ToTeleportToObject);
                break;

            case InteractableType.Grabbable:
                if (_SendStartSelectImpulse)
                    _sendHapticContinuous = false;  // turns off the haptic impulse
                if (_meshRenderer != null) _meshRenderer.material = _baseMat;
                break;
        }

        OnEndSelect?.Invoke();
    }

    // When stop hovering over the object
    public void EndHover()
    {
        _hovering = false;
        OnEndHover?.Invoke();

        switch (InteractableType)
        {
            case InteractableType.Teleportpoint:
                if (_meshRenderer != null) _meshRenderer.material = _baseMat;
                break;

            case InteractableType.Grabbable:
                //if (_SendStartSelectImpulse)
                    //_sendHapticContinuous = false; // turns off the haptic impulse
                if (_meshRenderer != null && _meshRenderer.material != _selectMat) _meshRenderer.material = _baseMat;
                break;
        }
    }
}
