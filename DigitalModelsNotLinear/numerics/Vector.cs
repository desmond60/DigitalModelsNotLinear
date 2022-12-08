namespace DigitalModelsNotLinear.numerics;

// % ****** Class Vector ***** % //
public class Vector<T> : ICloneable
                where T : System.Numerics.INumber<T>
{
    public T[] vector;                    // Вектор
    public int Length => vector.Length;   // Размерность вектора

    //: Перегрузка неявного преобразования
    public static explicit operator T[](Vector<T> vec) {
        return vec.vector;
    }

    //: Деконструктор
    public void Deconstruct(out T[] vec)
    {
        vec = this.vector;
    }

    //: Конструктор (с размерностью)
    public Vector(int lenght)
    {
        vector = new T[lenght];
    }

    //: Конструктор (с массивом)
    public Vector(T[] array)
    {
        vector = new T[array.Length];
        Array.Copy(array, vector, array.Length);
    }

    //: Индексатор
    public T this[int index]
    {
        get => vector[index];
        set => vector[index] = value;
    }

    //: Перегрузка умножения двух векторов
    public static T operator *(Vector<T> vec1, Vector<T> vec2)
    {
        T result = T.Zero;
        for (int i = 0; i < vec1.Length; i++)
            result += vec1[i] * vec2[i];
        return result;
    }

    //: Перегрузка умножения на констунту (double)
    public static Vector<T> operator *(T Const, Vector<T> vector)
    {
        var result = new Vector<T>(vector.Length);
        for (int i = 0; i < vector.Length; i++)
            result[i] = Const * vector[i];
        return result;
    }
    public static Vector<T> operator *(Vector<T> vector, T Const) => Const * vector;

    //: Перегрузка деления на константу (double)
    public static Vector<T> operator /(Vector<T> vector, T Const)
    {
        var result = new Vector<T>(vector.Length);
        for (int i = 0; i < vector.Length; i++)
            result[i] = vector[i] / Const;
        return result;
    }
    
    //: Перегрузка деления константы на вектор
    public static Vector<T> operator /(T Const, Vector<T> vector)
    {
        var result = new Vector<T>(vector.Length);
        for (int i = 0; i < vector.Length; i++)
            result[i] = Const / vector[i];
        return result;
    }

    //: Перегрузка сложения двух векторов
    public static Vector<T> operator +(Vector<T> vec1, Vector<T> vec2)
    {
        var result = new Vector<T>(vec1.Length);
        for (int i = 0; i < vec1.Length; i++)
            result[i] = vec1[i] + vec2[i];
        return result;
    }

    //: Перегрузка вычитания двух векторов
    public static Vector<T> operator -(Vector<T> vec1, Vector<T> vec2)
    {
        var result = new Vector<T>(vec1.Length);
        for (int i = 0; i < vec1.Length; i++)
            result[i] = vec1[i] - vec2[i];
        return result;
    }

    //: Перегрузка тернарного минуса
    public static Vector<T> operator -(Vector<T> vector)
    {
        var result = new Vector<T>(vector.Length);
        for (int i = 0; i < vector.Length; i++)
            result[i] = -vector[i];
        return result;
    }

    //: Заполнение вектора числом (double)
    public void Fill(T val)
    {
        for (int i = 0; i < Length; i++)
            vector[i] = val;
    }

    //: Копирование вектора
    public static void Copy(Vector<T> source, Vector<T> dest)
    {
        for (int i = 0; i < source.Length; i++)
            dest[i] = source[i];
    }

    //: Очистка вектора
    public static void Clear(Vector<T> vector)
    {
        for (int i = 0; i < vector.Length; i++)
            vector[i] = T.Zero;
    }

    //: Выделение памяти под вектор
    public static void Resize(ref Vector<T> vector, int lenght)
    {
        vector = new(lenght);
    }

    //: Строковое представление вектора
    public override string ToString()
    {
        StringBuilder vec = new StringBuilder();

        for (int i = 0; i < Length; i++)
            vec.Append(vector[i].ToString() + "\n");

        return vec.ToString();
    }

    //: Копирование объектов Vector
    public object Clone() { return MemberwiseClone(); }
}