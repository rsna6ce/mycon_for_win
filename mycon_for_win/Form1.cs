using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using SharpDX.DirectInput;
using System.Diagnostics; // Stopwatch 用
using System.Threading; // Thread 用

namespace mycon_for_win
{
    public partial class Form1 : Form
    {
        private const int UDP_PORT = 59630; // 送信先ポート番号
        private const int SEND_INTERVAL_MS = 20; // 送信間隔（20ms）
        private UdpClient udpClient; // スキャン用（初期化時にバインドしない）
        private UdpClient sendClient; // 送信用（ランダムポート）
        private DirectInput directInput;
        private Joystick joystick;
        private readonly char[] keyMessage = new char[17]; // 16キー + ハートビート
        private volatile bool isSending = false; // スレッド制御用（volatileでスレッドセーフ）
        private readonly List<string> targetIPs = new List<string>();
        private Thread sendThread; // 送信スレッド

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
                // DirectInput の初期化
                directInput = new DirectInput();
                // 接続されているジョイスティック（JY-P1R）を検索
                var joystickGuid = Guid.Empty;
                foreach (var device in directInput.GetDevices(DeviceType.Gamepad, DeviceEnumerationFlags.AllDevices))
                {
                    joystickGuid = device.InstanceGuid;
                    break; // 最初のゲームパッドを使用
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

                // ジョイスティックの初期化
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
                // スキャン用はバインドせず、nullのまま
                udpClient = null;
                // 送信用はランダムポート（0）で初期化
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
            // キー状態を'_'で初期化
            for (int i = 0; i < keyMessage.Length; i++)
            {
                keyMessage[i] = '_';
            }
        }

        private void buttonScan_Click(object sender, EventArgs e)
        {
            // buttonScanとbuttonStartを無効化
            buttonScan.Enabled = false;
            buttonStart.Enabled = false;
            listBoxIPaddrs.Items.Clear();
            targetIPs.Clear();

            try
            {
                // ローカルIPアドレスを取得し、ブロードキャストアドレスを計算
                var localIP = GetLocalIPAddress();
                if (localIP == null)
                {
                    MessageBox.Show("ネットワークインターフェースが見つかりません。");
                    buttonScan.Enabled = true;
                    buttonStart.Enabled = true;
                    return;
                }

                // Scan 開始時に UDP_PORT をバインド
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

                var ipParts = localIP.Split('.');
                var broadcastIP = $"{ipParts[0]}.{ipParts[1]}.{ipParts[2]}.255";

                // ハートビートメッセージをブロードキャスト送信
                byte[] heartbeat = Encoding.ASCII.GetBytes("________________H");
                udpClient.Send(heartbeat, heartbeat.Length, broadcastIP, UDP_PORT);

                // 応答を一定時間（2.56秒）待つ
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
                    catch (SocketException) { /* タイムアウト無視 */ }
                }

                // Scan 終了時に受信ポートを閉じる
                udpClient?.Close();
                udpClient = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"スキャンエラー: {ex.Message}");
            }
            finally
            {
                // ボタンを有効化
                buttonScan.Enabled = true;
                buttonStart.Enabled = true;
            }
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            // 選択中のIPアドレスがなく、かつローカルループバックがチェックされていない場合にエラー
            if (listBoxIPaddrs.SelectedItem == null && !checkBoxLocalLP.Checked)
            {
                MessageBox.Show("送信先IPアドレスを選択するか、ローカルループバックをチェックしてください。");
                return;
            }

            // スレッドを起動
            isSending = true;
            sendThread = new Thread(SendLoop);
            sendThread.IsBackground = true; // フォーム終了時にスレッドを自動終了
            sendThread.Start();

            // UIを更新
            buttonScan.Enabled = false;
            buttonStart.Enabled = false;
            buttonStop.Enabled = true;
            listBoxIPaddrs.Enabled = false;
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            // スレッドを終了
            isSending = false;
            sendThread?.Join(100); // 最大100ms待機してスレッド終了

            // 受信ポートが開いている場合、閉じる
            udpClient?.Close();
            udpClient = null;

            // UIを更新
            buttonScan.Enabled = true;
            buttonStart.Enabled = true;
            buttonStop.Enabled = false;
            listBoxIPaddrs.Enabled = true;
        }

        private void SendLoop()
        {
            Stopwatch sendTimer = new Stopwatch();
            sendTimer.Start();

            while (isSending)
            {
                // 最後の送信から20ms経過するまでビジーループ
                if (sendTimer.ElapsedMilliseconds < SEND_INTERVAL_MS)
                {
                    continue; // Thread.Sleep なしで高精度待機
                }

                if (joystick == null) continue;

                // ゲームパッドの状態を取得
                UpdateKeyMessage();

                // UIスレッドでメッセージを表示（スレッドセーフ）
                this.Invoke((MethodInvoker)delegate
                {
                    textBoxLatestMessage.Text = new string(keyMessage);
                });

                // キー入力がすべて '_' の場合、送信をスキップ
                if (keyMessage.All(c => c == '_'))
                {
                    sendTimer.Restart();
                    continue;
                }

                // UDPで送信
                try
                {
                    byte[] messageBytes = Encoding.ASCII.GetBytes(keyMessage);

                    // ローカルループバックに送信（チェック時のみ）
                    if (checkBoxLocalLP.Checked)
                    {
                        sendClient.Send(messageBytes, messageBytes.Length, "127.0.0.1", UDP_PORT);
                    }

                    // 選択中のIPアドレスに送信
                    string targetIP = null;
                    this.Invoke((MethodInvoker)delegate
                    {
                        targetIP = listBoxIPaddrs.SelectedItem?.ToString();
                    });
                    if (!string.IsNullOrEmpty(targetIP))
                    {
                        sendClient.Send(messageBytes, messageBytes.Length, targetIP, UDP_PORT);
                    }

                    // 送信時刻を更新
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
                // ジョイスティックの状態を取得
                var state = joystick.GetCurrentState();

                // JY-P1R のボタン配置に基づいてマッピング
                lock (keyMessage) // スレッドセーフのためにロック
                {
                    keyMessage[(int)KeyIndex.key_Upward] = state.Y == 0 ? 'U' : '_'; // 上: Y=0
                    keyMessage[(int)KeyIndex.key_Downward] = state.Y == 65535 ? 'D' : '_'; // 下: Y=65535
                    keyMessage[(int)KeyIndex.key_Left] = state.X == 0 ? 'L' : '_'; // 左: X=0
                    keyMessage[(int)KeyIndex.key_Right] = state.X == 65535 ? 'R' : '_'; // 右: X=65535
                    keyMessage[(int)KeyIndex.key_A] = state.Buttons[0] ? 'A' : '_'; // Aボタン: Buttons[0]
                    keyMessage[(int)KeyIndex.key_B] = state.Buttons[1] ? 'B' : '_'; // Bボタン: Buttons[1]
                    keyMessage[(int)KeyIndex.key_C] = state.Buttons[2] ? 'C' : '_'; // Cボタン: Buttons[2]
                    keyMessage[(int)KeyIndex.key_D] = state.Buttons[3] ? 'D' : '_'; // Dボタン: Buttons[3]
                    keyMessage[(int)KeyIndex.key_L1] = '_'; // JY-P1RにはL1なし
                    keyMessage[(int)KeyIndex.key_L2] = '_'; // JY-P1RにはL2なし
                    keyMessage[(int)KeyIndex.key_R1] = '_'; // JY-P1RにはR1なし
                    keyMessage[(int)KeyIndex.key_R2] = '_'; // JY-P1RにはR2なし
                    keyMessage[(int)KeyIndex.key_Select] = state.Buttons[5] ? 'E' : '_'; // セレクト: Buttons[5]
                    keyMessage[(int)KeyIndex.key_Start] = state.Buttons[4] ? 'T' : '_'; // スタート: Buttons[4]
                    keyMessage[(int)KeyIndex.key_1] = '_'; // JY-P1Rにはキー1なし
                    keyMessage[(int)KeyIndex.key_2] = '_'; // JY-P1Rにはキー2なし
                    keyMessage[(int)KeyIndex.key_heartbeat] = '_'; // ハートビートは常に'_'
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

        private string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return null;
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
            // スレッドを安全に終了
            isSending = false;
            sendThread?.Join(100);
            joystick?.Unacquire();
            joystick?.Dispose();
            directInput?.Dispose();
            udpClient?.Close();
            sendClient?.Close();
            base.OnFormClosing(e);
        }

        // ボタンマッピング確認用テストメソッド
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
            TestJoystickButtons(); // フォームロード時にボタンマッピングを確認
        }
    }
}