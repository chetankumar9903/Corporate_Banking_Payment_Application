namespace Corporate_Banking_Payment_Application.Utilities
{
    public class EmployeeCodeGenerator
    {


        public static string GenerateEmployeeCode(string companyName, int clientId, int EmployeeId)
        {

            var prefix = new string(companyName
                .Where(char.IsLetter)
                .Take(4)
                .ToArray())
                .ToUpper();

            if (string.IsNullOrWhiteSpace(prefix))
                prefix = "EMPL";


            int nextSequence = EmployeeId;

            //  Format: TCS-001-0005
            return $"{prefix}-{clientId:D3}-{nextSequence:D4}";
        }



    }
}
