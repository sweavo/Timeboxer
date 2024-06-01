using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Drawing.Imaging;
using System.Media;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;

namespace Timeboxer
{
    public partial class TimeboxerForm : Form
    {
        // for delegating mouse dragging of the form
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd,
                         int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();


        // For drawing clock faces, etc.
        private static Point ORIGIN = new Point(0, 0);

        private static float FACE_ANGLE_TOP = -90.0f;

        public DateTime alarm_time;
        private int alarm_time_show_ticks = 0;
        private double mouse_angle;

        private static Brush face_brush = Brushes.White;
        private static Brush pie_brush = Brushes.Coral;
        private static int border_pixels = 4;
        private static Pen border_pen = new Pen(Color.Black, border_pixels);
        private static Brush border_brush = Brushes.Black;
        private static Pen thick_tick_pen; // see ctor
        private static Pen thin_tick_pen = Pens.Black;

        // radii of ticks, as a proportion of the available radius
        private static float big_tick_r_from = 0.8f;
        private static float small_tick_r_from = 0.85f;
        private static float tick_r_to = 0.9f;

        private static bool is_active = false;

        public TimeboxerForm()
        {
            InitializeComponent();

            // Moar UI initialization, not managed by the designer
            thick_tick_pen = new Pen(Color.Black, 4);
            thick_tick_pen.StartCap = LineCap.Triangle;

            // Initialize app state
            alarm_time = DateTime.Now;

        }

        // RemainingSeconds: The number of seconds remaining before the alarm time, or 0 if in the past.
        private float RemainingSeconds
        {
            get
            {
                DateTime now = DateTime.Now;
                TimeSpan ts = alarm_time - now;
                return Math.Max((float)ts.TotalSeconds, 0f);
            }
        }

        private string RemainingTime
        {
            get
            {
                TimeSpan time = TimeSpan.FromSeconds(RemainingSeconds);
                return time.ToString(@"m\:ss");
            }
        }
        // Sweep: number of degrees (360 deg =1 hour) remaining
        private float Sweep
        {
            get
            {
                return (float)(RemainingSeconds / 10.0f);

            }
        }

        private void draw_tick(Graphics gr, int width, int height, Pen p, double angle, float inner_r, float outer_r)
        {
            float outer_x_factor = outer_r * width / 2;
            float outer_y_factor = outer_r * height / 2;
            float inner_x_factor = inner_r * width / 2;
            float inner_y_factor = inner_r * height / 2;
            float cos_angle = (float)Math.Cos(angle);
            float sin_angle = (float)Math.Sin(angle);
            PointF outer_pt = new PointF(
                outer_x_factor * cos_angle,
                outer_y_factor * sin_angle);
            PointF inner_pt = new PointF(
                inner_x_factor * cos_angle,
                inner_y_factor * sin_angle);
            gr.DrawLine(p, inner_pt, outer_pt);

        }

        private void draw_text_centered(Graphics g, Point where, string text, Font font, Brush brush)
        {
            SizeF text_bbox = g.MeasureString(text, font);
            Point text_point = new Point(
                where.X - (int)(text_bbox.Width / 2),
                where.Y - (int)(text_bbox.Height / 2));
            g.DrawString(text, font, brush, text_point);
        }

        private void draw_clockface(Graphics gr, Size clientSize, float sweep, bool showRemaining, string remainingTime, bool showAlarmTime) 
        {
            gr.Clear(this.TransparencyKey);

            gr.SmoothingMode = SmoothingMode.AntiAlias;
            gr.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            // Put origin in centre of client area
            gr.TranslateTransform(
                clientSize.Width / 2,
                clientSize.Height / 2);

            // We're going to draw arcs and pies in a common rectangle, which is the client rectangle minus some padding:
            Rectangle pad_rectangle = new Rectangle(
                -clientSize.Width / 2 + 4, -clientSize.Height / 2 + 4,
                clientSize.Width - 8, clientSize.Height - 8);

            // Now the drawing, in z-order

            // Clock Face
            gr.FillEllipse(face_brush, pad_rectangle);

            // Draw the sweep
            gr.FillPie(pie_brush, pad_rectangle, FACE_ANGLE_TOP, sweep);

            // Bezel and dot
            gr.DrawEllipse(border_pen, pad_rectangle);
            gr.FillEllipse(border_brush, new Rectangle(-border_pixels, -border_pixels, 2 * border_pixels, 2 * border_pixels));

            // Draw the tick marks.
            for (int minute = 1; minute <= 60; minute++)
            {
                double angle = Math.PI * minute / 30.0;
                if (minute % 5 == 0)
                {
                    draw_tick(gr, clientSize.Width, clientSize.Height, thick_tick_pen, angle, big_tick_r_from, tick_r_to);
                }
                else
                {
                    draw_tick(gr, clientSize.Width, clientSize.Height, thin_tick_pen, angle, small_tick_r_from, tick_r_to);
                }
            }

            // Write the time in the middle
            if (showRemaining)
            {
                draw_text_centered(gr, new Point(0, clientSize.Height / 6), remainingTime, Font, Brushes.Black);
            }
            if (showAlarmTime)
            {
                Font spindly = new Font(Font.Name, 8, FontStyle.Regular);
                draw_text_centered(gr, new Point(0, clientSize.Height / 4), "Until " + alarm_time.ToLocalTime().TimeOfDay.ToString("hh\\:mm"), spindly, Brushes.Black);
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics gr = e.Graphics;

            draw_clockface(gr, ClientSize, this.Sweep, is_active, RemainingTime, is_active && alarm_time_show_ticks > 0);

        }

        // Return angle from origin to point in positive degrees
        private double get_angle_from_vector(Point p)
        {
            double first_try = Math.Atan2(p.X, -p.Y) * (180 / Math.PI);
            if (first_try < 0.0)
            {
                return first_try + 360;
            }
            else
            {
                return first_try;
            }
        }

        // Round to nearest _granularity_
        private int Quantize( int input, int granularity)
        {
            return granularity * (int)((input + (granularity / 2)) / granularity);
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            Point mouse_at = e.Location;
            mouse_at.Offset(-ClientRectangle.Width / 2, -ClientRectangle.Height / 2);

            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }

            if (e.Button == MouseButtons.Right)
            {
                // Get angle through the mouse position
                mouse_angle = Quantize( (int)get_angle_from_vector(mouse_at), 3);

                double mouse_period = mouse_angle / 6.0 + (1.0/60.0);
                alarm_time = DateTime.Now.AddMinutes(mouse_period);
                alarm_time_show_ticks = 12; // 4 per second
            }

            Refresh();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (alarm_time_show_ticks > 0)
            {
               alarm_time_show_ticks--;
            }

            if (is_active && (alarm_time <= DateTime.Now)) // transition to inactive
            {
                SoundPlayer doneSound = new SoundPlayer(Properties.Resources.FinishedSound);
                doneSound.Play();
                is_active = false;
            }
            else if (!is_active && (alarm_time > DateTime.Now)) // transition to active
            {
                is_active = true;
            }
            
            Refresh();
        }
# ifdef GENERATE_ICONS
        private static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
#endif
        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            if (is_active)
            {
# ifdef GENERATE_ICONS
                 nasty hack to draw a set of icons
                Size canvasSize = new Size(96, 96);
                Bitmap bmp = new Bitmap(canvasSize.Width, canvasSize.Height);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    draw_clockface(g, canvasSize, Sweep, true, RemainingTime, false);
                }
                bmp.Save("timeboxer.png", ImageFormat.Icon);
#endif
                alarm_time = DateTime.Now;
                is_active = false;
            }
            else
            {
                Close();
            }

        }
    }
}
