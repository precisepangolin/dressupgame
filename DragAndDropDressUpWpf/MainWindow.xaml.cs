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

            // Update for more front-back layered items
            imagePairs.Add(HairFront1, HairBack1);
        }

        private void Img_LeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var frontImage = (Image)sender;
            if (imagePairs.TryGetValue(frontImage, out var backImage))
            {
                HandleMouseButtonDown(frontImage, backImage, e.GetPosition(MainGrid));
            }
            else
            {
                HandleMouseButtonDown(frontImage, null, e.GetPosition(MainGrid));
            }
        }

        private void HandleMouseButtonDown(Image frontImage, Image backImage, Point clickPosition)
        {
            isDragging = true;
            this.clickPosition = clickPosition;

            EnsureTransform(frontImage);
            frontImage.CaptureMouse();

            if (backImage != null)
            {
                EnsureTransform(backImage);
            }

            if (backImage == null)
            {
                transform = (TranslateTransform)frontImage.RenderTransform;
            }
        }

        private void EnsureTransform(Image image)
        {
            if (image.RenderTransform == null || !(image.RenderTransform is TranslateTransform))
            {
                image.RenderTransform = new TranslateTransform();
            }
        }

        private void Img_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                var frontImage = (Image)sender;
                if (imagePairs.TryGetValue(frontImage, out var backImage))
                {
                    HandleMouseMove(frontImage, backImage, e.GetPosition(MainGrid));
                }
                else
                {
                    HandleMouseMove(frontImage, null, e.GetPosition(MainGrid));
                }
            }
        }

        private void HandleMouseMove(Image frontImage, Image backImage, Point currentPosition)
        {
            double offsetX = currentPosition.X - clickPosition.X;
            double offsetY = currentPosition.Y - clickPosition.Y;

            var transformFront = (TranslateTransform)frontImage.RenderTransform;
            double newXFront = transformFront.X + offsetX;
            double newYFront = transformFront.Y + offsetY;

            double newXBack = 0, newYBack = 0;
            if (backImage != null)
            {
                var transformBack = (TranslateTransform)backImage.RenderTransform;
                newXBack = transformBack.X + offsetX;
                newYBack = transformBack.Y + offsetY;
            }

            double[] bounds = CalculateBounds(frontImage);

            newXFront = ApplyBounds(newXFront, bounds[0], bounds[1]);
            newYFront = ApplyBounds(newYFront, bounds[2], bounds[3]);

            if (backImage != null)
            {
                newXBack = ApplyBounds(newXBack, bounds[0], bounds[1]);
                newYBack = ApplyBounds(newYBack, bounds[2], bounds[3]);
            }

            transformFront.X = newXFront;
            transformFront.Y = newYFront;

            if (backImage != null)
            {
                var transformBack = (TranslateTransform)backImage.RenderTransform;
                transformBack.X = newXBack;
                transformBack.Y = newYBack;
            }

            clickPosition = currentPosition;
        }

        private double[] CalculateBounds(Image image)
        {
            int rowNumber = Grid.GetRow(image);
            double gridRowHeight = itemsGrid.RowDefinitions[1].ActualHeight;

            double leftBound = 0 - image.Margin.Left - DollPanel.ActualWidth;
            double topBound = 0 - rowNumber * gridRowHeight;
            double rightBound = itemsGrid.ColumnDefinitions[1].ActualWidth - image.ActualWidth - image.Margin.Left;
            double bottomBound = draggableItems.ActualHeight - image.ActualHeight - rowNumber * gridRowHeight;

            return new double[] { leftBound, rightBound, topBound, bottomBound };
        }

        private double ApplyBounds(double value, double min, double max)
        {
            return Math.Max(min, Math.Min(value, max));
        }

        private void Img_LeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var frontImage = (Image)sender;
            if (imagePairs.TryGetValue(frontImage, out var backImage))
            {
                HandleMouseButtonUp(frontImage, backImage);
            }
            else
            {
                HandleMouseButtonUp(frontImage, null);
            }
        }

        private void HandleMouseButtonUp(Image frontImage, Image backImage)
        {
            isDragging = false;
            frontImage.ReleaseMouseCapture();

            // Sort images and move the last clicked front image to top (Z-Index)
            draggableItems.Children
                .OfType<UIElement>()
                .Select((child, index) => new { child, index })
                .ToList()
                .ForEach(item => Panel.SetZIndex(item.child, item.index));

            Panel.SetZIndex(frontImage, 99);

            if (backImage != null)
            {
                Panel.SetZIndex(backImage, -99);
            }
        }
    }
}
