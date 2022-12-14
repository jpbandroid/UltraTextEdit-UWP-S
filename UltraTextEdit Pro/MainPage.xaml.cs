using Microsoft.Graphics.Canvas.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UltraTextEdit_Pro
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public List<string> fonts
        {
            get
            {
                return CanvasTextFormat.GetSystemFontFamilies().OrderBy(f => f).ToList();
            }
        }

        public MainPage()
        {
            this.InitializeComponent();
            fontbox.ItemsSource = fonts;
        }
        private async void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            // Open a text file.
            Windows.Storage.Pickers.FileOpenPicker open =
                new Windows.Storage.Pickers.FileOpenPicker();
            open.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            open.FileTypeFilter.Add(".rtf");

            Windows.Storage.StorageFile file = await open.PickSingleFileAsync();

            if (file != null)
            {
                try
                {
                    Windows.Storage.Streams.IRandomAccessStream randAccStream =
                await file.OpenAsync(Windows.Storage.FileAccessMode.Read);

                    // Load the file into the Document property of the RichEditBox.
                    box.Document.LoadFromStream(Windows.UI.Text.TextSetOptions.FormatRtf, randAccStream);
                }
                catch (Exception)
                {
                    ContentDialog errorDialog = new ContentDialog()
                    {
                        Title = "File open error",
                        Content = "Sorry, file could not be opened",
                        PrimaryButtonText = "OK"
                    };

                    await errorDialog.ShowAsync();
                }
            }
        }
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Windows.Storage.Pickers.FileSavePicker savePicker = new Windows.Storage.Pickers.FileSavePicker();
            savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;

            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("Rich Text", new List<string>() { ".rtf" });

            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = "New Document";

            Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                // Prevent updates to the remote version of the file until we 
                // finish making changes and call CompleteUpdatesAsync.
                Windows.Storage.CachedFileManager.DeferUpdates(file);
                // write to file
                Windows.Storage.Streams.IRandomAccessStream randAccStream =
                    await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite);

                box.Document.SaveToStream(Windows.UI.Text.TextGetOptions.FormatRtf, randAccStream);

                // Let Windows know that we're finished changing the file so the 
                // other app can update the remote version of the file.
                Windows.Storage.Provider.FileUpdateStatus status = await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);
                if (status != Windows.Storage.Provider.FileUpdateStatus.Complete)
                {
                    Windows.UI.Popups.MessageDialog errorBox =
                        new Windows.UI.Popups.MessageDialog("File " + file.Name + " couldn't be saved.");
                    await errorBox.ShowAsync();
                }
            }
        }
        private void FindBoxHighlightMatches()
        {
            FindBoxRemoveHighlights();

            Color highlightBackgroundColor = (Color)App.Current.Resources["SystemColorHighlightColor"];
            Color highlightForegroundColor = (Color)App.Current.Resources["SystemColorHighlightTextColor"];

            string textToFind = findBox.Text;
            if (textToFind != null)
            {
                ITextRange searchRange = box.Document.GetRange(0, 0);
                while (searchRange.FindText(textToFind, TextConstants.MaxUnitCount, FindOptions.None) > 0)
                {
                    searchRange.CharacterFormat.BackgroundColor = highlightBackgroundColor;
                    searchRange.CharacterFormat.ForegroundColor = highlightForegroundColor;
                }
            }
        }

        private void FindBoxRemoveHighlights()
        {
            ITextRange documentRange = box.Document.GetRange(0, TextConstants.MaxUnitCount);
            SolidColorBrush defaultBackground = box.Background as SolidColorBrush;
            SolidColorBrush defaultForeground = box.Foreground as SolidColorBrush;

            documentRange.CharacterFormat.BackgroundColor = defaultBackground.Color;
            documentRange.CharacterFormat.ForegroundColor = defaultForeground.Color;
        }

        private void Editor_GotFocus(object sender, RoutedEventArgs e)
        {
            box.Document.GetText(TextGetOptions.UseCrlf, out string currentRawText);

            // reset colors to correct defaults for Focused state
            ITextRange documentRange = box.Document.GetRange(0, TextConstants.MaxUnitCount);
            SolidColorBrush background = (SolidColorBrush)App.Current.Resources["TextControlBackgroundFocused"];

            if (background != null)
            {
                documentRange.CharacterFormat.BackgroundColor = background.Color;
            }
        }

        private void fileClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(BlankPage1));
        }

        private void inkc(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(GamesPage));
        }

        private void infinitec(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MusicPage));
        }

        private void colorapply(object sender, RoutedEventArgs e)
        {
            var color = colorpicker.SelectedColor;
            box.Document.Selection.CharacterFormat.ForegroundColor = color;
        }

        private void boldcheck(object sender, RoutedEventArgs e)
        {
            Windows.UI.Text.ITextSelection selectedText = box.Document.Selection;
            if (selectedText != null)
            {
                Windows.UI.Text.ITextCharacterFormat charFormatting = selectedText.CharacterFormat;
                charFormatting.Bold = Windows.UI.Text.FormatEffect.Toggle;
                selectedText.CharacterFormat = charFormatting;
            }
        }

        private void bolduncheck(object sender, RoutedEventArgs e)
        {
            Windows.UI.Text.ITextSelection selectedText = box.Document.Selection;
            if (selectedText != null)
            {
                Windows.UI.Text.ITextCharacterFormat charFormatting = selectedText.CharacterFormat;
                charFormatting.Bold = Windows.UI.Text.FormatEffect.Toggle;
                selectedText.CharacterFormat = charFormatting;
            }
        }

        private void italiccheck(object sender, RoutedEventArgs e)
        {
            Windows.UI.Text.ITextSelection selectedText = box.Document.Selection;
            if (selectedText != null)
            {
                Windows.UI.Text.ITextCharacterFormat charFormatting = selectedText.CharacterFormat;
                charFormatting.Italic = Windows.UI.Text.FormatEffect.Toggle;
                selectedText.CharacterFormat = charFormatting;
            }
        }

        private void italicuncheck(object sender, RoutedEventArgs e)
        {
            Windows.UI.Text.ITextSelection selectedText = box.Document.Selection;
            if (selectedText != null)
            {
                Windows.UI.Text.ITextCharacterFormat charFormatting = selectedText.CharacterFormat;
                charFormatting.Italic = Windows.UI.Text.FormatEffect.Toggle;
                selectedText.CharacterFormat = charFormatting;
            }
        }

        private void underlinecheck(object sender, RoutedEventArgs e)
        {
            Windows.UI.Text.ITextSelection selectedText = box.Document.Selection;
            if (selectedText != null)
            {
                Windows.UI.Text.ITextCharacterFormat charFormatting = selectedText.CharacterFormat;
                charFormatting.Underline = Windows.UI.Text.UnderlineType.Single;
                selectedText.CharacterFormat = charFormatting;
            }
        }

        private void underlineuncheck(object sender, RoutedEventArgs e)
        {
            Windows.UI.Text.ITextSelection selectedText = box.Document.Selection;
            if (selectedText != null)
            {
                Windows.UI.Text.ITextCharacterFormat charFormatting = selectedText.CharacterFormat;
                charFormatting.Underline = Windows.UI.Text.UnderlineType.None;
                selectedText.CharacterFormat = charFormatting;
            }
        }

        private void SaveAsButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Home_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(BlankPage1));
        }

        private void FontSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Windows.UI.Text.ITextSelection selectedText = box.Document.Selection;
            if (selectedText != null)
            {
                // Get the instance of ComboBox
                ComboBox fontbox = sender as ComboBox;

                // Get the ComboBox selected item text
                string selectedItems = fontbox.SelectedItem.ToString();
                box.Document.Selection.CharacterFormat.Name = selectedItems;
            }
        }

        private void undo(object sender, RoutedEventArgs e)
        {
            box.Document.Undo();
        }

        private void redo(object sender, RoutedEventArgs e)
        {
            box.Document.Redo();
        }

        private void paste(object sender, RoutedEventArgs e)
        {
            box.Document.Selection.Paste(0);
        }

        public List<double> FontSizes { get; } = new List<double>()
            {
                8,
                9,
                10,
                11,
                12,
                14,
                16,
                18,
                20,
                24,
                28,
                36,
                48,
                72,
                96};
        private void fontsizebox_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            fontsizebox.SelectedIndex = 2;

            if ((ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 7)))
            {
                fontsizebox.TextSubmitted += fontsizebox_TextSubmitted;
            }
        }

        private void fontsizebox_TextSubmitted(ComboBox sender, ComboBoxTextSubmittedEventArgs args)
        {
            Windows.UI.Text.ITextSelection selectedText = box.Document.Selection;
            if (selectedText != null)
            {
                bool isDouble = double.TryParse(sender.Text, out double newValue);

                // Set the selected item if:
                // - The value successfully parsed to double AND
                // - The value is in the list of sizes OR is a custom value between 8 and 100
                if (isDouble && (FontSizes.Contains(newValue) || (newValue < 100 && newValue > 8)))
                {
                    // Update the SelectedItem to the new value. 
                    sender.SelectedItem = newValue;
                    box.Document.Selection.CharacterFormat.Size = (float)newValue;
                }
                else
                {
                    // If the item is invalid, reject it and revert the text. 
                    sender.Text = sender.SelectedValue.ToString();

                    var dialog = new ContentDialog
                    {
                        Content = "The font size must be a number between 8 and 100.",
                        CloseButtonText = "Close",
                        DefaultButton = ContentDialogButton.Close
                    };
                    var task = dialog.ShowAsync();
                }
            }

            // Mark the event as handled so the framework doesn’t update the selected item automatically. 
            args.Handled = true;
        }
    }
}
