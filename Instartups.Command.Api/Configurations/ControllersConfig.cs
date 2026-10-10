using Instartups.Command.Api.DTOs;
using Instartups.Command.Domain.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Instartups.Command.Api.Configurations;

public static class ControllersConfig
{
    public static IServiceCollection AddControllersConfig(this IServiceCollection services)
    {
        services.AddControllers(options =>
        {
            options.ReturnHttpNotAcceptable = true;

            options.Filters.Add(new ProducesAttribute("application/json"));
            options.Filters.Add(new ConsumesAttribute("application/json"));
            options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
            options.AllowEmptyInputInBodyModelBinding = true;
        });

        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(e => e.Value?.Errors.Count > 0)
                    .SelectMany(e => e.Value!.Errors.Select(err =>
                        new ValidationErrorDTO(
                            e.Key,
                            CodigosErroConst.FormatoInvalido,
                            "Formato da requisição inválido."
                        )
                    ))
                    .ToList();

                var body = ResponseDTO<IEnumerable<ValidationErrorDTO>>.Error(
                    MensagensErroConst.FormatoInvalido, errors);

                return new BadRequestObjectResult(body)
                {
                    ContentTypes = { "application/json" }
                };
            };
        });

        return services;
    }
}