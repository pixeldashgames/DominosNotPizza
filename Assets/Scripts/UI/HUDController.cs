using System;
using System.Collections;
using System.Linq;
using DominoPlayer;
using TMPro;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    [SerializeField] private DominoGameController gameController;
    [SerializeField] private TMP_Text currentPlayerInfo;
    [SerializeField] private TMP_Text gameTimeInfo;
    [SerializeField] private TMP_Text lastMoveLabel;
    [SerializeField] private TMP_Text gameStartedText;
    [SerializeField] private GameObject gameStartedAnnounceParent;
    
    private bool _gameStarted;
    private DateTime _startGameTime;
    private void Awake()
    {
        gameController.OnGameStarted += OnGameStarted;
        gameController.OnTurnPassed += OnTurnPassed;
        gameController.OnMoveMade += OnMoveMade;
    }

    private string GetPlayerLabel(int player)
        => $"Player-{player}{(gameController.players[player].isAI? " (AI)" : "")}";
    
    private void OnMoveMade(Move move)
    {
        var playerLabel = GetPlayerLabel(move.playerID);
        if (move.passed)
        {
            lastMoveLabel.text = $"{playerLabel} passed...";
            return;
        }

        lastMoveLabel.text =
            $"{playerLabel} played {move.piecePlaced.Left}-{move.piecePlaced.Right}" +
            $" on the {(move.placedOnRight ? "right" : "left")}";
    }

    private void OnGameStarted(DominoGame obj)
    {
        OnTurnPassed(obj.CurrentPlayer);
        _startGameTime = DateTime.Now;
        _gameStarted = true;

        StartCoroutine(ShowGameStartAnnouncement(3f));
    }
    private void OnTurnPassed(int currentPlayer)
    {
        var isAI = gameController.players[currentPlayer].isAI;
        currentPlayerInfo.text = $"{GetPlayerLabel(currentPlayer)}'s Turn...";
    }

    private void Update()
    {
        if (!_gameStarted)
            return;

        var span = (DateTime.Now - _startGameTime);
        var hours = span.Hours;
        var minutes = span.Minutes;
        var seconds = span.Seconds;
        gameTimeInfo.text = $"{(hours != 0 ? $"{hours:D2}:" : "")}{minutes:D2}:{seconds:D2}";
    }

    private IEnumerator ShowGameStartAnnouncement(float duration)
    {
        gameStartedAnnounceParent.SetActive(true);
        gameStartedText.text = string.Join("\nvs.\n", gameController.teams.Select(t =>
            string.Join(", ", t.Select(GetPlayerLabel))));

        yield return new WaitForSeconds(duration);
        
        gameStartedAnnounceParent.SetActive(false);
    }
}
