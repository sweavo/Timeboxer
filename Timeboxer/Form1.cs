using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Timeboxer
{
    public partial class TimeboxerForm : Form
    {
        public TimeboxerForm()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Refresh();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics gr = e.Graphics;

            // Put origin in centre of client area
            gr.TranslateTransform(
                ClientSize.Width / 2,
                ClientSize.Height / 2);

            using (Pen thick_pen = new Pen(Color.Black, 4))
            {
                // Outline
                gr.DrawEllipse(thick_pen, 
                    -ClientSize.Width/2f+4f, -ClientSize.Height/2f + 4f, 
                    ClientSize.Width-8f, ClientSize.Height-8f);

                // Get scale factors.
                float outer_x_factor = 0.45f * ClientSize.Width;
                float outer_y_factor = 0.45f * ClientSize.Height;
                float inner_x_factor = 0.425f * ClientSize.Width;
                float inner_y_factor = 0.425f * ClientSize.Height;
                float big_x_factor = 0.4f * ClientSize.Width;
                float big_y_factor = 0.4f * ClientSize.Height;

                // Draw the tick marks.
                thick_pen.StartCap = LineCap.Triangle;
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
                        gr.DrawLine(thick_pen, inner_pt, outer_pt);
                    }
                    else
                    {
                        PointF inner_pt = new PointF(
                            inner_x_factor * cos_angle,
                            inner_y_factor * sin_angle);
                        gr.DrawLine(Pens.Blue, inner_pt, outer_pt);
                    }
                }
            }

            using (Pen thick_pen = new Pen(Color.Red, 4))
            {
                // Get the hour and minute plus any fraction that has elapsed.
                DateTime now = DateTime.Now;

                // Draw the second hand.
                PointF center = new PointF(0, 0);
                float second_x_factor = 0.4f * ClientSize.Width;
                float second_y_factor = 0.4f * ClientSize.Height;
                double second_angle = -Math.PI / 2 +
                    2 * Math.PI * (int)(now.Second) / 60.0;
                PointF second_pt = new PointF(
                    (float)(second_x_factor * Math.Cos(second_angle)),
                    (float)(second_y_factor * Math.Sin(second_angle)));
                gr.DrawLine(Pens.Red, second_pt, center);
            }
        }
    }
}
