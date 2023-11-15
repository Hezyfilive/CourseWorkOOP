using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace Polynom;

[Serializable]
public class PolynomialInterpolation
{
    public List<DataPoint> DataPoints { get; set; }
    public InterpolationSettings Settings { get; set; } = new();
    public List<double> Roots { get; set; } = new();
    public PolynomialInterpolation()
    {
        DataPoints = new List<DataPoint>();
    }
    public PolynomialInterpolation(List<DataPoint> dataPoints, InterpolationSettings settings)
    {
        DataPoints = dataPoints;
        Settings = settings;
    }

    public PolynomialInterpolation(List<DataPoint> dataPoints)
    {
        DataPoints = dataPoints;
    }

    public PolynomialInterpolation(DataPoint[] dataPoints)
    {
        DataPoints = dataPoints.ToList();
    }

    public void AddDataPoint(DataPoint dataPoint)
    {
        DataPoints.Add(dataPoint);
    }

    public void UpdateDataPoint(DataPoint dataPoint, int index)
    {
        DataPoints[index] = dataPoint;
    }

    public void RemoveDataPoint(int index)
    {
        DataPoints.RemoveAt(index);
    }

    public double PolynomialF(double x, int degree)
    {
        if (degree < 0)
        {
            throw new ArgumentException("Degree must be non-negative");
        }

        double result = 0;
        double term = 1;

        for (int i = 0; i <= degree; i++)
        {
            result += term;
            term *= x / (i + 1) * (degree - i);
        }

        return result;
    }

    public double LagrangeInterpolation(double x)
    {
        double result = 0;
        foreach (var dataPoint in DataPoints)
        {
            var term = dataPoint.Y;
            foreach (var otherPoint in DataPoints)
                if (otherPoint != dataPoint)
                    term *= (x - otherPoint.X) / (dataPoint.X - otherPoint.X);
            result += term;
        }

        return result;
    }
    public List<double> FindRoots(InterpolationSettings settings)
    {
        if (!Settings.Equals(settings))
        {
            Settings = settings;
            
            double x0 = settings.MinValue;
            double x1 = x0 + settings.Step;
            
            Roots.Clear();

            while (x1 <= settings.MaxValue) 
            {
                var fValue0 = PolynomialF(x0, settings.Degree);
                var fValue1 = PolynomialF(x1, settings.Degree);
                var gValue1 = LagrangeInterpolation(x1);

                var x2 = x1 - (fValue1 - gValue1) * (x1 - x0) / (fValue1 - fValue0);

                var fValue2 = PolynomialF(x2, settings.Degree);
                var gValue2 = LagrangeInterpolation(x2);

                if (Math.Abs(fValue2 - gValue2) < settings.Epsilon)
                    Roots.Add(x2);

                x0 = x1;
                x1 += settings.Step;
            }
        }
        
        return Roots;
    }

    public void SaveToXml(string path)
    {
        using (var writer = new XmlTextWriter(path, null))
        {
            writer.Formatting = Formatting.Indented;

            var serializer = new XmlSerializer(typeof(PolynomialInterpolation));

            serializer.Serialize(writer, this);
        }
    }

    public static PolynomialInterpolation LoadFromXml(string path)
    {
        using (var reader = new StreamReader(path))
        {
            var serializer = new XmlSerializer(typeof(PolynomialInterpolation));
            return (PolynomialInterpolation)serializer.Deserialize(reader);
        }
    }
}