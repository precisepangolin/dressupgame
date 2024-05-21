using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DragAndDropDressUpWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isDragging;
        private Point clickPosition;


        public MainWindow()
        {
            InitializeComponent();
        }

        private void Ellipse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDragging = true;
            clickPosition = e.GetPosition(MainGrid);
            var draggableEllipse = sender as Ellipse;
            draggableEllipse.CaptureMouse();
        }

        private void Ellipse_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                var draggableEllipse = sender as Ellipse;
                var currentPosition = e.GetPosition(MainGrid);
                var transform = draggableEllipse.RenderTransform as TranslateTransform;

                if (transform == null)
                {
                    transform = new TranslateTransform();
                    draggableEllipse.RenderTransform = transform;
                }

                transform.X += currentPosition.X - clickPosition.X;
                transform.Y += currentPosition.Y - clickPosition.Y;
                clickPosition = currentPosition;
            }
        }

        private void Ellipse_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            var draggableEllipse = sender as Ellipse;
            draggableEllipse.ReleaseMouseCapture();
        }


    }
}
