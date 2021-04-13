using System;
using Xunit;
using DemoApp.Infrastructure.Utilities;

namespace DemoApp.Tests
{
    public class PasswordUtilityTests
    {
        [Fact]
        public void EncryptPassword()
        {
            var pw = "Testing12";

            var hash = PasswordUtility.EncryptPassword(pw, 10000);
            var hashParts = hash.Split('.');

            Assert.Equal(3, hashParts.Length);
        }

        [Fact]
        public void CheckPassword_KnownValues_Validates()
        {
            var pw = "T3stP@ssw0rd";
            var hash = "10000.ZXhKk+k7lZ1xdHxnYO6TFA==.WPbmVg9TvVtjHub4l3e4Lb0N9PTRCSmOr81pTJOF72U=";
            //var hash = PasswordUtility.EncryptPassword(pw, 10000);

            var valid = PasswordUtility.CheckPassword(hash, pw);

            Assert.True(valid);
        }
    }
}
