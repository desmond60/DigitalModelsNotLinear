namespace DigitalModelsNotLinear;
public class Data
{
    //* Данные для задачи
    public double[][] Receivers   { get; set; }    /// Положение приемников
    public double[][] Sources     { get; set; }    /// Положение источника  

    public uint N { get; set; }  /// Номер решаемой задачи

    //* Данные для сетки
    public double[] start { get; set; }   /// Начальная точка
    public double[] end   { get; set; }   /// Конечная точка
    public double   hx    { get; set; }   /// Шаг по Оси X
    public double   hy1   { get; set; }   /// Шаг по Оси Y (первого промежутка)
    public double   hy2   { get; set; }   /// Шаг по Оси Y (второго промежутка)
    public double   kx    { get; set; }   /// Коэффициент деления по Оси X
    public double   ky1   { get; set; }   /// Коэффициент деления по Оси Y (первого промежутка)
    public double   ky2   { get; set; }   /// Коэффициент деления по Оси Y (второго промежутка)


    //* Деконструктор (для источников и приемников)
    public void Deconstruct(out Receiver[] receivers, 
                            out Source[]   sources) {
        receivers = new Receiver[Receivers.Length];
        sources   = new Source[Sources.Length];

        for (int i = 0; i < Receivers.Length; i++)
            receivers[i] = new Receiver(Receivers[i]);

        for (int i = 0; i < sources.Length; i++)
            sources[i] = new Source(Sources[i]);
    }

    //* Деконструктор (для сетки)
    public void Deconstruct(out Vector<double> Start,
                            out Vector<double> End,
                            out double         Hx,
                            out double         Hy1,
                            out double         Hy2,
                            out double         Kx,
                            out double         Ky1,
                            out double         Ky2) {
        Start = new Vector<double>(start);
        End   = new Vector<double>(end);
        Hx    = hx;
        Hy1   = hy1;
        Hy2   = hy2;
        Kx    = kx;
        Ky1   = ky1;
        Ky2   = ky2;
    }
}