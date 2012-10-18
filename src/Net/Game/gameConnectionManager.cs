using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

using Woodpecker.Core;
using Woodpecker.Storage;
using Woodpecker.Sessions;
using Woodpecker.Net.Game.Messages;

namespace Woodpecker.Net.Game
{
    /// <summary>
    /// Provides management for connected game clients.
    /// </summary>
    public class gameConnectionManager : connectionManager
    {
        #region Fields
        /// <summary>
        /// An integer which represents the max amount of simultaneous connections an IP address can have to the server.
        /// </summary>
        private int maxConnectionsPerIP;
        /// <summary>
        /// The System.Net.Sockets.Socket object that listens for incoming connections.
        /// </summary>
        private Socket Listener;
        #endregion

        #region Methods
        /// <summary>
        /// Attempts to start listening on a certain port. A boolean that indicates if the operation has succeeded is returned.
        /// </summary>
        /// <param name="Port">The number of the TCP port to listen on.</param>
        /// <param name="maxConnectionsPerIP">The max. amount of simultaneous connections that an IP address can have to the server.</param>
        public bool startListening(int Port, int maxConnectionsPerIP)
        {
            try
            {
                this.Listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.Listener.Bind(new IPEndPoint(IPAddress.Any, Port));
                this.Listener.Listen(4);

                this.maxConnectionsPerIP = maxConnectionsPerIP;
                Listener.BeginAccept(new AsyncCallback(connectionRequest), Listener);
                Logging.Log("Game connection listener running on port " + Port + ", max connections per IP: " + maxConnectionsPerIP + ".");
                return true;
            }
            catch { return false; }
        }
        /// <summary>
        /// Stops listening and disposes all connections and resources.
        /// </summary>
        public void stopListening()
        {
            try
            {
                this.Listener.Close();
                this.Listener = null;
            }
            catch { }
        }
        /// <summary>
        /// Invoked asynchronously when a new client requests connection. If the connection is not blacklisted and the max simultaenous connections amount isn't reached yet, then the connection will be processed and normal packet routine will start.
        /// </summary>
        /// <param name="iAr">The IAsyncResult object of the asynchronous BeginAccept operation.</param>
        private void connectionRequest(IAsyncResult iAr)
        {
            try
            {
                Socket Request = this.Listener.EndAccept(iAr);
                string requestIP = Request.RemoteEndPoint.ToString().Split(':')[0];

                if (this.ipIsBlacklisted(requestIP))
                {
                    Request.Close();
                    Logging.Log("Refused connection request from " + requestIP + ", this IP address is blacklisted for whatever reason.", Logging.logType.connectionBlacklistEvent);
                }
                else if (ObjectTree.Sessions.sessionCount >= 100 && !ObjectTree.Application.isLicensed)
                {
                    Request.Close();
                    Logging.Log("Refused connection request from " + requestIP + ", because you are running an unlicensed copy of Woodpecker, which allows max. 100 simultaneous connections.", Logging.logType.commonWarning);
                }
                else
                {
                    if (ObjectTree.Sessions.getSessionCountOfIpAddress(requestIP) >= maxConnectionsPerIP)
                    {
                        Request.Close();
                        Logging.Log("Refused connection request from " + requestIP + ", this IP already has " + this.maxConnectionsPerIP + " connections to the server, which is the maximum configured.", Logging.logType.sessionConnectionEvent);
                    }
                    else
                    {
                        ObjectTree.Sessions.createSession(Request);
                    }
                }
            }
            catch (ObjectDisposedException) { } // Nothing special
            catch (NullReferenceException) { } // Nothing special
            catch (Exception ex) { Logging.Log("Unhandled error during game connection request: " + ex.Message, Logging.logType.commonError); }
            finally 
            { 
                if(this.Listener != null)
                    this.Listener.BeginAccept(new AsyncCallback(this.connectionRequest), this.Listener);
            }
        }
        #endregion
    }
}
