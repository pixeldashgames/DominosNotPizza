using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PieceSelector : MonoBehaviour
{
    [FormerlySerializedAs("pieceGraphic")] public Transform pieceGraphicParent;
    [SerializeField] float followMouseDampingTime;
    [SerializeField] float scaleDampingTime;
    [SerializeField] float moveDampingTime;


    [Range(0, 1)]
    [SerializeField] float mouseFollowDistance;
    [SerializeField] Image outline;
    [SerializeField] Color selectableLeftColor;
    [SerializeField] Color selectableRightColor;
    [SerializeField] Color selectableBothColor;
    [SerializeField] Color selectedPieceColor;
    [SerializeField] private Vector2 hoverScale;

    Vector2 _targetPosition, _originalGraphicPosition, _targetGraphicPosition, _targetScale;
    Vector2 _followMouseVelocity, _moveVelocity, _scaleVelocity;

    bool _hovering;
    public bool playableLeft, playableRight, selected;

    public PiecesContainerController controller;

    void Start()
    {
        _originalGraphicPosition = pieceGraphicParent.localPosition;
        _targetGraphicPosition = _originalGraphicPosition;
        _targetScale = Vector2.one;
    }
    public void Initialize(PiecesContainerController containerController)
    {
        controller = containerController;
    }
    void Update()
    {
        transform.localPosition = Vector2.SmoothDamp(transform.localPosition,
                                                     _targetPosition,
                                                     ref _moveVelocity,
                                                     moveDampingTime);

        if (_hovering)
        {
            Vector2 director = Mouse.current.position.ReadValue() - (Vector2)transform.position;
            _targetGraphicPosition = director * mouseFollowDistance;
        }

        pieceGraphicParent.localPosition = Vector2.SmoothDamp(pieceGraphicParent.localPosition,
                                                        _targetGraphicPosition,
                                                        ref _followMouseVelocity,
                                                        followMouseDampingTime);

        pieceGraphicParent.localScale = Vector2.SmoothDamp(pieceGraphicParent.localScale,
                                                     _targetScale,
                                                     ref _scaleVelocity,
                                                     scaleDampingTime);
    }

    internal void MarkUnplayable()
    {
        (playableLeft, playableRight) = (false, false);
        outline.enabled = false;
    }

    internal void MarkPlayable(bool canMatchLeft, bool canMatchRight)
    {
        (playableLeft, playableRight) = (canMatchLeft, canMatchRight);

        if (!playableLeft && !playableRight)
        {
            MarkUnplayable();
            return;
        }
        
        outline.enabled = true;
        outline.color = canMatchLeft ? (canMatchRight ? selectableBothColor : selectableLeftColor) : selectableRightColor;
    }

    public void SetTargetPosition(Vector2 position)
    => _targetPosition = position;

    public void Deselect()
    {
        selected = false;
        MarkPlayable(playableLeft, playableRight);
    }

    public void Select()
    {
        selected = true;
        outline.enabled = true;
        outline.color = selectedPieceColor;
    }
    public void OnPointerEnter(BaseEventData _)
    {
        _hovering = true;
        _targetScale = hoverScale;
    }
    public void OnPointerClick(BaseEventData _)
    {
        if (selected)
        {
            controller.SelectPiece(null);
            return;
        }
        
        if (playableLeft || playableRight)
            controller.SelectPiece(this);
    }
    public void OnPointerExit(BaseEventData _)
    {
        _hovering = false;
        _targetGraphicPosition = Vector2.zero;
        _targetScale = Vector2.one;
    }
}
