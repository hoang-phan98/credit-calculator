using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Zip.Credit 
{
    internal class Program
    {
        public static void Main(string[] args) { }
    }
    
    interface ICreditCalculator
    {
        /// <summary>
        /// Calculates the available credit (in $) for a given customer
        /// </summary>
        /// <param name="customer">The customer for whom we are calculating credit</param>
        /// <returns>Available credit amount in $</returns>
        decimal CalculateCredit(Customer customer);
    }

    public class CreditCalculator : ICreditCalculator
    {
        private static readonly int CreditPerPoint = 100;
        
        private static readonly int[] PointsPerMissedPayment = {0, -1, -3, -6};
        private static readonly int[] PointsPerCompletedPayment = {0, 2, 3, 4};
        
        private static readonly int[] BureauScoreBoundaries = {451, 701, 851};
        private static readonly int[] BureauScoreBoundaryPoints = {0, 1, 2, 3};
        
        private static readonly int[] AgeGroups = {18, 26, 36, 51};
        private static readonly int[] AgeGroupsPointCap = {0, 3, 4, 5, 6};
        
        /// <summary>
        /// Finds the index of the first score group that's greater than the customer's credit score &
        /// use this to determine the corresponding credit point value.
        /// </summary>
        /// <param name="bureauScore">The credit score of a customer from a credit bureau</param>
        /// <returns>The credit point value corresponding to the given bureau score</returns>
        public static int GetPointFromBureauScore(int bureauScore)
        {
            if (bureauScore < 0)
            {
                throw new ArgumentException("Invalid bureau score.");
            }
            
            // This index represents the index of the first score boundary that's greater than the customer's score
            var index = Array.IndexOf(BureauScoreBoundaries,
                BureauScoreBoundaries.FirstOrDefault(score => score > bureauScore));

            // If no defined boundaries exceeds the customer's score, we can safely assign the max amount of credit
            return index < 0 ? BureauScoreBoundaryPoints.Last() : BureauScoreBoundaryPoints[index];
        }

        /// <summary>
        /// Returns the amount of credit point to be deducted for the given number of missed payments
        /// </summary>
        /// <param name="missedPaymentCount">The number of missed payments by a customer</param>
        /// <returns>
        /// The (zero or negative) credit point value corresponding to the given number of missed payments
        /// </returns>
        public static int GetPointFromMissedPaymentCount(int missedPaymentCount)
        {
            if (missedPaymentCount < 0)
            {
                throw new ArgumentException("The number of missed payments must be greater or equal to zero.");
            }

            return missedPaymentCount >= PointsPerMissedPayment.Length
                ? PointsPerMissedPayment.Last()
                : PointsPerMissedPayment[missedPaymentCount];
        }

        /// <summary>
        /// Returns the amount of credit point to be added for the given number of completed payments
        /// </summary>
        /// <param name="completedPaymentCount">The number of completed payments made by a customer</param>
        /// <returns>
        /// The (zero or positive) credit point value corresponding to the given number of completed payments
        /// </returns>
        public static int GetPointFromCompletedPaymentCount(int completedPaymentCount)
        {
            if (completedPaymentCount < 0)
            {
                throw new ArgumentException("The number of completed payments must be greater or equal to zero.");
            }

            return completedPaymentCount >= PointsPerCompletedPayment.Length
                ? PointsPerCompletedPayment.Last()
                : PointsPerCompletedPayment[completedPaymentCount];
        }

        /// <summary>
        /// Returns the credit point cap for a customer given their age in years
        /// </summary>
        /// <param name="ageInYears">The age of the customer in years</param>
        /// <returns>The maximum credit point for a customer of the given age</returns>
        public static int GetPointCapFromAge(int ageInYears)
        {
            if (ageInYears < 18)
            {
                throw new ArgumentException("Customers must be 18 years or older.");
            }

            // This index represents the index of the first age group that's greater than the customer's age
            var index = Array.IndexOf(AgeGroups,
                AgeGroups.FirstOrDefault(ageGroup => ageGroup > ageInYears));

            // If no defined age group exceeds the customer's score, we can safely assign highest credit cap
            return index < 0 ? AgeGroupsPointCap.Last() : AgeGroupsPointCap[index];
        }

        public decimal CalculateCredit(Customer customer)
        {
            // Customers with a bureau score smaller than the minimum defined are not able to use Zip
            if (customer.BureauScore < BureauScoreBoundaries.First())
            {
                return 0;
            }

            var creditPoint =
                GetPointFromBureauScore(customer.BureauScore) +
                GetPointFromMissedPaymentCount(customer.MissedPaymentCount) +
                GetPointFromCompletedPaymentCount(customer.CompletedPaymentCount);

            var availableCredit = Math.Min(creditPoint, GetPointCapFromAge(customer.AgeInYears)) * CreditPerPoint;

            return Math.Max(availableCredit, 0);
        }
    }
}