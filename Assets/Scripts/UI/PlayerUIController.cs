using System.Collections;
using System.Collections.Generic;
using DominoPlayer;
using Player = DominoPlayer.DominoPlayer;
using UnityEngine;
using System.Linq;
using TMPro;

public class PlayerUIController : MonoBehaviour
{
    [SerializeField] private GameObject playerUIParent;
    [SerializeField] private DominoPlayerController playerController;
    [SerializeField] private GameObject passTurnButton;
    [SerializeField] private GameObject leftSideSelector;
    [SerializeField] private GameObject rightSideSelector;
    [SerializeField] private PiecesContainerController container;
    [SerializeField] private TMP_Text observerResultText;
    
    DominoGame _game;
    HumanPlayer _player;
    HumanPlayer Player
    {
        get
        {
            if (_player == null)
            {
                _player = playerController.Player as HumanPlayer;
            }

            return _player;
        }
    }

    public void LogObserverScore(float score)
    {
        (string message, Color color) = score switch
        {
            > 0.95f => ("Great move!", Color.green),
            > 0.7f => ("Good move", new Color(0.73f, 1f, 0.34f)),
            > 0.5f => ("Not bad", Color.yellow),
            > 0.3f => ("Could be better...", new Color(1f, 0.61f, 0f)),
            _ => ("Bad move", Color.red)
        };

        StartCoroutine(LogObserverResult(message, color, 2f));
    }
    void Start()
    {
        container.OnPieceSelected += SelectPiece;
    }
    public void Initialize(DominoGame game)
    {
        this._game = game;
    }
    IEnumerable<(Piece piece, bool canMatchLeft, bool canMatchRight, bool reverseLeft, bool reverseRight)> _playables;
    public void StartUI()
    {
        playerUIParent.SetActive(true);

        container.SetPieces(Player.Pieces.ToArray());
        _playables = _game.GetPlayablePieces(Player.Pieces);

        var valueTuples = _playables as (Piece piece, bool canMatchLeft, bool canMatchRight, bool reverseLeft, bool reverseRight)[] ?? _playables.ToArray();
        if (!valueTuples.Any())
            passTurnButton.SetActive(true);

        container.SetPlayablePieces(valueTuples);
    }
    PieceSelector _selected;
    void SelectPiece(PieceSelector selector)
    {
        if (_selected != null)
        {
            _selected.Deselect();
        }
        
        _selected = selector;
        if (selector == null)
        {
            leftSideSelector.SetActive(false);
            rightSideSelector.SetActive(false);
            return;
        }
        
        selector.Select();

        leftSideSelector.SetActive(selector.playableLeft);
        rightSideSelector.SetActive(selector.playableRight);
    }
    public void SelectSide(bool right)
    {
        var pieceData = _selected.GetComponent<DominoPiece>().PieceData;

        var playable = _playables.First(p => p.piece == pieceData);
        Player.SelectMove(pieceData, right, right ? playable.reverseRight : playable.reverseLeft);
    }
    public void Pass()
    {
        Player.SelectPass();
    }
    void Cleanup()
    {
        leftSideSelector.SetActive(false);
        rightSideSelector.SetActive(false);
        _selected = null;
        passTurnButton.SetActive(false);
    }
    public void StopUI()
    {
        Cleanup();
        playerUIParent.SetActive(false);
    }

    private IEnumerator LogObserverResult(string message, Color color, float time)
    {
        observerResultText.text = message;
        observerResultText.color = color;
        yield return new WaitForSeconds(time);
        observerResultText.text = "";
    }
}
