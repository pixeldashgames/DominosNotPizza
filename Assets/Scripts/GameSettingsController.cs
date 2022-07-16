using System.Collections.Generic;
using UnityEngine;

public class GameSettingsController : MonoBehaviour
{
    private static GameSettingsController _instance;
    public static GameSettingsController Instance
    {
        get
        {
            if (_instance != null)
                return _instance;
            
            _instance = new GameObject("GameSettingsController")
            {
                tag = "GameSettingsController"
            }.AddComponent<GameSettingsController>();

            return _instance;
        }
        private set => _instance = value;
    }
    public enum PieceMatchRule
    {
        SameValue,
        OddsOrEvens
    }

    public enum Painter
    {
        Dots,
        Numbers
    }
    public enum WinMode
    {
        Classic,
    }
    public enum DrawWinMode
    {
        MinPoints,
        MaxPoints
    }
    public enum ScoreMode
    {
        Classic,
        Difference
    }
    
    public class GameSetting
    {
        public readonly GameSettingType type;
        public readonly string name;
        public readonly System.Type enumType;

        public GameSetting(GameSettingType type, string name, System.Type enumType = null)
        {
            this.type = type;
            this.name = name;
            this.enumType = enumType;
        }
    }

    public enum GameSettingType
    {
        Dropbox,
        Boolean,
        Integer,
        Float,
        String
    }

    private List<GameSetting> _pieceSettings;
    public List<GameSetting> PieceSettings =>
        _pieceSettings ??= new List<GameSetting>
        {
            new(type: GameSettingType.Integer, name: "Pieces Per Hand"),
            new(type: GameSettingType.Integer, name: "Max Piece Value"),
            new(type: GameSettingType.Dropbox, name: "Painter", enumType:typeof(Painter))
        };

    private List<GameSetting> _playerSettings;
    public List<GameSetting> PlayerSettings =>
        _playerSettings ??= new List<GameSetting>
        {
            new(type: GameSettingType.Integer, name: "AI Players"),
            new(type: GameSettingType.Integer, name: "Max Players"),
            new(type: GameSettingType.Integer, name: "Players Per Team"),
            new(type: GameSettingType.Boolean, name: "Even Teams")
        };

    private List<GameSetting> _rulesSettings;
    public List<GameSetting> RulesSettings =>
        _rulesSettings ??= new List<GameSetting>
        {
            new(type: GameSettingType.Dropbox, name: "Match Rule", enumType: typeof(PieceMatchRule)),
            new(type: GameSettingType.Dropbox, name: "Win Rule", enumType: typeof(WinMode)),
            new(type: GameSettingType.Dropbox, name: "Draw Rule", enumType: typeof(DrawWinMode)),
            new(type: GameSettingType.Dropbox, name: "Score Rule", enumType: typeof(ScoreMode))
        };

    [SerializeField] private GenericPainter dotPainter;
    [SerializeField] private GenericPainter numberPainter;

    public PieceMatchRule matchRule;
    public WinMode winRule;
    public DrawWinMode drawRule;
    public ScoreMode scoreRule;
    public Painter painterMode;
    
    public int piecesPerHand = 10;
    public int maxPieceValue = 9;
    public int maxPlayers = 2;
    public int minPlayers;
    public int aiPlayers = 1;
    public int playersPerTeam = 2;
    public bool evenTeams = true;
    
    private void Awake()
    {
        var settingsControllers = GameObject.FindGameObjectsWithTag("GameSettingsController");
        if (settingsControllers.Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
    }

    public IPiecePainter CreatePainter(Painter mode)
        => mode switch
        {
            Painter.Dots => dotPainter,
            Painter.Numbers => numberPainter,
            _ => null
        };
    public void ChangeSetting(GameSetting setting, object value)
    {
        switch (setting.name)
        {
            case "Players Per Team":
                playersPerTeam = int.Parse((string)value);
                break;
            case "Even Teams":
                evenTeams = (bool)value;
                break;
            case "Match Rule":
                matchRule = (PieceMatchRule)value;
                break;
            case "Win Rule":
                winRule = (WinMode)value;
                break;
            case "Draw Rule":
                drawRule = (DrawWinMode)value;
                break;
            case "Score Rule":
                scoreRule = (ScoreMode)value;
                break;
            case "Painter":
                painterMode = (Painter)value;
                break;
            case "Pieces Per Hand":
                piecesPerHand = int.Parse((string)value);
                break;
            case "Max Piece Value":
                maxPieceValue = int.Parse((string)value);
                break;
            case "Max Players":
                maxPlayers = int.Parse((string)value);
                break;
            case "Min Players":
                minPlayers = int.Parse((string)value);
                break;
            case "AI Players":
                aiPlayers = int.Parse((string)value);
                break;
        }
    }

    public object GetSettingValue(string settingName) =>
        settingName switch
        {
            "Even Teams" => evenTeams,
            "Players Per Team" => playersPerTeam,
            "Match Rule" => matchRule,
            "Win Rule" => winRule,
            "Draw Rule" => drawRule,
            "Score Rule" => scoreRule,
            "Pieces Per Hand" => piecesPerHand,
            "Max Piece Value" => maxPieceValue,
            "Max Players" => maxPlayers,
            "Min Players" => minPlayers,
            "AI Players" => aiPlayers,
            "Painter" => painterMode,
            _ => null
        };
}
