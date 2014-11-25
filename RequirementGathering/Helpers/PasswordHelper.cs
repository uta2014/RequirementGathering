using System;
using System.Web.Security;

namespace RequirementGathering.Helpers
{
    public class PasswordHelper
    {
        public static string GeneratePassword()
        {
            int length = 18;
            var random = new Random(length % new Random().Next()).Next(100, 999);
            var basePassword = Membership.GeneratePassword(10, 1);

            return Guid.NewGuid().ToString().Replace("-", "").Substring(0, 5) + basePassword + random;
        }
    }
}
