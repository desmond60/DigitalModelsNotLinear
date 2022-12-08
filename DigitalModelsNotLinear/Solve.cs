namespace DigitalModelsNotLinear;

// % ***** Class NotLinearSolution ***** % //
public class Solve
{
    //: Поля и свойства
    private Receiver[] Receivers;   //@ Положение приемников
    private Source[]   Sources;     //@ Положение источника
    
    private double I            { get; set; } //@ Сила тока
    private double Noise        { get; set; } //@ Шум
    private double SigmaInit    { get; set; } //@ Начальная сигма
    private double SigmaAbsolut { get; set; } //@ Точная сигма

    private Table table;

    public string Path { get; set; } //@ Путь к задаче
    
    
    //: Конструктор
    public Solve(Data _data, string _path)  {
        
        // Данные
        (Receivers, Sources, I, Noise, SigmaInit, SigmaAbsolut) = _data;

        // Табличка
        table = new Table("Iteration-Solution");
        table.AddColumn(
            ("Iter", 5),
            ("Sigma", 16),
            ("V", 16)
        );
        
        // Путь
        this.Path = _path;
    }
    
    //: Основной метод решения
    public void solve(double EPS) {

        // Подсчет потенциалов от sigmaAbsolut вместе с шумом
        var V_abs = new Vector<double>(3);
        for (int i = 0; i < V_abs.Length; i++)
            V_abs[i] = Potential(Sources[0], Sources[1], Receivers[2 * i], Receivers[2 * i + 1], I, SigmaAbsolut);

        // Добавление шума
        if (Noise != 0) {
            double znak = Noise / Abs(Noise);
            V_abs = V_abs + znak * V_abs * Abs(Noise);
        }

        
        // Подсчет весов
        Vector<double> W = 1 / V_abs;

        // Подготовка к итерационому процессу
        double F, a, b;
        double SigmaIter = SigmaInit;
        uint Iter = 0;

        // Подсчет потенциала
        var V = new Vector<double>(3);
        for (int i = 0; i < V.Length; i++)
            V[i] = Potential(Sources[0], Sources[1], Receivers[2 * i], Receivers[2 * i + 1], I, SigmaIter);

        // Подсчет производной потенциалов
        var V_diff = new Vector<double>(3);
        for (int i = 0; i < V_diff.Length; i++)
            V_diff[i] = DiffPotential(Sources[0], Sources[1], Receivers[2 * i], Receivers[2 * i + 1], I, SigmaIter);

        // Итерационный процесс
        do
        {
            // Обнуление компонент
            a = 0;
            b = 0;
            F = 0;

            for (int i = 0; i < 3; i++)
            {
                a += Pow((W[i] * V_diff[i]), 2);
                b += -Pow(W[i], 2) * V_diff[i] * (V[i] - V_abs[i]);
            }

            // Подсчет новой сигмы
            SigmaIter += b / a;

            // Подсчет потенциала
            for (int i = 0; i < V.Length; i++)
                V[i] = Potential(Sources[0], Sources[1], Receivers[2 * i], Receivers[2 * i + 1], I, SigmaIter);

            // Подсчет производной потенциалов
            for (int i = 0; i < V_diff.Length; i++)
                V_diff[i] = DiffPotential(Sources[0], Sources[1], Receivers[2 * i], Receivers[2 * i + 1], I, SigmaIter);

            // Подксчет компоненты для выхода
            for (int i = 0; i < 3; i++)
                F += Pow((W[i] * (V[i] - V_abs[i])), 2);

            // Добавление записи в табличку
            table.AddRow((++Iter).ToString(), SigmaIter.ToString("E6"), F.ToString("E6"));

        } while (F > EPS);
        
        table.WriteToFile(Path + "/solution.txt");
    }
}