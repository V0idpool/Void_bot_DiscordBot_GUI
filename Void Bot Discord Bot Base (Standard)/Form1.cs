﻿using Discord;
using Discord.WebSocket;
using MongoDB.Driver;
using System.Diagnostics;
using System.Net;
using System.Text;
using Voidbot_Discord_Bot_GUI.Properties;
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                                                                                                              ||
//                                                                                 VoidBot Discord Bot                                                                                          ||                
//                                                                                                                                                                                              ||
//                                                       Version: 1.0.0 (Public Release)                                                                                                        ||
//                                                       Author: VoidBot Development Team (Voidpool)                                                                                            ||
//                                                       Release Date: [10/06/2024]                                                                                                             ||
//                                                                                                                                                                                              ||
//                                                       Description:                                                                                                                           ||
//                                                       VoidBot is a highly customizable Discord bot with features such as role management (Mute system Example), XP tracking,                 ||
//                                                       user levels, moderation tools, and custom embeds. This release focuses on a GUI with MongoDB                                           ||
//                                                       integration for user data and server settings management, making it easier to get started.                                             ||
//                                                                                                                                                                                              ||
//                                                       Current Features:                                                                                                                      ||
//                                                       - Verification System (With Autorole given upon successful verification in the verification & welcome channel                          ||
//                                                       - Role management (mute system example)                                                                                                ||
//                                                       - User XP and level tracking, including top user rankings                                                                              ||
//                                                       - Customizable welcome messages and channels                                                                                           ||
//                                                       - Moderation tools such as warnings and bans                                                                                           ||
//                                                       - Lots more, Check out the source! :D                                                                                                  ||
//                                                                                                                                                                                              ||
//                                                       Future Features (More will be added periodically, expect this to change):                                                              ||
//                                                       - Custom embed creation for server admins                                                                                              ||
//                                                       - Advanced moderation and logging features                                                                                             ||
//                                                       - Additional customization for slash commands and interaction handling                                                                 ||
//                                                                                                                                                                                              ||
//                                                       Notes:                                                                                                                                 ||
//                                                       - This public release provides a core set of features for a C# Discord Bot with a GUI and background task handling.                    ||
//                                                       - Planned updates include further threading improvements and additional feature integration.                                           ||
//                                                       - Contributions and suggestions from the community are welcome to shape future releases.                                               ||
//                                                       - Donate or Subscribe to my BuyMeACoffee: https://buymeacoffee.com/voidbot                                                             ||
//                                                       - Join my Void Bot Discord Support Server for support, coding help, and code: https://discord.gg/nsSpGJ5saD                            ||
//                                                                                                                                                                                              ||
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

namespace Voidbot_Discord_Bot_GUI
{
    public partial class Form1 : Form
    {
        private static MainProgram _instance;
        private string selectedChannelId;
        private string selectedChannelName;
        public static MainProgram Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MainProgram();
                }
                return _instance;
            }
        }
        // Create an instance of MainProgram
        MainProgram botInstance = new MainProgram();
        private bool isFormVisible = true;
        // Access client
        DiscordSocketClient client;

        public Form1()
        {
            InitializeComponent();
            // Access _client through the property
            DiscordSocketClient client = botInstance.DiscordClient;
            botInstance.BotDisconnected += OnBotDisconnected;
            botInstance.LogReceived += LogMessageReceived;
            botInstance.BotConnected += OnBotConnected;
            nsListView1.SelectedIndexChanged += nsListView1_SelectedIndexChanged;
            nsListView2.SelectedIndexChanged += nsListView2_SelectedIndexChanged;
        }
        // Output Console logs to the Text Area Console log in the bots GUI
        public class TextBoxWriter : TextWriter
        {
            private System.Windows.Forms.TextBox textBox;

            public TextBoxWriter(System.Windows.Forms.TextBox textBox)
            {
                this.textBox = textBox;
            }

            public override Encoding Encoding => Encoding.UTF8;

            public override void Write(char value)
            {
                if (textBox.InvokeRequired)
                {
                    textBox.Invoke(new Action(() =>
                    {
                        textBox.AppendText(value.ToString());
                    }));
                }
                else
                {
                    textBox.AppendText(value.ToString());
                }
            }

        }

        // Method to handle received log messages
        private void LogMessageReceived(string logMessage)
        {
            // Update the TextBox with the log message
            Invoke(new Action(() =>
            {
                botConsoleView.Text += logMessage + Environment.NewLine;
                botConsoleView.SelectionStart = botConsoleView.Text.Length;
                botConsoleView.ScrollToCaret();

            }));
        }

        private async void OnBotDisconnected(string message)
        {
            // Handle stuff here when the bot disconnects if needed
            Console.WriteLine($"[SYSTEM] VB Discord Bot disconnected...");


        }
        private async Task Outputcmd()
        {
            //Output Console Logs to OutputCMD Textbox

            if (botInstance.DiscordClient != null && botInstance != null)
            {
                if (botConsoleView.InvokeRequired)
                {
                    botConsoleView.Invoke((MethodInvoker)delegate
                    {
                        TextBoxWriter writer = new TextBoxWriter(botConsoleView);
                        Console.SetOut(writer);
                    });
                }
                else
                {
                    TextBoxWriter writer = new TextBoxWriter(botConsoleView);
                    Console.SetOut(writer);
                }
            }

        }
        // Get bots connection status
        public async Task Getbotdeets()
        {
            await Task.Run(async () =>
             {
                 while (true)
                 {
                     if (botInstance.DiscordClient != null && botInstance != null)
                     {
                         ConnectionState currentState = botInstance.DiscordClient.ConnectionState;

                         if (label2.InvokeRequired)
                         {
                             label2.Invoke((MethodInvoker)delegate
                            {
                                UpdateLabel(currentState);
                            });
                         }
                         else
                         {
                             UpdateLabel(currentState);
                         }
                     }
                     if (botInstance.DiscordClient == null && botInstance == null)
                     {
                         break;
                     }
                     // Check every half a second
                     await Task.Delay(500);
                 }
             });
        }

        private void UpdateLabel(ConnectionState currentState)
        {
            if (label2.InvokeRequired)
            {
                label2.Invoke(new Action(() => UpdateLabel(currentState)));
                return;
            }

            string statusMessage = GetConnectionStatusMessage(currentState);

            // Update the label only if the status has changed
            if (label2.Text != statusMessage)
            {
                switch (currentState)
                {

                    case ConnectionState.Connecting:
                        label2.Text = " Connecting...";
                        label2.ForeColor = System.Drawing.Color.YellowGreen;
                        break;
                    case ConnectionState.Connected:
                        label2.Text = " Bot Connected...";
                        label2.ForeColor = System.Drawing.Color.LimeGreen;
                        break;
                    case ConnectionState.Disconnecting:
                        label2.Text = statusMessage;
                        label2.ForeColor = System.Drawing.Color.Orange;
                        break;
                    default:
                        // Handle unexpected ConnectionState values here
                        label2.Text = " Waiting...";
                        label2.ForeColor = System.Drawing.Color.Yellow;
                        break;
                }


            }
        }

        private string GetConnectionStatusMessage(ConnectionState connectionState)
        {
            switch (connectionState)
            {
                case ConnectionState.Connected:
                    return " Bot Connected...";
                case ConnectionState.Connecting:
                    return " Connecting...";
                case ConnectionState.Disconnected:
                    return " Not Connected...";
                case ConnectionState.Disconnecting:
                    return " No Connection...";
                default:
                    return "[ERROR: Check Settings]";
            }
        }
        //Get bot name
        private async Task GetBotName()
        {

            if (botInstance.DiscordClient != null)
            {
                if (nsLabel20.InvokeRequired)
                {
                    nsLabel20.Invoke((MethodInvoker)delegate
                    {
                        nsLabel20.Value2 = " " + botInstance.DiscordClient.CurrentUser?.Username;
                    });
                }
                else
                {
                    nsLabel20.Value2 = " " + botInstance.DiscordClient.CurrentUser?.Username;
                }
            }

        }

        private async Task GetBotStatus()
        {

            if (nsLabel22.InvokeRequired)
            {
                nsLabel22.Invoke((MethodInvoker)delegate
                {

                    if (botInstance.DiscordClient != null)
                    {
                        nsLabel22.Value2 = " " + botInstance.DiscordClient.CurrentUser?.Status;
                    }
                });
            }
            else
            {

                if (botInstance.DiscordClient != null)
                {
                    nsLabel22.Value2 = " " + botInstance.DiscordClient.CurrentUser?.Status;
                }
            }

        }


        private async Task OnBotConnected()
        {
            // Pass the instance of Form1 to MainProgram, run initial methods and logic (MAGIC)
            botInstance.SetForm1Instance(this);
            // Output console logs to GUI's console log text area
            await Outputcmd();
            // Initialize MongoDB
            await botInstance.InitializeMongoDBAsync();

            // Get the list of server IDs the bot is connected to to access later
            var serverIds = botInstance.DiscordClient.Guilds.Select(g => g.Id);

            // If MongoDB is initialized successfully, proceed with other tasks
            var tasks = new List<Task>
    {
        LoadAvatarIntoPictureBox(botInstance.DiscordClient.CurrentUser.GetAvatarUrl(ImageFormat.Auto, 256)),
        Getbotdeets(),
        GetBotStatus(),
        GetBotName(),
        botInstance.PopulateComboBoxWelcomeSettings(),
        botInstance.PopulateComboBoxWithRoles(),
        botInstance.PopulateComboBoxWithChannels(),
        botInstance.PopulateListViewWithBannedUsers(),
        botInstance.PopulateListViewWithConnectedUsersAsync(),
        botInstance.LoadAndScheduleMutesAsync(),
        botInstance.RemoveOldWarnings(),
        botInstance.SetupWarningCleanup()
    };


            await Task.WhenAll(tasks);

        }

        private async Task LoadAvatarIntoPictureBox(string avatarUrl)
        {
            // Load the avatar (Eventually cache)
            if (!string.IsNullOrEmpty(avatarUrl))
            {
                // Download the image(cache????)
                using (WebClient client = new WebClient())
                {
                    byte[] imageData = client.DownloadData(avatarUrl);

                    using (MemoryStream ms = new MemoryStream(imageData))
                    {
                        System.Drawing.Image avatarImage = System.Drawing.Image.FromStream(ms);
                        pictureBox1.Image = avatarImage;
                    }
                }
            }
        }
        private void nsButton1_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to save your settings?", "VoidBot Discord Bot [GUI]", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                var rolesDictionary = botInstance.NewRoles;
                // Access to inisettings.cs functions
                var INI2 = new Voidbot_Discord_Bot_GUI.inisettings();
                INI2.Path = Application.StartupPath + @"\UserCFG.ini";
                INI2.WriteValue("Settings", "ServerID", ServerID.Text, INI2.GetPath());
                INI2.WriteValue("Settings", "YoutubeAPIKey", YoutubeAPIKey.Text, INI2.GetPath());
                INI2.WriteValue("Settings", "YoutubeAppName", YoutubeAppName.Text, INI2.GetPath());
                INI2.WriteValue("Settings", "DiscordBotToken", DiscordBotToken.Text, INI2.GetPath());
                string selectedRoleName;
                string selectedChannelName;
                selectedRoleName = nsComboBox7.SelectedItem?.ToString();
                if (selectedRoleName != null && rolesDictionary.TryGetValue(selectedRoleName, out ulong selectedRoleId))
                {
                    INI2.WriteValue("Settings", "AutoRole", selectedRoleId.ToString(), INI2.GetPath());
                    INI2.WriteValue("Settings", "AutoRoleName", nsComboBox7.SelectedItem.ToString(), INI2.GetPath());
                    Console.WriteLine($"Selected Role ID for AutoRole: {selectedRoleId}");
                }
                else
                {
                    Console.WriteLine("Failed to retrieve the selected role ID for AutoRole.");
                }
                selectedRoleName = nsComboBox8.SelectedItem?.ToString();
                if (selectedRoleName != null && rolesDictionary.TryGetValue(selectedRoleName, out selectedRoleId))
                {
                    INI2.WriteValue("Settings", "ModeratorRole", selectedRoleId.ToString(), INI2.GetPath());
                    INI2.WriteValue("Settings", "ModeratorRoleName", nsComboBox8.SelectedItem.ToString(), INI2.GetPath());
                    Console.WriteLine($"Selected Role ID for ModeratorRole: {selectedRoleId}");
                }
                else
                {
                    Console.WriteLine("Failed to retrieve the selected role ID for ModeratorRole.");
                }
                selectedRoleName = nsComboBox9.SelectedItem?.ToString();
                if (selectedRoleName != null && rolesDictionary.TryGetValue(selectedRoleName, out selectedRoleId))
                {
                    INI2.WriteValue("Settings", "StreamerRole", selectedRoleId.ToString(), INI2.GetPath());
                    INI2.WriteValue("Settings", "StreamerRoleName", nsComboBox9.SelectedItem.ToString(), INI2.GetPath());
                    Console.WriteLine($"Selected Role ID for StreamerRole: {selectedRoleId}");
                }
                else
                {
                    Console.WriteLine("Failed to retrieve the selected role ID for StreamerRole.");
                }
                selectedChannelName = nsComboBox10.SelectedItem?.ToString();
                if (selectedChannelName != null)
                {
                    INI2.WriteValue("Settings", "VerifyWelcomeChannel", nsComboBox10.SelectedItem.ToString(), INI2.GetPath());
                    Console.WriteLine($"Selected name for Welcome Channel: {selectedChannelName}");
                }
                else
                {
                    Console.WriteLine("Failed to retrieve the selected Welcome Channel.");
                }
                selectedChannelName = nsComboBox11.SelectedItem?.ToString();
                if (selectedChannelName != null)
                {
                    INI2.WriteValue("Settings", "RulesChannel", nsComboBox11.SelectedItem.ToString(), INI2.GetPath());
                    Console.WriteLine($"Selected name for Rules Channel: {selectedChannelName}");
                }
                else
                {
                    Console.WriteLine("Failed to retrieve the selected Rules Channel.");
                }
                INI2.WriteValue("Settings", "BotNickname", BotNickname.Text, INI2.GetPath());
                INI2.WriteValue("Settings", "MongoClientLink", MongoClientLink.Text, INI2.GetPath());
                INI2.WriteValue("Settings", "MongoDBName", MongoDBName.Text, INI2.GetPath());
            }
            else if (dialogResult == DialogResult.No)
            {
                // do nothing, or something else? Don't need this but it's here if needed eventually.
            }
        }
        // Start button, handles starting the bot, and setting button enabled state based on await time
        private async void nsButton2_Click(object sender, EventArgs e)
        {
            if (!botInstance.isBotRunning)
            {


                string configFile = Path.Combine(Application.StartupPath, "UserCFG.ini");
                if (!File.Exists(configFile))
                {
                    MessageBox.Show("UserCFG.ini file not found. Please create the configuration file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string connectionString = UserSettings(configFile, "MongoClientLink");
                string databaseName = UserSettings(configFile, "MongoDBName");
                string botToken = UserSettings(configFile, "DiscordBotToken");
                string serverID = UserSettings(configFile, "ServerID");
                botInstance.UpdateMongoDBSettings(connectionString, databaseName);
                if (botToken.Length < 58)
                {
                    MessageBox.Show("Invalid Discord Bot Token, Please use a valid Bot Token.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Update UI to show connection status
                Invoke(new Action(() =>
                {
                    timer1.Enabled = true;
                    label2.Text = "Connecting...";
                    label2.ForeColor = System.Drawing.Color.YellowGreen;
                    consolebtnSend.Enabled = true;
                    nsButton11.Enabled = true;
                    nsButton2.Enabled = false;
                    nsComboBox1.Enabled = true;
                    nsButton3.Enabled = false;
                    YoutubeAPIKey.Enabled = false;
                    YoutubeAppName.Enabled = false;
                    DiscordBotToken.Enabled = false;
                }));

                // Delay and then re-enable nsButton3
                if (nsButton3?.IsHandleCreated == true)
                {
                    Task.Run(async () =>
                    {
                        await Task.Delay(4000);
                        nsButton3.BeginInvoke(new Action(() =>
                        {
                            nsButton3.Enabled = true;
                        }));
                    });
                }

                // Start the bot
                await botInstance.StartBotAsync();
            }
        }

        // stop bot method
        private async void nsButton3_Click(object sender, EventArgs e)
        {
            if (botInstance.isBotRunning)
            {
                Invoke(new Action(() =>
                {
                    // UI update logic here
                    timer1.Enabled = false;
                    label2.Text = " Disconnecting...";
                    label2.ForeColor = System.Drawing.Color.Orange;
                    consolebtnSend.Enabled = false;
                    nsButton11.Enabled = false;
                    nsButton3.Enabled = false;
                    nsComboBox1.Enabled = false;
                    pictureBox1.Image = Resources._2451296;
                    nsLabel22.Value2 = "";
                    nsLabel20.Value2 = "";
                    YoutubeAPIKey.Enabled = true;
                    YoutubeAppName.Enabled = true;
                    DiscordBotToken.Enabled = true;

                }));

                if (nsComboBox1?.IsHandleCreated == true)
                {
                    nsComboBox1.BeginInvoke(new Action(() =>
                    {
                        nsComboBox1.ClearComboBoxItems();

                    }));
                }

                // Attempt to stop the bot
                await botInstance.StopBot();

                if (nsButton2?.IsHandleCreated == true)
                {

                    Task.Run(async () =>
                    {
                        await Task.Delay(4000);

                        nsButton2.BeginInvoke(new Action(() =>
                        {
                            nsButton2.Enabled = true;
                        }));
                    });


                }


            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            string userfile;
            userfile = @"\UserCFG.ini";
            string userconfigs;
            userconfigs = Application.StartupPath + @"\UserCFG.ini";
            if (!System.IO.File.Exists(userconfigs))
            {

                MessageBox.Show("UserCFG.ini not found in Application Directory, Creating file...");

                Module1.SaveToDisk("UserCFG.ini", Application.StartupPath + @"\UserCFG.ini");

            }
            var INI2 = new Voidbot_Discord_Bot_GUI.inisettings();
            INI2.Path = Application.StartupPath + @"\UserCFG.ini";
            // Read the AutoRun setting from the INI file
            string autoRunSetting = UserSettings(Application.StartupPath + userfile, "AutoRun");
            // If AutoRun is set to "True", start the bot and check the checkbox
            if (!string.IsNullOrEmpty(autoRunSetting) && autoRunSetting.ToLower() == "true")
            {
                nsCheckBox4.Checked = true;

                // Start the bot automatically
                botInstance.StartBotAsync();
                nsButton2.Enabled = false;
                nsButton3.Enabled = true;
                nsButton7.Enabled = true;
                nsButton8.Enabled = true;
                nsButton9.Enabled = true;
                nsButton10.Enabled = true;
                nsButton11.Enabled = true;
                nsButton12.Enabled = true;
                consolebtnSend.Enabled = true;
            }
            else
            {
                nsCheckBox4.Checked = false;
            }

            if (string.IsNullOrEmpty(UserSettings(Application.StartupPath + userfile, "YoutubeAPIKey")))
            {
                YoutubeAPIKey.Text = "Input API Key...";
            }
            else
            {
                YoutubeAPIKey.Text = UserSettings(Application.StartupPath + userfile, "YoutubeAPIKey");
            }

            if (string.IsNullOrEmpty(UserSettings(Application.StartupPath + userfile, "YoutubeAppName")))
            {
                YoutubeAppName.Text = "Input Youtube API Keys App Name...";
            }
            else
            {
                YoutubeAppName.Text = UserSettings(Application.StartupPath + userfile, "YoutubeAppName");
            }

            if (string.IsNullOrEmpty(UserSettings(Application.StartupPath + userfile, "DiscordBotToken")))
            {
                DiscordBotToken.Text = "Input API Key...";
            }
            else
            {

                DiscordBotToken.Text = UserSettings(Application.StartupPath + userfile, "DiscordBotToken");
            }
            if (string.IsNullOrEmpty(UserSettings(Application.StartupPath + userfile, "ServerID")))
            {
                ServerID.Text = "Run bot to Auto-populate and save Server ID...";
            }
            else
            {
                ServerID.Text = UserSettings(Application.StartupPath + userfile, "ServerID");
            }

            if (string.IsNullOrEmpty(UserSettings(Application.StartupPath + userfile, "InviteLink")))
            {
                InviteLink.Text = "Permanent Discord Invite link...";
            }
            else
            {
                InviteLink.Text = UserSettings(Application.StartupPath + userfile, "InviteLink");
            }
            if (string.IsNullOrEmpty(UserSettings(Application.StartupPath + userfile, "VerifyWelcomeChannel")))
            {
                nsComboBox10.Items.Add("None Selected");
                nsComboBox10.SelectedItem = "None Selected";
            }
            else
            {
                if (!nsComboBox10.Items.Contains(UserSettings(Application.StartupPath + userfile, "VerifyWelcomeChannel")))
                {
                    nsComboBox10.Items.Add(UserSettings(Application.StartupPath + userfile, "VerifyWelcomeChannel"));
                    nsComboBox10.SelectedItem = UserSettings(Application.StartupPath + userfile, "VerifyWelcomeChannel");
                }
            }
            if (string.IsNullOrEmpty(UserSettings(Application.StartupPath + userfile, "RulesChannel")))
            {
                nsComboBox11.Items.Add("None Selected");
                nsComboBox11.SelectedItem = "None Selected";
            }
            else
            {
                if (!nsComboBox11.Items.Contains(UserSettings(Application.StartupPath + userfile, "RulesChannel")))
                {
                    nsComboBox11.Items.Add(UserSettings(Application.StartupPath + userfile, "RulesChannel"));
                    nsComboBox11.SelectedItem = UserSettings(Application.StartupPath + userfile, "RulesChannel");
                }
            }
            if (string.IsNullOrEmpty(UserSettings(Application.StartupPath + userfile, "AutoRoleName")))
            {
                nsComboBox7.Items.Add("None Selected");
                nsComboBox7.SelectedItem = "None Selected";
            }
            else
            {
                if (!nsComboBox7.Items.Contains(UserSettings(Application.StartupPath + userfile, "AutoRoleName")))
                {
                    nsComboBox7.Items.Add(UserSettings(Application.StartupPath + userfile, "AutoRoleName"));
                    nsComboBox7.SelectedItem = UserSettings(Application.StartupPath + userfile, "AutoRoleName");
                }
            }
            if (string.IsNullOrEmpty(UserSettings(Application.StartupPath + userfile, "ModeratorRoleName")))
            {
                nsComboBox8.Items.Add("None Selected");
                nsComboBox8.SelectedItem = "None Selected";
            }
            else
            {
                if (!nsComboBox8.Items.Contains(UserSettings(Application.StartupPath + userfile, "ModeratorRoleName")))
                {
                    nsComboBox8.Items.Add(UserSettings(Application.StartupPath + userfile, "ModeratorRoleName"));
                    nsComboBox8.SelectedItem = UserSettings(Application.StartupPath + userfile, "ModeratorRoleName");
                }
            }
            if (string.IsNullOrEmpty(UserSettings(Application.StartupPath + userfile, "StreamerRoleName")))
            {
                nsComboBox9.Items.Add("None Selected");
                nsComboBox9.SelectedItem = "None Selected";
            }
            else
            {
                if (!nsComboBox9.Items.Contains(UserSettings(Application.StartupPath + userfile, "StreamerRoleName")))
                {
                    nsComboBox9.Items.Add(UserSettings(Application.StartupPath + userfile, "StreamerRoleName"));
                    nsComboBox9.SelectedItem = UserSettings(Application.StartupPath + userfile, "StreamerRoleName");
                }
            }
            if (string.IsNullOrEmpty(UserSettings(Application.StartupPath + userfile, "BotNickname")))
            {
                BotNickname.Text = "VoidBot";
            }
            else
            {

                BotNickname.Text = UserSettings(Application.StartupPath + userfile, "BotNickname");
            }
            if (string.IsNullOrEmpty(UserSettings(Application.StartupPath + userfile, "MongoClientLink")))
            {
                MongoClientLink.Text = "mongodb+srv://USERNAME:12sds245s8sd@YOURcluster.qissfuu.mongodb.net/?retryWrites=true&w=majority&appName=DBNAMEcluster";
            }
            else
            {

                MongoClientLink.Text = UserSettings(Application.StartupPath + userfile, "MongoClientLink");
            }
            if (string.IsNullOrEmpty(UserSettings(Application.StartupPath + userfile, "MongoDBName")))
            {
                MongoDBName.Text = "DBNAMEcluster";
            }
            else
            {

                MongoDBName.Text = UserSettings(Application.StartupPath + userfile, "MongoDBName");
            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;


                this.WindowState = FormWindowState.Minimized;
                notifyIcon1.Visible = true;
                this.Hide();
            }
        }
        // Usersettings handler
        public string UserSettings(string File, string Identifier)
        {
            using var S = new System.IO.StreamReader(File);
            string Result = "";
            while (S.Peek() != -1)
            {
                string Line = S.ReadLine();
                if (Line.ToLower().StartsWith(Identifier.ToLower() + "="))
                {
                    Result = Line.Substring(Identifier.Length + 1);
                }
            }
            return Result;

        }

        private void nsCheckBox1_CheckedChanged(object sender)
        {
            bool showPassword = nsCheckBox1.Checked;
            YoutubeAPIKey.UseSystemPasswordChar = !showPassword;
            YoutubeAppName.UseSystemPasswordChar = !showPassword;
            DiscordBotToken.UseSystemPasswordChar = !showPassword;
            MongoClientLink.UseSystemPasswordChar = !showPassword;
            MongoDBName.UseSystemPasswordChar = !showPassword;
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("explorer.exe", "https://github.com/V0idpool");
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("explorer.exe", "https://buymeacoffee.com/voidbot");
        }

        private void nsButton4_Click(object sender, EventArgs e)
        {
            botConsoleView.Text = null;
        }
        private void nsButton5_Click(object sender, EventArgs e)
        {

            this.WindowState = FormWindowState.Minimized;
            notifyIcon1.Visible = true;
            this.Hide();
        }
        // System tray notify icon
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {

                this.Show();
                this.WindowState = FormWindowState.Normal;


                notifyIcon1.Visible = false;
            }
        }
        // Notify icon tool strip display
        private void openBotPanelToolStripMenuItem_Click(object sender, EventArgs e)
        {

            this.Show();
            this.WindowState = FormWindowState.Normal;


            notifyIcon1.Visible = false;
        }

        private void closeBotToolStripMenuItem_Click(object sender, EventArgs e)
        {

            Application.Exit();
        }

        private void SetNotifyIconTooltip()
        {
            string userfile = @"\UserCFG.ini";
            string botNickname = "VoidBot Discord Bot Running in Tray: " + UserSettings(Application.StartupPath + userfile, "BotNickname");

            string title = string.IsNullOrEmpty(botNickname) ? "VoidBot Discord Bot Running in Tray: Waiting..." : botNickname;

            if (botInstance.DiscordClient != null)
            {
                notifyIcon1.Text = title;
                notifyIcon1.ShowBalloonTip(1000, title, title, ToolTipIcon.Info);
            }
            else
            {
                //do nothing
            }

            // Check if the bot is stopped
            if (botInstance.DiscordClient == null)
            {
                notifyIcon1.Text = "VoidBot Discord Bot Running in Tray: Waiting...";
                notifyIcon1.ShowBalloonTip(1000, "VoidBot Discord Bot Running in Tray: Waiting...", "VoidBot Discord Bot Running in Tray: Waiting...", ToolTipIcon.Info);

            }


        }

        private void notifyIcon1_MouseMove(object sender, MouseEventArgs e)
        {

            SetNotifyIconTooltip();

        }

        private async void consolebtnSend_Click(object sender, EventArgs e)
        {
            if (botInstance.DiscordClient != null)
            {
                // Get the message from the TextBox
                string messageToSend = commandInputConsoleview.Text;

                if (!string.IsNullOrEmpty(messageToSend))
                {
                    string channelName = nsComboBox1.Text;
                    await botInstance.SendMessageToDiscord(messageToSend, channelName);
                    commandInputConsoleview.Text = null;
                }
                else
                {
                    MessageBox.Show("Please enter a message before sending.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        // helper method to get the channels id by its name
        private async Task<ulong> GetChannelIdByName(string channelName)
        {

            var guild = (botInstance.DiscordClient as DiscordSocketClient)?.Guilds.FirstOrDefault();

            if (guild != null)
            {
                var channels = guild.Channels;
                var channel = channels.FirstOrDefault(c => c.Name == channelName);
                return channel?.Id ?? 0;
            }

            return 0;
        }
        private void nsButton11_Click(object sender, EventArgs e)
        {
            string selectedChannelName = nsComboBox1.SelectedItem?.ToString();

            if (!string.IsNullOrEmpty(selectedChannelName))
            {
                // Get the selected number of messages to purge from the dropdown box
                if (int.TryParse(nsComboBox2.Text?.ToString(), out int messagesToPurge))
                {
                    DialogResult result = MessageBox.Show($"Are you sure you want to purge {messagesToPurge} messages in {selectedChannelName}?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        HandlePurgeForChannel(selectedChannelName, messagesToPurge);
                    }
                }
                else
                {
                    MessageBox.Show("Please select a valid number of messages to purge.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a channel from the dropdown box.", "Channel Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async Task HandlePurgeForChannel(string channelName, int messagesToPurge)
        {
            ulong channelId = await GetChannelIdByName(channelName);
            if (channelId != 0)
            {
                await HandlePurgeForChannel(channelId, messagesToPurge);
            }
            else
            {
                MessageBox.Show($"Channel ID not found for the selected channel name: {channelName}", "Channel ID Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task HandlePurgeForChannel(ulong channelId, int messagesToPurge)
        {
            var channel = botInstance.DiscordClient.GetChannel(channelId) as ISocketMessageChannel;
            Random rnd = new Random();

            if (channel != null)
            {
                // Fetch messages and filter out those older than two months
                var messages = await channel.GetMessagesAsync(messagesToPurge).FlattenAsync();
                var messagesToDelete = messages.Where(m => (DateTimeOffset.Now - m.CreatedAt).TotalDays < 60);

                int deletedCount = 0;

                foreach (var message in messagesToDelete)
                {
                    // Add a random delay between 1 and 5 seconds before deleting each message to avoid rate limits
                    await Task.Delay(rnd.Next(1000, 5001));
                    await message.DeleteAsync();
                    deletedCount++;
                }
                await channel.SendMessageAsync($"Purged {deletedCount} messages.");
                Console.WriteLine($"Successfully purged {deletedCount} messages.");
            }
        }
        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("explorer.exe", "https://discord.gg/nsSpGJ5saD");
        }
        // Set bot to autorun on start or not
        private void nsCheckBox4_CheckedChanged(object sender)
        {
            var isAutoRunEnabled = nsCheckBox4.Checked;


            var ini = new Voidbot_Discord_Bot_GUI.inisettings();
            ini.Path = Application.StartupPath + @"\UserCFG.ini";

            ini.WriteValue("Settings", "AutoRun", isAutoRunEnabled.ToString(), ini.GetPath());
        }
        // Global Ban System Beta (Not used) Left this as a basic example
        private void RemoveFromGlobalBans(string username, string userId)
        {
            try
            {
                string filePath = "bans_global.txt";
                var lines = File.ReadAllLines(filePath).ToList();
                string entryToRemove = $"Username: {username} | UserID: {userId}";
                lines.Remove(entryToRemove);

                File.WriteAllLines(filePath, lines);
                Console.WriteLine($"Entry removed from {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing entry from bans_global.txt: {ex.Message}");
            }
        }
        // Global Ban System Beta (Not used) Left this as a basic example
        private void SaveGlobalBanList(string content)
        {
            try
            {
                string filePath = "bans_global.txt";
                if (!File.Exists(filePath) || !File.ReadLines(filePath).Contains(content))
                {
                    System.IO.File.AppendAllText(filePath, content + Environment.NewLine);
                    Console.WriteLine($"Content saved to {filePath}");
                }
                else
                {
                    Console.WriteLine($"Content already exists in {filePath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving to text file: {ex.Message}");
            }
        }
        private void nsListView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                // If not on the UI thread, invoke this method on the UI thread
                Invoke(new Action(() => nsListView1_SelectedIndexChanged(sender, e)));
                return;
            }


            if (nsListView1.SelectedItems != null && nsListView1._SelectedItems.Count > 0)
            {
                var selectedItem = nsListView1.SelectedItems[0];

                // Debugging: Output the subitem count to understand the awful structure of NSListView LOL
                // Console.WriteLine($"Subitem count: {selectedItem.SubItems.Count}");

                if (nsListView2.SelectedItems != null && selectedItem.SubItems.Count >= 3)
                {
                    label6.Text = selectedItem.SubItems[0].Text; // username is in the first subitem
                    label7.Text = selectedItem.SubItems[1].Text; // ID is in the second subitem
                    nsTextBox1.Text = selectedItem.SubItems[2].Text; // Reason is in the third subitem
                }
                else
                {
                    // Handle the case where there are not enough subitems
                    label6.Text = "";
                    label7.Text = "";
                    nsTextBox1.Text = "";
                }
            }
            else
            {
                // Handle the case where no item is selected
                label6.Text = "";
                label7.Text = "";
                nsTextBox1.Text = "";
            }
        }

        private void nsListView2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => nsListView2_SelectedIndexChanged(sender, e)));
                return;
            }
            if (nsListView2.SelectedItems != null && nsListView2._SelectedItems.Count > 0)
            {
                var selectedItem = nsListView2.SelectedItems[0];

                // Debugging: Output the subitem count to understand the awful structure of NSListView LOL
                //Console.WriteLine($"Subitem 2 count: {selectedItem.SubItems.Count}");

                if (selectedItem.SubItems.Count >= 2)
                {
                    label12.Text = selectedItem.SubItems[0].Text; // username is in the first subitem
                    label11.Text = selectedItem.SubItems[1].Text; // userID is in the second subitem
                }
                else
                {
                    // Handle the case where there are not enough subitems
                    label12.Text = "";
                    label11.Text = "";

                }
            }
            else
            {
                // Handle the case where no item is selected
                label12.Text = "";
                label11.Text = "";

            }
        }

        private async void nsButton7_Click(object sender, EventArgs e)
        {
            try
            {
                if (nsListView1.SelectedItems != null && nsListView1._SelectedItems.Count > 0)
                {
                    var selectedItem = nsListView1.SelectedItems[0];

                    if (ulong.TryParse(label7.Text, out ulong userId))
                    {
                        string userfile2 = @"\UserCFG.ini";
                        string GuildIDString = UserSettings(Application.StartupPath + userfile2, "ServerID");

                        if (ulong.TryParse(GuildIDString, out ulong guildId))
                        {
                            var guild = botInstance.DiscordClient.GetGuild(guildId);

                            if (guild != null)
                            {
                                var ban = await guild.GetBanAsync(userId);

                                if (ban != null)
                                {
                                    // Ask for confirmation before unbanning
                                    DialogResult result = MessageBox.Show($"Are you sure you want to unban user {ban.User.Username}?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                                    if (result == DialogResult.Yes)
                                    {

                                        Console.WriteLine($"Removing ban for user with ID: {userId}");
                                        await guild.RemoveBanAsync(userId);
                                        RemoveFromGlobalBans(label6.Text, label7.Text);

                                        // Invoke UI updates for nsListView1
                                        nsListView1.BeginInvoke(new Action(() =>
                                        {
                                            nsListView1._Items.Remove(selectedItem);
                                            nsListView1.InvalidateLayout();
                                        }));

                                        // Wait for it to finish updating before updating other UI elements
                                        await Task.Delay(500);

                                        // Perform UI updates for other UI elements (if needed)
                                        Invoke(new Action(() =>
                                        {
                                            label6.Text = "";
                                            label7.Text = "";
                                        }));
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Unban for user with ID {userId} canceled.");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine($"User with ID {userId} is not banned in the guild.");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Invalid ServerID provided.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid ServerID format.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("No User selected");
                    }
                }
                else
                {
                    MessageBox.Show("No user selected. Please select a user to unban.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Unban CMD: {ex.Message}");
            }
        }
        // Global Ban System Beta (Not used) Left this as a basic example
        private void nsButton8_Click(object sender, EventArgs e)
        {
            if (nsListView1.SelectedItems != null && nsListView1._SelectedItems.Count > 0)
            {
                SaveGlobalBanList($"Username: {label6.Text} | UserID: {label7.Text}");
            }


        }

        private async void nsButton10_Click(object sender, EventArgs e)
        {
            // Check if a user is selected
            if (nsListView2.SelectedItems != null && nsListView2._SelectedItems.Count > 0)
            {
                var selectedItem = nsListView2.SelectedItems[0];

                // Get the user ID from label11.Text
                if (ulong.TryParse(label11.Text, out ulong userId))
                {
                    // Convert string to ulong for guild ID
                    string userfile2 = @"\UserCFG.ini";
                    string GuildIDString = UserSettings(Application.StartupPath + userfile2, "ServerID");

                    if (ulong.TryParse(GuildIDString, out ulong guildId))
                    {
                        var guild = botInstance.DiscordClient.GetGuild(guildId);

                        if (guild != null)
                        {
                            var user = guild.GetUser(userId);

                            if (user != null)
                            {
                                var username = user.Username;

                                // Ask for confirmation before kicking
                                DialogResult result = MessageBox.Show($"Are you sure you want to kick user {username}?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                                if (result == DialogResult.Yes)
                                {
                                    Console.WriteLine($"Bot Kicked 🦵: {username}" + " from the server. D:");
                                    await guild.AddBanAsync(userId, 0, label11.Text);
                                    await guild.RemoveBanAsync(userId);

                                    // Invoke UI updates for other UI elements
                                    Invoke(new Action(() =>
                                    {
                                        // Update other UI elements here, e.g., labels
                                        label6.Text = "";
                                        label7.Text = "";
                                    }));

                                    nsListView2.BeginInvoke(new Action(() =>
                                    {
                                        nsListView2._Items.Remove(selectedItem);
                                        nsListView2.InvalidateLayout();
                                    }));
                                }
                                // If 'No' is pressed, do nothing
                            }
                            else
                            {
                                Console.WriteLine("[SYSTEM] Could not load user...");
                            }
                        }
                        else
                        {
                            Console.WriteLine("[SYSTEM] Could not load ServerID...");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid ServerID format.");
                    }
                }
                else
                {
                    Console.WriteLine("No user selected. Please select a user to kick");
                }
            }
            else
            {
                MessageBox.Show("No user selected. Please select a user to kick.");
            }
        }

        private async void nsButton12_Click(object sender, EventArgs e)
        {


            if (nsListView2.SelectedItems != null && nsListView2._SelectedItems.Count > 0)
            {
                var selectedItem = nsListView2.SelectedItems[0];

                // Get the user ID from label11.Text
                if (ulong.TryParse(label11.Text, out ulong userId))
                {
                    // Convert string to ulong for guild ID
                    string userfile2 = @"\UserCFG.ini";
                    string GuildIDString = UserSettings(Application.StartupPath + userfile2, "ServerID");

                    if (ulong.TryParse(GuildIDString, out ulong guildId))
                    {
                        // Ask for confirmation before kicking
                        DialogResult result = MessageBox.Show($"Are you sure you want to softban the user {userId}?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                        if (result == DialogResult.Yes)
                        {
                            var guild = botInstance.DiscordClient.GetGuild(guildId);

                            if (guild != null)
                            {
                                var ban = await guild.GetBanAsync(userId);

                                if (ban == null)
                                {

                                    Console.WriteLine($"Bot Softbanned and Pruned 🦵: {userId}" + " from the server. D:");

                                    await guild.AddBanAsync(userId, 0, label11.Text);

                                    await guild.RemoveBanAsync(userId);
                                    // Invoke UI updates for other UI elements
                                    Invoke(new Action(() =>
                                    {
                                        // Update other UI elements here, e.g., labels
                                        label6.Text = "";
                                        label7.Text = "";
                                    }));


                                    nsListView2.BeginInvoke(new Action(() =>
                                    {
                                        nsListView2._Items.Remove(selectedItem);


                                        nsListView2.InvalidateLayout();


                                    }));
                                }
                                else
                                {
                                    //do nothing, they're just being kicked
                                }
                            }
                            else
                            {
                                Console.WriteLine("Invalid ServerID provided.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid guild ID provided.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid ServerID format.");
                    }
                }
                else
                {
                    Console.WriteLine("No user selected, Please select a user to kick");
                }
            }
            else
            {
                MessageBox.Show("No user selected. Please select a user to kick.");
            }

        }

        private async void nsButton9_Click(object sender, EventArgs e)
        {
            // Check if any item is selected in the nsListView1

            if (nsListView2.SelectedItems != null && nsListView2._SelectedItems.Count > 0)
            {
                var selectedItem = nsListView2.SelectedItems[0];

                // Get the user ID from label11.Text
                if (ulong.TryParse(label11.Text, out ulong userId))
                {
                    // Convert string to ulong for guild ID
                    string userfile2 = @"\UserCFG.ini";
                    string GuildIDString = UserSettings(Application.StartupPath + userfile2, "ServerID");

                    if (ulong.TryParse(GuildIDString, out ulong guildId))
                    {
                        var guild = botInstance.DiscordClient.GetGuild(guildId);
                        DialogResult result = MessageBox.Show($"Are you sure you want to ban the user {userId}?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                        if (result == DialogResult.Yes)
                        {
                            if (guild != null)
                            {
                                var ban = await guild.GetBanAsync(userId);

                                if (ban == null)
                                {

                                    Console.WriteLine($"Bot banned: " + label12.Text + "🔨 from the server. Reason: " + nsTextBox2.Text);

                                    await guild.AddBanAsync(userId, 7, nsTextBox2.Text);
                                    // Add entry to bans_global.txt (Not used)
                                    SaveGlobalBanList($"Username: {label12.Text} | UserID: {label11.Text}");


                                    nsListView2.BeginInvoke(new Action(() =>
                                    {

                                        nsListView2.InvalidateLayout();

                                    }));
                                }
                                else
                                {
                                    Console.WriteLine($"User with ID {userId} is already banned in the Server.");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Invalid ServerID provided.");
                            }
                        }
                        else
                        {
                            // Do nothing
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid ServerID format.");
                    }
                }
                else
                {
                    Console.WriteLine("No User selected, please select a user.");
                }
            }
            else
            {
                MessageBox.Show("No user selected. Please select a user to ban.");
            }


        }
    }
}


