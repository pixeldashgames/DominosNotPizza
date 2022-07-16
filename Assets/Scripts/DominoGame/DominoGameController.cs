using UnityEngine;
using DominoPlayer;
using System;
using System.Collections.Generic;
using System.Linq;

public class DominoGameController : MonoBehaviour
{
    [SerializeField] private RulesController rules;
    [SerializeField] private GameObject humanPlayerPrefab;
    [SerializeField] private GameObject aiPlayerPrefab;
    [SerializeField] private Transform playersParent;
    
    [HideInInspector]
    public DominoPlayerController[] players;
    private DominoGame _game;

    [HideInInspector] public int currentPlayer;
    public int[][] teams;
    
    public event Action<DominoGame> OnGameStarted;
    public event Action<Move> OnMoveMade;
    public event Action<int> OnGameOver;

    /// <summary>
    /// Event called when a turn passes, the argument is the PlayerID
    /// </summary>
    public event Action<int> OnTurnPassed;

    void Start()
    {
        StartGame();
    }

    public DominoGame GetGame() => _game;
    void StartGame()
    {
        _game = new DominoGame(rules);
        var playerCount = GameSettingsController.Instance.maxPlayers;
        
        var playerIDs = new List<int>();
        for (var i = 0; i < playerCount; i++)
            playerIDs.Add(i);

        var playersPerTeam = GameSettingsController.Instance.playersPerTeam;
        var evenTeams = GameSettingsController.Instance.evenTeams;
        var areTeamsEven = !evenTeams || playerCount % playersPerTeam == 0;
        var teamsList = new List<List<int>>();
        
        if (areTeamsEven)
        {
            var teamIds = new List<int>(playerIDs);
            while (true)
            {
                List<int> team = new();
                for (var i = 0; i < playersPerTeam; i++)
                {
                    if (teamIds.Count == 0)
                        break;
                    
                    var random = teamIds[UnityEngine.Random.Range(0, teamIds.Count)];
                    team.Add(random);
                    teamIds.Remove(random);
                }
                teamsList.Add(team);
                if (teamIds.Count == 0)
                    break;
            }
        }
        else
        {
            for (var i = 0; i < playerCount; i++)
            {
                teamsList.Add(new List<int> { i });
            }
        }

        this.teams = teamsList.Select(t => t.ToArray()).ToArray();
        
        players = new DominoPlayerController[rules.MaxPlayers];
        for (var i = 0; i < playerCount; i++)
        {
            var playerID = playerIDs[i];
            var prefab = i < rules.MaxPlayers - GameSettingsController.Instance.aiPlayers ? humanPlayerPrefab: aiPlayerPrefab;
            var playerController = Instantiate(prefab, playersParent).GetComponent<DominoPlayerController>();

            var partners = teams.First(t => t.Contains(playerID)).Where(p => p != playerID).ToArray();
            var opponents = teams.Where(t => !t.Contains(playerID)).SelectMany(l => l).ToArray();
            playerController.Initialize(playerID, partners, opponents, _game, this);
            
            players[i] = playerController;
        }
        _game.OnMoveMade += GameMoveMade;

        _game.StartGame(players.Select(p => p.Player).ToArray());
        foreach (var player in players)
        {
            player.SetUpObserver(_game);
        }
        currentPlayer = _game.CurrentPlayer;

        OnGameStarted?.Invoke(_game);

        players[_game.CurrentPlayer].ResolveTurn();
    }

    private void GameMoveMade(Move obj)
    {
        OnMoveMade?.Invoke(obj);
    }

    public void PassTurn()
    {
        currentPlayer = _game.CurrentPlayer;
        _game.NextTurn();
        if (_game.IsGameOver(out var winner))
        {
            OnGameOver?.Invoke(winner);
            return;
        }
        OnTurnPassed?.Invoke(_game.CurrentPlayer);
    }
}
