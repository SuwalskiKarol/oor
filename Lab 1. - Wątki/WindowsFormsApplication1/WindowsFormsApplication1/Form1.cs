using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Drawing.Text;
using System.Collections;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        private readonly object LockObject = new object();   
        ManualResetEvent _pauseEvent = new ManualResetEvent(false);
        private readonly string MUTEX_GUID = "969f7729-3867-4583-8bd6-e990d262c94e";
        private Semaphore semafor = null;
        private Mutex mutexxx = null;
 
        private ArrayList drawStrings;
        private DrawString selectedString;
        private Font normalFont = new Font("Arial", 10);
        private Font boldFont = new Font("Arial", 11, FontStyle.Bold);

        public Form1()
        {
            semafor = new Semaphore(1, 1);
            InitializeComponent();
            drawStrings = new ArrayList();
            Random rnd = new Random();
            int speed = rnd.Next(10) + 1;
            int x = rnd.Next(panel1.Width);
            int y = rnd.Next(panel1.Height);
            int angle = rnd.Next(360);
            int red = rnd.Next(256);
            int green = rnd.Next(256);
            int blue = rnd.Next(256);

            DrawString ds = new DrawString("pierwszy napis", Color.FromArgb(red, green, blue), angle, x, y, speed, panel1);
            drawStrings.Add(ds);
            selectedString = ds;

            ThreadStart startDelegate = new ThreadStart(ds.Start);
            Thread t = new Thread(startDelegate);
            t.IsBackground = true;
            t.Start();
        }

        [STAThread]
        static void main()
        {
            
            CheckForIllegalCrossThreadCalls = false;
            Application.Run(new Form1());
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

            Graphics dc = e.Graphics;
            StringFormat format = new StringFormat(StringFormat.GenericTypographic);
            foreach (DrawString ds in drawStrings)
            {
                lock (ds)
                {
                    dc.TranslateTransform(ds.X, ds.Y);
                    dc.RotateTransform(ds.Angle);
                    dc.DrawString(ds.Text, ds.Selected ? boldFont : normalFont, ds.Brush, 0, 0, format);
                    dc.ResetTransform();
                }
            }
        }

        private void label7_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = label7.BackColor;
            DialogResult dr = colorDialog1.ShowDialog();
            if (dr == DialogResult.OK) label7.BackColor = colorDialog1.Color;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
                if (textBox1.Text.Trim() == "") return;
                if (selectedString != null) selectedString.Selected = false;
                DrawString ds = new DrawString(textBox1.Text, label7.BackColor, (float)numericUpDown1.Value, (float)numericUpDown2.Value, (float)numericUpDown3.Value, (int)numericUpDown4.Value, panel1);
                selectedString = ds;
                drawStrings.Add(ds);
                panel1.Refresh();

            
            /*try
            {
               semafor.WaitOne();*/
                ThreadStart startDelegate = new ThreadStart(ds.Start);
                Thread t = new Thread(startDelegate);
                t.IsBackground = true;
                t.Start();
          /* }
            finally
            {
                semafor.Release();
            }*/
        }

        
            
        
        private void Multithread(object sender, System.Windows.Forms.KeyEventArgs e)
        {

            mutexxx = new Mutex(false, MUTEX_GUID);      

            if (selectedString == null) return;
            Keys keyPress = e.KeyCode;
            switch (keyPress)
            {
                case (Keys.F1):
                    {
                        lock (selectedString)
                        {
                            if (selectedString.Speed < 10) selectedString.Speed++;
                        }
                        break;
                    }
                case (Keys.F2):
                    {
                        lock (selectedString)
                        {
                            if (selectedString.Speed > 1) selectedString.Speed--;
                        }
                        break;
                    }
                case (Keys.F3):
                    {
                        Monitor.Enter(LockObject);
                        try
                       // lock (selectedString)
                        {
                            selectedString.Angle -= 10;
                            if (selectedString.Angle < 0) selectedString.Angle += 360;
                        }
                        finally
                        {
                            Monitor.Exit(LockObject);
                        }
                        panel1.Refresh();
                        break;
                    }
                case (Keys.F4):
                    {
                        mutexxx.WaitOne();

                        {
                            try
                            {
                                selectedString.Angle += 10;
                                if (selectedString.Angle >= 360) selectedString.Angle -= 360;
                            }
                            finally
                            {
                                mutexxx.ReleaseMutex();

                            }
                        }
                        panel1.Refresh();
                        break;
                    }
                case (Keys.F5):
                    {
                        int index = drawStrings.IndexOf(selectedString);
                        if (++index > drawStrings.Count - 1) index = 0;
                        lock (selectedString)
                        {
                            selectedString.Selected = false;
                        }
                        selectedString = (DrawString)drawStrings[index];
                        lock (selectedString)
                        {
                            selectedString.Selected = true;
                        }
                        panel1.Refresh();
                        break;
                    }
                case (Keys.F6):
                    {
                        int index = drawStrings.IndexOf(selectedString);
                        drawStrings.Remove(selectedString);
                        if ((selectedString.Thread.ThreadState & ThreadState.Suspended) == ThreadState.Suspended) selectedString.Thread.Resume();
                        selectedString.Thread.Abort();
                        selectedString.Dispose();
                        if (index > drawStrings.Count - 1) index = 0;
                        if (drawStrings.Count == 0)
                            selectedString = null;
                        else
                        {
                            selectedString = (DrawString)drawStrings[index];
                            selectedString.Selected = true;
                        }
                        panel1.Refresh();
                        break;
                    }
                case (Keys.F7):
                    {
                        /*_pauseEvent.WaitOne();
                        if (selectedString.Thread.ThreadState == System.Threading.ThreadState.WaitSleepJoin)
                        {
                            _pauseEvent.Set();
                        }

                        else
                        {
                            _pauseEvent.Reset();
                        } 
                            
                              
                            */
                        selectedString.Thread.Suspend();
                        break;
                    }
                case (Keys.F8):
                    {
                        if ((selectedString.Thread.ThreadState & ThreadState.Suspended) == ThreadState.Suspended)
                              selectedString.Thread.Resume();
                           // _pauseEvent.Set();
                         // wh.WaitOne();
                        break;
                    }
            }
        }


    }


    public class DrawString : IDisposable
    {

        private string text;
        private float angle;
        private float x;
        private float y;
        private bool selected;
        private Color colour;
        private int speed;
        private SolidBrush brush;
        private Control ctrl;
        private Thread thread;

        public DrawString(string text, Color colour, float angle, float x, float y, int speed, Control ctrl)
        {
            this.text = text;
            this.angle = angle;
            this.x = x;
            this.y = y;
            this.colour = colour;
            this.speed = speed;
            this.ctrl = ctrl;
            selected = true;
            brush = new SolidBrush(colour);
        }

        public string Text
        {
            get { return text; }
        }

        public float Angle
        {
            get { return angle; }
            set { angle = value; }
        }

        public float X
        {
            get { return x; }
        }

        public float Y
        {
            get { return y; }
        }

        public Color Colour
        {
            get { return colour; }
        }

        public SolidBrush Brush
        {
            get { return brush; }
        }

        public int Speed
        {
            get { return speed; }
            set { speed = value; }
        }

        public bool Selected
        {
            get { return selected; }
            set { selected = value; }
        }

        public Thread Thread
        {
            get { return thread; }
        }

        public void Start()
        {
            Control.CheckForIllegalCrossThreadCalls = false; 
            try
            {
                thread = Thread.CurrentThread;
                for (; ; )
                {
                    Thread.Sleep(new TimeSpan((11 - speed) * 100000));
                    lock (this)
                    {
                        x += (float)Math.Cos(Math.PI * angle / 180);
                        y += (float)Math.Sin(Math.PI * angle / 180);
                        if (x >= ctrl.Width) x = x - ctrl.Width;
                        if (y >= ctrl.Height) y = y - ctrl.Height;
                        if (x < 0) x = x + ctrl.Width;
                        if (y < 0) y = y + ctrl.Height;
                    }
                    
                    ctrl.Refresh();
                }
            }
            catch (ThreadAbortException)
            {
            }
        }

        public void Dispose()
        {
            brush.Dispose();
        }
    }





}
