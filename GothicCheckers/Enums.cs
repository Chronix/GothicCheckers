using System;

namespace GothicCheckers
{
    public enum PlayerControlType
    {
        Human,
        Computer
    }

    public enum PlayerColor
    {
        None,
        Black,
        White
    }

    public enum FieldColor
    {
        Light,
        Dark
    }

    public enum PieceType
    {
        None,
        Normal,
        King
    }

    public enum GameType
    {
        DoubleHuman,
        HumanAI,
        DoubleAI
    }

    public enum GameDifficulty
    {
        Easy = 1,
        Normal = 3,
        Hard = 5
    }

    public enum ValidationResult
    {
        Valid,
        Invalid,
        Enforced
    }

    public enum GameReplayState
    {
        NotAvailable,
        Stopped,
        Paused,
        Playing
    }
}
