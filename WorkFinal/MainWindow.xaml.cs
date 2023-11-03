using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using NPOI.SS.Formula.Functions;
using Polynom;

namespace WorkFinal;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private readonly DataGrid _dataGrid = DataGrid.GetInstance();

    public MainWindow()
    {
        InitializeComponent();
        _dataGrid.GridEvent += OnGridDataChange;
    }

    private void Logout_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void Load_Click(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            var filePath = openFileDialog.FileName;
            _dataGrid.LoadGrid(filePath);
        }
    }

    private void ShowChart_Click(object sender, RoutedEventArgs e)
    {
        var graphWindow = new GraphWindow(_dataGrid.GetCollection());
    }

    private void CalculateResult_Click(object sender, RoutedEventArgs e)
    {
        var degreeText = DegreeSelect.Text;
        if (int.TryParse(degreeText, out var degree))
        {
            var result = GetResult(degree);
            var graphWindow = new GraphWindow(result);
        }
        else
        {
            MessageBox.Show("Invalid double value entered.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }


    private void Save_Click(object sender, RoutedEventArgs e)
    {
        var saveFileDialog = new SaveFileDialog()
        {
            Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*"
        };
        if (saveFileDialog.ShowDialog() == true)
            try
            {
                var filePath = saveFileDialog.FileName;
                _dataGrid.SaveGrid(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
    }

    private void PdfResult_Click(object sender, RoutedEventArgs e)
    {
        var saveFileDialog = new SaveFileDialog()
        {
            Filter = "PDF Files (*.pdf)|*.pdf|All Files (*.*)|*.*"
        };
        if (saveFileDialog.ShowDialog() == true)
        {
            var filePath = saveFileDialog.FileName;

            var degreeText = DegreeSelect.Text;
            if (int.TryParse(degreeText, out var degree))
            {
                var result = GetResult(degree);
                var documentResult = new DocumentResult();
                documentResult.DocResult(result, filePath, degree);
            }
            else
            {
                MessageBox.Show("Invalid double value entered.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void DocxResult_Click(object sender, RoutedEventArgs e)
    {
        var saveFileDialog = new SaveFileDialog()
        {
            Filter = "Word Files (*.docx)|*.docx|All Files (*.*)|*.*"
        };
        if (saveFileDialog.ShowDialog() == true)
        {
            var filePath = saveFileDialog.FileName;

            var degreeText = DegreeSelect.Text;
            if (int.TryParse(degreeText, out var degree))
            {
                var result = GetResult(degree);
                var docxResult = new DocxResult();
                docxResult.DocResult(result, filePath, degree);
            }
            else
            {
                MessageBox.Show("Invalid double value entered.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void EditButton_Click(object sender, RoutedEventArgs e)
    {
        if (DataPointGrid.SelectedItem is DataPoint dataPoint)
        {
            var index = _dataGrid.GetCollection().IndexOf(dataPoint);

            var copiedPoint = new DataPoint { X = dataPoint.X, Y = dataPoint.Y };

            var editDataPointWindow = new EditDataPointWindow(copiedPoint);

            if (editDataPointWindow.ShowDialog() == true)
            {
                var editedDataPoint = editDataPointWindow.DataPoint;
                _dataGrid.UpdateGrid(editedDataPoint, index);
            }
        }
    }

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (DataPointGrid.SelectedItem is DataPoint dataPoint)
        {
            var index = _dataGrid.GetCollection().IndexOf(dataPoint);
            _dataGrid.RemovePoint(index);
        }
    }

    private void AddPoint_Click(object sender, RoutedEventArgs e)
    {
        var editDataPointWindow = new EditDataPointWindow();
        if (editDataPointWindow.ShowDialog() == true)
        {
            var newDataPoint = editDataPointWindow.DataPoint;
            _dataGrid.AddPoint(newDataPoint);
        }
    }

    private void OnGridDataChange(object? sender, PolynomialInterpolation interpolation)
    {
        var dataPoints = interpolation.DataPoints;
        ObservableCollection<DataPoint> collection = new(dataPoints);
        DataPointGrid.ItemsSource = collection;
    }

    private List<double> GetResult(int degree)
    {
        var interpolation = _dataGrid.GetPolynomialInterpolation();
        return interpolation.FindRoots(degree, 1e-6, 100);
    }
}