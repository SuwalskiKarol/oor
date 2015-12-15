using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
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

namespace SimpleCalculator
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

        /// <summary>
        /// Method that goes into the task
        /// </summary>
        /// <param name="numbersTuple"></param>
        /// <returns></returns>
        private int BeginCalculate(object numbersTuple)
        {
            Thread.Sleep(20000);
            Tuple<int, int> numbers = (Tuple<int, int>)numbersTuple;
            return numbers.Item1 + numbers.Item2;
        }

        private async void btnCalculate_Click(object sender, RoutedEventArgs e)
        {
            int number1 = int.Parse(txtNumber1.Text);
            int number2 = int.Parse(txtNumber1.Text);
            int result = 0;
            result = await Calculate(number1, number2);

            txtAnswer.Text = result.ToString();
            await UploadResult(result);

        }
        private async Task<int> Calculate(int number1, int number2)
        {
            return await Task.Run(() =>
            {
                Thread.Sleep(4000);
                return number1 + number2;
            });
        }
        private async Task UploadResult(int result)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(int));
            Stream JSonStream = new MemoryStream();
            ser.WriteObject(JSonStream, result);
            JSonStream.Seek(0, SeekOrigin.Begin);
            HttpClient client = new HttpClient();
            HttpContent content = new StreamContent(JSonStream);
            await client.PostAsync("http://localhost/LogResultService", content);
        }




    }
}
