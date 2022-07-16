using UnityEngine;
using DominoPlayer;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class DominoUIController : MonoBehaviour
{
    [SerializeField] private Transform gameCenter;
    [SerializeField] private DominoGameController gameController;
    [SerializeField] private DominoPiece piecePrefab;
    [SerializeField] private float pieceSpacing;
    [SerializeField] private AudioSource clickAudio;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private TMP_Text gameOverLabel;

    private DefaultActions _input;
    private IPiecePainter _piecePainter;
    private Vector3 _leftExtreme;
    private Vector3 _rightExtreme;

    void Awake()
    {
        _input = new DefaultActions();
        _input.Enable();

        _input.Standard.Pause.performed += TogglePauseMenu;
        
        _piecePainter = GameSettingsController.Instance.CreatePainter(GameSettingsController.Instance.painterMode);

        gameController.OnMoveMade += OnMoveMade;
        gameController.OnGameOver += OnGameOver;
    }

    private void OnGameOver(int winner)
    {
        _input.Standard.Pause.performed -= TogglePauseMenu;
        pauseMenu.SetActive(false);
        
        // ReSharper disable once Unity.NoNullPropagation
        gameController.players[gameController.currentPlayer].humanUI?.StopUI();

        gameOverScreen.SetActive(true);
        gameOverLabel.text = $"{GetPlayerLabel(winner)} WON!";
    }

    private string GetPlayerLabel(int player)
        => $"Player-{player}{(gameController.players[player].isAI? " (AI)" : "")}";

    private void OnMoveMade(Move move)
    {
        if (move.passed)
            return;

        if (_leftExtreme == _rightExtreme)
        {
            PlacePieceOnCenter(move.piecePlaced);
            return;
        }

        if (move.placedOnRight)
            PlacePieceOnRight(move.piecePlaced);
        else
            PlacePieceOnLeft(move.piecePlaced);
        
        clickAudio.Play();
    }

    private void TogglePauseMenu(InputAction.CallbackContext _ = default)
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        clickAudio.Play();
    }

    public void ResumeGame()
    {
        TogglePauseMenu();
    }

    public void ExitGame()
    {
        SceneManager.LoadScene("MainMenu");
    }
    
    void PlacePieceOnCenter(Piece piece)
    {
        DominoPiece placed;
        if(piece.Left!=piece.Right)
        {
            placed = Instantiate(piecePrefab, Vector3.zero,
                Quaternion.Euler(0, 0, 90), gameCenter);
            _leftExtreme = placed.leftExtreme.position;
            _rightExtreme = placed.rightExtreme.position;
        }
        else
        {
            placed = Instantiate(piecePrefab, Vector3.zero,
                Quaternion.Euler(0, 0, 0), gameCenter);
            _leftExtreme = placed.centerLeftExtreme.position;
            _rightExtreme = placed.centerRightExtreme.position;
        }
        placed.Initialize(piece, _piecePainter);
    }
    void PlacePieceOnLeft(Piece piece)
    {
        Vector3 extremeVector;
        float zAngle;
        if(piece.Left != piece.Right)
        {
            zAngle = 90;
            extremeVector = piecePrefab.leftExtreme.localPosition;
        }
        else
        {
            zAngle = 0;
            extremeVector = piecePrefab.centerLeftExtreme.localPosition;
        }
        
        var placed = Instantiate(piecePrefab,
            _leftExtreme + (Vector3.left * extremeVector.magnitude) + (Vector3.left * pieceSpacing),
            Quaternion.Euler(0, 0, zAngle), gameCenter);

        _leftExtreme = piece.Left == piece.Right ? 
                placed.centerLeftExtreme.position : placed.leftExtreme.position;

        placed.Initialize(piece, _piecePainter);
    }
    void PlacePieceOnRight(Piece piece)
    {
        Vector3 extremeVector;
        float zAngle;
        if (piece.Left != piece.Right)
        {
            zAngle = 90;
            extremeVector = piecePrefab.rightExtreme.localPosition;
        }
        else
        {
            zAngle = 0;
            extremeVector = piecePrefab.centerRightExtreme.localPosition;
        }
        var placed = Instantiate(piecePrefab,
            _rightExtreme + (Vector3.right * extremeVector.magnitude) + (Vector3.right * pieceSpacing),
            Quaternion.Euler(0, 0, zAngle), gameCenter);
        _rightExtreme = piece.Left == piece.Right ?
                placed.centerRightExtreme.position : placed.rightExtreme.position;
        placed.Initialize(piece, _piecePainter);
    }
}
