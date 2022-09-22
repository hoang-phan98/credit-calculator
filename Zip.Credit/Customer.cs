namespace Zip.Credit
{
    public class Customer
    {
        public int BureauScore { get; }

        public int MissedPaymentCount { get; }

        public int CompletedPaymentCount { get; }

        public int AgeInYears { get; }

        public Customer(int bureauScore, int missedPaymentCount, 
            int completedPaymentCount, int ageInYears)
        {
            BureauScore = bureauScore;
            MissedPaymentCount = missedPaymentCount;
            CompletedPaymentCount = completedPaymentCount;
            AgeInYears = ageInYears;
        }
    }
}