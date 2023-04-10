using FluentValidation.Results;

namespace Core.Helpers;

public class ErrorHandler
{
    public static string UnprocessableEntity(List<ValidationFailure> errors) 
    {
        string errorMessage = " ";
        foreach (ValidationFailure err in errors)
        {
            errorMessage += err;
        }
        return errorMessage;
    }
}
