using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UML_Designer.Models;

namespace UML_Designer.ViewModels
{
   public class UMLClassCanvasViewModel : ViewModelBase
   {
      public ObservableCollection<UMLClassViewModel> Classes { get; } = new();
      private UMLClassViewModel? _selectedClass;
      public UMLClassViewModel? SelectedClass
      {
         get => _selectedClass;
         set => SetProperty(ref _selectedClass, value);
      }

      public List<UMLClassViewModel> SelectedClasses { get; } = new();

      public void AddClass(double x, double y)
      {
         DeselectAllClasses();

         if (_selectedClass is null)
            Classes.Add(new UMLClassViewModel(new UMLClassModel() { X = x, Y = y }));
         else
            _selectedClass = null;
      }

      public void SelectClass(UMLClassViewModel passedClass, bool addToSelection)
      {
         if (addToSelection)
         {
            if (passedClass.IsSelected)
            {
               passedClass.IsSelected = false;
               SelectedClasses.Remove(passedClass);
            }
            else
            {
               passedClass.IsSelected = true;
               SelectedClass = passedClass;
               SelectedClasses.Add(passedClass);
            }
         }
         else
         {
            DeselectAllClasses();
            SelectedClass = passedClass;
            passedClass.IsSelected = true;
            SelectedClasses.Add(passedClass);
         }
         System.Diagnostics.Debug.WriteLine($"Items in selected Classes: {SelectedClasses.Count}");
      }

      private void DeselectAllClasses()
      {
         MarkSelectedAsFalse();
         foreach (var c in SelectedClasses)
            c.IsSelected = false;

         SelectedClasses.Clear();
      }

      public void DeleteSelectedClasses()
      {
         if (SelectedClass is not null)
            SelectedClass = null;
         var toDelete = SelectedClasses.ToList();
         foreach (var node in toDelete)
            Classes.Remove(node);

         SelectedClasses.Clear();
      }

      private void MarkSelectedAsFalse()
      {
         if (SelectedClass is not null)
            SelectedClass.IsSelected = false;
      }
   }
}
