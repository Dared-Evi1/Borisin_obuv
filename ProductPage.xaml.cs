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

namespace Borisin_41
{
    /// <summary>
    /// Логика взаимодействия для ProductPage.xaml
    /// </summary>
    public partial class ProductPage : Page
    {
        public User _user;
        private List<Product> _order = new List<Product>();
        private int count1 = 0;
        public string logi;
        public string rol;

        public ProductPage(User user)
        {
            InitializeComponent();
            _user = user;
            korzina.Visibility = Visibility.Hidden;

            var currentProducts = Borisin41Entities1.GetContext().Product.ToList();
            ProductListView.ItemsSource = currentProducts;
            if(user != null)
            {
                logi = user.UserSurname +" "+ user.UserName + " "+ user.UserPatronymic;
                ima.Text = logi;
                switch (user.UserRole)
                {
                    case 1:
                        rol = "Клиент";
                        break;
                    case 2:
                        rol = "Менеджер";
                        break;
                    case 3:
                        rol = "Администратор";
                        break;
                    default:
                        rol = "Гость";
                        break;

                }
            }
            else
            {
                rol = "Гость";
            }

            role.Text = rol;


            UpdateProducts();
        }

        private void UpdateProducts()
        {
            var currentProducts = Borisin41Entities1.GetContext().Product.ToList();
            if (FiltrSkidka.SelectedIndex == 0)
            {
                currentProducts = currentProducts.Where(p => (Convert.ToDouble(p.ProductDiscountAmount) >= 0 && Convert.ToDouble(p.ProductDiscountAmount) <= 100)).ToList();
            }
            if (FiltrSkidka.SelectedIndex == 1)
            {
                currentProducts = currentProducts.Where(p => (Convert.ToDouble(p.ProductDiscountAmount) >= 0 && Convert.ToDouble(p.ProductDiscountAmount) < 10)).ToList();
            }
            if (FiltrSkidka.SelectedIndex == 2)
            {
                currentProducts = currentProducts.Where(p => (Convert.ToDouble(p.ProductDiscountAmount) >= 10 && Convert.ToDouble(p.ProductDiscountAmount) < 15)).ToList();
            }
            if (FiltrSkidka.SelectedIndex == 3)
            {
                currentProducts  = currentProducts.Where(p => (Convert.ToDouble(p.ProductDiscountAmount) >= 15 )).ToList();
            }
            
            currentProducts = currentProducts.Where(p => p.ProductName.ToLower().Contains(PoiskPoNaim.Text.ToLower())).ToList();
            ProductListView.ItemsSource = currentProducts.ToList();
            if (Ubiv.IsChecked.Value)
            {
                ProductListView.ItemsSource = currentProducts.OrderByDescending(p => p.ProductCost).ToList();
            }
            if (Vozrast.IsChecked.Value)
            {
                ProductListView.ItemsSource = currentProducts.OrderBy(p => p.ProductCost).ToList();
            }
            Count.Text = "количество " + currentProducts.Count.ToString() + " из " + Borisin41Entities1.GetContext().Product.ToList().Count.ToString();
            if (count1 != 0)
            {
                korzina.Visibility = Visibility.Visible;
            }
        }
        private void FiltrSkidka_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateProducts();
        }

        private void Ubiv_Checked(object sender, RoutedEventArgs e)
        {
            UpdateProducts();

        }

        private void Vozrast_Checked(object sender, RoutedEventArgs e)
        {
            UpdateProducts();

        }

        private void PoiskPoNaim_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateProducts();

        }

        private void ProductListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProductListView.SelectedItems.Count > 1)
            {
                korzina.Visibility = Visibility.Visible;
            }
            else
            {
                korzina.Visibility = Visibility.Hidden;
            }
            UpdateProducts();
        }

        private void korzina_Click(object sender, RoutedEventArgs e)
        {

            
            if (_order.Any())
            {
                var select = _order.Select(p => new OrderProduct
                {
                    ProductArticleNumber = p.ProductArticleNumber,
                    ProductCount = p.Quantity
                }).ToList();
                var korzinaWindow = new Korzina(select, _order, _user);
                Manager.MainFrame.Navigate(new Korzina(select,_order, _user));

            }
            else
            {
                MessageBox.Show("Заказ пуст");
            }
        }

        private void ProductListView_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Product selectProduct = ProductListView.SelectedItem as Product;
            if (selectProduct != null)
            {
                count1++;
                _order.Add(selectProduct);
                korzina.Visibility = Visibility.Visible;
                string c = Convert.ToString(count1);
                MessageBox.Show("Товар добавлен к заказу");
            }
            UpdateProducts();
        }
    }
}
