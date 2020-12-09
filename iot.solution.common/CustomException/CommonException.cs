namespace iot.solution.common
{
    public struct CommonException
    {
        public struct Name
        {
            public const string NoRecordsFound = "No records found.";
            public const string EmptyDetails = "Cannot insert empty details.";
            public const string IsRequired = " is required.";
            public const string IsInvalid = " is invalid.";
            public const string InvalidData = "Invalid Data.";
            public const string AlreadyExist = " already exists.";
            public const string NotExist = " does not exists.";
            public const string Unauthorized = "You are not authorized to perform this operation";
            public const string InvalidCredentials = "The Credential to perform task is Invalid.";
            public const string InvalidRequest = "Invalid request.";
        }
    }
}
