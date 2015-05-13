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
    /// <summary>
    /// Klasa głównego kontekstu (zarządcy programem).
    /// </summary>
    public class Context
    {
        private static string playerXmlFileName = "DiggerPlayers.xml";

        private IXnaUseable _currentView;
        private ContentManager _content;
        private bool _readyToExit;
        private Player _player;
        private List<Player> _players;
        
        /// <summary>
        /// Konstruktor.
        /// Przytworzeniu obiektu tej klasy dochodzi do automatycznego wczytania zapisanych graczy.
        /// </summary>
        /// <param name="content">Obiekt zarządcy zawartością peryferyjną</param>
        public Context(ContentManager content)
        {
            _content = content;
            _readyToExit = false;
            _players = new List<Player>();

            // Wczytaj graczy z pliku 
            LoadPlayersFromSerializedFile();
        }

        /// <summary>
        /// Jeżeli domyślny plik zapisu istnieje dokonuje wczytania zapisanej zawartości.
        /// W przypadku zaistnienia błędów informację taką umieszcza w pliku logu.
        /// </summary>
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

            Logger.Report("Poprawnie wczytano plik z zapisanymi graczami");
        }

        /// <summary>
        /// Metoda zapisująca bieżącą konfigurację użytkowników do pliku.
        /// W przypadku zaistnienia błędów informację taką umieszcza w pliku logu.
        /// </summary>
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

            Logger.Report("Zapisano graczy do pliku.");
        }

        /// <summary>
        /// Metoda pozwalająca na utwożenie nowego gracza.
        /// </summary>
        /// <param name="playerName">Imię nowego gracza.</param>
        public void CreateNewPlayer(string playerName)
        {
            _player = new Player(playerName);
            _players.Add(_player);
            Logger.Report("Utworzono nowego gracza o imieniu: " + playerName);
        }

        /// <summary>
        /// Metoda logująca gracza.
        /// </summary>
        /// <param name="playerIndex">Numer gracza (id przydzielone na okres 1 działania gry).</param>
        public void LoadPlayerGame(int playerIndex)
        {
            _player = _players[playerIndex];
            Logger.Report("Wczytano rozgrywkę gracza o imieniu: " + _player.Name + ".");
        }

        #region Properites
        /// <summary>
        /// Obecnie wczytany obiekt widoku.
        /// </summary>
        public IXnaUseable CurrentView
        {
            get { return _currentView; }
            set { _currentView = value; }
        }

        /// <summary>
        /// Zmienna mówiąca, czy gra jest gotowa do wyłączenia.
        /// </summary>
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

        /// <summary>
        /// Obiekt zarządcy zwartością peryferyjną (zasobami).
        /// </summary>
        public ContentManager Content
        {
            get { return _content; }
        }

        /// <summary>
        /// Obiekt aktualnego gracza.
        /// </summary>
        public Player Player
        {
            get { return _player; }
        }

        /// <summary>
        /// Lista wczytanych graczy.
        /// </summary>
        public List<Player> Players
        {
            get { return _players; }
        }
        #endregion
    }
}