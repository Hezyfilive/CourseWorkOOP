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
        double a = 1.0;
        double b = 2.0;
        double c = 3.0;
        double d = 4.0;

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
            double term = dataPoint.Y;
            foreach (var otherPoint in DataPoints)
            {
                if (otherPoint != dataPoint)
                {
                    term *= (x - otherPoint.X) / (dataPoint.X - otherPoint.X);
                }
            }
            result += term;
        }
        return result;
    }
    public List<double> FindRoots(int degree, double epsilon, int maxIterations)
    {
        double x0 = 0;
        double x1 = 1;
        List<double> roots = new List<double>();
        int iteration = 0;

        while (iteration < maxIterations)
        {
            double fValue0 = PolynomialF(x0, degree);
            double fValue1 = PolynomialF(x1, degree);
            double gValue0 = LagrangeInterpolation(x0);
            double gValue1 = LagrangeInterpolation(x1);

            double x2 = x1 - (fValue1 - gValue1) * (x1 - x0) / (fValue1 - fValue0);

            double fValue2 = PolynomialF(x2, degree);
            double gValue2 = LagrangeInterpolation(x2);

            if (Math.Abs(fValue2 - gValue2) < epsilon)
            {
                // Root found, add to the list of roots
                roots.Add(x2);
            }

            x0 = x1;
            x1 = x2;

            iteration++;
        }

        return roots;
    }

    public void SaveToXml(string path)
    {
        using (XmlTextWriter writer = new XmlTextWriter(path, null))
        {
            writer.Formatting = Formatting.Indented;
            
            XmlSerializer serializer = new XmlSerializer(typeof(PolynomialInterpolation));
            
            serializer.Serialize(writer, this);
        }
    }

    public static PolynomialInterpolation LoadFromXml(string path)
    {
        using (StreamReader reader = new StreamReader(path))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(PolynomialInterpolation));
            return (PolynomialInterpolation)serializer.Deserialize(reader);
        }
    }
}