namespace ERMS.Constants
{
    namespace ERMS.Constants
    {
        public static class Messages
        {
            public static class Success
            {
                public const string EmployeeCreated = "Employee created successfully";
                public const string EmployeeUpdated = "Employee updated successfully";
                public const string EmployeeDeleted = "Employee deleted successfully";
                public const string DepartmentCreated = "Department created successfully";
                public const string DepartmentUpdated = "Department updated successfully";
                public const string DepartmentDeleted = "Department deleted successfully";
                public const string PositionCreated = "Position created successfully";
                public const string PositionUpdated = "Position updated successfully";
                public const string PositionDeleted = "Position deleted successfully";
            }

            public static class Error
            {
                public const string EmployeeNotFound = "Employee not found";
                public const string EmployeeDeleteFailed = "Failed to delete employee";
                public const string DepartmentNotFound = "Department not found";
                public const string DepartmentDeleteFailed = "Failed to delete department";
                public const string PositionNotFound = "Position not found";
                public const string PositionDeleteFailed = "Failed to delete position";
                public const string UnauthorizedAccess = "You are not authorized to access this resource";
                public const string InvalidInput = "Please check your input and try again";
            }

            public static class Warning
            {
                public const string NoDataAvailable = "No data available";
                public const string DuplicateEmail = "Email already exists";
                public const string DuplicateName = "Name already exists";
            }

            public static class Info
            {
                public const string LoginRequired = "Please login to continue";
                public const string SessionExpired = "Your session has expired. Please login again";
            }
        }
    }
}
