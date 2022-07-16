using System.Collections.Generic;
using UnityEngine;
using DominoPlayer;
using System.Linq;

public class RulesController : MonoBehaviour, IRules
{
    public GameSettingsController Settings => GameSettingsController.Instance;
    
    public int PiecesPerHand => Settings.piecesPerHand;

    public int MaxPieceValue => Settings.maxPieceValue;

    public int MaxPlayers => Settings.maxPlayers;

    public int MinPlayers => Settings.minPlayers;

    public bool CanPiecesMatch(Piece a, Piece b, bool leftToRight)
    => Settings.matchRule switch
    {
        GameSettingsController.PieceMatchRule.SameValue => leftToRight ? a.Right == b.Left : a.Left == b.Right,
        GameSettingsController.PieceMatchRule.OddsOrEvens => leftToRight ? a.Right % 2 == b.Left % 2 : a.Left % 2 == b.Right % 2,
        _ => true,
    };

    public bool GameOverCondition(DominoGame game, out int winner)
    {
        var players = game.Players;

        switch (Settings.winRule)
        {
            default:
            case GameSettingsController.WinMode.Classic:
                int tempWinner = 0;
                if (players.Any(p =>
                {
                    if (p.PiecesInHand == 0)
                    {
                        tempWinner = p.PlayerID;
                        return true;
                    }
                    return false;
                }))
                {
                    winner = tempWinner;
                    return true;
                }
                break;
        }

        var history = game.history;
        int passCount = 0;
        for (int i = history.Count - 1; i >= 0; i--)
        {
            if (history[i].passed == false)
                break;

            passCount++;
        }

        if (passCount >= game.Players.Count)
        {
            switch (Settings.drawRule)
            {
                case GameSettingsController.DrawWinMode.MaxPoints:
                    var max = players.OrderByDescending(p => p.GetPlayerScore()).First();
                    winner = max.PlayerID;
                    return true;
                case GameSettingsController.DrawWinMode.MinPoints:
                    var min = players.OrderBy(p => p.GetPlayerScore()).First();
                    winner = min.PlayerID;
                    return true;
            }
        }

        winner = -1;
        return false;
    }

    public double GetHandScore(DominoGame game, IEnumerable<Piece> hand)
    => hand.Sum(p => Settings.scoreRule switch
    {
        GameSettingsController.ScoreMode.Difference => Mathf.Abs(p.Left - p.Right),
        _ or GameSettingsController.ScoreMode.Classic => p.Left + p.Right
    });
}
