namespace DigitalModelsNotLinear;
public class Generate 
{
    protected Vector<double> start  { get; set; }      /// Начальная точка
    protected Vector<double> end    { get; set; }      /// Конечная точка
    protected double hx             { get; set; }      /// Шаг по Оси X
    protected double hy1            { get; set; }      /// Шаг по Оси Y (первый промежуток)
    protected double hy2            { get; set; }      /// Шаг по Оси Y (второй промежуток)
    protected double kx             { get; set; }      /// Коэффициент деления по Оси X  
    protected double ky1            { get; set; }      /// Коэффициент деления по Оси Y  (первый промежуток)
    protected double ky2            { get; set; }      /// Коэффициент деления по Оси Y  (второй промежуток)
    protected double mid_y          { get; set; }      /// Середина промежутков Оси Y

    protected string Path           { get; set; }      /// Путь к папке с задачей 

    private int N_X;                                            /// Количество узлов по Оси X
    private int N_Y;                                            /// Количество узлов по Оси Y (общее)
    private int N_Y1;                                           /// Количество узлов первого промежутка
    private int N_Y2;                                           /// Количество узлов второго промежутка
    private int Count_Node  => N_X * N_Y;                       /// Общее количество узлов
    private int Count_Elem  => (N_X - 1)*(N_Y - 1);             /// Общее количество КЭ
    private int Count_Kraev => 2*(N_X - 1) + 2*(N_Y - 1);       /// Количество краевых

    private int[]? SideKraev;                                   /// Номера краевых на сторонах

    //* Конструктор
    public Generate(Data data, string Path) {
        (start, end, hx, hy1, hy2, kx, ky1, ky2) = data;
        this.Path = Path;

        // Подсчет количества узлов на Осях 
        N_X = kx != 1
            ? (int)(Log(1 - (end[0] - start[0])*(kx - 1) / (hx*(-1))) / Log(kx) + 2)
            : (int)((end[0] - start[0]) / hx + 1);

        mid_y = (start[1] + end[1]) / 2.0; // подсчет серидину отрезка (будем делать слои по середине)
        N_Y1 = ky1 != 1
            ? (int)(Log(1 - (mid_y - start[1])*(ky1 - 1) / (hy1*(-1))) / Log(ky1) + 2)
            : (int)((mid_y - start[1]) / hy1 + 1);
        N_Y2 = ky2 != 1
            ? (int)(Log(1 - (end[1] - mid_y)*(ky2 - 1) / (hy2*(-1))) / Log(ky2) + 2)
            : (int)(Round((end[1] - mid_y) / hy2) + 1);
        N_Y = N_Y1 + N_Y2 - 1;
    }

    //* Инициализации сторон краевыми
    public void SetKraev(int side0, int side1, int side2, int side3) { 
        SideKraev = new int[] {side0, side1, side2, side3}; 
    }

    //* Основная функция генерации сетки
    public Grid generate() {
        if (SideKraev == null) throw new ArgumentException($"Boundary conditions are not set!\nUse the function \"SetKraev\"");

        Path += "/grid";
        Directory.CreateDirectory(Path);
        Node[]  nodes  = generate_coords(); //? Генерация координат
        Elem[]  elems  = generate_elems();  //? Генерация КЭ
        Kraev[] kraevs = generate_kraevs(); //? Генерация краевых

        return new Grid(N_X, N_Y, Count_Node, Count_Elem, Count_Kraev, nodes, elems, kraevs);
    }

    //* Генерация координат
    private Node[] generate_coords() {
        Vector<double> X_vec  = generate_array(start[0], end[0], hx, kx, N_X);
        Vector<double> Y_vec1 = generate_array(start[1], mid_y, hy1, ky1, N_Y1);
        Vector<double> Y_vec2 = generate_array(mid_y, end[1], hy2, ky2, N_Y2);

        Node[] nodes = new Node[Count_Node];

        int id = -1;
        for (int i = 0; i < Y_vec1.Length; i++)
            for (int j = 0; j < N_X; j++)
                nodes[++id] = new Node(X_vec[j], Y_vec1[i]);

        for (int i = 1; i < Y_vec2.Length; i++)
            for (int j = 0; j < N_X; j++)
                nodes[++id] = new Node(X_vec[j], Y_vec2[i]);

        File.WriteAllText(Path + "/coords.txt", String.Join("\n", nodes));
        return nodes;
    }

    //* Генерация массива по Оси (с шагом и коэффицентом разрядки)
    private Vector<double> generate_array(double start, double end, double h, double k, int n) {
        var coords = new Vector<double>(n);
        coords[0]     = start;
        coords[n - 1] = end;
        for (int i = 1; i < n - 1; i++, h *= k) 
            coords[i] = coords[i - 1] + h;
        return coords;
    }

    //* Генерация КЭ
    private Elem[] generate_elems() {
        Elem[] elems = new Elem[Count_Elem];

        for (int i = 0, id = 0; i < N_Y - 1; i++)
            for (int j = 0; j < N_X - 1; j++, id++) {
                    elems[id] = new Elem(
                          i   *N_X + j,
                          i   *N_X + j + 1,
                         (i+1)*N_X + j,
                         (i+1)*N_X + j + 1
                    );
            }

        File.WriteAllText(Path + "/elems.txt", String.Join("\n", elems));
        return elems;
    }

    //* Генерация краевых
    private Kraev[] generate_kraevs() {
        Kraev[] kraevs = new Kraev[Count_Kraev];
        int id = 0;

        // Нижняя сторона
        for (int i = 0; i < N_X - 1; i++, id++)
            kraevs[id] = new Kraev(
                SideKraev![0],
                0,
                i,
                i + 1
            );

        // Правая сторона
        for (int i = 0; i < N_Y - 1; i++, id++)
            kraevs[id] = new Kraev(
                 SideKraev![1],
                 1,
                 i     *N_X + (N_X - 1),
                (i + 1)*N_X + (N_X - 1)
            );

        // Верхняя сторона
        for (int i = 0; i < N_X - 1; i++, id++)
            kraevs[id] = new Kraev(
                SideKraev![2],
                2,
                N_X*(N_Y - 1) + i,
                N_X*(N_Y - 1) + (i + 1)
            );

        // Левая сторона
        for (int i = 0; i < N_Y - 1; i++, id++)
            kraevs[id] = new Kraev(
                 SideKraev![3],
                 3,
                  i     *N_X,
                 (i + 1)*N_X
            );

        File.WriteAllText(Path + "/kraevs.txt", String.Join("\n", kraevs));
        return kraevs;
    }


}