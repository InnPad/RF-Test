using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
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
    using Lucas.Drawing;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Program _program;

        public MainWindow()
        {
            InitializeComponent();

            MouseMove += OnMouseMove;

            _program = new Program(this);
        }

        void OnMouseMove(object sender, MouseEventArgs e)
        {
            var position = e.GetPosition(Canvas);
            MouseX.Content = position.X;
            MouseY.Content = position.Y;
            var point = new Point { X = position.X, Y = position.Y };
            
            var matches = string.Join("\n", _program.Canvas.Elements.Where(o => o.Value.Contains(point)));
            if (matches.Length > 0)
            {
                Status.Content = "Shape(s)";
                Message.Content = matches;
            }
            else
            {
                Status.Content = string.Empty;
                Message.Content = string.Empty;
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _program.Exit();
        }

        public void SetStatus(string text)
        {
            Action action = () =>
            {
                this.Status.Content = text;
            };

            this.Dispatcher.BeginInvoke(action);
        }

        public void SetMessage(string text)
        {
            Action action = () =>
            {
                this.Message.Content = text;
            };

            this.Dispatcher.BeginInvoke(action);
        }

        public void Add(int key, IShape shape)
        {
            Action action = () =>
            {
                int color = 0;
                var c = shape.Arguments.Select(o => color ^= o.GetHashCode()).Where(o => false).ToArray();

                var element = shape.ToWindowsShape();
                element.SetValue(FrameworkElement.NameProperty, "shape_" + key);

                this.Canvas.Children.Add(element);
                this.UpdateLayout();
            };

            this.Dispatcher.BeginInvoke(action);
        }

        public void Clear()
        {
            Action action = () =>
                {
                    this.Canvas.Children.Clear();
                    this.UpdateLayout();
                };

            this.Dispatcher.BeginInvoke(action);
        }

        public void Remove(int key)
        {
            Action action = () =>
            {
                var parent = this.Canvas;
                int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
                var elements = new List<UIElement>();

                for (int i = 0; i < childrenCount; i++)
                {
                    var child = VisualTreeHelper.GetChild(parent, i) as UIElement;

                    if (child != null && string.Equals("shape_" + key, (child.GetValue(FrameworkElement.NameProperty) ?? string.Empty).ToString()))
                    {
                        elements.Add(child);
                    }
                }

                foreach (var child in elements)
                {
                    parent.Children.Remove(child);
                }
            };

            this.Dispatcher.BeginInvoke(action);
        }
    }
}
