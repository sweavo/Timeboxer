using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Timeboxer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            TimeboxerForm tbf = new TimeboxerForm();
            if (args.Length> 0)
            {
                int minutes;
                Int32.TryParse(args[0], out minutes);
                tbf.alarm_time = DateTime.Now.AddMinutes(minutes);
            }

            Application.Run(tbf);
        }
    }
}
