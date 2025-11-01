using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using SharpDX.DirectInput;
using System.Diagnostics;
using System.Threading;

namespace mycon_for_win
{
    public partial class Form1 : Form
    {
        private const int UDP_PORT = 59630;
        private const int SEND_INTERVAL_MS = 20;
        private UdpClient udpClient;
        private UdpClient sendClient;
        private DirectInput directInput;
        private Joystick joystick;
        private readonly char[] keyMessage = new char[17];
        private volatile bool isSending = false;
        private readonly List<string> targetIPs = new List<string>();
        private Thread sendThread;

        public Form1()
        {
            InitializeComponent();
            InitializeDirectInput();
            InitializeUDP();
            InitializeKeyMessage();
        }

        private void InitializeDirectInput()
        {
            try
            {
                directInput = new DirectInput();
                var joystickGuid = Guid.Empty;
                foreach (var device in directInput.GetDevices(DeviceType.Gamepad, DeviceEnumerationFlags.AllDevices))
                {
                    joystickGuid = device.InstanceGuid;
                    break;
                }
                if (joystickGuid == Guid.Empty)
                {
                    foreach (var device in directInput.GetDevices(DeviceType.Joystick, DeviceEnumerationFlags.AllDevices))
                    {
                        joystickGuid = device.InstanceGuid;
                        break;
                    }
                }

                if (joystickGuid == Guid.Empty)
                {
                    MessageBox.Show("ゲームパッドが接続されていません。");
                    return;
                }

                joystick = new Joystick(directInput, joystickGuid);
                joystick.Properties.BufferSize = 128;
                joystick.Acquire();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"DirectInput 初期化エラー: {ex.Message}");
            }
        }

        private void InitializeUDP()
        {
            try
            {
                udpClient = null;
                sendClient = new UdpClient(0);
                sendClient.EnableBroadcast = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"UDP 初期化エラー: {ex.Message}");
            }
        }

        private void InitializeKeyMessage()
        {
            for (int i = 0; i < keyMessage.Length; i++)
            {
                keyMessage[i] = '_';
            }
        }

        private void buttonScan_Click(object sender, EventArgs e)
        {
            buttonScan.Enabled = false;
            buttonStart.Enabled = false;
            listBoxIPaddrs.Items.Clear();
            targetIPs.Clear();

            try
            {
                var localIPs = GetLocalIPAddresses();
                if (!localIPs.Any())
                {
                    buttonScan.Enabled = true;
                    buttonStart.Enabled = true;
                    return;
                }

                try
                {
                    udpClient = new UdpClient(UDP_PORT);
                    udpClient.EnableBroadcast = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"UDP ポートバインドエラー: {ex.Message}");
                    buttonScan.Enabled = true;
                    buttonStart.Enabled = true;
                    return;
                }

                foreach (var localIP in localIPs)
                {
                    var ipParts = localIP.Split('.');
                    var broadcastIP = $"{ipParts[0]}.{ipParts[1]}.{ipParts[2]}.255";

                    byte[] heartbeat = Encoding.ASCII.GetBytes("________________H");
                    udpClient.Send(heartbeat, heartbeat.Length, broadcastIP, UDP_PORT);
                }

                udpClient.Client.ReceiveTimeout = 2560;
                var startTime = DateTime.Now;

                while ((DateTime.Now - startTime).TotalMilliseconds < 2560)
                {
                    try
                    {
                        IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                        byte[] received = udpClient.Receive(ref remoteEP);
                        if (Encoding.ASCII.GetString(received) == "H")
                        {
                            string remoteIP = remoteEP.Address.ToString();
                            if (!targetIPs.Contains(remoteIP))
                            {
                                targetIPs.Add(remoteIP);
                                listBoxIPaddrs.Items.Add(remoteIP);
                            }
                        }
                    }
                    catch (SocketException) { }
                }

                udpClient?.Close();
                udpClient = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"スキャンエラー: {ex.Message}");
            }
            finally
            {
                buttonScan.Enabled = true;
                buttonStart.Enabled = true;
            }
        }

        private List<string> GetLocalIPAddresses()
        {
            var list = new List<string>();
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(ip))
                    {
                        list.Add(ip.ToString());
                    }
                }
            }
            catch { }
            return list;
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (listBoxIPaddrs.SelectedItem == null && !checkBoxLocalLP.Checked)
            {
                MessageBox.Show("送信先IPアドレスを選択するか、ローカルループバックをチェックしてください。");
                return;
            }

            isSending = true;
            sendThread = new Thread(SendLoop);
            sendThread.IsBackground = true;
            sendThread.Start();

            buttonScan.Enabled = false;
            buttonStart.Enabled = false;
            buttonStop.Enabled = true;
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            isSending = false;
            sendThread?.Join(100);

            udpClient?.Close();
            udpClient = null;

            buttonScan.Enabled = true;
            buttonStart.Enabled = true;
            buttonStop.Enabled = false;
        }

        private void SendLoop()
        {
            Stopwatch sendTimer = new Stopwatch();
            sendTimer.Start();

            while (isSending)
            {
                if (sendTimer.ElapsedMilliseconds < SEND_INTERVAL_MS)
                {
                    continue;
                }

                if (joystick == null) continue;

                UpdateKeyMessage();

                this.Invoke((MethodInvoker)delegate
                {
                    textBoxLatestMessage.Text = new string(keyMessage);
                });

                if (keyMessage.All(c => c == '_'))
                {
                    sendTimer.Restart();
                    continue;
                }

                try
                {
                    byte[] messageBytes = Encoding.ASCII.GetBytes(keyMessage);

                    if (checkBoxLocalLP.Checked)
                    {
                        sendClient.Send(messageBytes, messageBytes.Length, "127.0.0.1", UDP_PORT);
                    }

                    string targetIP = null;
                    this.Invoke((MethodInvoker)delegate
                    {
                        targetIP = listBoxIPaddrs.SelectedItem?.ToString();
                    });
                    if (!string.IsNullOrEmpty(targetIP))
                    {
                        sendClient.Send(messageBytes, messageBytes.Length, targetIP, UDP_PORT);
                    }

                    sendTimer.Restart();
                }
                catch (Exception ex)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        MessageBox.Show($"送信エラー: {ex.Message}");
                    });
                }
            }

            sendTimer.Stop();
        }

        private void UpdateKeyMessage()
        {
            if (joystick == null) return;

            try
            {
                var state = joystick.GetCurrentState();

                lock (keyMessage)
                {
                    keyMessage[(int)KeyIndex.key_Upward] = state.Y == 0 ? 'U' : '_';
                    keyMessage[(int)KeyIndex.key_Downward] = state.Y == 65535 ? 'D' : '_';
                    keyMessage[(int)KeyIndex.key_Left] = state.X == 0 ? 'L' : '_';
                    keyMessage[(int)KeyIndex.key_Right] = state.X == 65535 ? 'R' : '_';
                    keyMessage[(int)KeyIndex.key_A] = state.Buttons[0] ? 'A' : '_';
                    keyMessage[(int)KeyIndex.key_B] = state.Buttons[1] ? 'B' : '_';
                    keyMessage[(int)KeyIndex.key_C] = state.Buttons[2] ? 'C' : '_';
                    keyMessage[(int)KeyIndex.key_D] = state.Buttons[3] ? 'D' : '_';
                    keyMessage[(int)KeyIndex.key_L1] = '_';
                    keyMessage[(int)KeyIndex.key_L2] = '_';
                    keyMessage[(int)KeyIndex.key_R1] = '_';
                    keyMessage[(int)KeyIndex.key_R2] = '_';
                    keyMessage[(int)KeyIndex.key_Select] = state.Buttons[5] ? 'E' : '_';
                    keyMessage[(int)KeyIndex.key_Start] = state.Buttons[4] ? 'T' : '_';
                    keyMessage[(int)KeyIndex.key_1] = '_';
                    keyMessage[(int)KeyIndex.key_2] = '_';
                    keyMessage[(int)KeyIndex.key_heartbeat] = '_';
                }
            }
            catch (Exception ex)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    MessageBox.Show($"ゲームパッド読み取りエラー: {ex.Message}");
                });
            }
        }

        private enum KeyIndex
        {
            key_Upward,
            key_Downward,
            key_Left,
            key_Right,
            key_A,
            key_B,
            key_C,
            key_D,
            key_L1,
            key_L2,
            key_R1,
            key_R2,
            key_Select,
            key_Start,
            key_1,
            key_2,
            key_heartbeat
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            isSending = false;
            sendThread?.Join(100);
            joystick?.Unacquire();
            joystick?.Dispose();
            directInput?.Dispose();
            udpClient?.Close();
            sendClient?.Close();
            base.OnFormClosing(e);
        }

        private void TestJoystickButtons()
        {
            if (joystick == null) return;
            try
            {
                var state = joystick.GetCurrentState();
                var buttons = state.Buttons;
                var x = state.X;
                var y = state.Y;
                string message = $"X: {x}, Y: {y}\n";
                for (int i = 0; i < buttons.Length; i++)
                {
                    message += $"Button {i}: {(buttons[i] ? "Pressed" : "Released")}\n";
                }
                message += $"Stopwatch High Resolution: {Stopwatch.IsHighResolution}";
                MessageBox.Show(message);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"テストエラー: {ex.Message}");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            TestJoystickButtons();
        }

        private void buttonAddIPAddr_Click(object sender, EventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox(
                "IPv4アドレスを入力してください（例: 192.168.1.100）",
                "手動でIPアドレスを追加",
                "",
                -1, -1);

            if (string.IsNullOrWhiteSpace(input))
                return;

            if (System.Net.IPAddress.TryParse(input.Trim(), out System.Net.IPAddress ipAddress) &&
                ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                string ipStr = ipAddress.ToString();

                if (targetIPs.Contains(ipStr) || listBoxIPaddrs.Items.Contains(ipStr))
                {
                    MessageBox.Show($"IPアドレス {ipStr} はすでに登録されています。",
                                    "重複", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                listBoxIPaddrs.Items.Add(ipStr);
                targetIPs.Add(ipStr);

                MessageBox.Show($"IPアドレス {ipStr} を追加しました。",
                                "追加完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("有効なIPv4アドレスを入力してください。\n例: 192.168.1.100",
                                "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}