using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interaction;

[RequireComponent(typeof(FocusableObject))]
public class MapInteraction : MonoBehaviour, IInteractable
{
    #region IInteractable Properties

    [field: SerializeField] public bool IsInteractable { get; set; } = true;

    private int _previousLayer;

    public event System.Action OnSuccessfulInteraction;
    public event System.Action OnFailedInteraction;

    #endregion


    private FocusableObject _focusableObjectScript;


    [Header("Focus Position")]
    [SerializeField] private float _maxHorizontalDistanceMultiplier = 0.8f;
    [SerializeField] private float _maxVerticalDistanceMultiplier = 0.8f;
    private Vector2 _mapBounds;

    private float _maxHorizontalDistance => _mapBounds.x * _maxHorizontalDistanceMultiplier * _focusableObjectScript.CameraOffsetMultiplier;
    private float _maxVerticalDistance => _mapBounds.y * _maxVerticalDistanceMultiplier * _focusableObjectScript.CameraOffsetMultiplier;



    private void Awake() => _focusableObjectScript ??= GetComponent<FocusableObject>();
    void Start() => _mapBounds = new Vector2(transform.localScale.z, transform.localScale.y);
    

    private void Update()
    {
        if (!_focusableObjectScript.IsFocused)
        {
            return;
        }

        Vector2 currentPositionOffset = new Vector2(_focusableObjectScript.GetPositionOffset().z, _focusableObjectScript.GetPositionOffset().y); // Due to quirks with how the map is set up, the x position offset is actually z.
        currentPositionOffset = currentPositionOffset + (PlayerInput.UINavigate * Time.deltaTime);
        currentPositionOffset.x = Mathf.Clamp(currentPositionOffset.x, -_maxHorizontalDistance, _maxHorizontalDistance);
        currentPositionOffset.y = Mathf.Clamp(currentPositionOffset.y, -_maxVerticalDistance, _maxVerticalDistance);

        _focusableObjectScript.SetPositionOffset(new Vector3(0.0f, currentPositionOffset.y, currentPositionOffset.x));
    }


    public void Interact(PlayerInteraction player)
    {
        StartInteraction();
        OnSuccessfulInteraction?.Invoke();
    }

    public void Highlight() => IInteractable.StartHighlight(this.gameObject, ref _previousLayer);
    public void StopHighlighting() => IInteractable.StopHighlight(this.gameObject, _previousLayer);


    public void StartInteraction()
    {
        _focusableObjectScript.StartFocus();

        // Prevent Input.
        PlayerInput.PreventAllActions(typeof(MapInteraction), disableGlobalMaps: true);

        // Subscribe to cancel event.
        PlayerInput.OnUICancelPerformed += StopInteraction;
    }
    public void StopInteraction()
    {
        _focusableObjectScript.StopFocus();

        // Remove Input Prevention.
        PlayerInput.RemoveAllActionPrevention(typeof(MapInteraction));

        // Unsubscribe from cancel event.
        PlayerInput.OnUICancelPerformed -= StopInteraction;
    }
}
