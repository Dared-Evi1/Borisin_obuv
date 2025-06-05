using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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

namespace Borisin_41
{
    /// <summary>
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        public User user;
        int CountAuthFails = 0;
        private string CapchaChars = "";

        public AuthPage()
        {
            InitializeComponent();

        }

        private void Gost_Click(object sender, RoutedEventArgs e)
        {
            user = null;
            Manager.MainFrame.Navigate(new ProductPage(user));
            Login.Text = "";
            Password.Text = "";
            CapchaStackPanel.Visibility = Visibility.Hidden;
            CapchaTextBox.Visibility = Visibility.Hidden;
            CountAuthFails = 0;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string login = Login.Text;
            string password = Password.Text;
            if (login == "" || password == "")
            {
                MessageBox.Show("Есть пустые поля");
                return;
            }

            user = Borisin41Entities1.GetContext().User.ToList().Find(p => p.UserLogin == login && p.UserPassword == password);
            if (user != null )
            {
                Manager.MainFrame.Navigate(new ProductPage(user));
                Login.Text = "";
                Password.Text = "";
                captchaOneWord.Text = "";
                captchaTwoWord.Text = "";
                captchaThreeWord.Text = "";
                captchaFourWord.Text = "";
                CapchaStackPanel.Visibility = Visibility.Hidden;
                CapchaTextBox.Visibility = Visibility.Hidden;
                CountAuthFails = 0;
            }
            else
            {
                CountAuthFails++;
                CapchaTextBox.Text = "";

                MessageBox.Show("Введены неверные данные");

                CapchaStackPanel.Visibility = Visibility.Visible;
                CapchaTextBox.Visibility = Visibility.Visible;
                Random random = new Random();

                string chars = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM1234567890!@#$%&";
                CapchaChars = new string(Enumerable.Range(0, 4).Select(_ => chars[random.Next(chars.Length)]).ToArray());
                captchaOneWord.Text = CapchaChars[0].ToString();
                captchaTwoWord.Text = CapchaChars[1].ToString();
                captchaThreeWord.Text = CapchaChars[2].ToString();
                captchaFourWord.Text = CapchaChars[3].ToString();

                captchaOneWord.Margin = new Thickness(random.Next(0, 20), random.Next(0, 20), 0, 0);
                captchaTwoWord.Margin = new Thickness(random.Next(0, 20), random.Next(0, 20), 0, 0);
                captchaThreeWord.Margin = new Thickness(random.Next(0, 20), random.Next(0, 20), 0, 0);
                captchaFourWord.Margin = new Thickness(random.Next(0, 20), random.Next(0, 20), 0, 0);

                if (CountAuthFails >= 2)
                {
                    Button.IsEnabled = false;
                    await Task.Delay(10000);
                    Button.IsEnabled = true;
                }
            }
        }

    }
}
