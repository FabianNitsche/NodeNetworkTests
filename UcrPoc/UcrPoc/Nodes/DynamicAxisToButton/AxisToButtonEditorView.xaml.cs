﻿using System.Reactive.Disposables;
using System.Windows;
using ReactiveUI;
using UcrPoc.Nodes.DynamicAxisToButton;

namespace UcrPoc.Views.Editors
{
    /// <summary>
    /// Interaction logic for AxisToButtonEditorView.xaml
    /// </summary>
    public partial class AxisToButtonEditorView : IViewFor<AxisToButtonEditorViewModel>
    {
        #region ViewModel
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel),
            typeof(AxisToButtonEditorViewModel), typeof(AxisToButtonEditorView), new PropertyMetadata(null));

        public AxisToButtonEditorViewModel ViewModel
        {
            get => (AxisToButtonEditorViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (AxisToButtonEditorViewModel)value;
        }
        #endregion

        public AxisToButtonEditorView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.Bind(ViewModel, vm => vm.AxisFrom, v => v.AxisFrom.Text).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.AxisTo, v => v.AxisTo.Text).DisposeWith(d);
            });
        }
    }
}
