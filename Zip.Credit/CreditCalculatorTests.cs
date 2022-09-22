using System;
using Xunit;

namespace Zip.Credit
{
    public class CreditCalculatorTests
    {
        [Theory]
        [InlineData(400, 0, 1, 37, 0)]
        [InlineData(500, 2, 2, 22, 100)]
        [InlineData(750, 1, 3, 29, 400)]
        [InlineData(900, 0, 4, 60, 600)]
        public void Should_CorrectlyCalculateAvailableCredit_Given_CustomerInformation(
            int bureauScore, 
            int missedPaymentCount, 
            int completedPaymentCount, 
            int ageInYears, 
            int expectedCredit)
        {
            var customer = new Customer(bureauScore, missedPaymentCount, completedPaymentCount, ageInYears);
            var creditCalculator = new CreditCalculator(); 
            Assert.Equal(expectedCredit, creditCalculator.CalculateCredit(customer));
        }
        
        [Theory]
        [InlineData(450, 0)]
        [InlineData(451, 1)]
        [InlineData(700, 1)]
        [InlineData(701, 2)]
        [InlineData(850, 2)]
        [InlineData(851, 3)]
        [InlineData(10000, 3)]
        public void Should_CorrectlyCalculateCreditPoint_From_BureauScore(int bureauScore, int expectedCredit)
        {
            Assert.Equal(expectedCredit, CreditCalculator.GetPointFromBureauScore(bureauScore));
        }
        
        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, -1)]
        [InlineData(2, -3)]
        [InlineData(3, -6)]
        [InlineData(4, -6)]
        public void Should_CorrectlyCalculateCreditPoint_From_MissedPaymentCount(int missedPaymentCount, int expectedCredit)
        {
            Assert.Equal(expectedCredit, CreditCalculator.GetPointFromMissedPaymentCount(missedPaymentCount));
        }
        
        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 2)]
        [InlineData(2, 3)]
        [InlineData(3, 4)]
        [InlineData(4, 4)]
        public void Should_CorrectlyCalculateCreditPoint_From_CompletedPaymentCount(int completedPaymentCount, int expectedCredit)
        {
            Assert.Equal(expectedCredit, CreditCalculator.GetPointFromCompletedPaymentCount(completedPaymentCount));
        }
        
        [Theory]
        [InlineData(18, 3)]
        [InlineData(19, 3)]
        [InlineData(25, 3)]
        [InlineData(26, 4)]
        [InlineData(35, 4)]
        [InlineData(36, 5)]
        [InlineData(50, 5)]
        [InlineData(51, 6)]
        public void Should_CorrectlyCalculatePointCap_From_Age(int ageInYears, int expectedCap)
        {
            Assert.Equal(expectedCap, CreditCalculator.GetPointCapFromAge(ageInYears));
        }
        
        [Fact]
        public void Should_ThrowArgumentException_Given_InvalidMissedPaymentCount()
        {
            Assert.Throws<ArgumentException>(() => CreditCalculator.GetPointFromMissedPaymentCount(-1));
        }
        
        [Fact]
        public void Should_ThrowArgumentException_Given_InvalidCompletedPaymentCount()
        {
            Assert.Throws<ArgumentException>(() => CreditCalculator.GetPointFromCompletedPaymentCount(-1));
        }
        
        [Fact]
        public void Should_ThrowArgumentException_Given_InvalidAge()
        {
            Assert.Throws<ArgumentException>(() => CreditCalculator.GetPointCapFromAge(17));
        }
        
        [Fact]
        public void Should_ThrowArgumentException_Given_InvalidBureauScore()
        {
            Assert.Throws<ArgumentException>(() => CreditCalculator.GetPointFromBureauScore(-1));
        }
    }
}