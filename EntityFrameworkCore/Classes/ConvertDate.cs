using MD.PersianDateTime.Core;

namespace EntityFrameworkCore.Classes
{
    public class ConvertDate
    {
        public DateTime ConvertShamsiToMiladi(string Date)
        {
            PersianDateTime PersianDateTime = PersianDateTime.Parse(Date);
            return PersianDateTime.ToDateTime();
        }
        public String ConvertMiladiToShamsi(DateTime Date , string Format)
        {
            PersianDateTime persianDateTime = new PersianDateTime(Date);
            return persianDateTime.ToString(Format);
        }
    }
}
