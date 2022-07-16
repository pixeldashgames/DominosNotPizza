using System;
using UnityEngine;
using DominoPlayer;

public class DominoPiece : MonoBehaviour
{
    public Transform leftPlaceholder;
    public Transform rightPlaceholder;

    public Transform leftExtreme;
    public Transform rightExtreme;

    public Transform centerLeftExtreme;
    public Transform centerRightExtreme;

    public bool uiPiece;
    
    public Piece PieceData;
    private IPiecePainter _painter;
    public void Initialize(Piece pieceData, IPiecePainter painter)
    {
        this.PieceData = pieceData;
        this._painter = painter;
        Repaint();
    }

    private void Repaint()
    {
        // Left value
        _painter.PaintValue(PieceData.Left, leftPlaceholder, uiPiece);
        _painter.PaintValue(PieceData.Right, rightPlaceholder, uiPiece);
    }
}
