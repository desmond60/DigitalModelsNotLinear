try
{
    // Путь к задаче
    string path = @"files/task.json";
    
    // Десериализация данных
    string json = File.ReadAllText(path);
    Data data = JsonConvert.DeserializeObject<Data>(json)!;
    if (data is null) throw new FileNotFoundException("File uncorrected!");

    // Решение нелинейной задачи
    Solve task = new Solve(data, Path.GetDirectoryName(path));
    task.solve(data.Eps);
}
catch (FileNotFoundException ex) {
    WriteLine(ex.Message);
}