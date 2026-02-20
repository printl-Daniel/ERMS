namespace ERMS.Constants
{
    public static class Messages
    {
        // ── SUCCESS ───────────────────────────────────────────────────────────
        public static class Success
        {
            public static class Auth
            {
                public const string LoginSuccess = "Login successful";
                public const string LogoutSuccess = "You have been logged out successfully";
                public const string PasswordResetSent = "If an account with that email exists, a password reset link has been sent.";
                public const string PasswordResetSuccess = "Your password has been reset successfully. You can now log in with your new password.";
            }

            public static class Employee
            {
                public const string Created = "Employee created successfully";
                public const string Deleted = "Employee deleted successfully";
                public const string Updated = "was updated successfully.";
            }

            public static class Department
            {
                public const string Created = "Department created successfully";
                public const string Updated = "Department updated successfully";
                public const string Deleted = "Department deleted successfully";
            }

            public static class Position
            {
                public const string Created = "Position created successfully";
                public const string Updated = "Position updated successfully";
                public const string Deleted = "Position deleted successfully";
            }
        }

        // ── ERROR ─────────────────────────────────────────────────────────────
        public static class Error
        {
            public const string UnauthorizedAccess = "You are not authorized to access this resource";
            public const string InvalidInput = "Please check your input and try again";
            public static class Auth
            {
                public const string InvalidCredentials = "Invalid username or password";
                public const string AccountDeactivated = "Your account has been deactivated. Please contact HR.";
                public const string EmailRequired = "Email is required.";
                public const string InvalidToken = "Invalid or expired reset token.";
                public const string ExpiredToken = "This reset link has expired. Please request a new one.";
                public const string InvalidResetLink = "Invalid reset link.";
                public const string EmailSendFailed = "Failed to send reset email. Please try again or contact your administrator.";
                public const string PasswordUpdateFailed = "Failed to update password. Please try again.";
                public const string GenericError = "An error occurred. Please try again later.";
                public const string EmailNotFound = "Email Not Found";

                public static class Password
                {
                    public const string MinLength = "Password must be at least 8 characters long.";
                    public const string RequireUpper = "Password must contain at least one uppercase letter.";
                    public const string RequireLower = "Password must contain at least one lowercase letter.";
                    public const string RequireDigit = "Password must contain at least one number.";
                    public const string RequireSpecial = "Password must contain at least one special character.";
                }
            }

            public static class Employee
            {
                public const string NotFound = "Employee not found";
                public const string DeleteFailed = "Failed to delete employee";
                public const string EmailExists = "An employee with this email already exists";

                // Add these:
                public const string InvalidRequest = "Invalid request.";
                public const string UpdateFailed = "Update failed. Please try again.";
            }

            public static class Department
            {
                public const string NotFound = "Department not found";
                public const string DeleteFailed = "Failed to delete department";
                public const string NameExists = "A department with this name already exists";
                public const string HasEmployees = "Cannot delete a department that has active employees assigned.";
            }

            public static class Position
            {
                public const string NotFound = "Position not found";
                public const string DeleteFailed = "Failed to delete position";
                public const string NameExists = "A position with this name already exists";
            }
        }

        // ── WARNING ───────────────────────────────────────────────────────────
        public static class Warning
        {
            public const string NoDataAvailable = "No data available";
            public const string DuplicateEmail = "Email already exists";
            public const string DuplicateName = "Name already exists";

            public static class Auth
            {
                public const string ResetCooldown = "A reset link was recently sent. Please wait {0} more minute(s) before requesting another.";
            }
        }

        // ── INFO ──────────────────────────────────────────────────────────────
        public static class Info
        {
            public const string LoginRequired = "Please login to continue";
            public const string SessionExpired = "Your session has expired. Please login again";
            public const string InfoMessage = "Please set a new password before continuing.";
        }
    }
}