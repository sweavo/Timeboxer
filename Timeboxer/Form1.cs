using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace Timeboxer
{
    public partial class TimeboxerForm : Form
    {
        private static Point ORIGIN = new Point(0, 0);

        private const float FACE_ANGLE_TOP = -90.0f;

        private DateTime alarm_time;
        private double mouse_angle;

        private Brush face_brush = Brushes.White;
        private Brush pie_brush = Brushes.Coral;
        private Pen border_pen = new Pen(Color.Black, 4);
        private Pen thick_tick_pen; // see ctor
        private Pen thin_tick_pen = Pens.Black;

        // radii of ticks, as a proportion of the available radius
        private float big_tick_r_from = 0.8f;
        private float small_tick_r_from = 0.85f;
        private float tick_r_to = 0.9f;

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

        // Sweep: number of degrees (360 deg =1 hour) remaining
        private float Sweep
        {
            get
            {
                return (float)(RemainingSeconds / 10.0f);

            }
        }

        private void draw_tick(Graphics gr, Pen p, double angle, float inner_r, float outer_r)
        {
            float outer_x_factor = outer_r * ClientSize.Width / 2;
            float outer_y_factor = outer_r * ClientSize.Height / 2;
            float inner_x_factor = inner_r * ClientSize.Width / 2;
            float inner_y_factor = inner_r * ClientSize.Height / 2;
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

        private void draw_text_centered(Graphics g, Point where, string text, Font font, Brush brush )
        {
            SizeF text_bbox = g.MeasureString(text, font);
            Point text_point = new Point(
                where.X - (int)(text_bbox.Width / 2),
                where.Y - (int)(text_bbox.Height / 2));
            g.DrawString(text, font, brush, text_point);
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {

            Graphics gr = e.Graphics;
            gr.Clear(this.TransparencyKey);

            gr.SmoothingMode = SmoothingMode.AntiAlias;
            gr.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            // Put origin in centre of client area
            gr.TranslateTransform(
                ClientSize.Width / 2,
                ClientSize.Height / 2);

            // We're going to draw arcs and pies in a common rectangle, which is the client rectangle minus some padding:
            Rectangle pad_rectangle = new Rectangle(
                -ClientSize.Width / 2 + 4, -ClientSize.Height / 2 + 4,
                ClientSize.Width - 8, ClientSize.Height - 8);

            // Now the drawing, in z-order

            // Clock Face
            gr.FillEllipse(face_brush, pad_rectangle);

            // Draw the sweep
            gr.FillPie(pie_brush, pad_rectangle, FACE_ANGLE_TOP, this.Sweep);

            // Bezel
            gr.DrawEllipse(border_pen, pad_rectangle);

            // Draw the tick marks.
            for (int minute = 1; minute <= 60; minute++)
            {
                double angle = Math.PI * minute / 30.0;
                if (minute % 5 == 0)
                {
                    draw_tick(gr, thick_tick_pen, angle, big_tick_r_from, tick_r_to);
                }
                else
                {
                    draw_tick(gr, thin_tick_pen, angle, small_tick_r_from, tick_r_to);
                }
            }
            draw_text_centered(gr, ORIGIN, ((int)RemainingSeconds).ToString(), Font, Brushes.Black);
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

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            Point mouse_at = e.Location;
            // Convert to transformed coords
            mouse_at.X = mouse_at.X - ClientRectangle.Width / 2;
            mouse_at.Y = mouse_at.Y - ClientRectangle.Height / 2;

            if (e.Button == MouseButtons.Left)
            {
                // Get angle through the mouse position
                mouse_angle = get_angle_from_vector(mouse_at);

                double mouse_period = mouse_angle / 6.0;
                alarm_time = DateTime.Now.AddMinutes(mouse_period);
            }

            Refresh();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Refresh();
        }

    }
}
