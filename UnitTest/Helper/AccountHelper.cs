using Application.DTOs.Identity;

namespace UnitTest.Helper
{
    public class AccountHelper
    {
        public CreateAccountDto ValidCreateAccountDto()
        {
            return new CreateAccountDto
            {
                UserName = "test",
                Email = "test@test.com",
                FullName = "Test User",
                Password = "Password@123",
                RoleIds = new List<string>()
            };
        }

    }
}
