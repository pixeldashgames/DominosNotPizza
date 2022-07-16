using System.Collections;
using UnityEngine;
using DominoPlayer.AI;
using System;
using DominoPlayer;

public class DominoPlayerController : MonoBehaviour
{
    private static readonly Vector2 AIThinkingTimeRange = new(1f, 3f);

    private enum AIType
    {
        BotaGorda,
        Smart,
        Random
    }
    private DominoGameController _gameController;
    public bool isAI;
    public PlayerUIController humanUI;

    public DominoPlayer.DominoPlayer Player;
    private Observer _observer;
    public void Initialize(int playerID, int[] partners, int[] opponents, DominoPlayer.DominoGame game, DominoGameController gameController)
    {
        _gameController = gameController;
        if (isAI)
        {
            var aiType = (AIType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(AIType)).Length);
            Player = aiType switch
            {
                AIType.BotaGorda => new BotaGordaAI(playerID, game),
                AIType.Smart => new SmartAI(playerID, game),
                _ or AIType.Random => new RandomAI(playerID, game)
            };
        }
        else
        {
            Player = new HumanPlayer(playerID, game);
            ((HumanPlayer)Player).OnMoveMade += HumanMoveMade;
            humanUI.Initialize(gameController.GetGame());
        }
        Player.SetPartners(partners);
        Player.SetOpponents(opponents);
        
        gameController.OnTurnPassed += i =>
        {
            if (i == playerID)
                ResolveTurn();
        };
    }

    public void SetUpObserver(DominoGame game)
    {
        _observer = new Observer(Player.PlayerID, game);
    }
    private void HumanMoveMade()
    {
        var human = Player as HumanPlayer;
        var moveMade = human!.GetSelectedMove();
        if(!moveMade.passed)
        {
            var score = (float)_observer.GetValue(human.Pieces, moveMade.piecePlaced);
            humanUI.LogObserverScore(score);
        }
        
        humanUI.StopUI();
        _gameController.PassTurn();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void ResolveTurn()
    {
        if (isAI)
        {
            StartCoroutine(WaitForSeconds(2, _gameController.PassTurn));
        }
        else
            humanUI.StartUI();
    }

    IEnumerator WaitForSeconds(float seconds, Action onWaited)
    {
        yield return new WaitForSeconds(seconds);
        onWaited();
    }
}
