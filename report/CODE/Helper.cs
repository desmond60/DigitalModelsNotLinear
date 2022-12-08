namespace DigitalModelsNotLinear.other;

// % ***** Стркутура приемника ***** % //
public struct Receiver
{
    //: Поля и свойства
    public double X { get; set; }  //@ Координата X 
    public double Y { get; set; }  //@ Координата Y
    public double Z { get; set; }  //@ Координата Z
    
    //: Конструкторы
    public Receiver(double _x, double _y, double _z) {
        this.X = _x; this.Y = _y; this.Z = _z;
    }

    public Receiver(double[] point) {
        this.X = point[0]; this.Y = point[1]; this.Z = point[2];
    }
    
    //: Деконструктор
    public void Deconstruct(out double x, 
                            out double y,
                            out double z) 
    {
        x = this.X;
        y = this.Y;
        z = this.Z;
    }

    public override string ToString() => $"{X,20} {Y,24} {Z,26}";
}

// % ***** Структура источника ***** % //
public struct Source
{
    //: Поля и свойства
    public double X { get; set; }  //@ Координата X 
    public double Y { get; set; }  //@ Координата Y
    public double Z { get; set; }  //@ Координата Z

    //: Конструкторы
    public Source(double _x, double _y, double _z) {
        this.X = _x; this.Y = _y; this.Z = _z;
    }

    public Source(double[] point) {
        this.X = point[0]; this.Y = point[1]; this.Z = point[2];
    }

    //: Деконструктор
    public void Deconstruct(out double x, 
                            out double y,
                            out double z) 
    {
        x = this.X;
        y = this.Y;
        z = this.Z;
    }

    public override string ToString() => $"{X,20} {Y,24} {Z,26}";
}

public static class Helper
{
    // * Расстояние от точки измерения до электрода
    public static double Interval(Receiver R, Source S) {
        return Sqrt(Pow(S.X - R.X, 2) + Pow(S.Y - R.Y, 2) + Pow(S.Z - R.Z, 2));
    }
    
    // * Расчет потенциала
    public static double Potential(Source A, Source B, Receiver M, Receiver N, double I, double sigma) {
        double diff = (1 / Interval(M, B) - 1 / Interval(M, A)) - (1 / Interval(N, B) - 1 / Interval(N, A));
        return I / (2.0 * PI * sigma) * diff;
    }

    // * Расчет diff потенциала
    public static double DiffPotential(Source A, Source B, Receiver M, Receiver N, double I, double sigma) {
        double diff = (1 / Interval(M, B) - 1 / Interval(M, A)) - (1 / Interval(N, B) - 1 / Interval(N, A));
        return -1 / (2.0 * PI * sigma * sigma) * diff;
    }
}