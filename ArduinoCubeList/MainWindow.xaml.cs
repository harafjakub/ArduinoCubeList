using System;
using System.Collections.Generic;
using System.IO;
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
        bool[,,] states = new bool[5, 5, 5];
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
                for (int column = 0; column < columns; column++)
                {
                    for (int row = 0; row < rows; row++)
                    {
                        Button button = new Button();
                        button.Width = 10;
                        button.Height = 12;
                        button.Background = Brushes.White;

                        double left = rowSpacing[row] + column * (button.Width + buttonSpacingLeft);

                        double top = row * (button.Height + buttonSpacingTop) + layer * (rows * (button.Height + buttonSpacingTop));
                        Canvas.SetLeft(button, left);
                        Canvas.SetTop(button, top);
                        int[] t = { 4, 3, 2, 1, 0 }; // odwrócenie o 90 stopni
                        button.Tag = $"{layer},{column},{t[row]}";
                        canvas.Children.Add(button);

                        button.Click += (sender, e) =>
                        {
                            if (button.Background == Brushes.White)
                            {
                                button.Background = Brushes.Red;
                                var temp = button.Tag.ToString().Split(',');
                                states[Convert.ToInt32(temp[0]), Convert.ToInt32(temp[1]), Convert.ToInt32(temp[2])] = true;
                            }
                            else
                            {
                                button.Background = Brushes.White;
                                var temp = button.Tag.ToString().Split(',');
                                states[Convert.ToInt32(temp[0]), Convert.ToInt32(temp[1]), Convert.ToInt32(temp[2])] = false;
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

        private void EnterState_Click(object sender, RoutedEventArgs e)
        {
            int temp = 0;
            string text = default;
            for (int layer = 0; layer <= 4; layer++)
            {
                for (int column = 0; column <= 4; column++)   
                {
                    for (int row = 0; row <= 4; row++)
                    {
                        temp += (int)(Convert.ToInt32(states[layer, column, row]) * Math.Pow(2, row + 1));
                    }
                    text += temp + ",";
                    temp = 0;
                }
            }
            MessageBox.Show(text);
        }

        private void NewState_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SaveSequence_Click(object sender, RoutedEventArgs e)
        {
            string content = "";

            string filePath = "/plik.txt";

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.Write(content);
            }
        }
    }

}
