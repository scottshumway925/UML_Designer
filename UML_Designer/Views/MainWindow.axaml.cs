using Avalonia.Controls;
using Avalonia.Interactivity;

namespace UML_Designer.Views
{
   public partial class MainWindow : Window
   {
      public MainWindow()
      {
         InitializeComponent();
      }

      private void OnSelectClicked(object sender, RoutedEventArgs e)
      {
         System.Diagnostics.Debug.WriteLine("Select Clicked");
      }

      private void OnCreateClassNodeClicked(object sender, RoutedEventArgs e)
      {
         System.Diagnostics.Debug.WriteLine("Create Class Node Clicked");
      }

      private void OnCreateInterfaceNodeClicked(object sender, RoutedEventArgs e)
      {
         System.Diagnostics.Debug.WriteLine("Create Interface Clicked");
      }

      private void OnConnectClicked(object sender, RoutedEventArgs e)
      {
         System.Diagnostics.Debug.WriteLine("Connect Clicked");
      }
   }
}