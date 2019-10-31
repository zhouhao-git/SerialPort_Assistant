using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;  //路径绘图


namespace WindowsFormsApplication1
{
    public partial class Form4 : Form
    {
        private const int Unit_lenth = 32;//单位格大小
        private int Drawstep = 8; //默认的绘制单位
        private const int Y_MAX = 512;//y轴最大值
        private const int Max_step = 33;//最大绘制单位
        private const int Min_step = 1; //最小绘制单位
        private const int Startprint = 32;//点坐标偏移量 
        private List<byte> Datalist = new List<byte>();//数据结构------线性链表 ，存放接收到的数据

        private Pen AxislinesPen = new Pen(Color.FromArgb(0x00, 0x00,0x00)); //轴线颜色
        //private Pen WavelinsPen = new Pen(Color.FromArgb(0x04, 0x00, 0x00)); //波形颜色
        private Pen WavelinsPen = new Pen(Color.FromArgb(0xff, 0x00, 0x00)); //波形颜色

        public ShowWindow   ShowMainWindow;
        public HideWindow   HideMainWindow;
        public OpenPort     OpenSerialPort;
        public ClosePort    CloseSerialPort;
        public GetMainPos   GetMainFormPos;
        public GetMainWidth GetMainFormWidth;

        public Form4()
        {
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint 
                | ControlStyles.AllPaintingInWmPaint, true);//开启双缓冲，防止数据刷新时闪烁
            this.UpdateStyles();
            InitializeComponent();

        }

        private void Form4_Load(object sender, EventArgs e)
        {
            
        }

        private void Draw_Paint(object sender, PaintEventArgs e)
        {
            int x, y;
            int x_num = 0, y_num = 0;
            Graphics g = e.Graphics;
            Rectangle rect = new Rectangle(0, Startprint, ClientRectangle.Width, ClientRectangle.Height / 2);
            GraphicsPath path = new GraphicsPath(new Point[]{new Point(10,10),
                                                              new Point(50,50),
                                                              new Point(10,80),
                                                              new Point(50,100),
                                                              new Point(200,200) },
                                                  new byte[]{(byte)PathPointType.Start,
                                                             (byte)PathPointType.Line,
                                                             (byte)PathPointType.Line,
                                                             (byte)PathPointType.Line,
                                                             (byte)PathPointType.Line });

            GraphicsPath gp = new GraphicsPath();                         
            try
            {
                for (y = 0; y < this.ClientRectangle.Width / Unit_lenth; y++)
                {
                    g.DrawLine(AxislinesPen, Startprint + y * Unit_lenth, Startprint, Startprint + y * Unit_lenth,  Y_MAX + Startprint);
                    gp.AddString((y * (Unit_lenth / Drawstep)).ToString(), this.Font.FontFamily, (int)FontStyle.Regular, 12, new RectangleF(Startprint + y * Unit_lenth - 7, this.ClientRectangle.Height - Startprint + 4, 400, 50), null);
                }
                for (x = 0; x < this.ClientRectangle.Height / Unit_lenth; x++)  //横轴
                {
                    g.DrawLine(AxislinesPen, Startprint, (1 + x) * Unit_lenth, this.ClientRectangle.Width, (x +1) * Unit_lenth);
                }

            }
            catch 
            { }
        }

        //private void button1_Click(object sender, EventArgs e)
        //{
        //    OpenFileDialog dlg = new OpenFileDialog();
        //    dlg.Title = "open file";
        //    dlg.Filter = "Text documents(*.txt)|*.txt|All Files|*.*";
        //    dlg.ShowDialog();
        //}

    }
}
