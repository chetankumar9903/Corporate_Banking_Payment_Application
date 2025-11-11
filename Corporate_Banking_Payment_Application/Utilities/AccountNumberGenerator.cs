namespace Corporate_Banking_Payment_Application.Utilities
{
    public class AccountNumberGenerator
    {


        //{BANK3}{YYMM}{CID3}{RND4}
        public static string GenerateAccountNumber(string bankPrefix, int Id)
        {
            if (string.IsNullOrWhiteSpace(bankPrefix))
                bankPrefix = "BNK";

            var bankCode = bankPrefix.Length >= 3 ? bankPrefix[..3].ToUpper() : bankPrefix.ToUpper().PadRight(3, 'X');


            var IdSegment = (Id % 1000).ToString("D3");

            var indianTime = TimeZoneInfo.ConvertTimeFromUtc(
    DateTime.UtcNow,
    TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")
);

            var dateSegment = indianTime.ToString("yyMM");
            var randomSegment = new Random().Next(1000, 9999).ToString();



            return $"{bankCode}{IdSegment}{dateSegment}{randomSegment}";
        }
    }
}
