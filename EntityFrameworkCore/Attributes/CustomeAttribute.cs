using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Net;

namespace EntityFrameworkCore.Classes
{
    public class PersianDate : ValidationAttribute
    {
        private string ConvertToEnglishDigits(string input)
        {
            return input
                .Replace("۰", "0").Replace("۱", "1").Replace("۲", "2").Replace("۳", "3").Replace("۴", "4")
                .Replace("۵", "5").Replace("۶", "6").Replace("۷", "7").Replace("۸", "8").Replace("۹", "9")
                // جهت اطمینان اعداد عربی (کد اسکی متفاوت) را هم هندل می‌کنیم
                .Replace("٠", "0").Replace("١", "1").Replace("٢", "2").Replace("٣", "3").Replace("٤", "4")
                .Replace("٥", "5").Replace("٦", "6").Replace("٧", "7").Replace("٨", "8").Replace("٩", "9");
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;

            string DateString = value.ToString();
            DateString = ConvertToEnglishDigits(DateString);
            var parts = DateString.Split('/');
            if (parts.Length != 3)
            {
                return new ValidationResult("فرمت تاریخ صحیح نیست. مثال : 1402/01/01");
            }
            int Year, Month, Day;

            try
            {
                Year = int.Parse(parts[0]);
                Month = int.Parse(parts[1]);
                Day = int.Parse(parts[2]);
            }
            catch
            {
                return new ValidationResult("تاریخ باید شامل اعداد باشد.");
            }

            if (Year < 1300 || Year > 1500) return new ValidationResult("سال وارد شده معتبر نمی‌باشد.");

            if (Month < 1 || Month > 12) return new ValidationResult("ماه وارد شده باید بین 1 تا 12 باشد.");

            if (Month >= 1 && Month <= 6)
            {
                if (Day < 1 || Day > 31)
                {
                    return new ValidationResult("این ماه از سال 31 روزه می‌باشد.");
                }
            }
            else if (Month >= 7 && Month <= 11)
            {
                if (Day < 1 || Day > 31)
                {
                    return new ValidationResult("این ماه از سال 30 روزه می‌باشد.");
                }
            }
            else if (Month == 12)
            {
                PersianCalendar PC = new PersianCalendar();
                bool isleap = PC.IsLeapYear(Year);
                if (isleap)
                {
                    if (Day < 1 || Day > 30)
                    {
                        return new ValidationResult(" سال انتخابی کبیسه است و اسفند 30 روزه می‌باشد.");
                    }
                }
                else
                {
                    if(Day<1||Day>29)
                    {
                        return new ValidationResult("سال انتخابی کبیسه نمی‌باشد و اسفند 29 روزه است.");
                    }
                }
            }
            return ValidationResult.Success;
        }
    }

    public class GoogleRecaptchaValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            Lazy<ValidationResult> errorResult = new Lazy<ValidationResult>(() => new ValidationResult("لطفا با زدن تیک من ربات نیستم هویت خود را تایید کنید.", new String[] { validationContext.MemberName }));

            if (value == null || String.IsNullOrWhiteSpace(value.ToString()))
            {
                return errorResult.Value;
            }

            IConfiguration configuration = (IConfiguration)validationContext.GetService(typeof(IConfiguration));
            String reCaptchResponse = value.ToString();
            String reCaptchaSecret = configuration.GetValue<String>("GoogleReCaptcha:SecretKey");


            HttpClient httpClient = new HttpClient();
            var httpResponse = httpClient.GetAsync($"https://www.google.com/recaptcha/api/siteverify?secret={reCaptchaSecret}&response={reCaptchResponse}").Result;
            if (httpResponse.StatusCode != HttpStatusCode.OK)
            {
                return errorResult.Value;
            }

            String jsonResponse = httpResponse.Content.ReadAsStringAsync().Result;
            dynamic jsonData = JObject.Parse(jsonResponse);
            if (jsonData.success != true.ToString().ToLower())
            {
                return errorResult.Value;
            }

            return ValidationResult.Success;

        }
    }
}