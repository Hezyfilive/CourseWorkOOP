using System.Xml;
using System.Xml.Serialization;

namespace Polynom;

[Serializable]
public class PolynomialInterpolation
{
    public List<DataPoint> DataPoints { get; set; }

    public PolynomialInterpolation()
    {
        DataPoints = new List<DataPoint>();
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
        var a = 1.0;
        var b = 2.0;
        var c = 3.0;
        var d = 4.0;

        switch (degree)
        {
            case 1:
                return 2 * x + 3;
            case 2:
                return a * x * x + b * x + c;
            case 3:
                return a * x * x * x + b * x * x + c * x + d;
            default:
                throw new ArgumentException("Invalid degree value");
        }
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

    public List<double> FindRoots(int degree, double epsilon, int maxIterations)
    {
        double x0 = 0;
        double x1 = 1;
        var roots = new List<double>();
        var iteration = 0;

        while (iteration < maxIterations)
        {
            var fValue0 = PolynomialF(x0, degree);
            var fValue1 = PolynomialF(x1, degree);
            var gValue0 = LagrangeInterpolation(x0);
            var gValue1 = LagrangeInterpolation(x1);

            var x2 = x1 - (fValue1 - gValue1) * (x1 - x0) / (fValue1 - fValue0);

            var fValue2 = PolynomialF(x2, degree);
            var gValue2 = LagrangeInterpolation(x2);

            if (Math.Abs(fValue2 - gValue2) < epsilon)
                // Root found, add to the list of roots
                roots.Add(x2);

            x0 = x1;
            x1 = x2;

            iteration++;
        }

        return roots;
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