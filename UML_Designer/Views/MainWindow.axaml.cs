using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using UML_Designer.ViewModels;
using Avalonia;

namespace UML_Designer.Views
{
   public partial class MainWindow : Window
   {
      private UMLClassViewModel? _resizingClass;
      private Point _resizeStartPointerPosition;
      private Point _resizeStartClassPosition;

      private UMLClassViewModel? _draggingClass;
      private Point _dragStartPointerPosition;
      private Point _dragStartClassPosition;

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

      /****************************************************************************************
      * Below are all of the methods for when a keyboard key is pressed
      ****************************************************************************************/
      private void OnKeyboardKeyDown(object? sender, KeyEventArgs e)
      {
         var vm = (MainWindowViewModel)DataContext!;
         if (e.Key == Key.Delete)
         {
            vm.ClassCanvas.DeleteSelectedClasses();
         }
      }

      /****************************************************************************************
       * Below are all of the methods for when the canvas is clicked. This will be where the 
       * main crux of the system UI logic will be routed through
       ****************************************************************************************/
      private void OnCanvasClicked(object? sender, PointerPressedEventArgs e)
      {
         if (e.Handled) return;

         var vm = (MainWindowViewModel)DataContext!;
         var canvas = UMLCanvas.FindDescendantOfType<Canvas>();
         var position = e.GetPosition(canvas);
         vm.ClassCanvas.AddClass(position.X - 125, position.Y - 115);
      }
      
      private void OnUMLClassClicked(object? sender, PointerPressedEventArgs e)
      {
         e.Handled = true;

         _draggingClass = null;
         e.Pointer.Capture(null);

         var vm = (MainWindowViewModel)DataContext!;
         var border = (Border)sender!;
         var selectedClass = (UMLClassViewModel)border.DataContext!;
         bool isCtrlHeld = e.KeyModifiers.HasFlag(KeyModifiers.Control);

         vm.ClassCanvas.SelectClass(selectedClass, isCtrlHeld);

         _draggingClass = selectedClass;
         _dragStartPointerPosition = e.GetPosition(UMLCanvas);
         _dragStartClassPosition = new Point(selectedClass.X, selectedClass.Y);

         e.Pointer.Capture(border);
      }

      private void OnUMLClassMoved(object? sender, PointerEventArgs e)
      {
         if (_draggingClass is null) return;
         if (!e.GetCurrentPoint(null).Properties.IsLeftButtonPressed)
         {
            // Left button isn't held, clean up stale drag state
            _draggingClass = null;
            e.Pointer.Capture(null);
            return;
         }

         var currentPosition = e.GetPosition(UMLCanvas);

         var deltaX = currentPosition.X - _dragStartPointerPosition.X;
         var deltaY = currentPosition.Y - _dragStartPointerPosition.Y;

         _draggingClass.X = _dragStartClassPosition.X + deltaX;
         _draggingClass.Y = _dragStartClassPosition.Y + deltaY;
      }

      private void OnUMLClassReleased(object? sender, PointerReleasedEventArgs e)
      {
         e.Pointer.Capture(null);
         _draggingClass = null;
      }

      /****************************************************************************************
       * Below are all of the methods for handling the text changing of the classes.
       * This will hold seperate methods for the title section of the UMLClass object.
       ****************************************************************************************/
      private void OnClassTitleClicked(object? sender, PointerPressedEventArgs e)
      {
         System.Diagnostics.Debug.WriteLine("Title Pressed");
         if (e.ClickCount == 2)
         {
            e.Handled = true;
            var textBlock = (TextBlock)sender!;
            var node = (UMLClassViewModel)textBlock.DataContext!;
            node.IsEditingTitle = true;
         }
      }

      private void OnClassTitleLostFocus(object? sender, RoutedEventArgs e)
      {
         var textBox = (TextBox)sender!;
         var node = (UMLClassViewModel)textBox.DataContext!;
         node.IsEditingTitle = false;
      }

      private void OnClassTitleKeyDown(object? sender, KeyEventArgs e)
      {
         if (e.Key == Key.Escape)
         {
            var textBox = (TextBox)sender!;
            var selectedClass = (UMLClassViewModel)textBox.DataContext!;
            selectedClass.IsEditingTitle = false;
            Focus();
         }
      }

      /****************************************************************************************
       * Below are all of the methods for handling the text changing of the classes.
       * This will hold seperate methods for the attributes section of the UMLClass object.
       ****************************************************************************************/
      private void OnClassAttributesClicked(object? sender, PointerPressedEventArgs e)
      {
         System.Diagnostics.Debug.WriteLine("Attributes Pressed");
         if (e.ClickCount == 2)
         {
            e.Handled = true;
            var textBlock = (TextBlock)sender!;
            var node = (UMLClassViewModel)textBlock.DataContext!;
            node.IsEditingAttributes = true;
         }
      }

      private void OnClassAttributesLostFocus(object? sender, RoutedEventArgs e)
      {
         var textBox = (TextBox)sender!;
         var node = (UMLClassViewModel)textBox.DataContext!;
         node.IsEditingAttributes = false;
      }

      private void OnClassAttributesKeyDown(object? sender, KeyEventArgs e)
      {
         if (e.Key == Key.Escape)
         {
            var textBox = (TextBox)sender!;
            var selectedClass = (UMLClassViewModel)textBox.DataContext!;
            selectedClass.IsEditingAttributes = false;
            Focus();
         }
      }

      /****************************************************************************************
       * Below are all of the methods for handling the text changing of the classes.
       * This will hold seperate methods for the methods section of the UMLClass object.
       ****************************************************************************************/
      private void OnClassMethodsClicked(object? sender, PointerPressedEventArgs e)
      {
         System.Diagnostics.Debug.WriteLine("Attributes Pressed");
         if (e.ClickCount == 2)
         {
            e.Handled = true;
            var textBlock = (TextBlock)sender!;
            var node = (UMLClassViewModel)textBlock.DataContext!;
            node.IsEditingMethods = true;
         }
      }

      private void OnClassMethodsLostFocus(object? sender, RoutedEventArgs e)
      {
         var textBox = (TextBox)sender!;
         var node = (UMLClassViewModel)textBox.DataContext!;
         node.IsEditingMethods = false;
      }

      private void OnClassMethodsKeyDown(object? sender, KeyEventArgs e)
      {
         if (e.Key == Key.Escape)
         {
            var textBox = (TextBox)sender!;
            var selectedClass = (UMLClassViewModel)textBox.DataContext!;
            selectedClass.IsEditingMethods = false;
            Focus();
         }
      }

      /****************************************************************************************
       * This portion will handle all of the resizing operations performed on a UMLClass
       * node.
       ****************************************************************************************/
      private void OnResizeAttributesHandleClicked(object? sender, PointerPressedEventArgs e)
      {
         e.Handled = true;

         _resizingClass = null;
         e.Pointer.Capture(null);

         var vm = (MainWindowViewModel)DataContext!;
         var border = (Border)sender!;
         var selectedClass = (UMLClassViewModel)border.DataContext!;

         vm.ClassCanvas.SelectClass(selectedClass, false);

         _resizingClass = selectedClass;
         _resizeStartPointerPosition = e.GetPosition(UMLCanvas);
         _resizeStartClassPosition = new Point(_resizingClass.Width, _resizingClass.AttributeHeight);

         e.Pointer.Capture(border);
      }

      private void OnResizeMethodsHandleClicked(object? sender, PointerPressedEventArgs e)
      {
         e.Handled = true;

         _resizingClass = null;
         e.Pointer.Capture(null);

         var vm = (MainWindowViewModel)DataContext!;
         var border = (Border)sender!;
         var selectedClass = (UMLClassViewModel)border.DataContext!;

         vm.ClassCanvas.SelectClass(selectedClass, false);

         _resizingClass = selectedClass;
         _resizeStartPointerPosition = e.GetPosition(UMLCanvas);
         _resizeStartClassPosition = new Point(_resizingClass.Width, _resizingClass.MethodHeight);

         e.Pointer.Capture(border);
      }

      private void OnResizeAttributesHandleMoved(object? sender, PointerEventArgs e)
      {
         if (_resizingClass is null) return;
         if (!e.GetCurrentPoint(null).Properties.IsLeftButtonPressed)
         {
            // Left button isn't held, clean up stale drag state
            _resizingClass = null;
            e.Pointer.Capture(null);
            return;
         }

         var currentPosition = e.GetPosition(UMLCanvas);

         var deltaX = currentPosition.X - _resizeStartPointerPosition.X + _resizeStartClassPosition.X;
         var deltaY = currentPosition.Y - _resizeStartPointerPosition.Y + _resizeStartClassPosition.Y;

         _resizingClass.AddWidth(deltaX);
         _resizingClass.AddAttributeHeight(deltaY);
      }

      private void OnResizeMethodsHandleMoved(object? sender, PointerEventArgs e)
      {
         if (_resizingClass is null) return;
         if (!e.GetCurrentPoint(null).Properties.IsLeftButtonPressed)
         {
            // Left button isn't held, clean up stale drag state
            _resizingClass = null;
            e.Pointer.Capture(null);
            return;
         }

         var currentPosition = e.GetPosition(UMLCanvas);

         var deltaX = currentPosition.X - _resizeStartPointerPosition.X + _resizeStartClassPosition.X;
         var deltaY = currentPosition.Y - _resizeStartPointerPosition.Y + _resizeStartClassPosition.Y;

         _resizingClass.AddWidth(deltaX);
         _resizingClass.AddMethodHeight(deltaY);
      }

      private void OnResizeHandleReleased(object? sender, PointerReleasedEventArgs e)
      {
         e.Pointer.Capture(null);
         _resizingClass = null;
      }
   }
}