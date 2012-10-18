using System;

using Woodpecker.Core;
using Woodpecker.Storage;

namespace Woodpecker.Net
{
    /// <summary>
    /// Contains functions for both connection managers. This class is abstract.
    /// </summary>
    public abstract class connectionManager
    {
        #region Methods
        /// <summary>
        /// Adds a given IP address to the connection blacklist, thus refusing future connections (both game and MUS) from that IP address.
        /// </summary>
        /// <param name="IP">The IP address to add to the blacklist.</param>
        public void blackListIpAddress(string IP)
        {
            Database Database = new Database(false, true);
            Database.addParameterWithValue("ip", IP);
            Database.Open();
            if (Database.Ready)
            {
                Database.runQuery("INSERT INTO connections_blacklist(ip,added) VALUES (@ip, CURDATE())");
                Logging.Log("Blacklisted IP address '" + IP + "' for whatever reason.", Logging.logType.connectionBlacklistEvent);
            }
            else
                Logging.Log("Failed to add IP address '" + IP + "' to the connection blacklist, the database was not contactable.", Logging.logType.commonError);
        }
        /// <summary>
        /// Returns a boolean that indicates if a given IP address is present in the 'connections_blacklist' table of the database.
        /// </summary>
        /// <param name="IP">The IP address to check.</param>
        public bool ipIsBlacklisted(string IP)
        {
            Database Database = new Database(false, true);
            Database.addParameterWithValue("ip", IP);
            Database.Open();
            if (Database.Ready)
                return Database.findsResult("SELECT ip FROM connections_blacklist WHERE ip = @ip");
            else
                return false;
        }
        /// <summary>
        /// Empties the 'connections_blacklist' table in the database.
        /// </summary>
        public void clearBlacklist()
        {
            Database Database = new Database(true, true);
            if (Database.Ready)
                Database.runQuery("DELETE FROM connections_blacklist");
        }
        #endregion
    }
}
