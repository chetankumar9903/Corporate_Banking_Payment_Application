namespace Corporate_Banking_Payment_Application.Utilities
{
    public class EmployeeCodeGenerator
    {
        // Example: EMP-2510-004A
        //public static string GenerateEmployeeCode(int clientId)
        //{
        //    var dateSegment = DateTime.UtcNow.ToString("yyMM");
        //    var randomSegment = Guid.NewGuid().ToString("N").Substring(0, 4).ToUpper();
        //    return $"EMP-{dateSegment}-{clientId % 1000:D3}{randomSegment[0]}";
        //}



        //private static readonly object _lock = new object();
        //private static int _sequenceCounter = 0; // temporary in-memory counter (resets on app restart)


        /// Generates an employee code based on company name initials and auto-increment sequence.
        /// Example: INFY-001-0001

        public static string GenerateEmployeeCode(string companyName, int clientId, int EmployeeId)
        {
            //  Extract up to 4 uppercase letters from company name (prefix)
            var prefix = new string(companyName
                .Where(char.IsLetter)
                .Take(4)
                .ToArray())
                .ToUpper();

            if (string.IsNullOrWhiteSpace(prefix))
                prefix = "EMPL";

            //  Use total existing employees as base for incremental code
            int nextSequence = EmployeeId;

            //  Format: TCS-001-0005
            return $"{prefix}-{clientId:D3}-{nextSequence:D4}";
        }


        //public static string GenerateEmployeeCode(string companyName, int clientId, int existingCount)
        //{
        //    //  Extract up to 4 uppercase letters from company name (prefix)
        //    var prefix = new string(companyName
        //        .Where(char.IsLetter)
        //        .Take(4)
        //        .ToArray())
        //        .ToUpper();

        //    if (string.IsNullOrWhiteSpace(prefix))
        //        prefix = "EMPL";

        //    //  Use total existing employees as base for incremental code
        //    int nextSequence = existingCount + 1;

        //    //  Format: TCS-001-0005
        //    return $"{prefix}-{clientId:D3}-{nextSequence:D4}";
        //}
    }
}
