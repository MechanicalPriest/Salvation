using NUnit.Framework;
using Salvation.Core.State;
using System.Collections;

namespace Salvation.CoreTests.State
{
    [TestFixture]
    class StatDiminishingReturnsTests
    {
        [TestCaseSource(typeof(DiminshingReturnTestsData), nameof(DiminshingReturnTestsData.TestData))]
        public double DRRatingTest(double rating, double cost)
        {
            // Arrange
            var _gameStateService = new GameStateService();

            // Act
            var result = _gameStateService.GetDrRating(rating, cost);

            // Assert
            return result;
        }
    }

    public class DiminshingReturnTestsData
    {
        public static IEnumerable TestData
        {
            get
            {
                yield return new TestCaseData(217d, 35).Returns(217);
                yield return new TestCaseData(267d, 10.67083793).Returns(267);
                yield return new TestCaseData(319d, 10.67083793).Returns(319);
                // val is the required stat for the percent shown, while the input is the indicated stat
                yield return new TestCaseData(383d, 10.67083793d).Returns(376.79999999999859d);
                yield return new TestCaseData(399d, 10.06107577).Returns(389.29999999999779d);
                // make sure zero stat is zero
                yield return new TestCaseData(0, 10.06107577).Returns(0);
            }
        }
    }
}
