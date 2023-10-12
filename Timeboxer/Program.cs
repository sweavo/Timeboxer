using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                // The argument is a number of minutes
                int minutes;
                Int32.TryParse(args[0], out minutes);

                // Kill all other Timeboxer instances (Highlander: there shall be only one)
                string my_filename = System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location);
                System.Diagnostics.Process[] immortals = System.Diagnostics.Process.GetProcessesByName(my_filename);
                foreach (System.Diagnostics.Process foe in immortals)
                {
                    Debug.Print(foe.Handle.ToString());
                    if (foe.Handle != System.Diagnostics.Process.GetCurrentProcess().Handle)
                    {
                        foe.CloseMainWindow();
                    }
                }

                // If we were given non-zero then start, otherwise bail
                if (minutes == 0)
                {
                    return;
                }
                tbf.alarm_time = DateTime.Now.AddMinutes(minutes);

                
            }

            Application.Run(tbf);
        }
    }
}
