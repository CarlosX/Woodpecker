using System;
using System.Text;
using System.Threading;
using System.Net.Sockets;

using Woodpecker.Core;
using Woodpecker.Storage;
using Woodpecker.Specialized;
using Woodpecker.Security.Cryptography;

using Woodpecker.Net.Game;
using Woodpecker.Sessions;

using Woodpecker.Game.Externals;
using Woodpecker.Game.Users;
using Woodpecker.Game.Users.Roles;
using Woodpecker.Game.Store;
using Woodpecker.Game.Messenger;
using Woodpecker.Game.Moderation;
using Woodpecker.Game.Rooms;
using Woodpecker.Game.Items;
using Woodpecker.Game.Arcade;

namespace Woodpecker
{
    /// <summary>
    /// The 'hearth' of Woodpecker, containing references to all kinds of objects.
    /// </summary>
    public static class ObjectTree
    {
        #region Fields
        private static bool _gotLicense = false;

        private static mainForm _mainForm;
        private static sessionManager _sessionManager = new sessionManager();
        private static gameConnectionManager _gameConnectionManager = new gameConnectionManager();
        private static externalManager _externalManager = new externalManager();
        private static userManager _userManager = new userManager();
        private static roleManager _roleManager = new roleManager();
        private static storeManager _storeManager = new storeManager();
        private static messengerManager _messengerManager = new messengerManager();
        private static moderationManager _moderationManager = new moderationManager();
        private static roomManager _roomManager = new roomManager();
        private static itemManager _itemManager = new itemManager();
        private static arcadeManager _arcadeManager = new arcadeManager();
        private static md5Provider _md5Provider = new md5Provider();
        #endregion

        #region Properties
        public static class Application
        {
            #region Properties
            /// <summary>
            /// Gets the license status of this copy of Woodpecker as a boolean.
            /// </summary>
            public static bool isLicensed
            {
                get { return _gotLicense; }
            }
            /// <summary>
            /// A reference to the main form with the user interface.
            /// </summary>
            public static mainForm GUI
            {
                get { return _mainForm; }
                set { _mainForm = value; }
            }
            #endregion

            #region Methods
            /// <summary>
            /// Starts the booting procedure of Woodpecker and handles exceptions.
            /// </summary>
            public static void Start()
            {
                Logging.setDefaultLogSessings();

                Logging.Log("Welcome to Woodpecker!");
                Logging.Log("Checking db.config...");
                if (!Configuration.configFileExists)
                {
                    Logging.Log("db.config was not found in the same directory as the executable of this copy of Woodpecker!");
                    Stop("db.config not found");
                    return;
                }

                Configuration.createConnectionString();
                Logging.Log("Running database connectivity check...");

                Database Check = new Database(true, true);
                if (Check.Ready)
                {
                    Check.runQuery("INSERT INTO server_startups(startup) VALUES (NOW())"); // Log server startup
                    Check = null;
                    Logging.Log("Database is contactable!");
                }
                else
                {
                    Logging.Log("Failed to contact database, please check the error shown above!", Logging.logType.commonWarning);
                    Stop("failed to contact database");
                    return;
                }

                Configuration.loadConfiguration();
                _md5Provider = new md5Provider();
                _md5Provider.baseSalt = Configuration.getConfigurationValue("cryptography.md5.salt");
                _externalManager.loadEntries();

                _roleManager.loadRoles();
                _messengerManager.setConfiguration();

                _moderationManager.logChat = (Configuration.getConfigurationValue("moderation.logchat") == "1");
                _itemManager.loadDefinitions();

                _storeManager.loadSales();
                _storeManager.loadCataloguePages();

                _roomManager.initRoomCategories();
                _roomManager.initRoomModels();

                int Port = Configuration.getNumericConfigurationValue("connections.game.port");
                if (Port > 0)
                {
                    int maxConnectionsPerIP = Configuration.getNumericConfigurationValue("connections.game.maxperip");
                    if (maxConnectionsPerIP == 0)
                    {
                        Logging.Log("The field connections.game.maxperip in the `configuration` table of the database is not holding a valid value. The default value '3' will be used.", Logging.logType.commonWarning);
                        maxConnectionsPerIP = 3;
                    }
                    if (!_gameConnectionManager.startListening(Port, maxConnectionsPerIP))
                    {
                        Logging.Log("Failed to setup game connection listener, the port (" + Port + ") is probably in use by another application.", Logging.logType.commonWarning);
                        Stop("failed to setup game connection listener");
                        return;
                    }
                }
                else
                {
                    Logging.Log("The field 'connections.game.port' in the `configuration` table of the database is not present. Please fill in a numeric entry, which represents the port to listen on for game connections.", Logging.logType.commonWarning);
                    Stop("No valid port for game connection listener set.");
                    return;
                }
                _mainForm.logSacredText(null);

                _sessionManager.startPingChecker();
                _sessionManager.startReceivedBytesChecker();
                Game.startUpdater();

                Logging.Log("Ready for connections!");
                _mainForm.logSacredText(null);
                _mainForm.stripMenu.Enabled = true;
            }
            /// <summary>
            /// Sets Woodpecker in the 'allowed to close window'-mode, after shutting down all other tasks etc. A message is being displayed with the reason of shutdown.
            /// </summary>
            /// <param name="Message">The reason of the shutdown.</param>
            public static void Stop(string Message)
            {
                Logging.Log("Shutting down, reason: " + Message);
                _gameConnectionManager.stopListening();
                _sessionManager.stopPingChecker();
                _sessionManager.stopMessageSizeChecker();
                _sessionManager.Clear();
                //_roomManager.saveInstances();
                Game.stopUpdater();

                _mainForm.stripMenu.Enabled = false;
                _mainForm.mShutdown = true;
                Logging.Log("Woodpecker safely shutdown! You can now close the application.");
            }
            #endregion
        }
        /// <summary>
        /// Provides various functions for enciphering and hashing data.
        /// </summary>
        public static class Security
        {
            /// <summary>
            /// Provides functions for hashing data.
            /// </summary>
            public static class Cryptography
            {
                public static md5Provider MD5
                {

                    get { return _md5Provider; }
                }
            }
            /// <summary>
            /// Provides functions for ciphering data and generating public keys. Also features a factory for creating RC4 classes.
            /// </summary>
            public static class Ciphering
            {
                /// <summary>
                /// Generates a total random public key with a random length for the RC4 ciphering, containing a-z and 0-9 and returns it as a string.
                /// </summary>
                public static string generateKey()
                {
                    int keyLength = new Random(DateTime.Now.Millisecond).Next(60, 65);
                    Random v = new Random(DateTime.Now.Millisecond + DateTime.Now.Second + keyLength);
                    StringBuilder sb = new StringBuilder(keyLength);

                    for (int i = 0; i < keyLength; i++)
                    {
                        int j = 0;
                        if (v.Next(0, 2) == 1)
                            j = v.Next(97, 123);
                        else
                            j = v.Next(48, 58);
                        sb.Append((char)j);
                    }

                    return sb.ToString();
                }
            }
        }
        /// <summary>
        /// Provides various functions regarding player sessions.
        /// </summary>
        public static sessionManager Sessions
        {
            get { return _sessionManager; }
        }

        /// <summary>
        /// Provides various functions for game and MUS connections.
        /// </summary>
        public static class Net
        {
            /// <summary>
            /// Provides various functions for game connections.
            /// </summary>
            public static gameConnectionManager Game
            {
                get { return _gameConnectionManager; }
            }
            /// <summary>
            /// Provides various functions for MUS connections.
            /// </summary>
            public static gameConnectionManager Mus
            {
                get { return null; }
            }
        }
        /// <summary>
        /// Provides management for all kinds of in-game related things.
        /// </summary>
        public static class Game
        {
            #region Properties
            private static Thread updateWorker;
            /// <summary>
            /// Provides management for cached external texts.
            /// </summary>
            public static externalManager Externals
            {
                get { return _externalManager; }
            }
            /// <summary>
            /// Provides management and functions for logged in users, user data etc.
            /// </summary>
            public static userManager Users
            {
                get { return _userManager; }
            }
            /// <summary>
            /// Provides management and functions for user roles, such as rights, badges etc.
            /// </summary>
            public static roleManager Roles
            {
                get { return _roleManager; }
            }
            /// <summary>
            /// Contains various functions and fields for the catalogue and it's items, as well as other financial things.
            /// </summary>
            public static storeManager Store
            {
                get { return _storeManager; }
            }
            /// <summary>
            /// Provides various functions for the in-game messenger ('Console') for sending messages to users, adding users to buddylist etc. Also features a postmaster.
            /// </summary>
            public static messengerManager Messenger
            {
                get { return _messengerManager; }
            }
            /// <summary>
            /// Provides all kinds of functions for moderation in the virtual hotel, eg, bans etc.
            /// </summary>
            public static moderationManager Moderation
            {
                get { return _moderationManager; }
            }
            /// <summary>
            /// Provides all kinds of functions for rooms, room categories and the navigator.
            /// </summary>
            public static roomManager Rooms
            {
                get { return _roomManager; }
            }
            public static itemManager Items
            {
                get { return _itemManager; }
            }
            public static arcadeManager Arcade
            {
                get { return _arcadeManager; }
            }
            #endregion

            #region Methods
            /// <summary>
            /// Attempts to start the game updater worker thread.
            /// </summary>
            public static void startUpdater()
            {
                if(updateWorker == null)
                {
                    updateWorker = new Thread(new ThreadStart(Updater));
                    updateWorker.Start();
                    Logging.Log("Game update worker started.");
                    Rooms.resetRoomUserAmounts();
                }
            }
            /// <summary>
            /// Stops the game updater worker thread.
            /// </summary>
            public static void stopUpdater()
            {
                if (updateWorker != null)
                {
                    updateWorker.Abort();
                    updateWorker = null;
                    Logging.Log("Game update worker stopped.");
                }
            }
            /// <summary>
            /// An infinite loop at a configured interval, updating various things in game.
            /// </summary>
            private static void Updater()
            {
                while (true)
                {
                    Moderation.dropExpiredBans();
                    Messenger.Postmaster.dropInvalidMessages();
                    Rooms.dropInvalidFavoriteRoomEntries();
                    Rooms.destroyInactiveRoomInstances();

                    GC.Collect(); // Force garbage collecting
                    Thread.Sleep(3 * 60000); // 3 minutes interval
                }
            }
            #endregion
        }
        #endregion
    }
}
