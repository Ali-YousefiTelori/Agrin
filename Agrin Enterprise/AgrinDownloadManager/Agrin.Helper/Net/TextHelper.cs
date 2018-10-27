using System;
using System.Linq;

namespace Framesoft.Helpers.Helpers
{
    /// <summary>
    /// توابع کمک کننده ی متنی
    /// </summary>
    public static class TextHelper
    {
        /// <summary>
        /// حذف یک متن از ابتدای متن مورد نظر
        /// </summary>
        /// <param name="target">متن مورد نظر</param>
        /// <param name="trimString">متنی که میخواهید حذف شود از ابتدای متن مورد نظر</param>
        /// <returns>متن مورد نظر ویرایش شده</returns>
        public static string TrimStart(string target, string trimString)
        {
            string result = target;
            while (result.StartsWith(trimString))
            {
                result = result.Substring(trimString.Length);
            }
            return result;
        }

        /// <summary>
        /// ویرایش شماره تلفن
        /// </summary>
        /// <param name="phone">شماره تلفن</param>
        /// <returns>شماره تلفن ویرایش شده</returns>
        public static string CleanPhoneNumber(this string phone)
        {
            if (string.IsNullOrEmpty(phone))
                return phone;
            phone = TrimStart(phone, "0098");
            phone = TrimStart(phone, "098");
            phone = TrimStart(phone, "+98");
            phone = TrimStart(phone, "98");
            phone = TrimStart(phone, "0");
            phone = phone.Replace(" ", "");
            return "98" + phone;
        }
        /// <summary>
        /// بررسی اینکه اعداد ورودی برای شماره صحیح می باشد یا خیر
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static bool IsNumberValudate(this string number)
        {
            string numbers = "0123456789";
            foreach (char item in number)
            {
                if (!numbers.Contains(item))
                {
                    Console.WriteLine("number is not valid : " + number);
                    return false;
                }
            }
            return true;
        }

        private static Random Randomize { get; set; } = new Random();

        /// <summary>
        /// دریافت یک متن رندوم
        /// </summary>
        /// <returns></returns>
        public static string GetRandomString()
        {
            const string chars = "abcdefghijklmmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, 6).Select(s => s[Randomize.Next(s.Length)]).ToArray());
        }

        public static string Base64Encode(string plainText)
        {
            byte[] plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            byte[] base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes, 0, base64EncodedBytes.Length);
        }
    }
}
