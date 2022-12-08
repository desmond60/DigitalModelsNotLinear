namespace DigitalModelsNotLinear;

// % ***** Class for JSON ***** % //
public class Data  {
    
    //: Данные для задачи
    public double[][] Receivers   { get; set; }    //@ Положение приемников
    public double[][] Sources     { get; set; }    //@ Положение источников
    
    public double I            { get; set; } //@ Сила тока
    public double Noise        { get; set; } //@ Шум
    public double SigmaInit    { get; set; } //@ Начальная сигма
    public double SigmaAbsolut { get; set; } //@ Точная сигма
    public double Eps          { get; set; } //@ Точность решения

    //: Деконструктор
    public void Deconstruct(out Receiver[] receivers, 
                            out Source[]   sources,
                            out double i,
                            out double noise,
                            out double sigmaInit,
                            out double sigmaAbsolut) {

        receivers = new Receiver[Receivers.Length];
        sources   = new Source[Sources.Length];

        for (int k = 0; k < Receivers.Length; k++)
            receivers[k] = new Receiver(Receivers[k]);

        for (int k = 0; k < Sources.Length; k++)
            sources[k] = new Source(Sources[k]);
        i            = this.I;
        noise        = this.Noise;
        sigmaInit    = this.SigmaInit;
        sigmaAbsolut = this.SigmaAbsolut;
    }
}