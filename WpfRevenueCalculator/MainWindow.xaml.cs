using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
using TotalCalc;

namespace WpfRevenueCalculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Пуск таймера для проверки времени работы программы
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            //decimal total = 0; // старая версия

            // Инициализация класса, учитывающего синхронизацию доступа // новая версия
            LockTotalCounter lockTotalCounter = new LockTotalCounter();
            Monitor.Enter(lockTotalCounter);

            Task[] tasks = new Task[2];
            tasks[0] = Task.Factory.StartNew(() =>
            {
                string incomeFile = @"D:\FilesToRead\income.txt";
                string[] incomeLines = System.IO.File.ReadAllLines(incomeFile);
                foreach (string line in incomeLines)
                {
                    lockTotalCounter.AddTotal(decimal.Parse(line));
                    //lock(this) { total += decimal.Parse(line); } // старая версия
                }

                Dispatcher.Invoke(() => textBoxTotal.Text = $"Прибыль: {lockTotalCounter.Total.ToString()}");
                if (tasks[1].IsCompleted)
                {
                    // Останов таймера
                    stopWatch.Stop();
                    // Получение прошедшего времени
                    TimeSpan ts = stopWatch.Elapsed;
                    Dispatcher.Invoke(() => textBoxTime.Text = $"Время вычисления: {ts.Milliseconds.ToString()} мс."); 
                }
            });

            tasks[1] = Task.Factory.StartNew(() =>
            {
                string outcomeFile = @"D:\FilesToRead\outcome.txt";
                string[] outcomeLines = System.IO.File.ReadAllLines(outcomeFile);
                foreach (string line in outcomeLines)
                {
                    lockTotalCounter.DecTotal(decimal.Parse(line));
                    //lock(this) lock (this) { total -= decimal.Parse(line); } // старая версия
                }

                Dispatcher.Invoke(() => textBoxTotal.Text = $"Прибыль: {lockTotalCounter.Total.ToString()}");
                if (tasks[0].IsCompleted)
                {
                    // Останов таймера
                    stopWatch.Stop();
                    // Получение прошедшего времени
                    TimeSpan ts = stopWatch.Elapsed;
                    Dispatcher.Invoke(() => textBoxTime.Text = $"Время вычисления: {ts.Milliseconds.ToString()} мс.");
                }
            });

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }


    }
}
