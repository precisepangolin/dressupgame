using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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

        private Dictionary<Image, TranslateTransform> transformPair = new();
        private Dictionary<Image, Image> imagePairs = new();

        public MainWindow()
        {
            InitializeComponent();
            transform = new TranslateTransform();

            imagePairs.Add(HairFront1, HairBack1);
        }


        private void Img_LeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDragging = true;
            clickPosition = e.GetPosition(MainGrid);

            var draggableImage = (Image)sender;

            EnsureTransform(draggableImage);

            transform = (TranslateTransform)draggableImage.RenderTransform;
            draggableImage.CaptureMouse();
        }

        private void ImgPair_LeftButtonDown(object sender, MouseEventArgs e)
        {
            isDragging = true;
            clickPosition = e.GetPosition(MainGrid);

            var frontImage = (Image)sender;
            if (!imagePairs.TryGetValue(frontImage, out var backImage))
            {
                return;
            }

            EnsureTransform(frontImage);
            EnsureTransform(backImage);
            frontImage.CaptureMouse();
        }

        private void EnsureTransform(UIElement uIElement)
        {
            if (uIElement.RenderTransform is not TranslateTransform)
            {
                uIElement.RenderTransform = new TranslateTransform();
            }
        }

        private void Img_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                var draggableImage = sender as Image;
                var currentPosition = e.GetPosition(MainGrid);

                double offsetX = currentPosition.X - clickPosition.X;
                double offsetY = currentPosition.Y - clickPosition.Y;

                double newX = transform.X + offsetX;
                double newY = transform.Y + offsetY;

                var gridLabel = MainGrid.Children
                    .Cast<UIElement>()
                    .First(e => Grid.GetRow(e) == 0 && Grid.GetColumn(e) == 1);

                int rowNumber = Grid.GetRow(draggableImage);

                double leftBound = 0 - draggableImage.Margin.Left - DollPanel.ActualWidth;
                double topBound = 0 - rowNumber * 50;
                double rightBound = draggableItems.ActualWidth - draggableImage.ActualWidth;
                double bottomBound = MainGrid.ActualHeight - draggableImage.ActualHeight - draggableImage.Margin.Top;
                //Debug.WriteLine(draggableItems.GetType() + " " + gridLabel.GetType());

                newX = Math.Max(leftBound, Math.Min(newX, rightBound));
                newY = Math.Max(topBound, Math.Min(newY, bottomBound));

                transform.X = newX;
                transform.Y = newY;

                clickPosition = currentPosition;
            }
        }

        private void ImgPair_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                var frontImage = sender as Image;
                if (!imagePairs.TryGetValue(frontImage, out var backImage))
                {
                    return;
                }

                var currentPosition = e.GetPosition(MainGrid);

                double offsetX = currentPosition.X - clickPosition.X;
                double offsetY = currentPosition.Y - clickPosition.Y;

                var transformFront = (TranslateTransform)frontImage.RenderTransform;
                var transformBack = (TranslateTransform)backImage.RenderTransform;

                double newXFront = transformFront.X + offsetX;
                double newYFront = transformFront.Y + offsetY;
                double newXBack = transformBack.X + offsetX;
                double newYBack = transformBack.Y + offsetY;

        
                int rowNumber = Grid.GetRow(frontImage);

                double leftBound = 0 - frontImage.Margin.Left - DollPanel.ActualWidth;
                double topBound = 0 - rowNumber * 50;
                double rightBound = draggableItems.ActualWidth - frontImage.ActualWidth;
                double bottomBound = MainGrid.ActualHeight - frontImage.ActualHeight - frontImage.Margin.Top;
                //Debug.WriteLine(draggableItems.GetType() + " " + gridLabel.GetType());

                newXFront = Math.Max(leftBound, Math.Min(newXFront, rightBound));
                newYFront = Math.Max(topBound, Math.Min(newYFront, bottomBound));

                newXBack = Math.Max(leftBound, Math.Min(newXBack, rightBound));
                newYBack = Math.Max(topBound, Math.Min(newYBack, bottomBound));

                transformFront.X = newXFront;
                transformFront.Y = newYFront;
                transformBack.X = newXBack;
                transformBack.Y = newYBack;

                clickPosition = currentPosition;
            }
        }

        private void ImgPair_LeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            var frontImage = sender as Image;
            frontImage.ReleaseMouseCapture();
            if (!imagePairs.TryGetValue(frontImage, out var backImage))
            {
                return;
            }

            // Move the last clicked front image to top (Z-Index)
            draggableItems.Children
         .OfType<UIElement>()
         .Select((child, index) => new { child, index })
         .ToList()
         .ForEach(item => Panel.SetZIndex(item.child, item.index));
            Panel.SetZIndex(frontImage, 99);
            Panel.SetZIndex(backImage, -99);

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
