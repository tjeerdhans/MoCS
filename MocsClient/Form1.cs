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

namespace MocsClient
{
    public partial class Form1 : Form
    {
        ClientCommunicationService _client;        
        NotifyIcon _notifyIcon;
        ContextMenu _contextMenu;
        MenuItem _closeItem;
        MenuItem _alwaysOnTopItem;
        MenuItem _clearItem;
        MenuItem _sendMessageItem;
        MenuItem _blockChatMessageItem;
        MenuItem _configurationItem;

        MessageInterceptor _messageInterceptor;
        ChatForm _chatForm;
        ConfigurationForm _configurationForm;
        Configuration _configuration;

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

        public Form1()
        {
            InitializeComponent();

            _configuration = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);

            _notifyIcon = new NotifyIcon();
            _notifyIcon.Text = "Mocs Notification";
            _notifyIcon.Visible = true;
            _notifyIcon.Icon = new Icon(Assembly.GetExecutingAssembly().GetManifestResourceStream("MocsClient.sync.ico"));
            _notifyIcon.DoubleClick += new EventHandler(_notifyIcon_DoubleClick);

            _contextMenu = new ContextMenu();
            _contextMenu.Popup += new EventHandler(_contextMenu_Popup);

            _sendMessageItem = new MenuItem();
            _sendMessageItem.Click += new EventHandler(contextMenu_Click);
            _sendMessageItem.Text = "Send a Message";

            _clearItem = new MenuItem();
            _clearItem.Click += new EventHandler(contextMenu_Click);
            _clearItem.Text = "Clear all Messages";

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
            _contextMenu.MenuItems.Add(_configurationItem);
            _contextMenu.MenuItems.Add(_blockChatMessageItem);

            _contextMenu.MenuItems.Add(_alwaysOnTopItem);
            _contextMenu.MenuItems.Add("-");
            _contextMenu.MenuItems.Add(_closeItem);
            _notifyIcon.ContextMenu = _contextMenu;

            _messageInterceptor = new MessageInterceptor();
            _messageInterceptor.Initialize();
        }

        void _contextMenu_Popup(object sender, EventArgs e)
        {
            _sendMessageItem.Enabled = (_client != null);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ReadConfiguration();
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

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Cleanup();
        }

        private void Cleanup()
        {
            StopClient();
            _notifyIcon.Visible = false;
            _configuration = null;
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
                dataGridView1.Rows.Clear();
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

        private void ShowConfigurationForm()
        {
            _configurationForm = new ConfigurationForm(_teamId, _useMulticast, _serverIPAddress, _serverPort,_enableLogging,_logFileName);
            _configurationForm.StartPosition = FormStartPosition.CenterParent;
            DialogResult dialogResult = _configurationForm.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                _configuration.AppSettings.Settings["TeamId"].Value = _configurationForm.TeamId;
                _configuration.AppSettings.Settings["ServerIPAddress"].Value = _configurationForm.ServerIPAddress;
                _configuration.AppSettings.Settings["ServerPort"].Value = _configurationForm.ServerPort.ToString();
                _configuration.AppSettings.Settings["UseMulticast"].Value = _configurationForm.UseMulticast.ToString();
                _configuration.AppSettings.Settings["EnableLogging"].Value = _configurationForm.EnableLogging.ToString();
                _configuration.AppSettings.Settings["LogFileName"].Value = _configurationForm.LogFileName;
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
            _chatForm = new ChatForm();
            _chatForm.FormClosed += new FormClosedEventHandler(_chatForm_FormClosed);
            _chatForm.MessageToSend += new ChatForm.SendMessageEventHandler(_chatForm_MessageToSend);
            _chatForm.StartPosition = FormStartPosition.CenterParent;
            _chatForm.ShowDialog(this);
        }

        void _chatForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _chatForm.FormClosed -= new FormClosedEventHandler(_chatForm_FormClosed);
            _chatForm.MessageToSend -= new ChatForm.SendMessageEventHandler(_chatForm_MessageToSend);
            _chatForm = null;
        }

        void _chatForm_MessageToSend(string teamId, string text)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<Message><MessageType>");
            sb.Append("Chat");
            sb.Append("</MessageType>");
            sb.Append("<DateTime>");
            sb.Append(DateTime.Now.ToString("HH:mm:ss"));
            sb.Append("</DateTime>");
            sb.Append("<Category>");
            if (_teamId != string.Empty)
            {
                sb.Append(_teamId);
            }
            sb.Append("</Category><TeamId>");
            sb.Append(teamId);
            sb.Append("</TeamId><Text>");
            sb.Append(text + "</Text><Forward>0</Forward></Message>");
            _client.Enqueue(new CommunicationMessage(sb.ToString()));
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

        private void ParseMessage(DataReceivedEventArgs e)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(e.CommunicationMessage.Data);
            string messageType = xmlDocument.SelectSingleNode("Message/MessageType").InnerText;
            string teamId = xmlDocument.SelectSingleNode("Message/TeamId").InnerText;
            string category = xmlDocument.SelectSingleNode("Message/Category").InnerText;
            string text = xmlDocument.SelectSingleNode("Message/Text").InnerText;
            string dateTime = xmlDocument.SelectSingleNode("Message/DateTime").InnerText;

            // If it's a Chat Message and Blocking is on, forget it
            if (messageType == "Chat" && _blockChatMessages)
            {
                return;
            }

            // If teamId is filled, it's a message for a specific receiver
            if (teamId.Trim() != string.Empty)
            {
                if (teamId.Trim().ToLower() != _teamId.Trim().ToLower())
                {
                    return;
                }
            }

            UpdateGridView(messageType, category, text, dateTime);
            ShowBalloon(messageType, category, text, dateTime);
            _messageInterceptor.ProcessMessage(e.CommunicationMessage.Data);
        }

        private void ShowBalloon(string messageType, string category, string text, string dateTime)
        {
            _notifyIcon.ShowBalloonTip(2000, "Mocs message: " + messageType, dateTime + "  " + category + Environment.NewLine + text, GetToolTipIcon(messageType));
        }

        private void UpdateGridView(string messageType, string category, string text, string dateTime)
        {
            DataGridViewRow row = new DataGridViewRow();
            row.CreateCells(dataGridView1);
            row.Cells[0].Value = messageType;
            row.Cells[0].Style.BackColor = GetColor(messageType);
            row.Cells[1].Value = dateTime;
            row.Cells[2].Value = category;
            row.Cells[3].Value = text;
            dataGridView1.Rows.Insert(0, row);
            dataGridView1.ClearSelection();
        }

        private void SetTitle()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Mocs Noticification");
            if (_teamId != string.Empty)
            {
                sb.Append(" - ");
                sb.Append(_teamId);
            }
            sb.Append(" (");
            if(_useMulticast)
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
            if(_client== null)
            {
                sb.Append(" Not connected");
            }
            else
            {
                sb.Append(" Connected");
            }
            sb.Append(")");
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

        private Color GetColor(string messageType)
        {
            switch (messageType)
            {
                case "Info":
                    return Color.LightGreen;
                case "Warning":
                    return Color.Orange;
                case "Error":
                    return Color.Red;
                case "Chat":
                    return Color.Aquamarine;
                default:
                    return Color.White;
            }
        }

        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                _contextMenu.Show(dataGridView1, new Point(e.X, e.Y));
            }
        }
    }
}
