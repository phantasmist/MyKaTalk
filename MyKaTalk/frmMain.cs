using myLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyKaTalk
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        //global var
        const int BUF_SIZE = 512;
        const int NUMTCP = 10; //tcp 배열 개수

        Socket sock = null;   //서버 입장에서 socket, 클라이언트 입장에선 원서버로 구동
        List<tcpEx> tcp = new List<tcpEx>(); // List: mutable + generic type
        //int currClientNum = 0; // 현재 선택된 클라이언트 tcp 인덱스 + 1
        TcpListener listen = null;

        Thread threadServer = null; //Server용 Listen thread
        Thread threadRead = null; //Server용 Read thread
        Thread threadClient = null; //Client용 Read thread

        // 서버폼, 클라이언트폼: 여러 객체 생성 방지
        frmServer dlgServer = new frmServer();
        frmClient dlgClient = new frmClient();

        int serverPort = 9000;
        string connectIP = "127.0.0.1";
        int connectPort = 9000;
        bool operationMode = true; // true: server, false: client

        string sUID = "Noname";
        string sPWD = "";

        //server mode client mode 전환이 가능해야 한다

        iniClass ini = new iniClass(@".\chat.ini");

        

        private void frmMain_Load(object sender, EventArgs e)
        {
            int X = int.Parse(ini.GetPString("Location", "X", "0"));
            int Y = int.Parse(ini.GetPString("Location", "Y", "0"));
            Location = new Point(X, Y);
            int SX = int.Parse(ini.GetPString("Size", "X", "459"));
            int SY = int.Parse(ini.GetPString("Size", "Y", "472"));
            int DIST = int.Parse(ini.GetPString("Size", "DIST", "200"));
            Size = new Size(SX, SY);
            splitContainer1.SplitterDistance = DIST;

            serverPort = int.Parse(ini.GetPString("Operation", "serverPort", "9000"));
            connectPort = int.Parse(ini.GetPString("Operation", "connectPort", "9000"));
            connectIP = ini.GetPString("Operation", "connectIP", "127.0.0.1");
            //실 적용 시에는 암호화 필수
            sUID = ini.GetPString("Operation", "sUID", "USERNAME");
            sPWD = ini.GetPString("Operation", "sPWD", "");

        }


        //private void timer1_Tick(object sender, EventArgs e) //일정 시간 간격으로 텍스트 출력
        //{
        //    tbOutput.Text += strTxt;
        //    strTxt = "";
        //}

        class tcpEx
        {
            public TcpClient tp;
            public string id;
            public tcpEx(TcpClient t, string i)
            {   //생성자
                tp = t; id = i;
            }
            
        }

        //void AddText(string str) //Timer 방식
        //{
        //    strTxt += str;
        //}

        delegate void cbAddText(string str); //delegate 방식
        void AddText(string str)
        {
            if (tbOutput.InvokeRequired)
            {
                cbAddText cb = new cbAddText(AddText);
                object[] obj = { str };
                Invoke(cb, obj);
            }
            else
                tbOutput.Text += str;
        }

        //무명 메소드?
        //요거 없으면 에러 발생함
        delegate void cbAddLabel(string str);
        void AddLabel(string str)
        {
            if (statusBar.InvokeRequired)
            {
                cbAddLabel cb = new cbAddLabel(AddLabel);
                object[] obj = { str };
                Invoke(cb, obj);
            }
            else
            {
                sbClientList.DropDownItems.Add(str);
                sbClientList.Text = str;
            }

        }

        bool isAlive(Socket sck) //미완성..
        {
            if (sck == null) return false;
            if (sck.Connected == false) return false;

            bool b1 = sck.Poll(1000, SelectMode.SelectRead); //정상(false) 오류(true)
            //bool b2 = sck.Available > 0; //정상(true) 오류(false)
            if (b1) return false;

            try
            {
                sck.Send(new byte[1], 0, SocketFlags.OutOfBand);
                return true;
            }
            catch
            {
                return false;
            }
        }

        void ServerProcess() // 서버 세션 프로세스
        {         
            byte[] buf = new byte[100];
            while (true)
            {
                if (listen.Pending()) //클라이언트의 접속 요청 있으면..
                {
                    TcpClient tp = listen.AcceptTcpClient(); // 현재 받은 tcp client
                    string red = tp.Client.RemoteEndPoint.ToString(); // {IP:channel}
                    tp.Client.Send(Encoding.Default.GetBytes($"REQ:{red}")); // REQ: 연결수립통보 + my IP 수신
                    //GetString(buf, 0, n) 필수: Receive(buf)가 제대로 null 설정 안해줌
                    int n = tp.Client.Receive(buf); 
                    string sId = mylib.GetToken(1, Encoding.Default.GetString(buf, 0, n), ':'); // Out of idx-range Error
                    //if (sId == "") sId = "Anon"; // 기본 sId: "Anon"
                    if (sId == "Noname")
                    {
                        tp.Client.Send(Encoding.Default.GetBytes($"REJECT:사용자명을 변경해주세요"));
                        tp.Close();
                        AddText($"클라이언트의 접속을 거부했습니다\r\n");
                    }
                    else
                    {
                        tp.Client.Send(Encoding.Default.GetBytes($"ACK:{red}")); // 체크
                        tcp.Add(new tcpEx(tp, sId)); // List에 추가
                        AddText($"{sId}({red})로부터의 접속\r\n");
                        AddLabel(sId); // 아래 무명 메소드 방식으로 대체 가능
                        //if (InvokeRequired)
                        //{
                        //    Invoke(new MethodInvoker(delegate() 
                        //    { sbClientList.DropDownItems.Add(sId); } ));
                        //}
                    }
                    //InitServer 에서 서버 리드 스레드 생성함, 여기선 패스
                }
                Thread.Sleep(100); //while 문의 부담을 줄이기 위함
            }
        }

        //string strTxt = ""; // 타이머 방식
        void ReadProcess() //서버 사이드 읽기
        {
            byte[] buf = new byte[BUF_SIZE]; // 일관적인게 좋다?
            while (true)
            {
                for (int i = 0; i < tcp.Count; i++)
                {
                    if (tcp[i].tp.Available > 0)
                    {
                        int n = tcp[i].tp.Client.Receive(buf); //socket 멤버 Client 이용
                        AddText(Encoding.Default.GetString(buf, 0, n));
                    }
                }
                Thread.Sleep(100);
            }
        }

        void ClientProcess()
        {
            byte[] buf = new byte[BUF_SIZE];
            while (true)
            {
                if (sock.Available > 0) //if문 아규먼트도 서순이 존재함
                {                    
                    int n = sock.Receive(buf);
                    AddText(Encoding.Default.GetString(buf, 0, n));                    
                }
                Thread.Sleep(100);
            }
        }

        void initServer(int serverPort) // input은 수정할지도..
        {
            if (listen != null) listen.Stop();
            listen = new TcpListener(serverPort);
            listen.Start();

            // 서버 세션 스레드 시작
            if (threadServer != null) threadServer.Abort();
            threadServer = new Thread(ServerProcess);
            threadServer.Start();
            // 서버 리드 스레드 시작
            if (threadRead != null) threadRead.Abort();
            threadRead = new Thread(ReadProcess); //1대다 세션 위해 이동
            threadRead.Start();

            AddLabel("모두에게");
        }

        void closeServer()
        {
            if (listen != null) listen.Stop();
            if (threadServer != null) threadServer.Abort();
            if (threadRead != null) threadRead.Abort();
            if (timer1.Enabled) timer1.Stop();
        }

        
        private void mnOpenServer_Click(object sender, EventArgs e)
        {
            if(threadServer != null)
            {
                if (MessageBox.Show("서버를 다시 열겠습니까?", "", MessageBoxButtons.YesNo) == DialogResult.No) return;
                if (threadRead != null) threadRead.Abort();
                threadServer.Abort();
            }
            operationMode = true; // Server Mode
            initServer(serverPort);
            AddText($"Server started @ Port: [{serverPort}]\r\n");

        }

        
        private void mnConnect2Server_Click(object sender, EventArgs e)
        {
            
            if (sock != null) //기존 소켓, 스레드 닫기
            {
                if (MessageBox.Show("연결을 다시 수립하시겠습니까?", "", MessageBoxButtons.YesNo) == DialogResult.No) return;
                if (threadClient != null) threadClient.Abort();
                sock.Close();
            }
            operationMode = false; // 자동으로 클라이언트 모드로 진입

            byte[] buf = new byte[100];
            sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sock.Connect(connectIP, connectPort); // Connection Request
            //아래는 handshake 과정임
            int n = sock.Receive(buf);  // REQ: 연결수립통보 + myIP 수신
            string myIP = mylib.GetToken(1, Encoding.Default.GetString(buf, 0, n), ':');
            AddText($"My IP: {myIP}\r\n");
            sock.Send(Encoding.Default.GetBytes($"NAM:{sUID}"));

            n = sock.Receive(buf);  // 최종 수락/거부 통보
            string sRet = mylib.GetToken(0, Encoding.Default.GetString(buf, 0, n), ':');
            if(sRet == "REJECT")
            {
                AddText($"Server[{connectIP}:{connectPort}]로부터 접속이 거부되었습니다\r\n");
                return;
            }                    
            //클라이언트용 소켓 스레드
            threadClient = new Thread(ClientProcess);
            threadClient.Start(); // 요게 없어서 read를 못했음..
            tbInput.Text = "";
            //안내 메세지
            AddText($"Server[{connectIP}:{connectPort}]로 연결되었습니다\r\n");
        }

        private void mnCloseServer_Click(object sender, EventArgs e)
        {
            closeServer();
        }


        private void sbClientList_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            sbClientList.Text = e.ClickedItem.Text;
        }

        private void puSend2Server_Click(object sender, EventArgs e)
        {
            if (sock == null) return;
            string str = (tbInput.SelectedText == "") ? tbInput.Text : tbInput.SelectedText;
            byte[] bArr = Encoding.Default.GetBytes(str);
            sock.Send(bArr); // Client -> Server
        }

        //요거 동작 안하네;;
        private void puSend2Client_Click(object sender, EventArgs e)
        {
            string str = (tbInput.SelectedText == "") ? tbInput.Text : tbInput.SelectedText;
            byte[] bArr = Encoding.Default.GetBytes(str);
            //sbLabel에서 선택된 Client로 보내기 전에 전부 보내기?
            for (int i = 0; i < tcp.Count; i++)
            {
                tcp[i].tp.Client.Send(bArr); // Server -> Client
            }
        }

        
        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            //close threads when program ends
            closeServer();
            if (threadClient != null) threadClient.Abort();
            if (sock != null) sock.Close(); //이거 없으면 sock이 사는듯

            //프로그램 위치,크기 ini에 기록
            ini.WritePString("Location", "X", $"{Location.X}");
            ini.WritePString("Location", "Y", $"{Location.Y}");
            ini.WritePString("Size", "X", $"{Size.Width}");
            ini.WritePString("Size", "Y", $"{Size.Height}");
            ini.WritePString("Size", "DIST", $"{splitContainer1.SplitterDistance}");

            ini.WritePString("Operation", "serverPort", $"{serverPort}");
            ini.WritePString("Operation", "connectPort", $"{connectPort}");
            ini.WritePString("Operation", "connectIP", $"{connectIP}");
            //실 구현 시에는 암호화 필수
            ini.WritePString("Operation", "sUID", $"{sUID}");
            ini.WritePString("Operation", "sPWD", $"{sPWD}");
        }



        private void tbInput_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Shift && e.KeyCode == Keys.Enter)
            {   // Operation Mode에 따라 동작 변경
                if(operationMode == true) // Server Mode
                {
                    for(int i = 0; i < tcp.Count; i++)
                    {   //동명이인일 경우 동시 전송
                        if (tcp[i].id == sbClientList.Text | sbClientList.Text == "모두에게") 
                        {
                            TcpClient tp = tcp[i].tp;
                            if(isAlive(tp.Client)) // 살아있다면..
                                tp.Client.Send(Encoding.Default.GetBytes(tbInput.Text));                            
                        }
                    }
                    tbInput.Text = "";
                }
                else // Client Mode
                {
                    if(sock != null)
                    {
                        if (isAlive(sock))
                        {
                            sock.Send(Encoding.Default.GetBytes(tbInput.Text));
                            tbInput.Text = "";
                        }
                        else
                        {
                            AddText("Server Connection lost\r\n");
                            sock.Close();
                            sock = null;
                        } 
                    }                    
                }
            }
        }

        private void mnNetworkConfig_Click(object sender, EventArgs e)
        {
            frmNetConfig dlg = new frmNetConfig(serverPort, connectPort, connectIP, sUID, sPWD, operationMode);
            if(dlg.ShowDialog() == DialogResult.OK)
            {
                serverPort = int.Parse(dlg.tbServerPort.Text);
                connectPort = int.Parse(dlg.tbConnectPort.Text);
                connectIP = dlg.tbConnectIP.Text;
                sUID = dlg.tbUserID.Text;
                sPWD = dlg.tbPassword.Text;
                operationMode = dlg.rbServer.Checked;

            }

        }
    }
}
