using System;
using Xunit;

namespace PhoneNumberHelper.Test
{
    public class PhoneNumberHelperTests
    {
        [Theory]
        [InlineData("501111111", "SA", true, "+966501111111")]
        [InlineData("0501111111", "SA", true, "+966501111111")]
        [InlineData("O5o1111111", "SA", true, "+966501111111")]
        [InlineData("+966501111111", "SA", true, "+966501111111")]
        [InlineData("+9660501111111", "SA", true, "+966501111111")]
        [InlineData("+966O5o1111111", "SA", true, "+966501111111")]
        [InlineData("+966501111111", "AE", true, "+966501111111")]
        [InlineData("+9660501111111", "AE", true, "+966501111111")]
        [InlineData("+966O5o1111111", "AE", true, "+966501111111")]
        [InlineData("+966501111111", null, true, "+966501111111")]
        [InlineData("+9660501111111", null, true, "+966501111111")]
        [InlineData("+966O5o1111111", null, true, "+966501111111")]
        [InlineData("966501111111", "SA", true, "+966501111111")]
        [InlineData("9660501111111", "SA", true, "+966501111111")]
        [InlineData("966O5o1111111", "SA", true, "+966501111111")]

        [InlineData("9669901111111", "SA", false, "9669901111111")]
        [InlineData("501111111", null, false, "501111111")]
        public void TryNormalizeRcTests(string phoneNumber, string regionCode, bool expectedResult, string expectedNormalizedPhoneNumber)
        {
            var result = PhoneNumberHelper.TryNormalizeRc(phoneNumber, regionCode, out var normalizedPhoneNumber);
            Assert.Equal(expectedResult, result);
            Assert.Equal(expectedNormalizedPhoneNumber, normalizedPhoneNumber);
        }

        [Theory]
        [InlineData("501111111", "Asia/Riyadh", true, "+966501111111")]
        [InlineData("0501111111", "Asia/Riyadh", true, "+966501111111")]
        [InlineData("O5o1111111", "Asia/Riyadh", true, "+966501111111")]
        [InlineData("+966501111111", "Asia/Riyadh", true, "+966501111111")]
        [InlineData("+9660501111111", "Asia/Riyadh", true, "+966501111111")]
        [InlineData("+966O5o1111111", "Asia/Riyadh", true, "+966501111111")]
        [InlineData("+966501111111", "Asia/Dubai", true, "+966501111111")]
        [InlineData("+9660501111111", "Asia/Dubai", true, "+966501111111")]
        [InlineData("+966O5o1111111", "Asia/Dubai", true, "+966501111111")]
        [InlineData("+966501111111", null, true, "+966501111111")]
        [InlineData("+9660501111111", null, true, "+966501111111")]
        [InlineData("+966O5o1111111", null, true, "+966501111111")]
        [InlineData("966501111111", "Asia/Riyadh", true, "+966501111111")]
        [InlineData("9660501111111", "Asia/Riyadh", true, "+966501111111")]
        [InlineData("966O5o1111111", "Asia/Riyadh", true, "+966501111111")]

        [InlineData("9669901111111", "Asia/Riyadh", false, "9669901111111")]
        [InlineData("501111111", null, false, "501111111")]
        public void TryNormalizeTzTests(string phoneNumber, string timezone, bool expectedResult, string expectedNormalizedPhoneNumber)
        {
            var result = PhoneNumberHelper.TryNormalizeTz(phoneNumber, timezone, out var normalizedPhoneNumber);
            Assert.Equal(expectedResult, result);
            Assert.Equal(expectedNormalizedPhoneNumber, normalizedPhoneNumber);
        }

        [Theory]
        [InlineData("+966501111111", true)]
        [InlineData("+9660501111111", true)]
        [InlineData("+966901111111", false)]
        public void IsValidNumber(string phoneNumber, bool expectedResult)
        {
            var result = PhoneNumberHelper.IsValidNumber(phoneNumber);
            Assert.Equal(expectedResult, result);
        }
    }
}
