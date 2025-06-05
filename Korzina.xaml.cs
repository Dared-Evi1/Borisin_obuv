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
    /// Логика взаимодействия для Korzina.xaml
    /// </summary>
    public partial class Korzina : Page
    {
        public List<OrderProduct> selectedOrderProducts = new List<OrderProduct>();
        public List<Product> selectedProducts = new List<Product>();
        public OrderProduct currentOrderProduct = new OrderProduct();
        public Order currentOrder = new Order();
        public User use;
        public int count1 = Borisin41Entities1.GetContext().Order.ToList().Count;

        public Korzina(List<OrderProduct> selectedOrderProducts, List<Product> selectedProducts, User user) //selectedOrderProducts вместо этого было _order
        {

            
            InitializeComponent();
            /*
            korzinaListView.ItemsSource = _order;
            adress.ItemsSource = Borisin41Entities1.GetContext().PickUpPoint.ToList();
            adress.DisplayMemberPath = "PickUpPointFull";
            adress.SelectedValuePath = "PickUpPointID";
            FIO.Text = user;
            int count1 = Borisin41Entities1.GetContext().Order.ToList().Count;
            ordercount.Text = Convert.ToString(count1+1);*/
            use = user;
            int count1 = Borisin41Entities1.GetContext().Order.ToList().Count;
            ordercount.Text = Convert.ToString(count1 + 1);
            var currentPickups = Borisin41Entities1.GetContext().PickUpPoint.ToList();
            adress.ItemsSource = currentPickups;
            string us = user.UserSurname + " " + user.UserName + " " + user.UserPatronymic;
            FIO.Text = us;
            korzinaListView.ItemsSource = selectedProducts;
            foreach (Product p in selectedProducts)
            {
                p.Quantity = 1;
                foreach (OrderProduct q in selectedOrderProducts)
                {
                    var orderProduct = selectedOrderProducts.FirstOrDefault(op => op.ProductArticleNumber == p.ProductArticleNumber);
                    if (orderProduct != null)
                    {
                        p.Quantity = orderProduct.ProductCount;
                    }
                    if (p.ProductArticleNumber == q.ProductArticleNumber)
                    {
                        p.Quantity = q.ProductCount;
                    }
                }
            }
            this.selectedOrderProducts = selectedOrderProducts;
            this.selectedProducts = selectedProducts;

    formzak.Text = DateTime.Now.ToString();
            SetDeliveryDate();
            korzinaListView.Items.Refresh();
        }

        /*
        korzinaListView.ItemsSource = _order;
        adress.ItemsSource = Borisin41Entities1.GetContext().PickUpPoint.ToList();
        adress.DisplayMemberPath = "PickUpPointFull";
        adress.SelectedValuePath = "PickUpPointID";
        FIO.Text = user;
        int count1 = Borisin41Entities1.GetContext().Order.ToList().Count;
        ordercount.Text = Convert.ToString(count1+1);*/


        private void save_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                if (!selectedProducts.Any())
                {
                    MessageBox.Show("Корзина пуста. Добавьте товары для оформления заказа.");
                    return;
                }

                if (adress.SelectedValue == null)
                {
                    MessageBox.Show("Выберите пункт выдачи заказа!");
                    return;
                }

                var context = Borisin41Entities1.GetContext();
                decimal TotalSum = selectedProducts.Sum(p => p.ProductCost * p.Quantity);
                decimal TotalDiscount = selectedProducts
                .Where(p => p.ProductDiscountAmount > 0)
                .Sum(p => p.ProductCost * (decimal)p.ProductDiscountAmount / 100 * p.Quantity);
                // Создание нового заказа
                var newOrder = new Order
                {
                    OrderDate = DateTime.Now,
                    OrderDeliveryDate = dostzak.SelectedDate ?? DateTime.Now.AddDays(6),
                    OrderPickupPoint = (int)adress.SelectedValue,
                    OrderClientID = context.User.FirstOrDefault(u => (u.UserSurname + " " + u.UserName + " " + u.UserPatronymic) == FIO.Text)?.UserID,
                    OrderStatus = "Новый",
                    OrderKod = 910 + currentOrder.OrderID
                };

                // Добавление товаров в заказ
                foreach (var product in selectedProducts)
                {
                    newOrder.OrderProduct.Add(new OrderProduct
                    {
                        ProductArticleNumber = product.ProductArticleNumber,
                        ProductCount = product.Quantity,
                        OrderID = count1
                    });

                    // Обновление количества на складе
                    var dbProduct = context.Product.FirstOrDefault(p => p.ProductArticleNumber == product.ProductArticleNumber);
                    if (dbProduct != null)
                    {
                        dbProduct.ProductQuantityInStock -= product.Quantity;
                    }
                }

                // Сохранение изменений
                context.Order.Add(newOrder);
                context.SaveChanges();

                MessageBox.Show($"Заказ №{newOrder.OrderID} успешно сохранен!\n" +
                              $"Дата доставки: {newOrder.OrderDeliveryDate:dd.MM.yyyy}");

                // Закрытие окна
                Manager.MainFrame.Navigate(new ProductPage(use));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения заказа: {ex.Message}");
            }
        }        

        private void right_Click(object sender, RoutedEventArgs e)
        {
            var prod = (sender as Button).DataContext as Product;
            prod.Quantity++;
            var selectedOP = selectedOrderProducts.FirstOrDefault(p=>p.ProductArticleNumber == prod.ProductArticleNumber);
            if (selectedOP != null)
            {
                selectedOP.ProductCount++;
            }
            korzinaListView.Items.Refresh();
            SetDeliveryDate();
            korzinaListView.Items.Refresh();
        }
        public void SetDeliveryDate()
        {
            bool allInStock = selectedProducts.All(p => p.ProductQuantityInStock >= 3);
            dostzak.SelectedDate = DateTime.Now.AddDays(allInStock ? 3 : 6);
        }
        private void left_Click(object sender, RoutedEventArgs e)
        {
            var prod = (sender as Button).DataContext as Product;
            if (prod.Quantity > 1)
            {
                prod.Quantity--;
                var selectedOP = selectedOrderProducts.FirstOrDefault(p => p.ProductArticleNumber == prod.ProductArticleNumber);
                if (selectedOP != null)
                {
                    selectedOP.ProductCount--;
                }
            }
            else
            {
                selectedProducts.Remove(prod);
                var selectedOP = selectedOrderProducts.FirstOrDefault(p => p.ProductArticleNumber == prod.ProductArticleNumber);
                if (selectedOP != null)
                {
                    selectedOrderProducts.Remove(selectedOP);
                }
            }
            korzinaListView.Items.Refresh();
            SetDeliveryDate();
            korzinaListView.Items.Refresh();
        }
    }
}
