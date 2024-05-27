using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private TranslateTransform transform;

        public MainWindow()
        {
            InitializeComponent();
            transform = new TranslateTransform();
            //DraggableEllipse.RenderTransform = transform;
        }

        private void Img_SetDraggableStyle()
        {

        }

        private void Img_LeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDragging = true;
            clickPosition = e.GetPosition(MainGrid);

            var draggableImage = (Image)sender;

            // Ensure the RenderTransform is set to a TranslateTransform
            if (draggableImage.RenderTransform is not TranslateTransform)
            {
                draggableImage.RenderTransform = new TranslateTransform();
            }

            transform = (TranslateTransform)draggableImage.RenderTransform;
            draggableImage.CaptureMouse();
        }

        private void Img_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                var draggableImage = sender as Image;
                var currentPosition = e.GetPosition(MainGrid);

                // Calculate the offset
                double offsetX = currentPosition.X - clickPosition.X;
                double offsetY = currentPosition.Y - clickPosition.Y;

                // Calculate the new position relative to the current transform
                double newX = transform.X + offsetX;
                double newY = transform.Y + offsetY;

                // Get the bounds of the parent container
                double leftBound = 0 - draggableImage.Margin.Left - DollPanel.ActualWidth;
                double topBound = 0 - draggableImage.Margin.Top;
                double rightBound = draggableItems.ActualWidth - draggableImage.ActualWidth;
                double bottomBound = MainGrid.ActualHeight - draggableImage.ActualHeight - draggableImage.Margin.Top;
                Debug.WriteLine("margin: " + draggableImage.Margin.Left + "width: " + DollPanel.Width);

                // Ensure the new position is within bounds
                newX = Math.Max(leftBound, Math.Min(newX, rightBound));
                newY = Math.Max(topBound, Math.Min(newY, bottomBound));

                // Apply the new position to the transform
                transform.X = newX;
                transform.Y = newY;

                // Update the click position to the current position
                clickPosition = currentPosition;
            }
        }

        private void Img_LeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            var draggableImage = sender as Image;
            draggableImage.ReleaseMouseCapture();

            // Move the last clicked image to top (Z-Index)
            draggableItems.Children
         .OfType<UIElement>()
         .Select((child, index) => new { child, index })
         .ToList()
         .ForEach(item => Panel.SetZIndex(item.child, item.index));
            Panel.SetZIndex(draggableImage, 99);
        }

        private void Ellipse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDragging = true;
            clickPosition = e.GetPosition(MainGrid);

            var draggableEllipse = sender as Ellipse;

            transform = draggableEllipse.RenderTransform as TranslateTransform;

            //draggableEllipse.RenderTransform = transform;
            draggableEllipse.CaptureMouse();
        }

        private void Ellipse_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                var draggableEllipse = sender as Ellipse;
                var currentPosition = e.GetPosition(MainGrid);

                // Calculate the offset
                double offsetX = currentPosition.X - clickPosition.X;
                double offsetY = currentPosition.Y - clickPosition.Y;

                // Calculate the new position relative to the current transform
                double newX = transform.X + offsetX;
                double newY = transform.Y + offsetY;

                // Get the bounds of the parent container
                double leftBound = 0 - draggableEllipse.Margin.Left;
                double topBound = 0 - draggableEllipse.Margin.Top;
                double rightBound = MainGrid.ActualWidth - draggableEllipse.Width - draggableEllipse.Margin.Left;
                double bottomBound = MainGrid.ActualHeight - draggableEllipse.Height - draggableEllipse.Margin.Top;
                

                // Ensure the new position is within bounds
                newX = Math.Max(leftBound, Math.Min(newX, rightBound));
                newY = Math.Max(topBound, Math.Min(newY, bottomBound));

                // Apply the new position to the transform
                transform.X = newX;
                transform.Y = newY;

                // Update the click position to the current position
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
