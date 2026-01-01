namespace EntityFrameworkCore.Classes
{
    public interface IConvertDate
    {
        DateTime ConvertShamsiToMiladi(string Date);

        String ConvertMiladiToShamsi(DateTime Date, string Format);
    }
}
