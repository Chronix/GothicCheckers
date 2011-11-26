using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace GothicCheckers
{
    public static class SaveLoadManager
    {
        public const string SaveFileVersion = "1.2";

        public static void SaveGame(string filePath, GameManager manager)
        {
            using (XmlTextWriter w = new XmlTextWriter(filePath, Encoding.UTF8))
            {
                w.Formatting = Formatting.Indented;

                w.WriteStartDocument();
                w.WriteStartElement("GothicCheckers");
                w.WriteAttributeString("Version", GameManager.Version);
                w.WriteAttributeString("FormatVersion", SaveFileVersion);

                w.WriteStartElement("GameSettings");

                w.WriteStartElement("Difficulty");
                w.WriteAttributeString("White", GameManager.WhiteDifficulty.ToString());
                w.WriteAttributeString("Black", GameManager.BlackDifficulty.ToString());
                w.WriteEndElement(); // Difficulty

                w.WriteStartElement("Control");
                w.WriteAttributeString("White", manager.WhiteControl.ToString());
                w.WriteAttributeString("Black", manager.BlackControl.ToString());
                w.WriteEndElement(); // Control

                w.WriteEndElement(); // GameSettings

                w.WriteStartElement("Moves");

                foreach (IMove move in manager.History.Skip(1))
                {
                    w.WriteStartElement("Move");
                    w.WriteAttributeString("Player", move.Player.ToString());

                    w.WriteAttributeString("From", move.FromField.Representation);
                    w.WriteAttributeString("To", move.ToField.Representation);

                    if (move is CompoundMove) w.WriteAttributeString("Through", ((CompoundMove)move).GetMidFieldsSaveString());

                    w.WriteAttributeString("KingMove", move.KingMove.ToString());

                    if (move is SimpleMove && move.Capture != null)
                    {
                        w.WriteStartElement("Capture");
                        w.WriteAttributeString("Position", move.Capture.Position.Representation);
                        w.WriteAttributeString("Piece", move.Capture.Piece.ToString());
                        w.WriteEndElement(); // Capture
                    }
                    else if (move is CompoundMove)
                    {
                        List<GameField> modFields = new List<GameField>(((CompoundMove)move).Moves.Select(sm => sm.Capture));

                        foreach (GameField field in modFields)
                        {
                            w.WriteStartElement("Capture");
                            w.WriteAttributeString("Position", field.Position.Representation);
                            w.WriteAttributeString("Piece", field.Piece.ToString());
                            w.WriteEndElement(); // Capture
                        }
                    }

                    w.WriteEndElement(); // Move
                }

                w.WriteEndDocument();
            }
        }

        public static void LoadGame(string filePath, ref GameManager manager)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(filePath);

            try
            {
                XmlNode root = xDoc.SelectSingleNode("GothicCheckers");
                XmlAttribute formatVersionAttr = root.Attributes["FormatVersion"];

                ExceptionProvider.ThrowIf<FormatException>(formatVersionAttr == null || formatVersionAttr.Value != SaveFileVersion);
            }
            finally
            {
                XmlNode diffNode = xDoc.SelectSingleNode("//GameSettings/Difficulty");
                XmlNode ctrlNode = xDoc.SelectSingleNode("//GameSettings/Control");

                XmlNodeList moveNodes = xDoc.SelectNodes("//Moves/Move");

                GameManager.WhiteDifficulty = (GameDifficulty)Enum.Parse(typeof(GameDifficulty), diffNode.Attributes["White"].Value);
                GameManager.BlackDifficulty = (GameDifficulty)Enum.Parse(typeof(GameDifficulty), diffNode.Attributes["Black"].Value);

                manager.WhiteControl = (PlayerControlType)Enum.Parse(typeof(PlayerControlType), ctrlNode.Attributes["White"].Value);
                manager.BlackControl = (PlayerControlType)Enum.Parse(typeof(PlayerControlType), ctrlNode.Attributes["Black"].Value);

                foreach (XmlNode moveNode in moveNodes)
                {
                    PlayerColor player = (PlayerColor)Enum.Parse(typeof(PlayerColor), moveNode.Attributes["Player"].Value);
                    string from = moveNode.Attributes["From"].Value;
                    string to = moveNode.Attributes["To"].Value;
                    string through = moveNode.Attributes["Through"] == null ? string.Empty : moveNode.Attributes["Through"].Value;

                    IMove move = null;

                    if (string.IsNullOrEmpty(through))
                    {
                        move = new SimpleMove(player, new BoardPosition(from), new BoardPosition(to), bool.Parse(moveNode.Attributes["KingMove"].Value), false);
                    }
                    else
                    {
                        move = CompoundMove.FromSaveData(player, from, to, through, bool.Parse(moveNode.Attributes["KingMove"].Value));
                    }

                    if (moveNode.HasChildNodes)
                    {
                        move.IsCapture = true;

                        if (move is SimpleMove)
                        {
                            move.Capture = new GameField
                            {
                                Occupation = player == PlayerColor.Black ? PlayerColor.White : PlayerColor.Black,
                                Position = new BoardPosition(moveNode.FirstChild.Attributes["Position"].Value),
                                Piece = (PieceType)Enum.Parse(typeof(PieceType), moveNode.FirstChild.Attributes["Piece"].Value)
                            };
                        }
                        else
                        {
                            CompoundMove cMove = move as CompoundMove;

                            for (int i = 0; i < cMove.Length; ++i)
                            {
                                cMove.Moves[i].IsCapture = true;
                                cMove.Moves[i].Capture = new GameField
                                {
                                    Occupation = player == PlayerColor.Black ? PlayerColor.White : PlayerColor.Black,
                                    Position = new BoardPosition(moveNode.ChildNodes[i].Attributes["Position"].Value),
                                    Piece = (PieceType)Enum.Parse(typeof(PieceType), moveNode.ChildNodes[i].Attributes["Piece"].Value)
                                };
                            }
                        }
                    }

                    manager.History.Add(new GameHistoryItem(move));
                }
            }
        }
    }
}
