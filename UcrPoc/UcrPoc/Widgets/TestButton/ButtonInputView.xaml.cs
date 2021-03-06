﻿using System.Windows;
using System.Windows.Controls;
using ReactiveUI;
using UcrPoc.Widgets.TestButton;

namespace UcrPoc.Widgets.TestButton
{
    /// <summary>
    /// Interaction logic for ButtonInputView.xaml
    /// </summary>
    public partial class ButtonInputView : UserControl, IViewFor<ButtonInputViewModel>
    {
        #region ViewModel
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel),
            typeof(ButtonInputViewModel), typeof(ButtonInputView), new PropertyMetadata(null));

        public ButtonInputViewModel ViewModel
        {
            get => (ButtonInputViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (ButtonInputViewModel)value;
        }
        #endregion


        public ButtonInputView()
        {
            InitializeComponent();
        }
    }
}
