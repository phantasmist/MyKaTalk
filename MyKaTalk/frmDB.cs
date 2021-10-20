using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyKaTalk
{
    public partial class frmDB : Form
    {
        // 아래 openDB 프로세스를 sqldb 라이브러리 방식으로?
        SqlConnection sqlConn = new SqlConnection(); // app - db connection
        SqlCommand sqlCmd = new SqlCommand(); // 위 커넥션 기반으로 명령어 전달
        string ConnString = ""; // 

        public frmDB(string cs)
        {
            InitializeComponent();
            ConnString = cs;
        }

        void openDB()
        {
            //db open process
            sqlConn.ConnectionString = ConnString;
            sqlConn.Open();
            sqlCmd.Connection = sqlConn;
        }
        

        void drawDB()
        {
            try
            {
                string sql = "SELECT * FROM users ORDER BY code";
                sqlCmd.CommandText = sql;
                //string.Trim()으로 string 전후의 whitespace 제거
                string sCmd = sql.Trim().Substring(0, 6);
                //case sensitivity 
                if (sCmd.ToLower() == "select")
                {
                    //sql.ToLower().
                    SqlDataReader sdr = sqlCmd.ExecuteReader(); //모든 명령어 처리는 불가능함
                                                                //클리어
                    dbGrid.Rows.Clear(); dbGrid.Columns.Clear();
                    for (int i = 0; i < sdr.FieldCount; i++)
                    {
                        string s = sdr.GetName(i); //get name of ith field
                        dbGrid.Columns.Add(s, s);
                    }
                    //라인단위 처리
                    for (int i = 0; sdr.Read(); i++)
                    {
                        int rIdx = dbGrid.Rows.Add(); //한줄 추가 + Row 번호 int로 반환
                        for (int j = 0; j < sdr.FieldCount; j++)
                        {
                            object obj = sdr.GetValue(j);
                            dbGrid.Rows[rIdx].Cells[j].Value = obj;
                        }
                    }
                    sdr.Close();
                    // 소팅 기능 비활성화, 항상 addDate 다음에 활성화 되니까 ㄱㅊ
                    foreach (DataGridViewColumn column in dbGrid.Columns)
                    {
                        column.SortMode = DataGridViewColumnSortMode.NotSortable;
                    }
                }
            }
            catch (Exception e1)
            {
                return;
            }
        }

        private void sthToolStripMenuItem_Click(object sender, EventArgs e)
        {


        }

        private void frmDB_Load(object sender, EventArgs e)
        {
            openDB();
            drawDB();
        }
    }

    
}
