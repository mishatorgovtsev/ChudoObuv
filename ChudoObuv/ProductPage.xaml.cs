using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ChudoObuv;

public partial class ProductPage : Page
{
    private readonly ProductDetails? product;

    public ProductPage(int productId)
    {
        InitializeComponent();

        try
        {
            product = CatalogService.GetProductDetails(productId);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Не удалось загрузить данные товара: {ex.Message}",
                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        if (product == null)
        {
            Loaded += (_, _) => NavigationService?.GoBack();
            return;
        }

        ShowProduct(product);
    }

    private void ShowProduct(ProductDetails details)
    {
        ProductImage.Source = new BitmapImage(new Uri(details.ImagePath));
        TitleText.Text = $"{details.Manufacturer} | {details.ProductName}";
        CategoryText.Text = details.Category;
        FinalPriceText.Text = $"{details.FinalPrice:N0} ₽";
        OldPriceText.Text = $"{details.OldPrice:N0} ₽";
        OldPriceText.Visibility = details.HasDiscount ? Visibility.Visible : Visibility.Collapsed;
        CompositionText.Text = details.Composition;
        DescriptionText.Text = details.Description;
        SizeRangeText.Text = "Размерный ряд: " +
            string.Join(", ", details.Sizes.Select(s => $"{s.SizeValue:0.#}"));

        var availableSizes = details.Sizes.Where(s => s.Quantity > 0).ToList();
        SizeComboBox.ItemsSource = availableSizes;
        SizeComboBox.SelectedIndex = availableSizes.Count > 0 ? 0 : -1;

        if (!AppSession.IsAuthorized)
        {
            OrderPanel.Visibility = Visibility.Collapsed;
            HintText.Text = "Чтобы оформить заказ, войдите в систему.";
        }
        else if (availableSizes.Count == 0)
        {
            OrderPanel.Visibility = Visibility.Collapsed;
            HintText.Text = "Для этой модели нет размеров, доступных для заказа.";
        }
        else
        {
            HintText.Visibility = Visibility.Collapsed;
        }
    }

    private void QuantityTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = !e.Text.All(char.IsAsciiDigit);
    }

    private void AddToOrderButton_Click(object sender, RoutedEventArgs e)
    {
        if (product == null)
        {
            return;
        }

        if (SizeComboBox.SelectedItem is not ProductSize size)
        {
            ShowWarning("Выберите размер из списка.");
            return;
        }

        if (!int.TryParse(QuantityTextBox.Text, out var quantity) || quantity <= 0)
        {
            ShowWarning("Введите количество пар целым числом больше нуля.");
            return;
        }

        var alreadyInOrder = OrderCart.GetQuantity(product.ProductId, size.SizeId);
        var canAdd = Math.Max(size.Quantity - alreadyInOrder, 0);
        if (quantity > canAdd)
        {
            ShowWarning($"Размер {size.SizeValue:0.#}: в наличии {size.Quantity} пар, " +
                        $"в заказе уже {alreadyInOrder}. Можно добавить не больше {canAdd}.");
            return;
        }

        OrderCart.Add(new OrderLine
        {
            ProductId = product.ProductId,
            ProductTitle = $"{product.Manufacturer} | {product.ProductName}",
            SizeId = size.SizeId,
            SizeValue = size.SizeValue,
            Quantity = quantity,
            UnitPrice = product.FinalPrice
        });

        MessageBox.Show("Товар добавлен в заказ.", "Заказ",
            MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private static void ShowWarning(string message)
    {
        MessageBox.Show(message, "Добавление в заказ", MessageBoxButton.OK, MessageBoxImage.Warning);
    }
}