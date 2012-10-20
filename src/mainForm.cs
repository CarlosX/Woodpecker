using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;

using Woodpecker.Core;
using Woodpecker.Storage;
using Woodpecker.Tools;

namespace Woodpecker
{
    public partial class mainForm : Form
    {
        #region Fields
        public bool mShutdown = false;
        #region Delegates
        private delegate void textLogger(string Text, Logging.logType Type);
        private delegate void sacredLogger(string Text);
        private delegate void statusLabelPntr(string Caption);
        private static statusLabelPntr _statusLabelPointer;
        private static textLogger _Logger;
        private static sacredLogger _sacredLogger;
        #endregion
        private Thread statusUpdater;
        #endregion

        public mainForm()
        {
            InitializeComponent();
        }
        private void mainForm_Load(object sender, EventArgs e)
        {
            this.Show();
            this.stripMenu.Enabled = false;
            _Logger = new textLogger(logText);
            _sacredLogger = new sacredLogger(logSacredText);
            _statusLabelPointer = new statusLabelPntr(setStatusLabel);

            ObjectTree.Application.GUI = this;
            ObjectTree.Application.Start();

            statusUpdater = new Thread(this.updateStatusCycle);
            statusUpdater.Priority = ThreadPriority.Lowest;
            statusUpdater.Start();
        }
        #region Logging
        public void logSacredText(string Text)
        {
            try
            {
                if (this.InvokeRequired)
                    this.BeginInvoke(_sacredLogger, Text);
                else
                {
                    //lock (this.txtLog)
                    {
                        if (Text != null)
                        {
                            this.txtLog.AppendText("# ");
                            this.txtLog.AppendText(Text);
                        }
                        this.txtLog.AppendText(Environment.NewLine);
                        this.txtLog.SelectionStart = this.txtLog.Text.Length;
                        this.txtLog.ScrollToCaret();
                        this.txtLog.Refresh();
                    }
                }
            }
            catch (StackOverflowException) { }
        }
        public void _logSacredText(string Text)
        {
            this.BeginInvoke(new MethodInvoker(delegate()
            {
                lock (this.txtLog)
                {
                    if (Text != null)
                    {
                        this.txtLog.AppendText("# ");
                        this.txtLog.AppendText(Text);
                    }
                    this.txtLog.AppendText(Environment.NewLine);
                    this.txtLog.SelectionStart = this.txtLog.Text.Length;
                    //this.txtLog.ScrollToCaret();
                    this.txtLog.Refresh();
                }
            }));
        }
        /// <summary>
        /// Logs text to the textbox on the main form.
        /// </summary>
        /// <param name="Text">The text to log.</param>
        /// <param name="Type">A Logging.logType enum value, indicating the log's type.</param>
        public void logText(string Text, Logging.logType Type)
        {
            if (this.InvokeRequired)
                this.BeginInvoke(_Logger, Text, Type);
            else
            {
                try
                {
                    lock (this.txtLog)
                    {
                        switch (Type)
                        {
                            case Logging.logType.commonWarning:
                                {
                                    StringBuilder Data = new StringBuilder();
                                    Data.Append("Warning!" + Environment.NewLine);
                                    Data.Append("Time: " + DateTime.Now.ToShortTimeString() + Environment.NewLine);
                                    Data.Append("Description: " + Text + Environment.NewLine + Environment.NewLine);

                                    this.txtLog.SelectionColor = Color.Brown;
                                    this.txtLog.SelectedText = Data.ToString();
                                }
                                break;

                            case Logging.logType.reactorError:
                            case Logging.logType.commonError:
                                {
                                    StackFrame st = new StackTrace().GetFrame(2);
                                    StringBuilder Data = new StringBuilder();

                                    Data.Append("ERROR!" + Environment.NewLine);
                                    Data.Append("Time: " + DateTime.Now.ToShortTimeString() + Environment.NewLine);
                                    Data.Append("Object: " + st.GetMethod().ReflectedType.Name + Environment.NewLine);
                                    Data.Append("Method: " + st.GetMethod().Name + Environment.NewLine);
                                    Data.Append("Description: " + Text + Environment.NewLine + Environment.NewLine);

                                    this.txtLog.SelectionColor = Color.DarkRed;
                                    this.txtLog.SelectedText = Data.ToString();
                                }
                                break;

                            default:
                                {
                                    this.txtLog.AppendText("# ");
                                    this.txtLog.AppendText(Text);
                                    this.txtLog.AppendText(Environment.NewLine);
                                }
                                break;
                        }

                        this.txtLog.SelectionStart = this.txtLog.Text.Length;
                        this.txtLog.ScrollToCaret();
                        this.txtLog.Refresh();
                    }
                }
                catch (Exception) { }
            }
        }
        /// <summary>
        /// Logs text to the textbox on the main form.
        /// </summary>
        /// <param name="Text">The text to log.</param>
        /// <param name="Type">A Logging.logType enum value, indicating the log's type.</param>
        public void _logText(string Text, Logging.logType Type)
        {
            this.BeginInvoke(new MethodInvoker(delegate()
            {
                lock (this.txtLog)
                {
                    switch (Type)
                    {
                        case Logging.logType.commonWarning:
                            {
                                StringBuilder Data = new StringBuilder();
                                Data.Append("Warning!" + Environment.NewLine);
                                Data.Append("Time: " + DateTime.Now.ToShortTimeString() + Environment.NewLine);
                                Data.Append("Description: " + Text + Environment.NewLine + Environment.NewLine);

                                this.txtLog.SelectionColor = Color.Brown;
                                this.txtLog.SelectedText = Data.ToString();
                            }
                            break;

                        case Logging.logType.reactorError:
                        case Logging.logType.commonError:
                            {
                                StackFrame st = new StackTrace().GetFrame(2);
                                StringBuilder Data = new StringBuilder();

                                Data.Append("ERROR!" + Environment.NewLine);
                                Data.Append("Time: " + DateTime.Now.ToShortTimeString() + Environment.NewLine);
                                Data.Append("Object: " + st.GetMethod().ReflectedType.Name + Environment.NewLine);
                                Data.Append("Method: " + st.GetMethod().Name + Environment.NewLine);
                                Data.Append("Description: " + Text + Environment.NewLine + Environment.NewLine);

                                this.txtLog.SelectionColor = Color.DarkRed;
                                this.txtLog.SelectedText = Data.ToString();
                            }
                            break;

                        default:
                            {
                                this.txtLog.AppendText("# ");
                                this.txtLog.AppendText(Text);
                                this.txtLog.AppendText(Environment.NewLine);
                            }
                            break;
                    }

                    this.txtLog.SelectionStart = this.txtLog.Text.Length;
                    this.txtLog.ScrollToCaret();
                }
                this.txtLog.Refresh();
            }));
        }
        private void setStatusLabel(string Caption)
        {
            if (this.InvokeRequired)
                this.BeginInvoke(_statusLabelPointer, Caption);
            else
            {
                this.lblStatus.Text = Caption;
            }
        }
        #endregion

        private void updateStatusCycle()
        {
            while (true)
            {
                int sessionCount = ObjectTree.Sessions.sessionCount;
                int userCount = ObjectTree.Game.Users.userCount;
                int roomCount = ObjectTree.Game.Rooms.roomCount;

                long memKB = GC.GetTotalMemory(false) / 1024;
                setStatusLabel(sessionCount + " session(s) running, " + userCount + " user(s) logged in, " + roomCount + " room instance(s) active. Memory usage: " + memKB + "KB");
                Thread.Sleep(30000); // 30 seconds
            }
        }
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show
                (
                "Woodpecker\n" + 
                "Habbo Hotel V13 emulator\n" +
                "Platform: C#.NET 3.5\n" +
                "Written by Nillus\n" +
                "Mental support by Moogly\n" + 
                "Copyright (C) 2008\n\n" + 
                "Credits:\n" +
                "Joe 'Joeh' Hegarty - for his V9 emulator 'Thor', which has learnt me alot of stuff and actually inspired me to do Woodpecker III.\nThe system structure and stacking is based off Thor.",
                "About Woodpecker",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
                );
        }

        
        private void mainForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
            if (this.mShutdown)
                Environment.Exit(-1);
            else
            {
                if (this.stripMenu.Enabled)
                    Logging.Log("Please use File > Shutdown to shutdown the server.");
                e.Cancel = true;
            }
        }

        private void shutdownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to shutdown Woodpecker?\nAll active connections and resources will be disposed.", "Confirm shutdown", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                ObjectTree.Application.Stop("none");
        }

        private void blacklistUnknownPacketSendersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Configuration.blacklistBastards = !Configuration.blacklistBastards;
        }

        private void clearLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.txtLog.Clear();
            this.logSacredText("Log cleared.");
        }

        private void sessionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ObjectTree.Sessions.listSessions();
        }

        private void usersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ObjectTree.Game.Users.listUserStats();
        }

        private void clearConnectionBlacklistToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ObjectTree.Net.Game.clearBlacklist();
            Logging.Log("Connection blacklist wiped.");
        }

        private void logCommonWarningsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logging.toggleLogStatus(Logging.logType.commonWarning);
        }

        private void logCommonErrorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logging.toggleLogStatus(Logging.logType.commonError);
        }

        private void logDebugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logging.toggleLogStatus(Logging.logType.debugEvent);
        }

        private void logSessionCreatedestroyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logging.toggleLogStatus(Logging.logType.sessionConnectionEvent);
        }

        private void logClientserverMessagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logging.toggleLogStatus(Logging.logType.clientMessageEvent);
        }

        private void logServerclientMessagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logging.toggleLogStatus(Logging.logType.serverMessageEvent);
        }

        private void logReactorErrorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logging.toggleLogStatus(Logging.logType.reactorError);
        }

        private void loglistenerNotFoundErrorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logging.toggleLogStatus(Logging.logType.targetMethodNotFoundEvent);
        }

        private void logLogonlogoffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logging.toggleLogStatus(Logging.logType.userVisitEvent);
        }

        private void logModerationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logging.toggleLogStatus(Logging.logType.moderationEvent);
        }

        private void logChatlogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ObjectTree.Game.Moderation.logChat = !ObjectTree.Game.Moderation.logChat;
        }

        private void logInstanceCreatedestroyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logging.toggleLogStatus(Logging.logType.roomInstanceEvent);
        }

        private void logUserEnterleaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logging.toggleLogStatus(Logging.logType.roomUserRegisterEvent);
        }

        private void logHaxEventsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logging.toggleLogStatus(Logging.logType.haxEvent);
        }

        private void logConnectionBlacklistEventsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logging.toggleLogStatus(Logging.logType.connectionBlacklistEvent);
        }

        private void cmdBroadcoastMessage_Click(object sender, EventArgs e)
        {
            string Text = this.txtBroadcoastMessage.Text;
            Woodpecker.Specialized.Text.stringFunctions.filterVulnerableStuff(ref Text, true);
            Text = Text.Replace(Convert.ToChar(1), ' ');
            Text = Text.Trim();
            if (Text.Length > 0)
            {
                ObjectTree.Game.Users.broadcastHotelAlert(Text);
                Logging.Log("Broadcoasted hotel alert to all online users.");
            }
        }
        private void itemDefinitionEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            itemDefinitionEditorMainForm Editor = new itemDefinitionEditorMainForm();
            Editor.Show();
        }
    }
}
