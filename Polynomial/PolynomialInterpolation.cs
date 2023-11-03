using System.Text;
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