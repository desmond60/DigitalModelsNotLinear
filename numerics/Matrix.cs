namespace DigitalModelsNotLinear.numerics;
public class Matrix
{
    private double[,] matrix;           /// Матрица (квадратная)
    public int Dim { get; init; }       /// Размерность матрицы

    //* Конструктор (с размерностью)
    public Matrix(int _dim) {
        matrix = new double[_dim, _dim];
        this.Dim = _dim;
    }

    //* Конструктор (с двумерным массивом)
    public Matrix(double[,] _mat) {        
        Dim = _mat.GetUpperBound(0) + 1;
        matrix = new double[Dim, Dim];
        for (int i = 0; i < Dim; i++)
            for (int j = 0; j < Dim; j++)
                matrix[i, j] = _mat[i, j];           
    }
 
    //* Деконструктор
    public void Deconstruct(out double[,] mat) {
        mat = this.matrix;
    }

    //* Индексаторы
    public double this[int i, int j] {
        get { return matrix[i, j];  }
        set { matrix[i, j] = value; }
    }

    //* Перегрузка умножение числа на матрицу
    public static Matrix operator *(double Const, Matrix mat) {
        var result = new Matrix(mat.Dim);
        for (int i = 0; i < result.Dim; i++)
            for (int j = 0; j < result.Dim; j++)
                result[i, j] = Const * mat[i, j];
        return result;
    }

    //* Перегрузка сложение матриц 
    public static Matrix operator +(Matrix mat1, Matrix mat2) {
        var result = new Matrix(mat1.Dim);
        for (int i = 0; i < result.Dim; i++)
            for (int j = 0; j < result.Dim; j++)
                result[i, j] = mat1[i, j] + mat2[i, j];
        return result;
    }

    //* Копирование матриц
    public static void Copy(Matrix source, Matrix dest) {
        for (int i = 0; i < source.Dim; i++)
            for (int j = 0; j < source.Dim; j++)
                dest[i, j] = source[i, j];
    }

    //* Очистка матрицы
    public static void Clear(Matrix matrix) {
        for (int i = 0; i < matrix.Dim; i++)
            for (int j = 0; j < matrix.Dim; j++)
                matrix[i,j] = 0;
    }

    //* Строковое представление матрицы
    public override string ToString() {
        StringBuilder mat = new StringBuilder();
        if (matrix == null) return mat.ToString();

        for (int i = 0; i < Dim; i++) {
            for (int j = 0; j < Dim; j++)
                mat.Append(matrix[i, j] + "\t");
            mat.Append("\n");
        }
        return mat.ToString();
    }

}