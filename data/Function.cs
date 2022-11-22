namespace DigitalModelsNotLinear;
public static class Function
{
    public static uint           numberFunc;     /// Номер задачи
    public static Vector<double> Absolut_sigma;  /// Абсолютное значение Sigma 
    public static Vector<double> sigma_Init;     /// Начальное значение Sigma

    //* Инициализация переменных
    public static void Init(uint numF) {
        numberFunc = numF;

        switch(numberFunc) {
            case 1:
            Absolut_sigma = new Vector<double>(new double[]{0.1, 0.1});
            sigma_Init    = new Vector<double>(new double[]{0.01, 0.01});
            break;
        }
    }

    //* Значение sigma
    public static double Sigma(double layer, Vector<double> sigma) {
        return (layer >= -200) ? sigma[0] : sigma[1];
    }

    //* Функция Ug
    public static double Ug(int numKraev, int numSide, Node node) => 0;

}