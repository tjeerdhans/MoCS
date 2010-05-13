using System;
using System.Drawing;
using System.Windows.Forms;
using System.Configuration;
using System.Reflection;
using System.Xml;
using MocsMessageInterceptor;
using MocsCommunication;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Security;

namespace MocsClient
{
    public partial class MocsForm : Form
    {
        NotifyIcon _notifyIcon;
        ContextMenu _contextMenu;
        MenuItem _closeItem;
        MenuItem _alwaysOnTopItem;
        MenuItem _clearItem;
        MenuItem _sendMessageItem;
        MenuItem _blockChatMessageItem;
        MenuItem _configurationItem;
        MenuItem _teamIdItem;

        ClientCommunicationService _client;
        MessageInterceptor _messageInterceptor;

        Configuration _configuration;
        bool _controlKeyDown;
        bool _masterMode;

        bool _blockChatMessages = false;

        string _multicastIPAddress;
        int _multicastPort;
        string _serverIPAddress;
        int _serverPort;
        bool _useMulticast;
        bool _enableLogging;
        string _logFileName;
        LogLevel _logLevel;
        string _teamId = string.Empty;

        public MocsForm()
        {
            InitializeComponent();

            _configuration = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);

            _notifyIcon = new NotifyIcon();
            _notifyIcon.Text = "Mocs Notification";
            _notifyIcon.Visible = true;
            _notifyIcon.Icon = new Icon(Assembly.GetExecutingAssembly().GetManifestResourceStream("MocsClient.sync.ico"));
            _notifyIcon.DoubleClick += new EventHandler(_notifyIcon_DoubleClick);

            // Old school balloon tip
            //_notifyIcon.BalloonTipClicked += new EventHandler(_notifyIcon_BalloonTipClicked);

            BuildContextMenu();
            _notifyIcon.ContextMenu = _contextMenu;

            _messageInterceptor = new MessageInterceptor();
            _messageInterceptor.Initialize();

            dataGridViewNotification.MouseWheel += new MouseEventHandler(dataGridViewNotification_MouseWheel);
            dataGridViewNotification.KeyDown += new KeyEventHandler(dataGridViewNotification_KeyDown);
            dataGridViewNotification.KeyUp += new KeyEventHandler(dataGridViewNotification_KeyUp);
        }

        void _notifyIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
        }

        void dataGridViewNotification_KeyUp(object sender, KeyEventArgs e)
        {
            _controlKeyDown = false;
        }

        void dataGridViewNotification_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                _controlKeyDown = true;
            }
            if (e.Control && e.KeyCode == Keys.M)
            {
                _masterMode = !_masterMode;
                SetTitle();
            }
        }

        void dataGridViewNotification_MouseWheel(object sender, MouseEventArgs e)
        {
            if (!_controlKeyDown)
            {
                return;
            }
            float newSize = dataGridViewNotification.Font.Size;
            if (e.Delta > 0)
            {
                newSize++;
            }
            else
            {
                newSize--;
                if (newSize <= 0)
                {
                    newSize = 1;
                }
            }
            Font newFont = new Font(dataGridViewNotification.Font.FontFamily, newSize);
            dataGridViewNotification.Font = newFont;
        }

        private void BuildContextMenu()
        {
            _contextMenu = new ContextMenu();
            _contextMenu.Popup += new EventHandler(_contextMenu_Popup);

            _sendMessageItem = new MenuItem();
            _sendMessageItem.Click += new EventHandler(contextMenu_Click);
            _sendMessageItem.Text = "Send a Message";

            _clearItem = new MenuItem();
            _clearItem.Click += new EventHandler(contextMenu_Click);
            _clearItem.Text = "Clear all Messages";

            _teamIdItem = new MenuItem();
            _teamIdItem.Click += new EventHandler(contextMenu_Click);
            _teamIdItem.Text = "Edit Team Name";

            _configurationItem = new MenuItem();
            _configurationItem.Click += new EventHandler(contextMenu_Click);
            _configurationItem.Text = "Edit Configuration";

            _blockChatMessageItem = new MenuItem();
            _blockChatMessageItem.Click += new EventHandler(contextMenu_Click);
            _blockChatMessageItem.Text = "Block Personal Messages";

            _alwaysOnTopItem = new MenuItem();
            _alwaysOnTopItem.Click += new EventHandler(contextMenu_Click);
            _alwaysOnTopItem.Text = "Always on Top";

            _closeItem = new MenuItem();
            _closeItem.Click += new EventHandler(contextMenu_Click);
            _closeItem.Text = "Close Applicaton";

            _contextMenu.MenuItems.Add(_sendMessageItem);
            _contextMenu.MenuItems.Add(_clearItem);
            _contextMenu.MenuItems.Add(_teamIdItem);
            _contextMenu.MenuItems.Add(_configurationItem);
            _contextMenu.MenuItems.Add(_blockChatMessageItem);

            _contextMenu.MenuItems.Add(_alwaysOnTopItem);
            _contextMenu.MenuItems.Add("-");
            _contextMenu.MenuItems.Add(_closeItem);
        }

        void _contextMenu_Popup(object sender, EventArgs e)
        {
            _sendMessageItem.Enabled = (_client != null);
        }

        private void MocsForm_Load(object sender, EventArgs e)
        {
            ReadConfiguration();
            if (string.IsNullOrEmpty(_teamId))
            {
                ShowTeamForm();
            }
            SetTitle();
            StartClient();
            SetTitle();
        }

        private void StartClient()
        {
            _client = new ClientCommunicationService();
            if (_enableLogging)
            {
                ILoggerService loggerService = new LoggerService();
                _client.CurrentLogLevel = _logLevel;
                _client.LogFileName = _logFileName;
                _client.LoggerService = loggerService;
            }

            _client.MulticastIPAddress = _multicastIPAddress;
            _client.MulticastPort = _multicastPort;
            _client.ServerIPAddress = _serverIPAddress;
            _client.ServerPort = _serverPort;
            _client.UseMulticast = _useMulticast;
            _client.DataReceived += new EventHandler<DataReceivedEventArgs>(OnDataReceived);
            try
            {
                _client.Start();
                ClearGrid();
                SendRequestForMessageList();
            }
            catch (SocketException)
            {
                StopClient();
                MessageBox.Show(this, "Error connecting to server " + _serverIPAddress + ":" +
                                    _serverPort + Environment.NewLine + "Configure server address and port.", "Mocs Notification", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ShowConfigurationForm();
            }
        }

        private void ReadConfiguration()
        {
            _teamId = _configuration.AppSettings.Settings["TeamId"].Value;
            _multicastIPAddress = _configuration.AppSettings.Settings["MulticastIPAddress"].Value;
            _multicastPort = Int32.Parse(_configuration.AppSettings.Settings["MulticastPort"].Value);
            _serverIPAddress = _configuration.AppSettings.Settings["ServerIPAddress"].Value;
            _serverPort = Int32.Parse(_configuration.AppSettings.Settings["ServerPort"].Value);
            _useMulticast = bool.Parse(_configuration.AppSettings.Settings["UseMulticast"].Value);
            _enableLogging = bool.Parse(_configuration.AppSettings.Settings["EnableLogging"].Value);
            _logFileName = _configuration.AppSettings.Settings["LogFileName"].Value;
            _logLevel = (LogLevel)Enum.Parse(typeof(LogLevel), _configuration.AppSettings.Settings["LogLevel"].Value);
        }

        private void MocsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Cleanup();
        }

        private void Cleanup()
        {
            StopClient();
            _notifyIcon.Visible = false;
            _configuration = null;
            CleanUpNotifyWindow();
        }

        private void StopClient()
        {
            if (_client != null)
            {
                _client.DataReceived -= new EventHandler<DataReceivedEventArgs>(OnDataReceived);
                if (_client.IsStarted)
                {
                    _client.Stop();
                }
                _client = null;
            }
        }

        void contextMenu_Click(object sender, EventArgs e)
        {
            if (sender == _sendMessageItem)
            {
                ShowChatForm();
            }
            else if (sender == _clearItem)
            {
                ClearGrid();
            }
            else if (sender == _teamIdItem)
            {
                ShowTeamForm();
            }
            else if (sender == _configurationItem)
            {
                ShowConfigurationForm();
            }
            else if (sender == _blockChatMessageItem)
            {
                _blockChatMessages = !_blockChatMessages;
                _blockChatMessageItem.Checked = _blockChatMessages;
            }
            else if (sender == _alwaysOnTopItem)
            {
                TopMost = !_alwaysOnTopItem.Checked;
                _alwaysOnTopItem.Checked = !_alwaysOnTopItem.Checked;
            }
            else if (sender == _closeItem)
            {
                Close();
            }
        }

        private void ClearGrid()
        {
            dataGridViewNotification.Rows.Clear();
        }

        private void ShowTeamForm()
        {
            TeamForm teamForm = new TeamForm(_teamId);
            teamForm.StartPosition = FormStartPosition.CenterParent;
            DialogResult dialogResult = teamForm.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                _configuration.AppSettings.Settings["TeamId"].Value = teamForm.TeamId;
                _configuration.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
                ReadConfiguration();
                SetTitle();
            }
        }


        private void ShowConfigurationForm()
        {
            ConfigurationForm configurationForm = new ConfigurationForm(_useMulticast, _serverIPAddress, _serverPort, _enableLogging, _logFileName);
            configurationForm.StartPosition = FormStartPosition.CenterParent;
            DialogResult dialogResult = configurationForm.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                _configuration.AppSettings.Settings["ServerIPAddress"].Value = configurationForm.ServerIPAddress;
                _configuration.AppSettings.Settings["ServerPort"].Value = configurationForm.ServerPort.ToString();
                _configuration.AppSettings.Settings["UseMulticast"].Value = configurationForm.UseMulticast.ToString();
                _configuration.AppSettings.Settings["EnableLogging"].Value = configurationForm.EnableLogging.ToString();
                _configuration.AppSettings.Settings["LogFileName"].Value = configurationForm.LogFileName;
                _configuration.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
                ReadConfiguration();
                StopClient();
                SetTitle();
                StartClient();
                SetTitle();
            }
        }

        private void ShowChatForm()
        {
            ChatForm chatForm = new ChatForm();
            chatForm.StartPosition = FormStartPosition.CenterParent;
            DialogResult dialogResult = chatForm.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<Message><MessageType>");
                sb.Append("Chat");
                sb.Append("</MessageType>");
                sb.Append("<DateTime>");
                sb.Append(DateTime.Now.ToString("HH:mm:ss"));
                sb.Append("</DateTime>");
                sb.Append("<Category>");
                sb.Append("</Category><TeamId>");
                sb.Append(SecurityElement.Escape(chatForm.TeamId));
                sb.Append("</TeamId><Text>");
                sb.Append("(from " + SecurityElement.Escape(_teamId) + ") ");
                sb.Append(SecurityElement.Escape(chatForm.TextToSend) + "</Text><Forward>0</Forward><Synchronized>0</Synchronized></Message>");

                _client.Enqueue(new CommunicationMessage(sb.ToString()));
            }
        }

        void _notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
        }

        public void OnDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate { OnDataReceived(sender, e); });
            }
            else
            {
                ParseMessage(e);
            }
        }

        private void SendRequestForMessageList()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<Message><MessageType>");
            sb.Append("Info");
            sb.Append("</MessageType>");
            sb.Append("<DateTime>");
            sb.Append(DateTime.Now.ToString("HH:mm:ss"));
            sb.Append("</DateTime>");
            sb.Append("<Category>MessageList</Category><TeamId>");
            sb.Append(_teamId);
            sb.Append("</TeamId><Text></Text><Forward>0</Forward><Synchronized>0</Synchronized></Message>");

            _client.Enqueue(new CommunicationMessage(sb.ToString()));
        }


        private void ParseMessage(DataReceivedEventArgs e)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(e.CommunicationMessage.Data);
            string messageType = xmlDocument.SelectSingleNode("Message/MessageType").InnerText;
            string teamId = xmlDocument.SelectSingleNode("Message/TeamId").InnerText;
            string category = xmlDocument.SelectSingleNode("Message/Category").InnerText;
            string text = xmlDocument.SelectSingleNode("Message/Text").InnerText;
            string dateTime = xmlDocument.SelectSingleNode("Message/DateTime").InnerText;

            // Ignore request for MessageList
            if (string.Equals(category, "MessageList", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // If it's a Chat Message and Blocking is on, forget it
            if (string.Equals(messageType, "Chat", StringComparison.OrdinalIgnoreCase) && _blockChatMessages)
            {
                return;
            }

            // In MasterMode we see all the messages
            if (!_masterMode)
            {
                // If teamId is filled, it's a message for a specific receiver
                if (teamId.Trim() != string.Empty)
                {
                    if (!string.Equals(teamId.Trim(), _teamId.Trim()))
                    {
                        return;
                    }
                }
            }

            UpdateGridView(messageType, category, text, dateTime);
            if (!e.CommunicationMessage.IsSynchronized())
            {
                ShowBalloon(messageType, category, text, dateTime);
                _messageInterceptor.ProcessMessage(e.CommunicationMessage.Data);
            }
        }

        NotifyWindow _notifyWindow;
        private void ShowBalloon(string messageType, string category, string text, string dateTime)
        {
            CleanUpNotifyWindow();
            _notifyWindow = new NotifyWindow("Mocs message: " + messageType, dateTime + "  " + category + Environment.NewLine + text);

            _notifyWindow.SetDimensions(160, 130);

            _notifyWindow.BackColor = GetColor(messageType, category);            
            _notifyWindow.GradientColor = Color.White;

            _notifyWindow.TitleFont = new Font(_notifyWindow.Font, FontStyle.Underline);
            _notifyWindow.Notify();
            // Old school balloon Tip
            //_notifyIcon.ShowBalloonTip(2000, "Mocs message: " + messageType, dateTime + "  " + category + Environment.NewLine + text, GetToolTipIcon(messageType));
        }

        private void CleanUpNotifyWindow()
        {
            if (_notifyWindow != null)
            {
                _notifyWindow.TitleClicked -= new EventHandler(_notifyWindow_TitleClicked);
                _notifyWindow.TextClicked -= new EventHandler(_notifyWindow_TextClicked);
                _notifyWindow.Dispose();
                _notifyWindow.Close();

                _notifyWindow = null;
            }
        }

        void _notifyWindow_TextClicked(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
        }

        void _notifyWindow_TitleClicked(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
        }

        private void UpdateGridView(string messageType, string category, string text, string dateTime)
        {
            DataGridViewRow row = new DataGridViewRow();
            row.CreateCells(dataGridViewNotification);
            row.Cells[0].Value = messageType;
            row.Cells[0].Style.BackColor = GetColor(messageType,category);
            row.Cells[1].Value = dateTime;
            row.Cells[2].Value = category;
            row.Cells[3].Value = text;
            dataGridViewNotification.Rows.Insert(0, row);
            dataGridViewNotification.ClearSelection();
        }

        private void SetTitle()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Mocs Notification");
            if (_teamId != string.Empty)
            {
                sb.Append(" - ");
                sb.Append(_teamId);
            }
            sb.Append(" (");
            if (_useMulticast)
            {
                sb.Append("Multicast ");
                sb.Append(_multicastIPAddress);
                sb.Append(":" + _multicastPort);
            }
            else
            {
                sb.Append("TCP ");
                sb.Append(_serverIPAddress);
                sb.Append(":" + _serverPort);
            }
            if (_client == null)
            {
                sb.Append(" Not connected");
            }
            else
            {
                sb.Append(" Connected");
            }
            sb.Append(")");
            if (_masterMode)
            {
                sb.Append(" MM");
            }
            Text = sb.ToString();
        }

        private ToolTipIcon GetToolTipIcon(string messageType)
        {
            switch (messageType)
            {
                case "Info":
                    return ToolTipIcon.Info;
                case "Warning":
                    return ToolTipIcon.Warning;
                case "Error":
                    return ToolTipIcon.Error;
                case "Chat":
                    return ToolTipIcon.None;
                default:
                    return ToolTipIcon.Info;
            }
        }

        private Color GetColor(string messageType, string category)
        {
            switch (messageType)
            {
                case "Info":
                    if (category.ToLower() == "success" || category.ToLower() == "firstplace")
                    {
                       return Color.LightGreen;
                    }
                    return Color.LightBlue;
                case "Warning":
                    return Color.Orange;
                case "Error":
                    return Color.Red;
                case "Chat":                    
                    return Color.PaleVioletRed;
                default:
                    return Color.White;
            }
        }

        private void dataGridViewNotification_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                _contextMenu.Show(dataGridViewNotification, new Point(e.X, e.Y));
            }
        }
    }
}
