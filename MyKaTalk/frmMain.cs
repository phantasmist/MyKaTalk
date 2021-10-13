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
        const int NUMTCP = 10; //tcp 배열 개수
        Socket sock = null;   //서버 입장에서 socket, 클라이언트 입장에선 원서버로 구동
        // mutable object + generic type
        List<tcpEx> tcp = new List<tcpEx>(); //TcpClient Type의 List Object 선언
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
        int operationMode = 0; // 0: server, 1: client

        //server mode client mode 전환이 가능해야 한다

        iniClass ini = new iniClass(@".\chat.ini");

        const int BUF_SIZE = 512;


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

        //int getTcpIdx() //tcp list 중 선택된 인덱스 반환
        //{
        //    string str = sbClientList.Text;
        //    for (int i = 0; i < currClientNum; i++)
        //    {
        //        if (tcp[i].Client.RemoteEndPoint.ToString() == str)
        //        {
        //            return i;
        //        }
        //    }
        //    return -1; //error code
        //}

        void ServerProcess() // 서버 세션 프로세스
        {
            byte[] buf = new byte[100];
            while (true)
            {
                if (listen.Pending()) //클라이언트의 접속 요청 있으면..
                {
                    // AcceptTcpClient(): Blocking 함수
                    TcpClient tp = listen.AcceptTcpClient(); // 현재 받은 tcp client
                    // 클라이언트한테 IP + Session 번호를 전송 -> 상대방 확인 + 이름 부여
                    tp.Client.Send(Encoding.Default.GetBytes($"REQ:{tp.Client.RemoteEndPoint.ToString()}"));
                    int n = tp.Client.Receive(buf); // buf[n:100]이 '\0'로 채워져 있음
                    // 따라서 올바르게 string 변환하려면 GetString(buf, 0, n) 사용 필수
                    string sId = Encoding.Default.GetString(buf,0,n).Split(':')[1]; // Out of idx-range Error
                    tcp.Add(new tcpEx(tp, sId)); // List에 추가
                    string wit = tp.Client.RemoteEndPoint.ToString(); // 한번 확인, 변수명 체크
                    AddText($"{sId}({wit})로부터의 접속");
                    //드랍다운 레이블에 클라이언트 추가 + 레이블로 설정
                    sbClientList.DropDownItems.Add(wit);   //크로스 스레드 작업 오류
                    sbClientList.Text = wit;
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
                        tcp[i].tp.Client.Receive(buf); //socket 멤버 Client 이용
                        AddText(Encoding.Default.GetString(buf));
                    }
                }
                Thread.Sleep(100);
            }
        }

        //void ServerProcessOld() // 서버 세션 프로세스
        //{
        //    while (true)
        //    {
        //        if (listen.Pending()) //클라이언트의 접속 요청 있으면..
        //        {
        //            // AcceptTcpClient(): Blocking 함수
        //            TcpClient tp = listen.AcceptTcpClient();
        //            tcp.Add(tp); // List에 추가
        //            // 클라이언트한테 IP + Session 번호를 전송
        //            tp.Client.Send(Encoding.Default.GetBytes($"ACK{tp.Client.RemoteEndPoint.ToString()}"));
        //            // tcp 인덱스에서 넘어가려 하면 프로세스 종료
        //            //if (currClientNum == (NUMTCP - 1)) break; 
        //            //tcp[currClientNum] = listen.AcceptTcpClient();
        //            AddText($"Server: Connection from Client [{tcp[currClientNum].Client.RemoteEndPoint.ToString()}]\r\n");
        //            //드랍다운 레이블에 클라이언트 추가 + 레이블로 설정
        //            sbClientList.DropDownItems.Add(tcp[currClientNum].Client.RemoteEndPoint.ToString());
        //            sbClientList.Text = tcp[currClientNum].Client.RemoteEndPoint.ToString();
        //            //threadRead 한개 돌려쓰기 + 복수 생성 방지
        //            if (threadRead == null)
        //            {
        //                threadRead = new Thread(ReadProcess); //1대다 세션 위해 이동
        //                threadRead.Start();
        //            }
        //            currClientNum++; //스레드 안정성 위해 여기서 변경
        //        }
        //        Thread.Sleep(100); //while 문의 부담을 줄이기 위함
        //    }
        //}


        //TODO: msg 수정
        //void ReadProcessOld() //서버 읽기 프로세스: cross block 주의
        //{
        //    // 문자열 버퍼
        //    byte[] buf = new byte[512];
        //    while (true)
        //    {   // 현재 연결된 모든 클라이언트에 대해..
        //        for (int i = 0; i < currClientNum; i++) 
        //        {   
        //            NetworkStream ns = tcp[i].GetStream(); // 이 라인 주의
        //            if (ns.DataAvailable)
        //            {
        //                int n = ns.Read(buf, 0, 512); //버퍼로 클라이언트가 보낸 내용 받기
        //                string msg = "Client: " + Encoding.Default.GetString(buf, 0, n).Trim() + "\r\n";
        //                AddText(msg);
        //            }
        //        }
        //        Thread.Sleep(100);
        //    }
        //}

        //TODO: msg 수정
        void ClientProcess()
        {
            byte[] buf = new byte[BUF_SIZE];
            while (true)
            {
                if (sock.Available > 0)
                {
                    sock.Receive(buf);        
                    AddText(Encoding.Default.GetString(buf));
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

            //timer1.Start();
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
            if(dlgServer.ShowDialog() == DialogResult.OK)
            {
                //다이얼로그에서 받은 포트 번호
                int serverPort = int.Parse(dlgServer.port);
                initServer(serverPort);
                AddText($"Server started @ Port: [{serverPort}]\r\n");

            }
        }

        //TODO: NAM -> 
        private void mnConnect2Server_Click(object sender, EventArgs e)
        {
            if(dlgClient.ShowDialog() == DialogResult.OK)
            {
                try
                {   //기존 소켓 닫고, 스레드 재설정
                    if (sock != null) sock.Close();
                    if(threadClient != null) threadClient.Abort();
                    
                    byte[] buf = new byte[100];
                    string serverIP = dlgClient.strIP;
                    string serverPort = dlgClient.port;
                    sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    sock.Connect(serverIP, int.Parse(serverPort)); // Connection Request
                    int n = sock.Receive(buf);
                    string myIP = Encoding.Default.GetString(buf, 0, n).Split(':')[1];
                    sock.Send(Encoding.Default.GetBytes($"NAM:{tbInput.Text}"));
                    //AddText($"Client: Server [{serverIP}:{serverPort}] Connection OK\r\n");
                    //클라이언트용 소켓 스레드
                    threadClient = new Thread(ClientProcess);
                    tbInput.Text = "";

                }
                catch (Exception e1)
                {
                    MessageBox.Show(e1.Message);
                }
            }
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
            if(sock == null)
            {
                //MessageBox.Show("Only Server Present");
                return;
            }
            string str = (tbInput.SelectedText == "") ? tbInput.Text : tbInput.SelectedText;
            byte[] bArr = Encoding.Default.GetBytes(str);
            sock.Send(bArr); // Client -> Server
        }

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
        }



        private void tbInput_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Shift && e.KeyCode == Keys.Enter)
            {
                // 메세지 전송 + 비우기
                sock.Send(Encoding.Default.GetBytes(tbInput.Text));
                tbInput.Text = "";
            }
        }

        
    }
}
