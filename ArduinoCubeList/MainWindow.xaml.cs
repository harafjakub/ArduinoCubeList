using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
        List<Sequence> sequences = new List<Sequence>();
        bool[,,] states = new bool[5, 5, 5];
        int x = 1;
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
            if(listBox.SelectedItem != null)
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
                        if (layer == 4 && column == 4)
                        {
                            text += temp;
                        }
                        else
                        {
                            text += temp + ",";
                        }
                        temp = 0;
                    }
                }
                int a;
                if (int.TryParse(DelayTextBox.Text, out a))
                {
                    sequences[listBox.SelectedIndex].Diody = "{" + text + "}";
                    sequences[listBox.SelectedIndex].Delay = a;
                }
                else
                {
                    MessageBox.Show("Enter correct delay");
                }
            }
            else
            {
                MessageBox.Show("Please select item from listbox");
            }
        }

        private void NewState_Click(object sender, RoutedEventArgs e)
        {
            listBox.Items.Add("Sequence" + x++);
            sequences.Add(new Sequence());
        }

        private void SaveSequence_Click(object sender, RoutedEventArgs e)
        {      
            string diody = "const PROGMEM byte diody_r1[][25] = {";
            string delay = "const PROGMEM int wait_r1[]={";
            foreach (Sequence sequence in sequences)
            {
                diody += sequence.Diody;
                delay += sequence.Delay;
                if (sequence != sequences[sequences.Count-1])
                {
                    diody += ",";
                    delay += ",";
                }
                else
                {
                    diody += "};";
                    delay += "};";
                }
            }
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Plik txt|*.txt";
            sfd.Title = "Podaj nazwę pliku do zapisu danych";
            sfd.ShowDialog();
            if (sfd.FileName != "")
            {
                StreamWriter streamWriter = new StreamWriter(sfd.FileName);
                streamWriter.WriteLine(diody);
                streamWriter.WriteLine(delay);
                streamWriter.Close();
            }
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }
    }
}
