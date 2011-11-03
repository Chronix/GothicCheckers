using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace GothicCheckers
{
    public static class SaveLoadManager
    {
        public static void SaveGame(string filePath, GameManager manager)
        {
            using (XmlTextWriter w = new XmlTextWriter(filePath, Encoding.UTF8))
            {
                w.Formatting = Formatting.Indented;

                w.WriteStartDocument();
                w.WriteStartElement("GothicCheckers");
                w.WriteAttributeString("Version", GameManager.Version);

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

                foreach (IMove move in manager.History)
                {
                    w.WriteStartElement("Move");
                    w.WriteAttributeString("Player", move.Player.ToString());

                    if (move is CompoundMove) w.WriteAttributeString("Through", ((CompoundMove)move).GetMidFieldsSaveString());

                    w.WriteAttributeString("From", move.FromField.Representation);
                    w.WriteAttributeString("To", move.ToField.Representation);

                    if (move.ModifiedField != null)
                    {
                        w.WriteStartElement("ModField");
                        w.WriteAttributeString("Position", move.ModifiedField.Position.Representation);
                        w.WriteAttributeString("Piece", move.ModifiedField.Piece.ToString());
                        w.WriteEndElement(); // ModField
                    }

                    w.WriteEndElement(); // Move
                }

                w.WriteEndDocument();
            }
        }

        public static void LoadGame(string filePath, GameManager manager)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(filePath);

            XmlNode diffNode = xDoc.SelectSingleNode("GameSettings/Difficulty");
            XmlNode ctrlNode = xDoc.SelectSingleNode("GameSettings/Control");

            XmlNodeList moveNodes = xDoc.SelectNodes("Moves/Turn");

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
                    move = new SimpleMove(player, new BoardPosition(from), new BoardPosition(to));
                }
                else
                {
                    move = CompoundMove.FromSaveData(player, from, to, through);
                }

                if (moveNode.HasChildNodes)
                {
                    move.ModifiedField = new GameField
                    {
                        Occupation = player == PlayerColor.Black ? PlayerColor.White : PlayerColor.Black,
                        Position = new BoardPosition(moveNode.FirstChild.Attributes["Position"].Value),
                        Piece = (PieceType)Enum.Parse(typeof(PieceType),moveNode.FirstChild.Attributes["Piece"].Value)
                    };
                }

                manager.History.Add(move);
            }
        }
    }
}
