using Avalonia.Controls;
using Avalonia.Input;
using System.Diagnostics;

namespace Dashboard.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        // Find controls
        WhiteboardBorder = this.FindControl<Border>("WhiteboardBorder");
        Whiteboard = this.FindControl<Canvas>("Whiteboard");

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

    private void Control_PointerPressed(object sender, PointerPressedEventArgs e)
    {
        if (sender is Control control && control.Tag is string controlType)
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
        }
        else
        {
            e.DragEffects = DragDropEffects.None;
        }

        e.Handled = true;
    }

    private void Whiteboard_Drop(object sender, DragEventArgs e)
    {
        Debug.WriteLine("Whiteboard_Drop");

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
                Canvas.SetLeft(newControl, position.X);
                Canvas.SetTop(newControl, position.Y);
                Whiteboard.Children.Add(newControl);
                Debug.WriteLine($"Added {controlType} at {position}");
            }

            e.Handled = true;
        }
    }
}