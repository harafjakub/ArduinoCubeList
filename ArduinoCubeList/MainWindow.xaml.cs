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

namespace ArduinoCubeList
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            GenerateButtons();
        }

        private void GenerateButtons()
        {
            int rows = 5;
            int columns = 5;
            int layers = 5;

            double buttonSpacingLeft = 20;
            double buttonSpacingTop = 11;
            double[] rowSpacing = { 102, 79, 56, 33, 10 };

            Canvas canvas = new Canvas();

            for (int layer = 0; layer < layers; layer++)
            {
                for (int row = 0; row < rows; row++)
                {
                    for (int column = 0; column < columns; column++)
                    {
                        Button button = new Button();
                        button.Content = "";
                        button.Width = 10;
                        button.Height = 12;
                        button.Background = Brushes.White;

                        double left = rowSpacing[row] + column * (button.Width + buttonSpacingLeft);

                        double top = row * (button.Height + buttonSpacingTop) + layer * (rows * (button.Height + buttonSpacingTop));
                        Canvas.SetLeft(button, left);
                        Canvas.SetTop(button, top);

                        canvas.Children.Add(button);

                        button.Click += (sender, e) =>
                        {
                            if (button.Background == Brushes.White)
                            { 
                                button.Background = Brushes.Red;
                            }
                            else
                            {
                                button.Background = Brushes.White;
                            }
                        };
                    }
                }
            }

            double canvasWidth = columns * (60 + buttonSpacingLeft) - buttonSpacingLeft;
            double canvasHeight = rows * (60 + buttonSpacingTop) - buttonSpacingTop;
            canvas.Width = canvasWidth;
            canvas.Height = canvasHeight;

            mainCanvas.Children.Add(canvas);
        }
    }

}
