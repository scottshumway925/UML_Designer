using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UML_Designer.Models;
using UML_Designer.Services;

namespace UML_Designer.Views
{
   public partial class FeedbackWindow : Window
   {
      private readonly AIFeedbackService _aiService;
      private readonly DiagramFileService _fileService;
      private readonly DiagramSaveModel _diagram;

      public FeedbackWindow(AIFeedbackService ai, DiagramFileService file, DiagramSaveModel diagram)
      {
         InitializeComponent();
         _aiService = ai;
         _fileService = file;
         _diagram = diagram;
      }

      public void SetFeedback(string feedback)
      {
         FeedbackText.Text = feedback;
      }

      private async void OnSubmitClicked(object sender, RoutedEventArgs e)
      {
         FeedbackText.Text = "Getting feedback. This may take a while...";

         var userPrompt = UserPromptBox.Text ?? "";
         var fullPrompt = _fileService.CreateAIPrompt(_diagram, userPrompt);
         var feedback = await _aiService.GetFeedback(fullPrompt);

         FeedbackText.Text = feedback;
      }

      private void OnCloseClicked(object sender, RoutedEventArgs e)
      {
         Close();
      }
   }
}
