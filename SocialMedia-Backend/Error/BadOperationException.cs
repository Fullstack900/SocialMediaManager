using SocialMedia_Backend.Model.DTO;

namespace SocialMedia_Backend.Error;
public class BadOperationException : Exception
{
    public string TranslationKey { get; set; }
    public string? FieldName { get; set; }
    public Dictionary<string, object>? ErrorContext { get; }

    public BadOperationException(string message, string translationKey = "bad_operation_error", Dictionary<string, object>? errorContext = null, string? fieldName = null)
        : base(message)
    {
        TranslationKey = translationKey;
        ErrorContext = errorContext;
        FieldName = fieldName;
    }

    public object WriteResponse()
    {
        var model = new FieldValidationErrorModel
        {
            Context = ErrorContext,
            DefaultMessage = Message,
            Key = TranslationKey
        };

        if (string.IsNullOrEmpty(FieldName))
        {
            return model;
        }
        else
        {
            var fieldErrors = new Dictionary<string, List<FieldValidationErrorModel>>
            {
                [FieldName] = new List<FieldValidationErrorModel>() {
                        new FieldValidationErrorModel
                        {
                            Context = ErrorContext,
                            DefaultMessage = Message,
                            Key = TranslationKey
                        }
                    }
            };
            return new { message = "Bad Request", fields = fieldErrors };
        }
    }
}
