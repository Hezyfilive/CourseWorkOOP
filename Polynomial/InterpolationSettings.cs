namespace Polynom;

[Serializable]
public class InterpolationSettings
{
    public int Degree { get; set; }
    public double Epsilon { get; set; }
    public double MinValue { get; set; }
    public double MaxValue { get; set; }
    public double Step { get; set; }
    
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        InterpolationSettings otherSettings = (InterpolationSettings)obj;

        return Degree == otherSettings.Degree &&
               Math.Abs(Epsilon - otherSettings.Epsilon) < double.Epsilon &&
               Math.Abs(MinValue - otherSettings.MinValue) < double.Epsilon &&
               Math.Abs(MaxValue - otherSettings.MaxValue) < double.Epsilon &&
               Math.Abs(Step - otherSettings.Step) < double.Epsilon;
    }

    public override int GetHashCode()
    {
        return Degree.GetHashCode() ^
               Epsilon.GetHashCode() ^
               MinValue.GetHashCode() ^
               MaxValue.GetHashCode() ^
               Step.GetHashCode();
    }
}