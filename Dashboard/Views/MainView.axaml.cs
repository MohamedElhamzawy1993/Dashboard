using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System.Diagnostics;

namespace Dashboard.Views
{
    public partial class MainView : UserControl
    {
        private Border DragPreview;

        public MainView()
        {
            InitializeComponent();

            // Find controls
            WhiteboardBorder = this.FindControl<Border>("WhiteboardBorder");
            Whiteboard = this.FindControl<Canvas>("Whiteboard");

            // Create and style the drag preview border
            DragPreview = new Border
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(2),
                
                IsVisible = false,
                ZIndex = 1000
            };

            Whiteboard.Children.Add(DragPreview);

            // Attach event handlers
            WhiteboardBorder.AddHandler(DragDrop.DragOverEvent, Whiteboard_DragOver);
            WhiteboardBorder.AddHandler(DragDrop.DropEvent, Whiteboard_Drop);

            // Attach PointerPressed handlers
            var buttonTemplate = this.FindControl<Border>("ButtonTemplate");
            var textBoxTemplate = this.FindControl<Border>("TextBoxTemplate");
            var textBlockTemplate = this.FindControl<Border>("TextBlockTemplate");

            buttonTemplate.PointerPressed += Control_PointerPressed;
            textBoxTemplate.PointerPressed += Control_PointerPressed;
            textBlockTemplate.PointerPressed += Control_PointerPressed;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void Control_PointerPressed(object sender, PointerPressedEventArgs e)
        {
            if (sender is Border border && border.Tag is string controlType)
            {
                Debug.WriteLine($"Control_PointerPressed: {controlType}");

                var dataObject = new DataObject();
                dataObject.Set(DataFormats.Text, controlType);
                DragDrop.DoDragDrop(e, dataObject, DragDropEffects.Copy);
            }
        }

        private void Whiteboard_DragOver(object sender, DragEventArgs e)
        {
            Debug.WriteLine("Whiteboard_DragOver");

            if (e.Data.Contains(DataFormats.Text))
            {
                e.DragEffects = DragDropEffects.Copy;

                var position = e.GetPosition(Whiteboard);

                // Set the size and position of the drag preview based on the control type
                if (e.Data.Get(DataFormats.Text) is string controlType)
                {
                    DragPreview.Width = 100;
                    DragPreview.Height = 30;
                }

                Canvas.SetLeft(DragPreview, position.X);
                Canvas.SetTop(DragPreview, position.Y);

                DragPreview.IsVisible = true;
            }
            else
            {
                e.DragEffects = DragDropEffects.None;
                DragPreview.IsVisible = false;
            }

            e.Handled = true;
        }

        private void Whiteboard_Drop(object sender, DragEventArgs e)
        {
            Debug.WriteLine("Whiteboard_Drop");

            DragPreview.IsVisible = false;

            if (e.Data.Contains(DataFormats.Text))
            {
                var controlType = e.Data.Get(DataFormats.Text);
                var position = e.GetPosition(Whiteboard);

                Control newControl = controlType switch
                {
                    "Button" => new Button { Content = "Button", Width = 100, Height = 30 },
                    "TextBox" => new TextBox { Width = 100, Height = 30 },
                    "TextBlock" => new TextBlock { Text = "TextBlock", Width = 100, Height = 30 },
                    _ => null
                };

                if (newControl != null)
                {
                    // Position the new control
                    Canvas.SetLeft(newControl, position.X);
                    Canvas.SetTop(newControl, position.Y);

                    // Add the new control to the whiteboard
                    Whiteboard.Children.Add(newControl);

                    Debug.WriteLine($"Added {controlType} at {position}");
                }

                e.Handled = true;
            }
        }
    }
}
