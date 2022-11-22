namespace DigitalModelsNotLinear.methods;
public abstract class LUMethods
{   
    private SLAU slau; 
    protected Vector<double> lgl, lgu, ldi;
    protected int N, N_el;

    //* Инициализация 
    protected void Init(SLAU slau) {
        this.slau = slau;
        this.N    = slau.N;
        this.N_el = slau.N_el;
        lgl       = new Vector<double>(N_el);
        lgu       = new Vector<double>(N_el);
        ldi       = new Vector<double>(N);
        Vector<double>.Copy(slau.gl, lgl);
        Vector<double>.Copy(slau.gu, lgu);
        Vector<double>.Copy(slau.di, ldi);
    }

    //* lU-разложение 
    protected void LU_decomposition() {
        double sumDI, sumGL, sumGU;
        for (int i = 0; i < N; i++) {
            sumDI = 0;
            for (int j = slau.ig[i]; j < slau.ig[i + 1]; j++) {
                sumGL = sumGU = 0;
                int p_s = slau.ig[slau.jg[j]], p_s1 = slau.ig[slau.jg[j] + 1];
                for (int k = slau.ig[i]; k < j; k++)
                    for (int p = p_s; p < p_s1; p++)
                        if (slau.jg[k] == slau.jg[p]) {
                            sumGL += lgl[k]*lgu[p];
                            sumGU += lgl[p]*lgu[k];
                            p_s++;
                        }
                lgl[j] -= sumGL;
                lgu[j]  = (lgu[j] - sumGU) / ldi[slau.jg[j]]; 
                sumDI  += lgl[j]*lgu[j];
            }
            ldi[i] -= sumDI;
        }
    }

    //* Решение СЛАУ L^(-1)
    protected Vector<double> Solve_L(Vector<double> f) {
        Vector<double> x = new Vector<double>(N);
        double sum;
        for (int i = 0; i < N; i++) {
            sum = 0;
            for (int j = slau.ig[i]; j < slau.ig[i + 1]; j++)
                sum += lgl[j]*x[slau.jg[j]];
            x[i] = (f[i] - sum) / ldi[i];
        }
        return x;
    }

    //* Решение СЛАУ U^(-1)
    protected Vector<double> Solve_U(Vector<double> f) {
        Vector<double> x = new Vector<double>(N);
        Vector<double> f_copy = new Vector<double>(N);
        Vector<double>.Copy(f, f_copy);
        for (int i = N - 1; i >= 0; i--) {
            x[i] = f_copy[i] / ldi[i];
            for (int j = slau.ig[i]; j < slau.ig[i + 1]; j++)
                f_copy[slau.jg[j]] -= lgu[j]*x[i];
        }
        return x;
    }

}