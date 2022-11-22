namespace DigitalModelsNotLinear;
public class Solve 
{
    private Node[]     Nodes;               /// Узлы
    private Elem[]     Elems;               /// КЭ
    private Kraev[]    Kraevs;              /// Краевые
    private SLAU       slau;                /// Структура СЛАУ
    private Grid       grid;                /// Структура сетки

    private Receiver[] Receivers;           /// Положение приемников
    private Source[]   Sources;             /// Положение источника

    private Matrix G_sml;
    private Matrix M_sml_h;
    private Matrix M_sml_r;

    public string Path { get; set; }        /// Путь к задаче

    //* Конструктор
    public Solve(Grid grid, Data data, string path) {
        (Nodes, Elems, Kraevs) = grid;
        (Receivers, Sources)   = data;
        this.Path              = path;

        G_sml = new Matrix(new double[2,2] {
            {1, -1}, {-1, 1}
        });

        M_sml_h = new Matrix(new double[2,2] {
            {1, 1}, {1, 3}
        });

        M_sml_r = new Matrix(new double[2,2] {
            {2, 1}, {1, 2}
        });
    }

    //* Основной метод решения
    public void solve() {
        portrait();                //? Составление портрета матрицы
        global(Absolut_sigma);     //? Составление глобальной матрицы
        LOS los = new LOS(slau, 1e-30, 10000, Path);
        slau.q = los.solve(true);
        //WriteLine(slau.q);
    }

    //* Составление портрета матрицы (ig, jg, выделение памяти)
    private void portrait() {
        Portrait port = new Portrait(Nodes.Count());
        
        // Генерируем массивы ig и jg и размерность
        slau.N_el = port.GenPortrait(ref slau.ig, ref slau.jg, Elems);
        slau.N    = Nodes.Count();

        // Выделяем память
        slau.gl = new Vector<double>(slau.N_el);
        slau.gu = new Vector<double>(slau.N_el);
        slau.di = new Vector<double>(slau.N);
        slau.f  = new Vector<double>(slau.N);
        slau.q  = new Vector<double>(slau.N);
    }

    //* Составление глобальной матрицы
    private void global(Vector<double> sig) {

        // Для каждого конечного элемента
                // Для каждого конечного элемента
        for (int index_fin_el = 0; index_fin_el < Elems.Count(); index_fin_el++) {
            // Составление локальной матрицы
            (Matrix local_matrix, Vector<double> local_f) = local(index_fin_el, sig);

            // Добавление в глобальную
            EntryGlobalMatrix(local_matrix, local_f, index_fin_el);

            // Поставим дельта-функцию в левый верхний узел
            slau.f[(grid.N_Y - 1)*grid.N_X] += 1.0 / (2.0 * PI);
        }

        // Учет только первых краевых 
        for (int index_kraev = 0; index_kraev < Kraevs.Count(); index_kraev++) {
            Kraev cur_kraev = Kraevs[index_kraev];
            if (cur_kraev.numKraev == 1)
                First_Kraev(cur_kraev, cur_kraev.numSide);  
        }
    }

    //* Составление локальной матрицы и вектора
    public (Matrix, Vector<double>) local(int index, Vector<double> sig) {
        Matrix matrix = new Matrix(4);
        Vector<double> vector = new Vector<double>(4);

        // Локальные функции
        int mu(int index) => index % 2;
        int nu(int index) => index / 2;

        // Переменные для удобства расчета
        double node1_x = Nodes[Elems[index].Node[0]].x; // x - левого-нижнего узла КЭ
        double node1_y = Nodes[Elems[index].Node[0]].y; // y - левого-нижнего узла КЭ
        double sigma  = Sigma(node1_y, sig);
        double hx     = Nodes[Elems[index].Node[1]].x - Nodes[Elems[index].Node[0]].x;
        double hy     = Nodes[Elems[index].Node[3]].y - Nodes[Elems[index].Node[0]].y;

        
        for (int i = 0; i < matrix.Dim; i++) {
            for (int j = 0; j <= i; j++) {
                // Построение матрицы
                matrix[i,j] = sigma * (
                    (node1_x * hy / (hx * 6.0) + hy / 12.0) * G_sml[mu(i), mu(j)] * M_sml_r[nu(i), nu(j)] +
                    (hx * node1_x) / (6.0 * hy) * G_sml[nu(i), nu(j)] * M_sml_r[mu(i), mu(j)] +
                    (hx * hx) / (12.0 * hy) * G_sml[nu(i), nu(j)] * M_sml_h[mu(i), mu(j)]
                );
                if (i != j)
                    matrix[j,i] = matrix[i,j];
            }
        }

        return (matrix, vector);
    }

    //* Поиск столбца
    private int find(int f, int s) {
        int col = 0;
        for (int i = slau.ig[f]; i < slau.ig[f + 1]; i++) {
            if (slau.jg[i] == s) {
                col = i;
                break;
            }
        }
        return col;
    }

    //* Занесение матрицы и вектора в глобальную
    private void EntryGlobalMatrix(Matrix mat, Vector<double> vec, int index) {
        for (int i = 0; i < 4; i++) {
            int row = Elems[index].Node[i];
            for (int j = 0; j < i; j++) {
                if (row > Elems[index].Node[j]) {
                    int col = find(row, Elems[index].Node[j]);
                    slau.gl[col] += mat[i,j];
                    slau.gu[col] += mat[j,i];
                }
                else {
                    int col = find(Elems[index].Node[j], row);
                    slau.gu[col] += mat[i,j];
                    slau.gl[col] += mat[j,i];
                }
            }
            slau.di[row] += mat[i,i];
            slau.f[row]  += vec[i];
        }
    }

    //* Учет первого краевого условия
    private void First_Kraev(Kraev kraev, int side) {
        slau.di[kraev.Node[0]] = 1e+10;
        slau.di[kraev.Node[1]] = 1e+10;

        slau.f[kraev.Node[0]] = 1e+10 * Ug(kraev.numKraev, kraev.numSide, Nodes[kraev.Node[0]]);
        slau.f[kraev.Node[1]] = 1e+10 * Ug(kraev.numKraev, kraev.numSide, Nodes[kraev.Node[1]]);
    }

}