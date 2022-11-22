namespace DigitalModelsNotLinear.other;

public struct Grid    /// Структура сетки
{
    public int Count_Node  { get; set; }    /// Общее количество узлов
    public int Count_Elem  { get; set; }    /// Общее количество КЭ
    public int Count_Kraev { get; set; }    /// Количество краевых
    public int N_X         { get; set; }    /// Количество узлов по Оси X
    public int N_Y         { get; set; }    /// Количество узлов по Оси Y

    public Node[]  Nodes;      /// Узлы
    public Elem[]  Elems;      /// КЭ
    public Kraev[] Kraevs;     /// Краевые           

    public Grid(int n_x,
                int n_y,
                int count_node,
                int count_elem, 
                int count_kraev, 
                Node[] nodes, 
                Elem[] elem, 
                Kraev[] kraevs) {
        this.N_X         = n_x;
        this.N_Y         = n_y;
        this.Count_Node  = count_node;
        this.Count_Elem  = count_elem;
        this.Count_Kraev = count_kraev;
        this.Nodes       = nodes;
        this.Elems       = elem;
        this.Kraevs      = kraevs;
    }

    public void Deconstruct(out Node[]  nodes,
                            out Elem[]  elems,
                            out Kraev[] kraevs) {
        nodes  = this.Nodes;
        elems  = this.Elems;
        kraevs = this.Kraevs;
    }
}

public struct Node     /// Структура Узла
{
    public double x { get; set; }  /// Координата X 
    public double y { get; set; }  /// Координата Y 

    public Node(double _x, double _y) {
        x = _x; y = _y;
    }

    public void Deconstruct(out double[] param) {
        param = new double[]{this.x, this.y};
    } 

    public void Deconstruct(out double x, 
                            out double y) 
    {
        x = this.x;
        y = this.y;
    }

    public override string ToString() => $"{x,20} {y,24}";
}

public struct Elem     /// Структура КЭ
{
    public int[] Node;  /// Узлы КЭ

    public Elem(params int[] node) { Node = node; }

    public void Deconstruct(out int[] nodes) { nodes = this.Node; }

    public override string ToString() {
        StringBuilder str_elem = new StringBuilder();
        str_elem.Append($"{Node[0],5}");
        for (int i = 1; i < Node.Count(); i++)
            str_elem.Append($"{Node[i],8}");
        return str_elem.ToString();
    }
}

public struct Kraev    /// Структура краевого
{
    public int[] Node;      /// Узлы КЭ
    public int   numKraev;  /// Номер краевого
    public int   numSide;   /// Номер стороны на котором задано краевое

    public Kraev(int num, int side, params int[] node) { 
        this.numKraev = num;
        this.numSide  = side;
        this.Node     = node; 
    }

    public void Deconstruct(out int[] nodes, out int num, out int side) { 
        nodes = this.Node;
        num   = this.numKraev;
        side  = this.numSide; 
    }

    public override string ToString() {
        StringBuilder str_elem = new StringBuilder();
        str_elem.Append($"{numKraev} {numSide,5} {Node[0],5}");
        for (int i = 1; i < Node.Count(); i++)
            str_elem.Append($"{Node[i],8}");
        return str_elem.ToString();
    }
}

public struct SLAU     /// Структура СЛАУ
{
    public Vector<double> di, gl, gu;    /// Матрица
    public Vector<int> ig, jg;           /// Массивы с индексами
    public Vector<double> f, q;          /// Правая часть и решение
    public int N;                        /// Размерность матрицы
    public int N_el;                     /// Размерность gl и gu

    //* Очистка СЛАУ
    public void Clear() {
        Vector<double>.Clear(q);
        Vector<double>.Clear(f);
        Vector<double>.Clear(di);
        Vector<double>.Clear(gl);
        Vector<double>.Clear(gu);
    }

    //* Умножение матрицы на вектор
    public Vector<double> mult(Vector<double> x) {
        Vector<double> result = new Vector<double>(N);
        for (int i = 0; i < N; i++) {
            result[i] = di[i]*x[i];
            for (int j = ig[i]; j < ig[i + 1]; j++) {
                result[i]      += gl[j]*x[jg[j]];
                result[jg[j]]  += gu[j]*x[i];
            }
        }
        return result;
    }
}

public struct Receiver      /// Структура приемника
{
    public double x { get; set; }  /// Координата X 
    public double y { get; set; }  /// Координата Y
    public double z { get; set; }  /// Координата Z

    public Receiver(double _x, double _y, double _z) {
        this.x = _x; this.y = _y; this.z = _z;
    }

    public Receiver(double[] point) {
        this.x = point[0]; this.y = point[1]; this.z = point[2];
    }

    public void Deconstruct(out double x, 
                            out double y,
                            out double z) 
    {
        x = this.x;
        y = this.y;
        z = this.z;
    }

    public override string ToString() => $"{x,20} {y,24} {z,26}";
}

public struct Source      /// Структура источника
{
    public double x { get; set; }  /// Координата X 
    public double y { get; set; }  /// Координата Y
    public double z { get; set; }  /// Координата Z

    public Source(double _x, double _y, double _z) {
        this.x = _x; this.y = _y; this.z = _z;
    }

    public Source(double[] point) {
        this.x = point[0]; this.y = point[1]; this.z = point[2];
    }

    public void Deconstruct(out double x, 
                            out double y,
                            out double z) 
    {
        x = this.x;
        y = this.y;
        z = this.z;
    }

    public override string ToString() => $"{x,20} {y,24} {z,26}";
}

public static class Helper
{
    //* Вычисление нормы вектора
    public static double Norma(Vector<double> vec) {
        double res = 0;
        for (int i = 0; i < vec.Length; i++)
            res += vec[i]*vec[i];
        return Sqrt(res);
    }

    //* Скалярное произведение векторов
    public static double Scalar(Vector<double> frst, Vector<double> scnd) {
        double res = 0;
        for (int i = 0; i < frst.Length; i++)
            res += frst[i]*scnd[i];
        return res;
    }

    //* Расстояние от точки измерения до электрода
    public static double interval(Receiver R, Source S) {
        return Sqrt(Pow(S.x - R.x, 2) + Pow(S.y - R.y, 2) + Pow(S.z - R.z, 2));
    }

    //* Расчет потенциала
    // public static double potential(Source A, Source B, Receiver M, Receiver N, double I) {
    //     double diff = (1 / interval(M, B) - 1 / interval(M, A)) - (1 / interval(N, B) - 1 / interval(N, A));
    //     return I / (2.0 * PI * sigma) * diff;
    // }

    //* Расчет diff потенциала
    // public static double diff_potential(Source A, Source B, Receiver M, Receiver N, double I) {
    //     double diff = (1 / interval(M, B) - 1 / interval(M, A)) - (1 / interval(N, B) - 1 / interval(N, A));
    //     return -I / (2.0 * PI * sigma * sigma) * diff;
    // }

    //* Расчет суммарного потенциала (1 приемник от 3 источников)
    // public static double summPotential(Source[] sources, Receiver receiver, Receiver next_receiver, Vector<double> I) {
    //     double v1 = potential(sources[0], sources[1], receiver, next_receiver, I[0]);
    //     double v2 = potential(sources[2], sources[3], receiver, next_receiver, I[1]);
    //     double v3 = potential(sources[4], sources[5], receiver, next_receiver, I[2]);
    //     return v1 + v2 + v3;
    // }

    //* Окно помощи при запуске (если нет аргументов или по команде)
    public static void ShowHelp() {
        WriteLine("----Команды----                        \n" + 
        "-help             - показать справку             \n" + 
        "-i                - входной файл                 \n");
    }
}