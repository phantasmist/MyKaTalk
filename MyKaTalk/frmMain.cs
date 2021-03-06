using myLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace MyKaTalk
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        #region global var

        const int BUF_SIZE = 512;
        const int FILE_BUF = 134217728;
        //const int NUMTCP = 10; //tcp 배열 개수

        Socket sock = null;   //서버 입장에서 socket, 클라이언트 입장에선 원서버로 구동
        List<tcpEx> tcp = new List<tcpEx>(); // List: mutable + generic type
        //int currClientNum = 0; // 현재 선택된 클라이언트 tcp 인덱스 + 1
        TcpListener listen = null;

        Thread threadServer = null; //Server용 Listen thread
        Thread threadRead = null; //Server용 Read thread
        Thread threadClient = null; //Client용 Read thread

        Thread filethread = null; //FileRead thread
        string FileName = "";
        int filemode = 0;
        // static 효과 검색
        // 가능하면 상대경로 설정
        // 이거 한줄만 바뀌면 frmDB도 자동 변경됨..
        // 경로 static string connString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\phantasmist\source\repos\myDataBase.mdf;Integrated Security=True;Connect Timeout=30";
        static string connString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\snows\source\repos\Project_AttendApp\MyKaTalk\bin\Debug\myDataBase.mdf;Integrated Security=True;Connect Timeout=30";
        SqlDB sqldb = new SqlDB(connString);

        int serverPort = 9000;
        string connectIP = "127.0.0.1";
        int connectPort = 9000;
        // true: server, false: client
        bool operationMode = true; 

        string sUID = "Noname";
        string sPWD = "";
        string TODAY = "";

        iniFile ini = new iniFile(@".\chat.ini");
        #endregion

        private void Initialization()
        {
            sock = null;
            tcp = new List<tcpEx>();
            listen = null;
            threadServer = null;
            threadRead = null;
            threadClient = null;
            int serverPort = 9000;
            string connectIP = "127.0.0.1";
            int connectPort = 9000;
            bool operationMode = true;
            string sUID = "Noname";
            string sPWD = "";

            tbInput.Text = "";
            tbOutput.Text = "";

            if (threadClient != null) threadClient.Abort();
            if (threadRead != null) threadRead.Abort();
            if (threadServer != null) threadServer.Abort();
            if (filethread != null) filethread.Abort();
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

            serverPort = int.Parse(ini.GetPString("Operation", "serverPort", "9000"));
            connectPort = int.Parse(ini.GetPString("Operation", "connectPort", "9000"));
            connectIP = ini.GetPString("Operation", "connectIP", "127.0.0.1");
            //실 적용 시에는 암호화 필수
            sUID = ini.GetPString("Operation", "sUID", "USERNAME");
            sPWD = ini.GetPString("Operation", "sPWD", "");
            TODAY = dashDate(); // "_YYMMDD" 
        }


        class tcpEx
        {
            public TcpClient tp;
            public string id;
            public tcpEx(TcpClient t, string i)
            {   //생성자
                tp = t; id = i;
            }
            
        }


        delegate void cbAddText(string str, Color col); //delegate 방식
        void AddText(string str, Color col)
        {
            try
            {
                if (tbOutput.Text != null)       // Client, Server 안 켜고 그냥 창 닫은 경우의 에러 방지
                {
                    if (this.tbOutput.InvokeRequired)
                    {
                        cbAddText cb = new cbAddText(AddText);
                        object[] obj = { str, col };
                        Invoke(cb, obj);
                    }
                    else if(tbInput != null)
                    {
                        tbOutput.SelectionColor = col;
                        tbOutput.AppendText(str); // AppendText로 수정 //오토스크롤되고
                        tbOutput.SelectionColor = Color.Black;
                    }
                }
            }
            catch (Exception e1)
            {
                //MessageBox.Show(e1.Message);
            }
        }       
       
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
            if (sck == null)
            {
                Initialization();
                return false;
            }
            if (sck.Connected == false)
            {
                Initialization();
                return false;
            }

            bool b1 = sck.Poll(1000, SelectMode.SelectRead); //정상(false) 오류(true)
            bool b2 = (sck.Available == 0); //오류(true) 정상(false)    /
            if (b1 && b2)
            {
                Initialization();
                return false;
            }
            //return true;

            // 아래 더 있는 게
            try
            {
                sck.Send(new byte[1], 0, SocketFlags.OutOfBand);
                return true;
            }
            catch
            {
                Initialization();
                return false;
            }
        }

        //여기엔 isAlive가 없어도 되긴 한데..
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
                    string sId = mylib.GetToken(1, Encoding.Default.GetString(buf, 0, n), ':');
                    string sPw = mylib.GetToken(2, Encoding.Default.GetString(buf, 0, n), ':');
                    string ret = sqldb.GetString($"select password from users where name = N'{sId}'"); //
                    if (ret == null || sPw != ret) 
                    {
                        tp.Client.Send(Encoding.Default.GetBytes($"REJECT:올바른 사용자가 아닙니다"));
                        tp.Close();
                        AddText($"클라이언트의 접속을 거부했습니다\r\n", Color.Red);
                    }
                    else
                    {
                        tp.Client.Send(Encoding.Default.GetBytes($"ACK:{red}")); // 체크
                        tcp.Add(new tcpEx(tp, sId)); // List에 추가
                        AddText($"{sId}({red})로부터의 접속\r\n", Color.Blue);
                        AddLabel(sId);
                        checkAttendance(sId); //출석체크                                                   
                    }
                    //InitServer 에서 threadRead 생성/관리
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
                        int n = tcp[i].tp.Client.Receive(buf);
                        string msg = Encoding.Default.GetString(buf, 0, n);
                        if (msg.Split('|')[0].Trim() == "File")
                        {
                            if (filemode != 1)
                            {
                                FileName = Encoding.Default.GetString(buf, 0, n).Split('|')[1].Trim();
                                AddText(Encoding.Default.GetString(buf, 0, n).Split('|')[2].Trim(), Color.FromArgb(0, 255, 0));
                                AddText("\r\n", Color.FromArgb(0, 255, 0));
                                filemode = 1;
                                filethread = new Thread(FileProcess);
                                filethread.Start();
                            }
                        }
                        else if (msg.StartsWith("/EXIT:"))
                        {
                            checkExitClass(msg); break;
                        }
                        else
                        {
                            AddText(msg, Color.Black);
                        }
                        for (int k = 0; k < tcp.Count; k++)
                        {
                            TcpClient tp = tcp[k].tp;
                            if (k != i)
                            {
                                if(isAlive(tp.Client))
                                    tp.Client.Send(Encoding.Default.GetBytes(msg));
                            }
                        }
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
                try
                {
                    if (isAlive(sock) || sock.Available > 0)      // socket이 살아있다면 + socket에 읽어올 것이 있다면             ************************* 오른쪽이 0이라서 else
                    {
                        int n = sock.Receive(buf);
                        if (Encoding.Default.GetString(buf, 0, n).Split('|')[0].Trim() == "File")
                        {
                            if (filemode != 2)
                            {
                                FileName = Encoding.Default.GetString(buf, 0, n).Split('|')[1].Trim();
                                AddText(Encoding.Default.GetString(buf, 0, n).Split('|')[2].Trim(), Color.FromArgb(0, 255, 0));
                                AddText("\r\n", Color.FromArgb(0, 255, 0));
                                filemode = 2;
                                filethread = new Thread(FileProcess);
                                filethread.Start();
                            }
                        }
                        else
                        {
                            AddText(Encoding.Default.GetString(buf, 0, n), Color.Black);
                        }
                    }
                    else
                    {
                        //if (sock == null)
                        //if (sock.Available > 0) //if문 아규먼트도 서순이 존재함
                        MessageBox.Show("Server와의 연결이 끊어졌습니다.");
                        break;  // Server 연결된 상태에서 Server 끄고 Client 메시지 던지면 socket null로 탈출하는 것 방지
                    }
                }
                catch (Exception e1)
                {
                    //MessageBox.Show(e1.Message);
                    if (threadClient != null)
                    {
                        threadClient.Abort(); 
                    }
                }
                Thread.Sleep(100);
            }

        }
        void FileProcess()
        {
            byte[] buf = new byte[FILE_BUF];
            if (filemode == 1)
            {
                while (true)
                {
                    for (int i = 0; i < tcp.Count; i++)
                    {
                        if (tcp[i].tp.Available > 0)
                        {
                            int n = tcp[i].tp.Client.Receive(buf);
                            string filenamestr;
                            if (FileName != "")
                            {
                                filenamestr = @"" + FileName;
                            }
                            else
                            {
                                filenamestr = @"downFile";
                            }
                            FileStream fs = new FileStream(filenamestr, FileMode.Create);
                            fs.Write(buf, 0, n);
                            buf = Encoding.Default.GetBytes("");
                            fs.Flush();
                            fs.Close();
                            filemode = 0;
                            FileName = "";
                            filethread.Abort();
                        }
                    }
                }
            }
            else if (filemode == 2)
            {
                while (true)
                {
                    if (sock.Available > 0)
                    {
                        int n = sock.Receive(buf);
                        string filenamestr;
                        if (FileName != "")
                        {
                            filenamestr = @"" + FileName;
                        }
                        else
                        {
                            filenamestr = @"downFile";
                        }
                        FileStream fs = new FileStream(filenamestr, FileMode.Create);
                        fs.Write(buf, 0, n);
                        buf = Encoding.Default.GetBytes("");
                        fs.Flush();
                        fs.Close();
                        filemode = 0;
                        FileName = "";
                        filethread.Abort();
                    }
                }
            }
        }

        void initServer(int serverPort) // input은 수정할지도..
        {
            try
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

                AddText($"Server started @ Port: [{serverPort}]\r\n", Color.Blue);
                AddLabel("모두에게");
                addDate(); //요걸로 날짜 칼럼 신설
            }
            catch (Exception e2)
            {
                MessageBox.Show(e2.Message);
                Initialization();
            }
        }

        void closeServer()
        {
            try
            {

                if (listen != null) listen.Stop();
                if (threadServer != null) threadServer.Abort();
                if (threadRead != null) threadRead.Abort();
                if (filethread != null) filethread.Abort();
                AddText($"서버를 닫습니다. Port: [{serverPort}]\r\n", Color.Red);
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.Message);
                Initialization();
            }
            //if (timer1.Enabled) timer1.Stop();                            // 수정, timer1 지금 안 씀
        }


        private void mnOpenServer_Click(object sender, EventArgs e)
        {
            if(threadServer != null)
            {
                if (MessageBox.Show("서버를 다시 열겠습니까?", "", MessageBoxButtons.YesNo) == DialogResult.No) return;
                if (threadRead != null) threadRead.Abort();
                if (filethread != null) filethread.Abort();
                threadServer.Abort();
            }
            operationMode = true; // Server Mode
            initServer(serverPort);
            //AddText($"Server started @ Port: [{serverPort}]\r\n");
            //AddLabel() 못쓴다..
            sbClientList2.Text = "KOSTA";
            sbLabel1.Text = $"{connectIP}";
        }


        private void mnConnect2Server_Click(object sender, EventArgs e)         // try catch문 추가(Server연결 없이 Client 연결 시 뻑남)
        {
            try
            {
                if (sock != null) //기존 소켓, 스레드 닫기
                {
                    if (MessageBox.Show("연결을 다시 수립하시겠습니까?", "", MessageBoxButtons.YesNo) == DialogResult.No) return;
                    if (threadClient != null) threadClient.Abort();
                    if (filethread != null) filethread.Abort();
                    sock.Close();
                }
                operationMode = false; // 자동으로 클라이언트 모드로 진입

                byte[] buf = new byte[100];
                sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sock.Connect(connectIP, connectPort); // Connection Request
                //아래는 handshake 과정임
                int n = sock.Receive(buf);  // REQ: 연결수립통보 + myIP 수신
                string myIP = myLibrary.mylib.GetToken(1, Encoding.Default.GetString(buf, 0, n), ':');
                AddText($"My IP: {myIP}\r\n", Color.Blue);
                sock.Send(Encoding.Default.GetBytes($"NAM:{sUID}:{sPWD}")); // 패스워드 추가 전송  ******

                n = sock.Receive(buf);  // 최종 수락/거부 통보
                string sRet = mylib.GetToken(0, Encoding.Default.GetString(buf, 0, n), ':');
                if (sRet == "REJECT")
                {
                    AddText($"Server[{connectIP}:{connectPort}]로부터 접속이 거부되었습니다\r\n", Color.Red);
                    return;
                }
                //클라이언트용 소켓 스레드
                threadClient = new Thread(ClientProcess);
                threadClient.Start(); // 요게 없어서 read를 못했음..
                tbInput.Text = "";
                //안내 메세지
                AddText($"Server[{connectIP}:{connectPort}]로 연결되었습니다\r\n", Color.Blue);
                sbClientList2.Text = $"{sUID}";
                sbLabel1.Text = $"{connectIP}";
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.Message);
                Initialization();
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

        //private void puSend2Server_Click(object sender, EventArgs e)
        //{
        //    if (sock == null) return;
        //    // 서버 끊기면 알아서 사리기
        //    string str = (tbInput.SelectedText == "") ? tbInput.Text : tbInput.SelectedText;
        //    byte[] bArr = Encoding.Default.GetBytes(str);
        //    sock.Send(bArr); // Client -> Server
        //}

        ////요거 동작 안하네;;
        //private void puSend2Client_Click(object sender, EventArgs e)
        //{
        //    string str = (tbInput.SelectedText == "") ? tbInput.Text : tbInput.SelectedText;
        //    byte[] bArr = Encoding.Default.GetBytes(str);
        //    //sbLabel에서 선택된 Client로 보내기 전에 전부 보내기?
        //    for (int i = 0; i < tcp.Count; i++)
        //    {
        //        if( isAlive(tcp[i].tp.Client) ) // 모두 보내기 기능이라던가 오류 발생 차단
        //            tcp[i].tp.Client.Send(bArr); // Server -> Client
        //    }
        //}


        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                //clientMode 일 경우 EXIT 보내고 종료
                if (operationMode == false)
                    clientSendExit();
                //close threads when program ends
                closeServer();
                if (threadClient != null) threadClient.Abort();
                if (filethread != null) filethread.Abort();
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
            catch (Exception e1)
            {
                MessageBox.Show(e1.Message);
                Initialization();
            }
        }



        private void tbInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift && e.KeyCode == Keys.Enter)
            {   // Operation Mode에 따라 동작 변경
                if (operationMode == true) // Server Mode
                {
                    for (int i = 0; i < tcp.Count; i++)
                    {   //동명이인일 경우 동시 전송
                        if (tcp[i].id == sbClientList.Text | sbClientList.Text == "모두에게")
                        {
                            TcpClient tp = tcp[i].tp;
                            if(isAlive(tp.Client)) // 살아있다면..
                            {
                                tp.Client.Send(Encoding.Default.GetBytes($" KOSTA : {tbInput.Text.Trim()}\r\n"));
                                
                            }                               
                        }
                    }
                    AddText($" KOSTA : {tbInput.Text.Trim()}\r\n", Color.Black); // 자기화면에 표시
                    tbInput.Text = "";
                }
                else // Client Mode
                {
                    if (sock != null)
                    {
                        if (isAlive(sock))
                        {
                            sock.Send(Encoding.Default.GetBytes($" {sUID} : {tbInput.Text.Trim()} \r\n"));
                            AddText($" {sUID} : {tbInput.Text.Trim()}\r\n", Color.Black); // 자기 화면에 기록
                            tbInput.Text = "";
                        }
                        else
                        {
                            AddText("Server Connection lost\r\n", Color.Black);
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
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                serverPort = int.Parse(dlg.tbServerPort.Text);
                connectPort = int.Parse(dlg.tbConnectPort.Text);
                connectIP = dlg.tbConnectIP.Text;
                sUID = dlg.tbUserID.Text;
                sPWD = dlg.tbPassword.Text;
                if (dlg.rbServer.Checked)
                {
                    initServer(serverPort);         // server 문제 생기면 탈출
                    if (threadServer != null)        // 문제 
                    {
                        operationMode = true; // Server Mode
                    }
                }
                else
                {
                    operationMode = false;
                }
            }

        }

        void checkAttendance(string sId)
        {
            //오늘 날짜의 출석 기록이 비어 있으면 출석시간을 기록
            string sql = $"select {TODAY} from users where name = N'{sId}'";
            string chk = sqldb.GetString(sql);
            if (chk == "") // 이거 나중에 정확하게 확인
            {   // UPDATE users SET _211018 = 1 WHERE code='1'
                sql = $"UPDATE users SET {TODAY} = N'{currTime()}' where name = N'{sId}'";
                sqldb.Run(sql);
            }
        }

        // 퇴실 확인 기능 : 서버 단에서 실행되는 함수
        // 핸드셰이크 추가할지도
        void checkExitClass(string msg)
        {
            string clientName = msg.Split(':')[1];
            // N'{}' 명심: 인코딩 관련 매크로임
            string sql = $"select {TODAY} from users where name =N'{clientName}'";
            string result = sqldb.GetString(sql);
            if (result != null) // 출석 시간이 null이 아니면 퇴실 시간을 함께 기록
            {
                string begEnd = result + '/' + currTime(); // 인코딩 오류방지용 변수
                sql = $"UPDATE users SET {TODAY} = N'{begEnd}' where name = N'{clientName}'";
                sqldb.Run(sql);
                AddText($"{clientName}이/가 퇴실하였습니다\r\n", Color.Silver);
            }

            List<int> removeIdx = new List<int>(); //제거할 인덱스 리스트
            // for 문으로 드랍다운아이템을 전부 조회하고 해당 아이템을 삭제
            for(int i = 0; i < sbClientList.DropDownItems.Count; i++)
            {
                
                if(sbClientList.DropDownItems[i].Text == clientName)
                {
                    removeIdx.Add(i);
                }
            }
            foreach(int idx in removeIdx)
            {
                sbClientList.DropDownItems.RemoveAt(idx);
            }
        }

        void mnCurrTime_Click(object sender, EventArgs e)
        {
            string ct = currDate();
            MessageBox.Show(ct);
        }

        //컬럼 '_'
        void addDate()
        {
            string sql = $"ALTER TABLE users ADD {dashDate()} NVARCHAR(20) NULL";

            sqldb.Run(sql); // 중복 있으면 추가 안됨
        }        

        // returns "211018" -> 이거 날짜 넘어가면 어떻게 처리할거냐?? 일단 패스할까..
        string currDate()
        {   // currTime ex) "2021-10-18 오후 4:14:09"
            DateTime now = DateTime.Now;
            return now.ToString("yyMMdd"); // mm:분 MM:월
        }

        string dashDate()
        {
            // currTime ex) "2021-10-18 오후 4:14:09"
            DateTime now = DateTime.Now;
            string str = "_" + now.ToString("yyMMdd");
            return str; // mm:분 MM:월
        }

        // returns "18:45"
        string currTime()
        {
            DateTime now = DateTime.Now;
            return now.ToString("HH:mm");
        }

        private void mnAddDate_Click(object sender, EventArgs e)
        {
            addDate();
        }

        private void mnShowDB_Click(object sender, EventArgs e)
        {
            frmDB dlg = new frmDB(connString);
            dlg.Show();
        }

        //학생 추가
        private void mnAddStudent_Click(object sender, EventArgs e)
        {
            frmAddStudent dlg = new frmAddStudent();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string code = dlg.tbStudentCode.Text;
                string name = dlg.tbStudentName.Text;
                string pass = dlg.tbStudentPwrd.Text;
                // N'{}' : '{}'의 문자열이 유니코드임을 말해주는 매크로
                string sql = $"INSERT INTO users (code, name, password) VALUES (N'{code}', N'{name}', N'{pass}'); ";
                sqldb.Run(sql);
            }

        }

        private void mnDEBUG_Click(object sender, EventArgs e)
        {
            string sNull = null;
            string empty = "";
            string emoty2 = string.Empty;

            if (sNull == empty)
                tbOutput.Text += "";
        }

        void clientSendExit()
        {
            if (sock == null) return;
            // 서버 끊기면 알아서 사리기
            string str = $"/EXIT:{sUID}"; //EXIT 명령어 + 사용자명
            byte[] bArr = Encoding.Default.GetBytes(str);
            if (isAlive(sock))
                sock.Send(bArr);
            // 요 앞에 주고 받는 시퀀스가 있으면 좋지만 당장은 패스..
            AddText("퇴실했습니다\r\n", Color.Silver);
        }

        // 퇴실 버튼: 오늘 날짜 칼럼에 기록이 있으면 퇴실 신호를 전송
        private void mnExitClass_Click(object sender, EventArgs e)
        {
            clientSendExit();
        }

        private void fsms_Click(object sender, EventArgs e)
        {
            string sendFile;
            string Filename;
            byte[] buf = new byte[FILE_BUF];
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "모든파일(*.*) | *.*; | 텍스트 파일(.txt) | *.txt;*.TXT | 이미지 파일(.png,.bmp,.jpg,.gif) | *.png; *.bmp; *.jpg; *.gif"; https://github.com/phantasmist/MyKaTalk.git
            if (threadClient == null && threadRead == null)
            {
                AddText("파일 전송 실패 : 연결된 사용자가 없습니다. \r\n", Color.DarkRed);
            }
            else if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                sendFile = openFileDialog1.FileName;
                Filename = Path.GetFileName(openFileDialog1.FileName);
                if (operationMode)
                {
                    if (MessageBox.Show("전송 하시겠습니까?", "파일 전송", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        for (int i = 0; i < tcp.Count; i++)
                        {
                            if(isAlive(tcp[i].tp.Client))
                                tcp[i].tp.Client.Send(Encoding.Default.GetBytes($"File | {Filename} | 서버로부터 파일전송입니다.\r\n"));
                        }
                        for (int i = 0; i < tcp.Count; i++)
                        {
                            if(isAlive(tcp[i].tp.Client))
                                tcp[i].tp.Client.SendFile(sendFile);
                        }
                    }
                }
                else
                {
                    if (isAlive(sock))
                    {
                        sock.Send(Encoding.Default.GetBytes($"File | {Filename} | 클라이언트로부터 파일전송입니다.\r\n"));
                        if (MessageBox.Show("전송 하시겠습니까?", "파일 전송", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            if (isAlive(sock))
                                sock.SendFile(sendFile);
                        }
                    }
                       

                    
                }
            }
        }

        private void tbOutput_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Process.Start(e.LinkText);
        }
    }
}
