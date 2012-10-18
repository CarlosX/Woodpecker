using System;

using Woodpecker.Specialized.Text;

namespace Woodpecker.Game
{
    public class globalReactor : Reactor
    {
        /// <summary>
        /// 9 - "@I"
        /// </summary>
        public void GETAVAILABLESETS()
        {
            Session.refreshFigureParts();
        }
        /// <summary>
        /// 42 - "@j"
        /// </summary>
        public void APPROVENAME()
        {
            Response.Initialize(36); // "@d"

            bool isPet = (Request.Content[Request.Content.Length - 1] == 'I');
            int errorID = ObjectTree.Game.Users.getNameCheckError(isPet, Request.getParameter(0));
            Response.appendWired(errorID);
            sendResponse();
        }
        /// <summary>
        /// 49 - "@q"
        /// </summary>
        public void GDATE()
        {
            Response.Initialize(163); // "Bc"
            Response.Append(DateTime.Today.ToString("dd-MM-yyyy"));
            sendResponse();
        }
        /// <summary>
        /// 196 - "CD"
        /// </summary>
        public void PONG()
        {
            Session.pongReceived = true;
        }
        /// <summary>
        /// 197 - "CE"
        /// </summary>
        public void APPROVEEMAIL()
        {
            Response.Initialize(271); // "DO"
            sendResponse();
        }
        /// <summary>
        /// 203 - "CK"
        /// </summary>
        public void APPROVE_PASSWORD()
        {
            Response.Initialize(282); // "DZ"
            string Username = Request.getParameter(0);
            string Password = Request.getParameter(1);
            bool OK = stringFunctions.passwordIsValid(Username, Password);
            Response.appendWired(!OK);
            sendResponse();
        }
    }
}
