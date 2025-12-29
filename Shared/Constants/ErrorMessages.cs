namespace Shared.Constants
{
    public static class ErrorMessages
    {
        //General
        public const string InvalidInput = "Invalid input provided";
        public const string NotFound = "Resource not found";
        public const string Unauthorized = "Unauthorized access";
        public const string Forbidden = "Access forbidden";
        public const string InternalServerError = "Internal server error occurred";

        public const string ErrorRetrivingEntity = "Error retrieving entity";
        public const string ErrorRetrivingEntities = "Error retrieving entities";
        public const string ErrorCreatingEntity = "Error creating entity";
        public const string ErrorUpdatingEntity = "Error updating entity";
        public const string ErrorDeletingEntity = "Error deleting entity";
        public const string ErrorRetrivingPagedResult = "Error retrieving paged result";

        //Authentication
        public const string InvalidToken = "Invalid token";
        public const string InvalidRefreshToken = "Invalid refresh token";
        public const string EmailExists = "Email already exists";
        public const string InvalidOTPCode = "Invalid OTP code";
        public const string InvalidCredentials = "Invalid username or password";
        public const string InvalidPassword = "Invalid password";
        public const string TwoFactorNotEnabled = "Two-factor authentication is not enabled for this user";
        // User
        public const string UserNotFound = "User not found";
        public const string UserAlreadyExists = "User already exists";
        public const string UserNameExits = "Username already exists";
        public const string GetAccountFailed = "Get account failed";
        public const string CreateAccountFailed = "Create account failed";
        public const string UpdateAccountFailed = "Update account failed";
        public const string EmailNotConfirmed = "You haven't confirmed your email yet";
        public const string SendEmailFailed = "Failed to send OTP email.";
    }
}
