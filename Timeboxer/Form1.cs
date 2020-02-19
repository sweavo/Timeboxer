using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace Timeboxer
{
    public partial class TimeboxerForm : Form
    {
        private DateTime alarm_time;
        private double mouse_angle;

        private Pen border_pen= new Pen(Color.Black, 4);
        private Pen thick_tick_pen; // see ctor
        private Pen thin_tick_pen = Pens.Black;

        public TimeboxerForm()
        {
            InitializeComponent();

            // Moar UI initialization, not managed by the designer
            thick_tick_pen = new Pen(Color.Black, 4);
            thick_tick_pen.StartCap = LineCap.Triangle;

            // Initialize app state
            alarm_time = DateTime.Now;

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

            // Get the hour and minute plus any fraction that has elapsed.
            DateTime now = DateTime.Now;
            TimeSpan ts = alarm_time - now;
            float sweep = 6f * (float)(ts.TotalMinutes);

            // We're going to draw arcs and pies in a common rectangle, which i the client rectangle minus some padding:
            Rectangle pad_rectangle = new Rectangle(
                -ClientSize.Width / 2 + 4, -ClientSize.Height / 2 + 4,
                ClientSize.Width - 8, ClientSize.Height - 8);

            gr.FillEllipse(Brushes.White, pad_rectangle);
            // Draw the second hand.
            gr.FillPie(Brushes.Coral,
                pad_rectangle,
                -90.0f, sweep);

            // Outline
            gr.DrawEllipse(border_pen,
                pad_rectangle);

            // Get scale factors.
            float outer_x_factor = 0.45f * ClientSize.Width;
            float outer_y_factor = 0.45f * ClientSize.Height;
            float inner_x_factor = 0.425f * ClientSize.Width;
            float inner_y_factor = 0.425f * ClientSize.Height;
            float big_x_factor = 0.4f * ClientSize.Width;
            float big_y_factor = 0.4f * ClientSize.Height;

            // Draw the tick marks.
            for (int minute = 1; minute <= 60; minute++)
            {
                double angle = Math.PI * minute / 30.0;
                float cos_angle = (float)Math.Cos(angle);
                float sin_angle = (float)Math.Sin(angle);
                PointF outer_pt = new PointF(
                    outer_x_factor * cos_angle,
                    outer_y_factor * sin_angle);
                if (minute % 5 == 0)
                {
                    PointF inner_pt = new PointF(
                        big_x_factor * cos_angle,
                        big_y_factor * sin_angle);
                    gr.DrawLine(thick_tick_pen, inner_pt, outer_pt);
                }
                else
                {
                    PointF inner_pt = new PointF(
                        inner_x_factor * cos_angle,
                        inner_y_factor * sin_angle);
                    gr.DrawLine(thin_tick_pen, inner_pt, outer_pt);
                }
            }
            //TextRenderer.DrawText(gr, ((int)ts.TotalSeconds).ToString(), this.Font, ClientRectangle, Color.Black, Color.WhiteSmoke, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter );
            Point text_point = new Point(-16, -8);
            gr.DrawString(((int)ts.TotalSeconds).ToString(), this.Font, Brushes.Black, text_point);
        }

        // Return angle from origin to point in positive degrees
        private double get_angle_from_vector(Point p)
        {
            double first_try = Math.Atan2(p.X, -p.Y) * (180 / Math.PI);
            if ( first_try < 0.0 )
            {
                return first_try + 360 ;
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
