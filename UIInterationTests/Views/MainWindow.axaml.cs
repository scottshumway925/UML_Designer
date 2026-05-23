using Avalonia.Controls;
using Avalonia.Input;
using System;
using UIInterationTests.ViewModels;

namespace UIInterationTests.Views
{
   public partial class MainWindow : Window
   {
      public MainWindow()
      {
         InitializeComponent();

      }

      private void WindowKeyDown(object? sender, KeyEventArgs e)
      {
         if (e.Key == Key.Delete)
         {
            var vm = (MainWindowViewModel)DataContext!;
            vm.Canvas.DeleteSelectedNodes();
         }
      }

      private void NodeClicked(object? sender, PointerPressedEventArgs e)
      {
         var vm = (MainWindowViewModel)DataContext!;
         e.Handled = true;

         var border = (Border)sender!;
         var node = (AddableObjectViewModel)border.DataContext!;
         bool isCtrlHled = e.KeyModifiers.HasFlag(KeyModifiers.Control);

         vm.Canvas.SelectNode(node, isCtrlHled);
      }

      private void CanvasClicked(object? sender, PointerPressedEventArgs e)
      {
         var vm = (MainWindowViewModel)DataContext!;
         var position = e.GetPosition(RootCanvas);
         vm.Canvas.AddItem(position.X - 40, position.Y - 40);
      }
   }
}