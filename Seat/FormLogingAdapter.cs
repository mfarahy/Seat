using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Seat
{
    public class FormLogingAdapter : Exir.Framework.Common.Logging.LogAdapter
    {
        public override void Debug(object message, Exception exception)
        {
            log(message, exception, System.Drawing.Color.Green);
        }

        private static void log(object message, Exception exception, System.Drawing.Color color)
        {
            if (ListView != null)
            {
                ListView.Invoke(new MethodInvoker(() =>
                {
                    if (exception != null)
                    {
                        var exitem = new ListViewItem(new string[] { "", (string)exception.Message });
                        exitem.ForeColor = System.Drawing.Color.Red;
                        ListView.Items.Insert(0, exitem);
                    }

                    string time = DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss\.fff");
                    var item = new ListViewItem(new[] { time, (string)message });
                    item.ForeColor = color;
                    ListView.Items.Insert(0, item);

                    while (ListView.Items.Count > 1000)
                        ListView.Items.RemoveAt(ListView.Items.Count - 1);
                }));
            }
        }

        public override void Error(object message, Exception exception)
        {
            log(message, exception, System.Drawing.Color.Red);
        }

        public override void Fatal(object message, Exception exception)
        {
            log(message, exception, System.Drawing.Color.Red);
        }

        public static ListView ListView { get; internal set; }

        public override void Info(object message, Exception exception)
        {
            log(message, exception, System.Drawing.Color.LightGray);
        }

        public override void Trace(object message, Exception exception)
        {
            log(message, exception, System.Drawing.Color.Yellow);
        }

        public override void Warn(object message, Exception exception)
        {
            log(message, exception, System.Drawing.Color.Cyan);
        }
    }
}
