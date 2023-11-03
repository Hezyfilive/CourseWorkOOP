
using Polynom;

abstract class Program
{
    static void Main()
    {
        PolynomialInterpolation polynomialInterpolation = new PolynomialInterpolation();

        Console.WriteLine("Enter the number of data points:");
        int numDataPoints = int.Parse(Console.ReadLine());

        for (int i = 0; i < numDataPoints; i++)
        {
            Console.Write($"Enter data point {i + 1} (X Y): ");
            string[] data = Console.ReadLine().Split(' ');
            double x = double.Parse(data[0]);
            double y = double.Parse(data[1]);
            polynomialInterpolation.AddDataPoint(new DataPoint { X = x, Y = y });
        }

        Console.Write("Enter x value for Lagrange Interpolation: ");
        double xValue = double.Parse(Console.ReadLine());

        double interpolation = polynomialInterpolation.LagrangeInterpolation(xValue);
        Console.WriteLine($"Lagrange Interpolation Result at x={xValue}: {interpolation}");
        
        polynomialInterpolation.SaveToXml("test.xml");
        Console.WriteLine("Data points saved to test.xml.");
        PolynomialInterpolation loadedInterpolation = PolynomialInterpolation.LoadFromXml("test.xml");
        Console.WriteLine("Data points loaded from test.xml:");
        foreach (var dataPoint in loadedInterpolation.DataPoints)
        {
            Console.WriteLine($"X: {dataPoint.X}, Y: {dataPoint.Y}");
        }
        
        Console.WriteLine("Specify the degree n of the polynomial f(x):");
        int degree = int.Parse(Console.ReadLine());
        
        Console.WriteLine("Enter the eps:");
        double eps = Double.Parse(Console.ReadLine());

        Console.WriteLine("Enter max iteration value:");
        int maxIteration = int.Parse(Console.ReadLine());

        List<double> results = polynomialInterpolation.FindRoots(degree, eps, maxIteration);
        foreach (var result in results)
        {
            Console.WriteLine($"Root found within x: {result}");
        }
        


    }
}