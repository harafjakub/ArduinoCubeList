﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using static System.Net.Mime.MediaTypeNames;

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
        Canvas canvas = new Canvas();
        private void GenerateButtons()
        {
            int rows = 5;
            int columns = 5;
            int layers = 5;

            double buttonSpacingLeft = 20;
            double buttonSpacingTop = 11;
            double[] rowSpacing = { 102, 79, 56, 33, 10 };

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
                int a;
                if (int.TryParse(DelayTextBox.Text, out a))
                {
                    sequences[listBox.SelectedIndex].Diody = (bool[,,])states.Clone();
                    sequences[listBox.SelectedIndex].Delay = a;
                    int b = 3;
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
            for (int i = 0; i < states.GetLength(0); i++)
            {
                for (int j = 0; j < states.GetLength(1); j++)
                {
                    for (int k = 0; k < states.GetLength(2); k++)
                    {
                        states[i, j, k] = false;
                    }
                }
            }
            sequences.Add(new Sequence(states,0));
            foreach (Button button in canvas.Children.OfType<Button>())
            {
                button.Background = Brushes.White;
            }
            canvas.InvalidateVisual();
        }

        private void SaveSequence_Click(object sender, RoutedEventArgs e)
        {      
            string diody = "const PROGMEM byte diody_r1[][25] = {";
            string delay = "const PROGMEM int wait_r1[]={";
            foreach (Sequence sequence in sequences)
            {
                diody += ArrayToString(sequence.Diody);
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

                foreach (Sequence sequence in sequences)
                {
                    for (int layer = 0; layer <= 4; layer++)
                    {
                        for (int column = 0; column <= 4; column++)
                        {
                            for (int row = 0; row <= 4; row++)
                            {
                                if(layer==4 && column == 4 && row == 4)
                                {
                                    if (sequence.Diody[layer,column,row])
                                        streamWriter.Write("true");
                                    else
                                        streamWriter.Write("false");
                                }
                                else
                                {
                                    if (sequence.Diody[layer, column, row])
                                        streamWriter.Write("true,");
                                    else
                                        streamWriter.Write("false,");
                                }
                            }
                        }
                    }
                    streamWriter.Write("\n");
                    streamWriter.WriteLine(sequence.Delay);
                }

                streamWriter.WriteLine("[END]");

                foreach (Sequence sequence in sequences)
                {
                    streamWriter.WriteLine("");
                    streamWriter.WriteLine("const PROGMEM byte diody_r1[][25] = {" + ArrayToString(sequence.Diody) + "};");
                    streamWriter.WriteLine("const PROGMEM int wait_r1[]={" + sequence.Delay + "};");
                }

                streamWriter.WriteLine("");
                streamWriter.WriteLine(diody);
                streamWriter.WriteLine(delay);
                streamWriter.Close();
            }
        }

        private void LoadSequence_Click(object sender, RoutedEventArgs e)
        {
            listBox.Items.Clear();
            sequences.Clear();
            string tempString;
            string[] tempString2;
            bool[,,] tempBool = new bool[5,5,5];
            int tempInt;
            int tempInt2 = 1;
            Sequence sequence;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Plik txt|*.txt";
            ofd.Title = "Podaj nazwę pliku do odczytu danych";
            ofd.ShowDialog();
            if (ofd.FileName != "")
            {
                StreamReader streamReader= new StreamReader(ofd.FileName);
                tempString = streamReader.ReadLine();
                while (!tempString.Equals("[END]"))
                {
                    tempString2 = tempString.Split(',');
                    tempInt = 0;
                    for (int layer = 0; layer <= 4; layer++)
                    {
                        for (int column = 0; column <= 4; column++)
                        {
                            for (int row = 0; row <= 4; row++)
                            {
                                tempBool[layer,column,row] = Convert.ToBoolean(tempString2[tempInt]);
                                tempInt++;
                            }
                        }
                    }

                    tempInt = Convert.ToInt32(streamReader.ReadLine());
                    sequence = new Sequence(tempBool,tempInt);
                    listBox.Items.Add("Sequence" + tempInt2);
                    sequences.Add(sequence);
                    tempInt2++;
                    tempString = streamReader.ReadLine();
                }
                streamReader.Close();
            }
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = listBox.SelectedIndex;
            states = (bool[,,])sequences[selectedIndex].Diody.Clone();
            //states = sequences[selectedIndex].Diody;
            foreach (Button button in canvas.Children.OfType<Button>())
            {
                var temp = button.Tag.ToString().Split(',');
                if (states[Convert.ToInt32(temp[0]), Convert.ToInt32(temp[1]), Convert.ToInt32(temp[2])]) {
                    button.Background = Brushes.Red;
                }
                else
                {
                    button.Background = Brushes.White;
                }    
            }
            canvas.InvalidateVisual();
        }

        private String ArrayToString(bool[,,] states)
        {
            int temp = 0;
            string text = "{";
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
            return text+"}";
        }
    }
}
