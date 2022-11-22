namespace DigitalModelsNotLinear.methods;
public class LOS : LUMethods
{
    private SLAU slau;                       /// Матрица
    protected double EPS;                    /// Точность решения 
    protected int maxIter;                   /// Максимальное количество итераций

    public string Path { get; set; }         /// Путь к задаче

    //: Табличка с итерациями и невязкой
    StringBuilder table_iter;                /// Табличка каждой итерации и невязки
    //: -----------------------------------------------------

    public LOS(SLAU slau, double EPS, int maxIter, string path) {
        this.slau    = slau;
        this.EPS     = EPS;
        this.maxIter = maxIter;
        this.Path    = path;
    }

    public Vector<double> solve(bool log = false) {
        //: Таблички
        if (log == true)
            table_iter = new StringBuilder($"Iter{" ", 5} Nev{" ", 12}\n");
        //: ---------------------------------------------------

        Vector<double> r   = new Vector<double>(slau.N);
        Vector<double> z   = new Vector<double>(slau.N);
        Vector<double> Az  = new Vector<double>(slau.N);
        Vector<double> LAU = new Vector<double>(slau.N);
        Vector<double> p   = new Vector<double>(slau.N);

        double f_norm = Sqrt(Scalar(slau.f, slau.f));

        // LU-разложение 
        Init(slau);
        LU_decomposition();

        // Начальное приближение
        slau.q.Fill(1);

        r = Solve_L(slau.f - slau.mult(slau.q));
        z = Solve_U(r);
        p = Solve_L(slau.mult(z));

        double alpha, betta, Eps = 0;
        int Iter = 0;
        do {
            betta  = Scalar(p, p);

            alpha  = Scalar(p, r) / betta;
            slau.q = slau.q + alpha * z;
            r      = r - alpha * p;
            LAU    = Solve_L(slau.mult(Solve_U(r)));
            betta  = -Scalar(p, LAU) / betta;
            z      = Solve_U(r) + betta * z;
            p      = LAU + betta * p;
            Eps    = Scalar(r, r);

            //: Табличка
            if (log == true) {
                table_iter.Append($"{String.Format("{0,4}", ++Iter)}"  + 
                                  $"{String.Format("{0,19}",  Eps.ToString("E6"))}\n");
            }
            //: -------------------------------------------------
        } while(Iter < maxIter &&
                Eps  > EPS);    

        //: Запись таблички в файл
        if (log == true)
            File.WriteAllText(Path + "/table_iter.txt", table_iter.ToString());
        //: -------------------------------------------------

        return slau.q;
    }

}