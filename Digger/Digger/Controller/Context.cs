using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Digger.Data;
using Digger.Views;
using Digger.Views.Common;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Digger.Controller
{
    public class Context
    {
        private static string playerXmlFileName = "DiggerPlayers.xml";

        private IXnaUseable _currentView;
        private ContentManager _content;
        private bool _readyToExit;
        private Player _player;
        private List<Player> _players;
        
        public Context(ContentManager content)
        {
            _content = content;
            _readyToExit = false;
            _players = new List<Player>();

            // Wczytaj graczy z pliku 
            LoadPlayersFromSerializedFile();
        }

        public void LoadPlayersFromSerializedFile()
        {
            // Sprawdź, czy plik istnieje
            if (!File.Exists(playerXmlFileName)) return;

            try
            {
                using (FileStream fs = new FileStream(playerXmlFileName, FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof (List<Player>));
                    _players = (List<Player>) serializer.Deserialize(fs);
                }
            }
            catch (Exception e)
            {
                Logger.Report(e.Message);
                _players = new List<Player>();
            }
        }

        public void SavePlayersToSerializedFile()
        {
            if (_players.Count == 0) return;

            try
            {
                using (FileStream fs = new FileStream(playerXmlFileName, FileMode.Create))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<Player>));
                    serializer.Serialize(fs, _players);
                }
            }
            catch (Exception e)
            {
                Logger.Report(e.Message);
            }
        }

        public void CreateNewPlayer(string playerName)
        {
            _player = new Player(playerName);
            _players.Add(_player);
        }

        public void LoadPlayerGame(int playerIndex)
        {
            _player = _players[playerIndex];
        }

        public IXnaUseable CurrentView
        {
            get { return _currentView; }
            set { _currentView = value; }
        }

        public bool ReadyToExit
        {
            get { return _readyToExit; }
            set
            {
                _readyToExit = value;
                if (_readyToExit)
                {
                    // Rozpocznij procedurę zakończenia
                    SavePlayersToSerializedFile();
                }
            }
        }

        public ContentManager Content
        {
            get { return _content; }
        }

        public Player Player
        {
            get { return _player; }
        }

        public List<Player> Players
        {
            get { return _players; }
        }
    }
}