﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
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
            Pen border_pen = new Pen(Color.Black, 4);

            Pen thick_tick_pen = new Pen(Color.Black, 4);
            thick_tick_pen.StartCap = LineCap.Triangle;

            Pen thin_tick_pen = Pens.Black;

            Graphics gr = e.Graphics;
            gr.SmoothingMode = SmoothingMode.AntiAlias;
            gr.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            // Put origin in centre of client area
            gr.TranslateTransform(
                ClientSize.Width / 2,
                ClientSize.Height / 2);

            // Get the hour and minute plus any fraction that has elapsed.
            DateTime now = DateTime.Now;

            // Draw the second hand.
            gr.FillPie(Brushes.Coral,
                -ClientSize.Width / 2f + 4f, -ClientSize.Height / 2f + 4f,
                ClientSize.Width - 8f, ClientSize.Height - 8f,
                -90.0f, 6*(now.Second % 60));

            // Outline
            gr.DrawEllipse(border_pen,
                -ClientSize.Width / 2f + 4f, -ClientSize.Height / 2f + 4f,
                ClientSize.Width - 8f, ClientSize.Height - 8f);

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

            
        }
    }
}