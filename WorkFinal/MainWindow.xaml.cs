using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using Microsoft.Win32;
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
        _dataGrid.SettingsEvent += OnSettingsLoad;
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
        GenerateResult(generateFile: false);
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
        GenerateResult(true);
    }

    private void DocxResult_Click(object sender, RoutedEventArgs e)
    {
        GenerateResult();
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

    private void GenerateResult(bool pdfResult = false, bool generateFile = true)
    {
        InterpolationSettings settings;
        try
        {
            settings = new InterpolationSettings
            {
                Degree = int.Parse(DegreeSelect.Text),
                Epsilon = double.Parse(TxtSearch.Text),
                MinValue = double.Parse(MinValueText.Text),
                MaxValue = double.Parse(MaxValueText.Text),
                Step = double.Parse(StepText.Text)
            };
        }

        catch (FormatException)
        {
            MessageBox.Show("Invalid value entered.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        catch (OverflowException)
        {
            MessageBox.Show("Value is too large.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var result = new List<double>();
        try
        {
            result = GetResult(settings);
        }
        catch (ArgumentException ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            if (generateFile)
            {
                var saveFileDialog = new SaveFileDialog()
                {
                    Filter = pdfResult
                        ? "PDF Files (*.pdf)|*.pdf|All Files (*.*)|*.*"
                        : "Word Files (*.docx)|*.docx|All Files (*.*)|*.*"
                };
                if (saveFileDialog.ShowDialog() == true)
                {
                    var filePath = saveFileDialog.FileName;

                    if (pdfResult)
                        GenerateResultDoc(new DocumentResult(), result, filePath, settings);
                    else
                        GenerateResultDoc(new DocxResult(), result, filePath, settings);
                }
            }
            else
            {
                var graphWindow = new GraphWindow(result);
            }
        }
    }


    private void GenerateResultDoc(IDocResult docResult, List<double> result, string filePath,
        InterpolationSettings settings)
    {
        docResult.DocResult(result, filePath, settings.Degree, settings.Epsilon, settings.MinValue, settings.MaxValue,
            settings.Step);
    }

    private void OnGridDataChange(object? sender, PolynomialInterpolation interpolation)
    {
        var dataPoints = interpolation.DataPoints;
        ObservableCollection<DataPoint> collection = new(dataPoints);
        DataPointGrid.ItemsSource = collection;
    }

    private List<double> GetResult(InterpolationSettings settings)
    {
        var interpolation = _dataGrid.GetPolynomialInterpolation();
        return interpolation.FindRoots(settings);
    }

    private void OnSettingsLoad(object? sender, InterpolationSettings settings)
    {
        DegreeSelect.Text = settings.Degree.ToString();
        TxtSearch.Text = settings.Epsilon.ToString(CultureInfo.CurrentCulture);
        MinValueText.Text = settings.MinValue.ToString(CultureInfo.CurrentCulture);
        MaxValueText.Text = settings.MaxValue.ToString(CultureInfo.CurrentCulture);
        StepText.Text = settings.Step.ToString(CultureInfo.CurrentCulture);
    }
}