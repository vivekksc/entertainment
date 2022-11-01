using FluentValidation.AspNetCore;
using FluentValidation.Results;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Entertainment.Microservices.Middleware.ErrorHandling;

namespace microservices.entertainment.Validation
{
    public abstract class BaseValidator<T> : AbstractValidator<T>, IValidatorInterceptor
    {
        public IValidationContext BeforeAspNetValidation(ActionContext controllerContext,
            IValidationContext commonContext)
        {
            return commonContext;
        }

        public ValidationResult AfterAspNetValidation(ActionContext actionContext, IValidationContext validationContext,
            ValidationResult result)
        {
            if (!result.IsValid) throw new ValidationResultException(result);

            return result;
        }

        public class Factory
        {
            public static ValidationResult CreateNonPropertyValidationResult(string message)
            {
                return
                    new(
                        new List<ValidationFailure>
                        {
                            new("NonPropertyFailure", message)
                        });
            }
        }
    }
}