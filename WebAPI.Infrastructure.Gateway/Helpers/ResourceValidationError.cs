namespace WebAPI.Infrastructure.Gateway.Helpers
{
    public class ResourceValidationError
    {
        public string ValidatorKey { get; set; }
        public string Message { get; set; }

        public ResourceValidationError(string validatorKey,string message)
        {
            ValidatorKey = validatorKey;
            Message = message;
        }
    }
}