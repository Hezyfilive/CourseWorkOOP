using System.Windows;
using Polynom;

namespace WorkFinal;

public partial class EditDataPointWindow : Window
{
    public DataPoint DataPoint { get; set; }

    public EditDataPointWindow()
    {
        var dataPoint = new DataPoint { X = 0, Y = 0 };
        DataPoint = dataPoint;
        DataContext = DataPoint;
        InitializeComponent();
    }

    public EditDataPointWindow(DataPoint dataPoint)
    {
        DataPoint = dataPoint;
        DataContext = DataPoint;
        InitializeComponent();
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}