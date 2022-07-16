using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using System.Linq;
using DominoPlayer;

public class PiecesContainerController : MonoBehaviour
{
    [SerializeField] PieceSelector piecePrefab;
    [SerializeField][Min(0)] float pieceSpacing;

    private IPiecePainter _painter;
    public event System.Action<PieceSelector> OnPieceSelected;

    private float _pieceWidth = -1;

    private float PieceWidth
    {
        get
        {
            if (_pieceWidth < 0)
            {
                _pieceWidth = piecePrefab.GetComponent<RectTransform>().rect.width;
            }

            return _pieceWidth;
        }
    }
    private float TotalWidth => (_pieces.Count * PieceWidth) + (_pieces.Count - 1) * pieceSpacing;

    private readonly List<PieceSelector> _pieces = new();

    void Awake()
    {
        _painter = GameSettingsController.Instance.CreatePainter(GameSettingsController.Instance.painterMode);
    }

    [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
    public void SetPlayablePieces(IEnumerable<
    (Piece piece,
    bool canMatchLeft,
    bool canMatchRight,
    bool reverseLeft,
    bool reverseRight)> playables)
    {
        foreach (var selector in _pieces)
        {
            var piece = selector.GetComponent<DominoPiece>().PieceData;

            var query = playables.Where(p => p.piece == piece);

            if (query.Any())
            {
                var p = query.First();

                selector.MarkPlayable(p.canMatchLeft, p.canMatchRight);
            }
            else
                selector.MarkUnplayable();
        }
    }

    private void AddPieces(IEnumerable<Piece> pcs, bool rearrange = true)
    {
        foreach (var piece in pcs)
            AddPiece(piece, false);

        if (rearrange)
            RearrangePieces();
    }
    public void SetPieces(IEnumerable<Piece> pcs, bool rearrange = true)
    {
        ClearPieces();
        AddPieces(pcs, rearrange);
    }

    private void ClearPieces()
    {
        foreach (var piece in _pieces)
            Destroy(piece.gameObject);

        _pieces.Clear();
    }
    private void AddPiece(Piece piece, bool rearrange = true)
    {
        var pSelector = Instantiate(piecePrefab, transform).GetComponent<PieceSelector>();
        pSelector.GetComponent<DominoPiece>().Initialize(piece, _painter);

        pSelector.Initialize(this);

        _pieces.Add(pSelector);

        if (rearrange)
            RearrangePieces();
    }
    public void RemovePiece(Piece piece, bool rearrange = true)
    {
        int index = _pieces.FindIndex(s => s.GetComponent<DominoPiece>().PieceData.Equals(piece));
        if (index < 0) return;

        Destroy(_pieces[index].gameObject);
        _pieces.RemoveAt(index);

        if (rearrange)
            RearrangePieces();
    }

    private void RearrangePieces()
    {
        for (var i = 0; i < _pieces.Count; i++)
            _pieces[i].SetTargetPosition(new Vector2(i * (PieceWidth + pieceSpacing) - (TotalWidth / 2f) + (PieceWidth / 2f), 0));

        GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, TotalWidth);
    }

    internal void SelectPiece(PieceSelector pieceSelector)
    {
        OnPieceSelected?.Invoke(pieceSelector);
    }
}
