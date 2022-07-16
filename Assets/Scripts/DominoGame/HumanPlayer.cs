using System.Collections.Generic;
using DominoPlayer;

public class HumanPlayer : DominoPlayer.DominoPlayer
{
    public HumanPlayer(int playerID, DominoGame game) : base(playerID, game)
    { }

    public event System.Action OnMoveMade;

    Move _selectedMove;
    public List<Piece> Pieces => Hand;
    public Move GetSelectedMove() => _selectedMove;
    public void SelectMove(Piece piece, bool right, bool reverse)
    {
        if (reverse)
            piece.Reverse();

        _selectedMove = Move.CreateMove(PlayerID, piece, right);
        OnMoveMade?.Invoke();
    }
    public void SelectPass()
    {
        _selectedMove = Move.CreatePass(PlayerID);
        OnMoveMade?.Invoke();
    }

    protected override Move InternalGetMove()
    => _selectedMove;
}
