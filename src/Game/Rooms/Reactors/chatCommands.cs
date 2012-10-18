using System;
using System.Collections.Generic;

using Woodpecker.Sessions;
using Woodpecker.Game.Users;
using Woodpecker.Game.Rooms.Units;

namespace Woodpecker.Game.Rooms.Instances.Interaction
{
    public partial class roomReactor : Reactor
    {
        #region Methods
        /// <summary>
        /// Tries to process a given chat message as a chat command and returns true if the operation has succeeded.
        /// </summary>
        /// <param name="Text">The chat message to process.</param>
        private bool handleSpecialChatCommand(string Text)
        {
            if (Text[0] == ':' && Text.Length > 3) // Could be a chat command (emotes too btw)
            {
                try
                {
                    string Command = Text.Substring(1).Split(' ')[0];
                    string Body = "";
                    if(Text.Contains(" "))
                        Body = Text.Substring(Command.Length + 2);

                    switch (Command)
                    {
                        #region Common commands for all users
                        case "about":
                            {
                                Response.Initialize(139); // "BK"
                                Response.Append("About Woodpecker");
                                Response.Append("<br>");
                                Response.Append("Woodpecker is a V13 Habbo Hotel emulator written in C#.NET by Nillus.");
                                Response.Append("<br><br>");
                                Response.Append("Currently there are ");
                                Response.Append(ObjectTree.Game.Users.userCount);
                                Response.Append(" user(s) online.");
                                sendResponse();
                            }
                            break;
                        #endregion

                        #region Fancy commands
                        case "descenditem":
                            {
                                if (!Session.User.hasFuseRight("fuse_funchatcommands"))
                                    return false;

                                int itemID = int.Parse(Body);
                                if (Session.roomInstance.containsFloorItem(itemID))
                                {
                                    Woodpecker.Game.Items.floorItem pItem = Session.roomInstance.getFloorItem(itemID);
                                    Woodpecker.Net.Game.Messages.serverMessage Message = new Woodpecker.Net.Game.Messages.serverMessage(230); // "Cf"
                                    Message.appendWired(pItem.X - 1);
                                    Message.appendWired(pItem.Y + 1);
                                    Message.appendWired(pItem.X);
                                    Message.appendWired(pItem.Y);
                                    Message.appendWired(true);
                                    Message.appendWired(itemID);
                                    Message.appendClosedValue("12.0");
                                    Message.appendClosedValue(Woodpecker.Specialized.Text.stringFunctions.formatFloatForClient(pItem.Z));
                                    Message.appendWired(new Random(DateTime.Now.Millisecond).Next(0, 10000));
                                    Session.roomInstance.sendMessage(Message);
                                }
                            }
                            break;

                        case "handitem":
                            {
                                roomUser Me = Session.roomInstance.getRoomUser(Session.ID);
                                Items.carryItemHelper.setHandItem(ref Me, Body);
                            }
                            break;
                        #endregion

                        #region Moderation commands
                        case "alert":
                            {
                                if (!Session.User.hasFuseRight("fuse_alert"))
                                    return false;

                                string Username = Body.Substring(0, Body.IndexOf(' '));
                                string Message = Body.Substring(Body.IndexOf(' ') + 1);
                                if (Username.Length > 0 && Message.Length > 0)
                                {
                                    if(ObjectTree.Game.Moderation.requestAlert(Session.User.ID, Username, Message, ""))
                                        Session.castWhisper("Alert sent to user.");
                                }
                            }
                            break;

                        case "kick":
                            {
                                if (!Session.User.hasFuseRight("fuse_kick"))
                                    return false;

                                string Username = Body.Substring(0, Body.IndexOf(' '));
                                string Message = Body.Substring(Body.IndexOf(' ') + 1);
                                if (Username.Length > 0)
                                {
                                    if(ObjectTree.Game.Moderation.requestKickFromRoom(Session.User.ID, Username, Message, ""))
                                        Session.castWhisper("Kick sent to user.");
                                }
                            }
                            break;

                        case "ban":
                            {
                                if (!Session.User.hasFuseRight("fuse_ban"))
                                    return false;

                                string Username = Body.Substring(0, Body.IndexOf(' '));
                                Body = Body.Substring(Body.IndexOf(' ') + 1);
                                int Hours = int.Parse(Body.Substring(0, Body.IndexOf(' ')));
                                string Message = Body.Substring(Body.IndexOf(' ') + 1);

                                if (Message.Length > 0)
                                {
                                    if (ObjectTree.Game.Moderation.requestBan(Session.User.ID, Username, Hours, false, false, Message, ""))
                                        Session.castWhisper("User banned for " + Hours + " hours.");
                                }
                            }
                            break;

                        case "unban":
                            {
                                if (!Session.User.hasFuseRight("fuse_superban")) // Only mods can unban
                                    return false;

                                if (Body.Length > 0)
                                {
                                    if (ObjectTree.Game.Moderation.requestUnban(Session.User.ID, Body, null, null))
                                    {
                                        Session.castWhisper("Matching bans for given user have been removed.");
                                        ObjectTree.Game.Moderation.logModerationAction(Session.User.ID, "unban", ObjectTree.Game.Users.getUserID(Body), "", "Unban via :unban %username%");
                                    }
                                }
                            }
                            break;

                        case "blacklist":
                            {
                                if (!Session.User.hasFuseRight("fuse_administrator_access"))
                                    return false;

                                if (Body.Length > 0)
                                {
                                    int userID = ObjectTree.Game.Users.getUserID(Body);
                                    if (userID > 0)
                                    {
                                        userAccessInformation lastAccess = ObjectTree.Game.Users.getLastAccess(userID);
                                        if (lastAccess != null)
                                        {
                                            ObjectTree.Net.Game.blackListIpAddress(lastAccess.IP);
                                            ObjectTree.Sessions.destroySessions(lastAccess.IP);
                                            Session.castWhisper("IP " + lastAccess.IP + " has been added to the connection blacklist.");
                                        }
                                    }
                                }
                            }
                            break;

                        case "roomalert":
                            {
                                if (!Session.User.hasFuseRight("fuse_room_alert"))
                                    return false;

                                if (Body.Length > 0)
                                {
                                    if(ObjectTree.Game.Moderation.requestRoomAlert(Session.User.ID, Session.roomID, Body, ""))
                                        Session.castWhisper("Alert sent to room.");
                                }
                            }
                            break;

                        case "roomkick":
                            {
                                if (!Session.User.hasFuseRight("fuse_room_kick"))
                                    return false;

                                if (Body.Length > 0)
                                {
                                    if(ObjectTree.Game.Moderation.requestRoomKick(Session.User.ID, Session.roomID, Body, ""))
                                        Session.castWhisper("Kick sent to room.");
                                }
                            }
                            break;

                        case "ha":
                        case "hotelalert":
                            {
                                if (!Session.User.hasFuseRight("fuse_hotelalert"))
                                    return false;

                                if (Body.Length > 0)
                                {
                                    ObjectTree.Game.Moderation.castHotelAlert(Session.User.ID, Body);
                                    Session.castWhisper("Hotel alert sent.");
                                }
                            }
                            break;

                        #endregion

                        #region Debug commands

                        case "debug":
                            {
                                if (!Session.User.hasFuseRight("fuse_debug"))
                                    return false;

                                switch (Body)
                                {
                                    case "reloadstore":
                                        ObjectTree.Game.Items.loadDefinitions();
                                        ObjectTree.Game.Store.loadSales();
                                        ObjectTree.Game.Store.loadCataloguePages();
                                        Session.castWhisper("Re-loaded item definitions, store sales and catalogue pages.");
                                        break;

                                    case "handitems":
                                        Session.itemStripHandler.loadHandItems();
                                        Session.castWhisper("Re-loaded handitems.");
                                        break;
                                }
                            }
                            break;

                        case "createitem":
                            {
                                if (!Session.User.hasFuseRight("fuse_debug"))
                                    return false;

                                int definitionID = int.Parse(Body.Substring(0, Body.IndexOf(' ')));
                                int Amount = int.Parse(Body.Substring(Body.IndexOf(' ') + 1));
                                if (Amount <= 0)
                                    Amount = 1;
                                for (int x = 1; x <= Amount; x++)
                                {
                                    ObjectTree.Game.Items.createItemInstance(definitionID, Session.User.ID, null);
                                }
                                Session.itemStripHandler.loadHandItems();

                                Session.sendHandStrip("last");
                            }
                        break;

                        #endregion

                        default:
                            return false;
                    }
                }
                catch { Session.castWhisper("Error occurred during processing of the chat command, check your parameters."); }
                return true;
            }
            return false;
        }
        #endregion
    }
}
