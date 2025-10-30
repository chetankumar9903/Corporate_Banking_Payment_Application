namespace Corporate_Banking_Payment_Application.Utilities
{
    public class AccountNumberGenerator
    {
        // You can customize prefix, format, or length easily later
        //public static string GenerateAccountNumber(string bankCode = "AC")
        //{
        //    // Example: AC-202510-XYZ12345
        //    var randomSegment = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
        //    var dateSegment = DateTime.UtcNow.ToString("yyyyMM");
        //    return $"{bankCode}-{dateSegment}-{randomSegment}";
        //}


        // Generates a unique, traceable account number.
        // Format Example: HDFC-00012-202510-A1B2C3D4

        // {BANK}-{CLIENTID}-{DATE}-{RANDOM}

        //{BANK3}{YYMM}{CID3}{RND4}
        public static string GenerateAccountNumber(string bankPrefix, int Id)
        {
            if (string.IsNullOrWhiteSpace(bankPrefix))
                bankPrefix = "BNK";

            //// Bank code limited to 4 uppercase letters
            //var bankCode = bankPrefix.Length > 4 ? bankPrefix[..4].ToUpper() : bankPrefix.ToUpper();

            //// Pad ClientId to fixed length for uniform look
            //var paddedClientId = clientId.ToString().PadLeft(5, '0');

            //var dateSegment = DateTime.UtcNow.ToString("yyyyMM");
            //var randomSegment = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();

            //// ✅ Example result: HDFC-00012-202510-A1B2C3D4
            //return $"{bankCode}-{paddedClientId}-{dateSegment}-{randomSegment}";



            // Use only first 3 uppercase characters of bank
            var bankCode = bankPrefix.Length >= 3 ? bankPrefix[..3].ToUpper() : bankPrefix.ToUpper().PadRight(3, 'X');

            // Last 3 digits of ClientId (zero-padded)
            var IdSegment = (Id % 1000).ToString("D3");

            //// YYMM
            //var dateSegment = DateTime.UtcNow.ToString("yyMM");

            //// 4-character random alphanumeric
            //var random = Guid.NewGuid().ToString("N").Substring(0, 4).ToUpper();

            var indianTime = TimeZoneInfo.ConvertTimeFromUtc(
    DateTime.UtcNow,
    TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")
);

            var dateSegment = indianTime.ToString("yyMM");   // e.g., "2510"
            var randomSegment = new Random().Next(1000, 9999).ToString(); // e.g., "4827"



            return $"{bankCode}{IdSegment}{dateSegment}{randomSegment}";
        }
    }
}
