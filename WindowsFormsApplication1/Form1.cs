
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;      //用于开发Windows应用程序messagebox\form等
using System.IO.Ports;          //用于操作文件
using System.Collections;
using System.Text.RegularExpressions; //命名空间，包含构造和执行正则表达式的类
using System.Threading;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Diagnostics;//进程类对象
using System.IO;
using Excel;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;

namespace WindowsFormsApplication1
{
    //委托定义
    public delegate void ShowWindow();
    public delegate void HideWindow();
    public delegate void OpenPort();
    public delegate void ClosePort();
    public delegate PointF GetMainPos();
    public delegate int GetMainWidth();



    public partial class Form1 : Form
    {
       /*object类：*/

        /*TextBox 文本控件类*/
        /*Console 输入输出流类*/
        /*Exception 系统异常类*/
        /*process 进程类*/
        /*Thread 线程类*/
        public static SerialPort SerialPort1 = new SerialPort();


        Form2 F1;//实例化窗口Form2
        Form3 F3;
        Form4 F4;

        string hour_bit;
        string minute_bit;
        string second_bit;
        
        int COM_MAX = 20;//COM口号最大值
        bool Button2Status;
        static bool Button2_perss_flag = false; //定义图形按键释放标志位

        int send_data_length, receive_data_length; //记录接收\发送的字节数

        Excel.Application cel;

        //从系统的DLL文件中到处连个函数
        [DllImport("kernel32")]
        private static extern long  WritePrivateProfileString(string sention, string key, string val, string path);

        [DllImport("kernel32")]
        private static extern int   GetPrivateProfileString(string sention, string key, string def, StringBuilder retVal, int size, string path);
        string FileName = System.AppDomain.CurrentDomain.BaseDirectory + "data.ini";//文件名
        StringBuilder readdataini = new StringBuilder(256);//读出ini文件里的值



        public struct save_winsdow_s
        {
            public string CurrentPortName;
            public string CurrentBoundRate;
            public string CurrentSingleData;
            public string CurrentMulData1;
            public string CurrentMulData2;
            public string CurrentMulData3;
            public string CurrentMulData4;
            public string CurrentMulData5;
            public string CurrentMulData6;
            public string CurrentMulData7;
            public string CurrentMulData8;
            public string CurrnetRadioBut1;
            public string CurrnetRadioBut2;
            public string CurrnetRadioBut3;
            public string CurrnetRadioBut4;
            public string CurrnetRadioBut1_flag;
            public string CurrnetRadioBut2_flag;
            public string CurrnetRadioBut3_flag;
            public string CurrnetRadioBut4_flag;

        };
        save_winsdow_s savedata = new save_winsdow_s();

        public Form1()
        {
            InitializeComponent();
            cel = new Excel.Application();

            //由于串口组件双击不能添加串口接收事件，所以必须受到添加串口接收事件，如下语句。
            serialPort1.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived);
            serialPort1.Encoding = Encoding.GetEncoding("GB2312"); //支持汉字显示

            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;

            //注册窗口关闭事件
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(Form1_FormClosing);


        }




        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Enabled = true;


            for (int i = 1; i < COM_MAX; i++)
            {
                comboBox1.Items.Add("COM" + i.ToString());
            }

            //设置默认值
            comboBox1.Text = "COM1";
            comboBox2.Text = "9600";
            comboBox3.Text = "8";
            comboBox4.Text = "NONE";
            comboBox5.Text = "1";


            ovalShape1.FillColor = Color.Gray;   //初始化灯图形的颜色

            //this.BackColor = Color.Gray;

            /*串口号与波特率保存数据读出*/
            GetPrivateProfileString("portdata", "Serial_Num", "COM1", readdataini, 256, FileName);
            comboBox1.Text = Convert.ToString(readdataini);
            GetPrivateProfileString("portdata1", "BoundRate", "9600", readdataini, 256, FileName);
            comboBox2.Text = Convert.ToString(readdataini);

            /*单条发送与多条发送的保存数据读出*/
            Download_Data("portdata2", "SingleData", "", send_data);
            Download_Data("portdata3", "MulData1", "", textBox1);
            Download_Data("portdata4", "MulData2", "", textBox2);
            Download_Data("portdata5", "MulData3", "", textBox3);
            Download_Data("portdata6", "MulData4", "", textBox4);
            Download_Data("portdata7", "MulData5", "", textBox5);
            Download_Data("portdata8", "MulData6", "", textBox6);
            Download_Data("portdata9", "MulData7", "", textBox7);
            Download_Data("portdata10", "MulData8", "", textBox8);


            /*字符与HEX选项卡的保存数据读出*/
            GetPrivateProfileString("portdata11", "Radio1", "radio1_false", readdataini, 256, FileName);
            savedata.CurrnetRadioBut1_flag = Convert.ToString(readdataini);
            if (savedata.CurrnetRadioBut1_flag == "radio1_true")
                radioButton1.Checked = true; 
            else
                radioButton1.Checked = false;

            GetPrivateProfileString("portdata12", "Radio2", "radio2_false", readdataini, 256, FileName);
            savedata.CurrnetRadioBut2_flag = Convert.ToString(readdataini);
            if (savedata.CurrnetRadioBut2_flag == "radio2_true")
                radioButton2.Checked = true;
            else
                radioButton2.Checked = false;

            GetPrivateProfileString("portdata13", "Radio3", "radio3_false", readdataini, 256, FileName);
            savedata.CurrnetRadioBut3_flag = Convert.ToString(readdataini);
            if (savedata.CurrnetRadioBut3_flag == "radio3_true")
                radioButton3.Checked = true;
            else
                radioButton3.Checked = false;

            GetPrivateProfileString("portdata14", "Radio4", "radio4_false", readdataini, 256, FileName);
            savedata.CurrnetRadioBut4_flag = Convert.ToString(readdataini);
            if (savedata.CurrnetRadioBut4_flag == "radio4_true")
                radioButton4.Checked = true;
            else
                radioButton4.Checked = false;

        }

        private void Download_Data(string section, string name, string rawdata, TextBoxBase textdata)
        {
            GetPrivateProfileString(section, name, rawdata, readdataini, 256, FileName);
            textdata.Text = Convert.ToString(readdataini);

        }
        private void Form1_FormClosing(object sender, EventArgs e)
        {
            res_status();
            res_data();


            //窗口关闭时，将串口号等存储起来
            WritePrivateProfileString("portdata", "Serial_Num", savedata.CurrentPortName, FileName);
            WritePrivateProfileString("portdata1", "BoundRate", savedata.CurrentBoundRate, FileName);

            WritePrivateProfileString("portdata2", "SingleData", savedata.CurrentSingleData, FileName);
            WritePrivateProfileString("portdata3", "MulData1", savedata.CurrentMulData1, FileName);
            WritePrivateProfileString("portdata4", "MulData2", savedata.CurrentMulData2, FileName);
            WritePrivateProfileString("portdata5", "MulData3", savedata.CurrentMulData3, FileName);
            WritePrivateProfileString("portdata6", "MulData4", savedata.CurrentMulData4, FileName);
            WritePrivateProfileString("portdata7", "MulData5", savedata.CurrentMulData5, FileName);
            WritePrivateProfileString("portdata8", "MulData6", savedata.CurrentMulData6, FileName);
            WritePrivateProfileString("portdata9", "MulData7", savedata.CurrentMulData7, FileName);
            WritePrivateProfileString("portdata10", "MulData8", savedata.CurrentMulData8, FileName);

            WritePrivateProfileString("portdata11", "Radio1", savedata.CurrnetRadioBut1, FileName);
            WritePrivateProfileString("portdata12", "Radio2", savedata.CurrnetRadioBut2, FileName);
            WritePrivateProfileString("portdata13", "Radio3", savedata.CurrnetRadioBut3, FileName);
            WritePrivateProfileString("portdata14", "Radio4", savedata.CurrnetRadioBut4, FileName);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            F1 = new Form2();//C/窗口实例化
            //F1.ShowDialog();
            F1.Show();//ShowDialog()窗体模式，Show()非窗体模式
        }


        private void open_serialport_button_Click(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)//如果串口是开
            {
                try
                {
                    serialPort1.PortName = comboBox1.Text;
                    serialPort1.BaudRate = Convert.ToInt32(comboBox2.Text, 10);
                    float f = Convert.ToSingle(comboBox5.Text.Trim());
                    if (f == 0)//设置停止位
                        serialPort1.StopBits = StopBits.None;
                    else if (f == 1.5)
                        serialPort1.StopBits = StopBits.OnePointFive;
                    else if (f == 1)
                        serialPort1.StopBits = StopBits.One;
                    else if (f == 2)
                        serialPort1.StopBits = StopBits.Two;
                    else
                        serialPort1.StopBits = StopBits.One;
                    //设置数据位
                    serialPort1.DataBits = Convert.ToInt32(comboBox3.Text.Trim());
                    //设置奇偶校验位
                    string parity_bit = comboBox4.Text.Trim();
                    if (parity_bit.CompareTo("None") == 0)
                        serialPort1.Parity = Parity.None;
                    else if (parity_bit.CompareTo("Odd") == 0)
                        serialPort1.Parity = Parity.Odd;
                    else if (parity_bit.CompareTo("Even") == 0)
                        serialPort1.Parity = Parity.Even;
                    else
                        serialPort1.Parity = Parity.None;

                    serialPort1.Open();     //打开串口
                    ovalShape1.FillColor = Color.Red;
                    comboBox1.Enabled = false;//关闭使能
                    comboBox2.Enabled = false;
                    comboBox3.Enabled = false;
                    comboBox4.Enabled = false;
                    comboBox5.Enabled = false;
                    open_serialport_button.Text = "关闭串口";
                    //button2.Enabled = false;    //打开串口按钮失能
                    //button5.Enabled = true;    //关闭串口按钮使能

                }
                catch
                {
                    MessageBox.Show("串口打开失败，请检查串口！", "提示");
                    return;
                }
            }
            else
            {
                serialPort1.Close();
                ovalShape1.FillColor = Color.Gray;
                comboBox1.Enabled = true;//关闭使能
                comboBox2.Enabled = true;
                comboBox3.Enabled = true;
                comboBox4.Enabled = true;
                comboBox5.Enabled = true;
                open_serialport_button.Text = "打开串口";
            }
        }//end of function open_serialport_button_Click();

        private void received_TextChanged(object sender, EventArgs e)
        {

        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            /*
            Control.CheckForIllegalCrossThreadCalls = false;

            string str = serialPort1.ReadExisting().ToString();

            receive_data_length += str.Length;
            label13.Text = Convert.ToString(receive_data_length);

            string a = " ", b = "-";
            a = str;
            List<byte> list_table = Encoding.ASCII.GetBytes(a).ToList<byte>();//List可以存储任何数据类型
            byte[] Write_data = list_table.ToArray();//转化为HTML码
            a = (BitConverter.ToString(Write_data));//转化为16进制，并且之间加上“-”
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] == b[0])
                {
                    a = a.Remove(i, 1);
                    a = a.Insert(i, " ");
                }
            }
            if (!radioButton3.Checked)
            {
                received_data.Text += str;
            }
            else
            {
                received_data.Text += a;
            }
            */

            /****************/
            string str_receicve = "";
            byte data;  //定义一个字节的变量

            if (!radioButton3.Checked)      //如果不是数值模式，即是字符模式
            {
                str_receicve = serialPort1.ReadExisting();  //直接以字符串方式读取
                received_data.AppendText(str_receicve + " ");   //在接收区尾部添加内容  相当于received_data.Text += str;
            }
            else
            {//如果为数值接收模式
                try
                {
                    data = (byte)serialPort1.ReadByte();  //因为串口读出的数据为int（32位）型，所以需要强制类型转换
                    str_receicve = Convert.ToString(data, 16).ToUpper(); // 转换为大写
                    received_data.AppendText("0x" + (str_receicve.Length == 1 ? "0" + str_receicve : str_receicve) + "" + " ");  //0xA----->0x0A,每个字节之间空格隔开
                }
                catch
                {
 
                }
             }
            /*****************/
            //string strL = serialPort1.ReadExisting().ToString();

            receive_data_length += (str_receicve.Length / 2);
            label13.Text = Convert.ToString(receive_data_length);
            //时间戳
            if (checkBox4.Checked)
            {
                received_data.Text += "[" + label7.Text + "]";
            }
            //自动换行
            if (checkBox3.Checked)
            {
                received_data.Text += "\r\n";
            }
        }


        private void serialPort1_PinChanged(object sender, SerialPinChangedEventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        private void timer_button_Click(object sender, EventArgs e)
        {
            F3 = new Form3();//C/窗口实例化
            F3.Show();
            //F1.Show();//ShowDialog()窗体模式，Show()非窗体模式
        }

        //private void received_TextChanged_1(object sender, EventArgs e)
        //{

        //}

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button_send_data_Click(object sender, EventArgs e)
        {
            send_data_fun(send_data);
        }

        private void clear_button_Click(object sender, EventArgs e)
        {
            received_data.Clear();    //清空接收框内容
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
        
        //自动扫描串口函数/*self-define*/
        private void autosearch_serialPort_number(SerialPort usefulport,ComboBox usefulBox)
        {
            string[] mystring = new string[COM_MAX];//存放所有可用的端口
            string buffer;
            usefulBox.Items.Clear();//清除可用的端口号

            for (int i = 1; i < (COM_MAX + 1); i++)
            {
                try
                {
                    buffer = "COM" + i.ToString();  //串口号放入缓存
                    usefulport.PortName = buffer;
                    usefulport.Open(); //如果串口可以打开，执行下一步，否则，调到catch
                    mystring[i - 1] = buffer; //将可以打开的串口存放在缓存数组中，以防止有多个可用串口
                    usefulBox.Items.Add(mystring[i - 1]);
                    usefulport.Close();
                }
                catch
                {
 
                }//catch执行完成后调回for循环
            }
        }/**/

        //搜索串口按钮事件
        private void searchport_button_Click(object sender, EventArgs e)
        {
            autosearch_serialPort_number(serialPort1,comboBox1);
        }

        //图形按钮事件，开关计算器
        private void button2_Click(object sender, EventArgs e)
        {
            Process calculator = new Process();//声明一个进程类对象
            calculator.StartInfo.FileName = "calc.exe";

            if (!Button2_perss_flag)
            {
                calculator.Start();
                button2.BackgroundImage = Properties.Resources.image3;
                //Button2Status = true;
                Button2_perss_flag = true;
            }
            //if (ovalShape1.FillColor == Color.Red)
            else
            {
                calculator.Close();
                button2.BackgroundImage = Properties.Resources.image1;
                //Button2Status = false;
                Button2_perss_flag = false;
            }
        }

        /*
        private void button2_MouseHover(object sender, EventArgs e)
        {
            if (!Button2Status)
            {
                button2.BackgroundImage = Properties.Resources.image2;
            }
            else
            {
                return;//退出函数
            }
        }

        private void button2_MouseLeave(object sender, EventArgs e)
        {
            if (Button2Status)
            {
                button2.BackgroundImage = Properties.Resources.image3;
            }
            else
            {
                button2.BackgroundImage = Properties.Resources.image1; 
            }
        }
        */


        private void timer1_Tick_1(object sender, EventArgs e)
        {
            DateTime time_now = DateTime.Now;
            label7.Text = time_now.Year.ToString();
            label7.Text += "/";
            label7.Text += time_now.Month.ToString();
            label7.Text += "/";
            label7.Text += time_now.Day.ToString();
            label7.Text += " ";
            hour_bit = time_now.Hour.ToString();
            if (hour_bit.Length == 1)
            {
                label7.Text += ("0" + time_now.Hour.ToString()); //一位补0
            }
            else
            {
                label7.Text += time_now.Hour.ToString();
            }
            label7.Text += ":";
            minute_bit = time_now.Minute.ToString();
            if (minute_bit.Length == 1)
            {
                label7.Text += ("0" + time_now.Minute.ToString());
            }
            else
            {
                label7.Text += time_now.Minute.ToString();
            }
            label7.Text += ":";
            second_bit = time_now.Second.ToString();
            if (second_bit.Length == 1)
            {
                label7.Text += ("0" + time_now.Second.ToString());
            }
            else
            {
                label7.Text += time_now.Second.ToString();
            }
        }//end of timer1_click

        bool checkBox2_flag = false;
        private void checkBox2_CheckedChanged(object sender, EventArgs e)//相当于状态翻转
        {
            if (checkBox2_flag)
            {
                checkBox2_flag = false;
                timer2.Enabled = false;
            }
            else
            {
                checkBox2_flag = true;
                timer2.Enabled = true;
                timer2.Interval = System.Convert.ToInt32(numericUpDown1.Value);
            }
        }

        //定时发送
        private void timer2_Tick(object sender, EventArgs e)
        {
            byte[] data = new byte[1];
            if (serialPort1.IsOpen)
            {//如果串口开启
                if (send_data.Text != "")  //如果发送区内容非空
                {
                    string str = send_data.Text;
                    str = str.Replace(" ", ""); //去掉空格

                    if (!radioButton1.Checked)  //如果发送模式为字符格式
                    {
                        try
                        {
                            serialPort1.WriteLine(str);
                        }
                        catch//catch(Exception err)   如果出现错误
                        {
                            System.Media.SystemSounds.Beep.Play();
                            MessageBox.Show("数据写入错误，请重试！", "提示");
                            serialPort1.Close();

                        }
                    }
                    else
                    {

                        //发送模式为数字格式,需要判断数字个数的奇偶
                        //如果是奇数个，则先处理偶数个，剩余一个
                        try
                        {
                            for (int i = 0; i < (str.Length - str.Length % 2) / 2; i++)
                            {
                                data[0] = Convert.ToByte(str.Substring(i * 2, 2), 16);
                                serialPort1.Write(data, 0, 1);//从第0位写，写1个字节
                            }
                            if (str.Length % 2 != 0)  //单独处理最后一位
                            {
                                data[0] = Convert.ToByte(str.Substring(str.Length - 1, 1), 16);
                                serialPort1.Write(data, 0, 1);
                            }
                        }
                        catch
                        {
                            System.Media.SystemSounds.Beep.Play();
                            MessageBox.Show("数据写入错误，请重试！", "提示");
                            serialPort1.Close();

                        }
                    }
                }
            }
            else
            { return; }
        }

        //鼠标移动事件
        private void lable6_MouseMove(object sender, MouseEventArgs e)
        {
            label6.Text = String.Format("X:{0},Y:{1}", e.X, e.Y);
        }


        /*******************************************************************************
                                   Excel对象及描述
                --------------------------------------------------------
               |    Application     |   代表整个excel应用程序          |
                --------------------------------------------------------
               |    Workbook        |   一个工作蒲可以包含多个工作表   |
                --------------------------------------------------------
               |    Worksheet       |   Excel工作表                    |
                --------------------------------------------------------
               |    Range           |   工作表中单元格的范围           |
                --------------------------------------------------------
               |    Chart           |   用于指定创建的图表类型         |
                --------------------------------------------------------
               |    Charts          |   对工作蒲中所以图表对象的引用   |
                --------------------------------------------------------
       *********************************************************************************/
        string[] table = new string[]{"时间","电压","电流","速度","扭矩","Ia","Ib","Ic","温度"};
        int flag_n = 0;
        private void save_data_button_Click(object sender, EventArgs e)
        {
            cel.Visible = true;      //打开Excel窗口

            //添加表格
            Workbook wb = cel.Workbooks.Add(XlSheetType.xlWorksheet);
            Worksheet ws = (Worksheet)cel.ActiveSheet;
            for (int j = 2,i = 2;  i < (table.Length + 2); j++,i++)
            {
                Range rg = (Range)ws.Cells[1, i];
                rg.Value2 = table[i-2];
                rg = (Range)ws.Cells[j, 1];
                rg.Value2 = "M"+j;
                flag_n = j; 
            }
            for (int a = 2; a <= flag_n ; a++)
            {
                for (int b = 2;b < (table.Length + 2)  ;b++ )
                {
                    Range rg = (Range)ws.Cells[a, b];
                    rg.Value2 = a + b;
                }

            }
        }

        private void res_data()
        {
            savedata.CurrentMulData1 = textBox1.Text;
            savedata.CurrentMulData2 = textBox2.Text;
            savedata.CurrentMulData3 = textBox3.Text;
            savedata.CurrentMulData4 = textBox4.Text;
            savedata.CurrentMulData5 = textBox5.Text;
            savedata.CurrentMulData6 = textBox6.Text;
            savedata.CurrentMulData7 = textBox7.Text;
            savedata.CurrentMulData8 = textBox8.Text;
            savedata.CurrentSingleData = send_data.Text;
            savedata.CurrentPortName = comboBox1.Text; //保存串口号
            savedata.CurrentBoundRate = comboBox2.Text;
        }
        private void res_status()
        {
            if (radioButton1.Checked)           
                savedata.CurrnetRadioBut1 = "radio1_true";          
            else
                savedata.CurrnetRadioBut1 = "radio1_false";
            if (radioButton2.Checked)
                savedata.CurrnetRadioBut2 = "radio2_true";
            else
                savedata.CurrnetRadioBut2 = "radio2_false";
            if (radioButton3.Checked)
                savedata.CurrnetRadioBut3 = "radio3_true";
            else
                savedata.CurrnetRadioBut3 = "radio3_false";
            if (radioButton4.Checked)
                savedata.CurrnetRadioBut4 = "radio4_true";
            else
                savedata.CurrnetRadioBut4 = "radio4_false";
        }

        //多条发送时的发送按钮事件
        private void multi_send_button_Click(object sender, EventArgs e)
        {
            ButtonBase mybutton = (ButtonBase)sender;
            byte button_tag = Convert.ToByte(mybutton.Tag);   //属性里面设置Tag值作为返回值标志
            switch (button_tag)
            {
                case 0: send_data_fun(textBox1); break;
                case 1: send_data_fun(textBox2); break;                  
                case 2: send_data_fun(textBox3); break;                 
                case 3: send_data_fun(textBox4); break;
                case 4: send_data_fun(textBox5); break;
                case 5: send_data_fun(textBox6); break;
                case 6: send_data_fun(textBox7); break;
                case 7: send_data_fun(textBox8); break;

            }
        }


        public void send_data_fun(TextBoxBase mytext)
        {
            /*****************************************************************
             *****************************************************************/
            byte[] data = new byte[1]; //定义数组元素的长度为一个字节   data[0] 
            if (serialPort1.IsOpen)
            {//如果串口开启
                if (mytext.Text != "")  //如果发送区内容非空
                {
                    //if (checkBox1.Checked)  //发送新行
                    //{
                    //    mytext.Text += "\r\n";
                    //}

                    string str = mytext.Text;

                    str = str.Replace(" ", ""); //去掉空格

                    if (!radioButton1.Checked)  //如果发送模式为字符格式
                    {
                        try
                        {

                            serialPort1.WriteLine(str);
                        }
                        catch//catch(Exception err)   如果出现错误
                        {
                            System.Media.SystemSounds.Beep.Play();
                            MessageBox.Show("数据写入错误，请重试！", "提示");
                            serialPort1.Close();

                        }
                    }
                    else
                    {

                        //发送模式为数字格式,需要判断数字个数的奇偶
                        //如果是奇数个，则先处理偶数个，剩余一个
                        try
                        {
                            for (int i = 0; i < (str.Length - str.Length % 2) / 2; i++)
                            {
                                data[0] = Convert.ToByte(str.Substring(i * 2, 2), 16);
                                serialPort1.Write(data, 0, 1);//从第0位写，写1个字节
                            }
                            if (str.Length % 2 != 0)  //单独处理最后一位
                            {
                                data[0] = Convert.ToByte(str.Substring(str.Length - 1, 1), 16);
                                serialPort1.Write(data, 0, 1);
                            }
                        }
                        catch 
                        {
                            System.Media.SystemSounds.Beep.Play();
                            MessageBox.Show("数据写入错误，请重试！", "提示");
                            serialPort1.Close();

                        }

                        /***************************************************************/
                        //*********正则表达式理解以及Unicode//
                        /*
                        List<byte> buf = new List<byte>();//填充到这个临时列表中

                        // \d :匹配一个数字字符;
                        // ?i :定义一个编号为i的捕获组
                        // {2} :一次读取两个字符
                        String pattern = @"(?i)[\d a-f]{2}";
                        //使用正则表达式获取发送区的有效数据
                        MatchCollection mc = Regex.Matches(send_data.Text, pattern);                       
                        //将mc装换为16进制数据并添加到buf列表中
                        foreach (Match m in mc)
                        {
                            byte data = Convert.ToByte(m.Value, 16);
                            buf.Add(data);
                        }
                        serialPort1.Write(buf.ToArray(), 0, buf.Count); 
                         * */
                        /****************************************************************/
                    }
                    send_data_length += (str.Length / 2);//  （/2）取一个字节数
                    label11.Text = Convert.ToString(send_data_length);

                }
                else
                {
                    System.Media.SystemSounds.Beep.Play();
                    MessageBox.Show("请写入要发送的数据！", "提示");
                }
            }
            else
            {
                System.Media.SystemSounds.Beep.Play();
                MessageBox.Show("串口未打开", "提示");
            }
            /****************************************************************************
             ****************************************************************************/


            /*（2）
            string str = "";
            str = mytext.Text;
            if (serialPort1.IsOpen)
            {
                if (mytext.Text != "")
                {
                    if (checkBox1.Checked)
                    {
                        str += "\r\n";
                    }
                    string a = "", b = "-";
                    a = str;
                    List<byte> list_table = Encoding.ASCII.GetBytes(a).ToList<byte>();//List可以存储任何数据类型
                    byte[] Write_data = list_table.ToArray();//转化为HTML码
                    a = BitConverter.ToString(Write_data);//转化为16进制，并且之间加上“-”
                    for (int i = 0; i < a.Length; i++)
                    {
                        if (a[i] == b[0])
                        {
                            a = a.Remove(i, 1);
                            a = a.Insert(i, " ");
                        }
                    }
                    if (!radioButton1.Checked)
                    {
                        try
                        {
                            serialPort1.Write(str);
                            send_data_length += str.Length;
                            label11.Text = Convert.ToString(send_data_length);
                        }
                        catch//若出现错误
                        {
                            System.Media.SystemSounds.Beep.Play();
                            MessageBox.Show("数据写入错误，请重试！", "提示");
                            serialPort1.Close();
                        }
                    }
                    else
                    {
                        try
                        {
                            serialPort1.Write(a);
                            send_data_length += a.Length;
                            label11.Text = Convert.ToString(send_data_length);

                        }
                        catch//若出现错误
                        {
                            System.Media.SystemSounds.Beep.Play();
                            MessageBox.Show("数据写入错误，请重试！", "提示");
                            serialPort1.Close();
                        }
                    }
                }
                else
                {
                    System.Media.SystemSounds.Beep.Play();
                    MessageBox.Show("请写入要发送的数据！", "提示");
                }
            }
            else
            {
                System.Media.SystemSounds.Beep.Play();
                MessageBox.Show("串口未打开", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Hand);
            }
            */
        }//end of function  send_data_fun();

        private void button7_Click(object sender, EventArgs e)
        {
            F4 = new Form4();
            F4.Show();
        }

        private void toolStripDropDownButton1_Click(object sender, EventArgs e)
        {

        }


        private void toolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process calculator = new Process();//声明一个进程类对象
            calculator.StartInfo.FileName = "calc.exe";

            Process printscreen = new Process();//声明一个进程类对象
            printscreen.StartInfo.FileName = "SnippingTool.exe"/*截图工具*/;    // "mspaint.exe"/*画图工具*/;//

            ToolStripMenuItem myItem = (ToolStripMenuItem)sender;
            byte myItem_tag = Convert.ToByte(myItem.Tag);
            switch(myItem_tag)
            {
                case 1:calculator.Start(); break;

                case 2: printscreen.Start(); break;/*打开截图工具时出现异常*/

                case 3: cel.Visible = true;      //打开Excel窗口
                    //添加表格
                    Workbook wb = cel.Workbooks.Add(XlSheetType.xlWorksheet);
                    Worksheet ws = (Worksheet)cel.ActiveSheet;
                    break;

            }

        }

        /**
        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.BackColor == Color.Gray)
            {
                this.BackColor = Color.Yellow;
            }
        }
        **/


        public static class CommonRes
        {
            public static SerialPort SerialPort1;
        }

    }
}
