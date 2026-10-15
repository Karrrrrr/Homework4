using System;
using Microsoft.Maui.Controls;
using SubscriptionsTracker.ViewModels;
using SubscriptionsTracker.Models;

namespace SubscriptionsTracker.Views
{
    public partial class SubscriptionEditPage : ContentPage
    {
        public SubscriptionViewModel ViewModel { get; }

        Entry _nameEntry;
        Entry _amountEntry;
        DatePicker _startPicker;
        Button _monthlyBtn;
        Button _yearlyBtn;
        Button _btnVideo;
        Button _btnMusic;
        Button _btnCloud;
        Button _btnWork;
        Button _btnOther;

        public SubscriptionEditPage(SubscriptionViewModel viewModel)
        {
            ViewModel = viewModel;
            BindingContext = ViewModel;

            Title = "Редактировать";
            BackgroundColor = Colors.Transparent;

            // Header with close
            var headerGrid = new Grid { ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) }, Padding = new Thickness(16,12) };
            headerGrid.Add(new Label { Text = "Редактировать", FontSize = 18, FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center }, 0, 0);
            var closeBtn = new Button { Text = "✕", BackgroundColor = Colors.Transparent, WidthRequest = 36, HeightRequest = 36 }; 
            closeBtn.Clicked += async (s, e) => await Navigation.PopAsync();
            headerGrid.Add(closeBtn, 1, 0);

            // Name field (inside frame for rounded background)
            _nameEntry = new Entry { Placeholder = "Название" };
            _nameEntry.SetBinding(Entry.TextProperty, "Model.Name");
            var nameFrame = new Frame { Padding = new Thickness(12), CornerRadius = 8, HasShadow = false, BackgroundColor = Colors.White, Content = _nameEntry, Margin = new Thickness(16,8) };

            // Amount and Start date row
            _amountEntry = new Entry { Placeholder = "Стоимость, ₽", Keyboard = Keyboard.Numeric };
            _startPicker = new DatePicker { HorizontalOptions = LayoutOptions.End };
            _startPicker.SetBinding(DatePicker.DateProperty, "Model.StartDate");

            var amtFrame = new Frame { Padding = new Thickness(8), CornerRadius = 8, HasShadow = false, BackgroundColor = Colors.White, Margin = new Thickness(0) };
            var amtGrid = new Grid { ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) }, Padding = new Thickness(16,8), ColumnSpacing = 12 };
            amtGrid.Add(_amountEntry, 0, 0);
            amtGrid.Add(_startPicker, 1, 0);
            amtFrame.Content = amtGrid;

            // Payment period
            var periodLabel = new Label { Text = "Период оплаты", FontSize = 14, Margin = new Thickness(16,8,16,4) };
            var periodStack = new HorizontalStackLayout { Spacing = 12, Padding = new Thickness(16,0) };
            _monthlyBtn = new Button { Text = "Ежемесячно", CornerRadius = 10 };
            _yearlyBtn = new Button { Text = "Ежегодно", CornerRadius = 10 };
            _monthlyBtn.Clicked += (s, e) => SetFrequency(Frequency.Monthly);
            _yearlyBtn.Clicked += (s, e) => SetFrequency(Frequency.Yearly);
            periodStack.Add(_monthlyBtn);
            periodStack.Add(_yearlyBtn);

            // Categories
            var catLabel = new Label { Text = "Категория", FontSize = 14, Margin = new Thickness(16,8,16,4) };
            var catGrid = new Grid { ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Star) }, RowDefinitions = { new RowDefinition(GridLength.Auto), new RowDefinition(GridLength.Auto) }, Padding = new Thickness(16,0), RowSpacing = 8, ColumnSpacing = 8 };

            _btnVideo = CreateCategoryButton("Видео", Category.Video);
            _btnMusic = CreateCategoryButton("Музыка", Category.Music);
            _btnCloud = CreateCategoryButton("Облако", Category.Cloud);
            _btnWork = CreateCategoryButton("Работа", Category.Work);
            _btnOther = CreateCategoryButton("Другое", Category.Other);

            catGrid.Add(_btnVideo, 0, 0);
            catGrid.Add(_btnMusic, 1, 0);
            catGrid.Add(_btnCloud, 2, 0);
            catGrid.Add(_btnWork, 0, 1);
            catGrid.Add(_btnOther, 1, 1);

            // Bottom actions
            var deleteBtn = new Button { Text = "Удалить", TextColor = Colors.Red, BackgroundColor = Colors.Transparent, BorderColor = Color.FromArgb("#E6E6E6"), CornerRadius = 8, HorizontalOptions = LayoutOptions.FillAndExpand };
            deleteBtn.Clicked += async (s, e) =>
            {
                var ok = await DisplayAlert("Подтвердите", "Удалить подписку?", "Да", "Нет");
                if (ok)
                {
                    await ViewModel.DeleteAsync();
                    await Navigation.PopAsync();
                }
            };

            var saveBtn = new Button { Text = "Сохранить", BackgroundColor = Color.FromArgb("#2563EB"), TextColor = Colors.White, CornerRadius = 8, HorizontalOptions = LayoutOptions.FillAndExpand };
            saveBtn.Clicked += async (s, e) =>
            {
                // parse amount
                if (decimal.TryParse(_amountEntry.Text, out var a))
                    ViewModel.Model.Amount = a;
                await ViewModel.SaveAsync();
                await Navigation.PopAsync();
            };

            var actionsGrid = new Grid { ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Star) }, Padding = new Thickness(16,12), ColumnSpacing = 12 };
            actionsGrid.Add(deleteBtn, 0, 0);
            actionsGrid.Add(saveBtn, 1, 0);

            // Build main layout
            var container = new Frame { CornerRadius = 16, HasShadow = false, Padding = 0, Margin = new Thickness(0,24,0,0), BackgroundColor = Colors.White };
            var main = new VerticalStackLayout { Spacing = 0 };
            main.Add(headerGrid);
            main.Add(nameFrame);
            main.Add(amtFrame);
            main.Add(periodLabel);
            main.Add(periodStack);
            main.Add(catLabel);
            main.Add(catGrid);
            main.Add(actionsGrid);
            container.Content = main;

            Content = new ScrollView { Content = container, BackgroundColor = Colors.Transparent };

            // initialize fields
            _amountEntry.Text = ViewModel.Model.Amount.ToString();
            // set initial frequency and category button states
            UpdateFrequencyButtons();
            UpdateCategoryButtonStates();
        }

        Button CreateCategoryButton(string text, Category category)
        {
            var b = new Button { Text = text, CornerRadius = 10, BackgroundColor = Colors.Transparent, BorderColor = Color.FromArgb("#E6E6E6") };
            b.Clicked += (s, e) =>
            {
                ViewModel.Model.Category = category;
                UpdateCategoryButtonStates();
            };
            return b;
        }

        void UpdateCategoryButtonStates()
        {
            var selected = ViewModel.Model.Category;
            void Style(Button btn, Category cat)
            {
                if (btn == null) return;
                if (cat == selected)
                {
                    btn.BackgroundColor = Colors.Black;
                    btn.TextColor = Colors.White;
                }
                else
                {
                    btn.BackgroundColor = Colors.Transparent;
                    btn.TextColor = Colors.Black;
                }
            }

            Style(_btnVideo, Category.Video);
            Style(_btnMusic, Category.Music);
            Style(_btnCloud, Category.Cloud);
            Style(_btnWork, Category.Work);
            Style(_btnOther, Category.Other);
        }

        void UpdateFrequencyButtons()
        {
            if (ViewModel.Model.Frequency == Frequency.Yearly)
            {
                _yearlyBtn.BackgroundColor = Colors.Black;
                _yearlyBtn.TextColor = Colors.White;
                _monthlyBtn.BackgroundColor = Colors.Transparent;
                _monthlyBtn.TextColor = Colors.Black;
            }
            else
            {
                _monthlyBtn.BackgroundColor = Colors.Black;
                _monthlyBtn.TextColor = Colors.White;
                _yearlyBtn.BackgroundColor = Colors.Transparent;
                _yearlyBtn.TextColor = Colors.Black;
            }
        }

        void SetFrequency(Frequency freq)
        {
            ViewModel.Model.Frequency = freq;
            UpdateFrequencyButtons();
        }
    }
}
