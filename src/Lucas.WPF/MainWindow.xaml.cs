using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace Lucas
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Thread _thread;

        public MainWindow()
        {
            InitializeComponent();

            _thread = new Thread(MessageLoop);
            _thread.Start();
        }

        void Display(object content)
        {
            this.Dispatcher.BeginInvoke((Action)(() => this.Content = content));
        }

        void MessageLoop()
        {
            for (var msgQ = new MessageQueue(".\\private$\\Lucas"); ; )
            {
                Thread.SpinWait(10);

                var msg = msgQ.Receive();

                if (msg == null)
                    continue;

                if (string.Equals("exit", msg.Label, StringComparison.OrdinalIgnoreCase))
                    break;

                if (string.Equals("draw", msg.Label, StringComparison.OrdinalIgnoreCase))
                {
                    var stringReader = new StringReader(msg.Body.ToString());
                    var xmlReader = XmlReader.Create(stringReader);
                    var content = XamlReader.Load(xmlReader);

                    Display(content);
                }
            }
        }
    }
}
