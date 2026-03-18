namespace OrganisationRegistry.Api.Infrastructure.Swagger;

using System;
using System.Linq;
using System.Reflection;
using Infrastructure.Search.Pagination;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

/// <summary>
/// Voegt automatisch een opmerking toe aan GET-operaties die paginering ondersteunen,
/// zodat deze tekst niet in elke controller herhaald moet worden.
/// </summary>
public class PaginationHeaderOperationFilter : IOperationFilter
{
    private static readonly MethodInfo AddPaginationResponseMethod =
        typeof(AddPaginationExtension).GetMethod(nameof(AddPaginationExtension.AddPaginationResponse))!;

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (!CallsAddPaginationResponse(context.MethodInfo))
            return;

        const string hint = "Geef de header `x-pagination: none` mee om alle resultaten op te vragen.";

        operation.Description = string.IsNullOrWhiteSpace(operation.Description)
            ? hint
            : $"{operation.Description.TrimEnd()}\n\n{hint}";
    }

    private static bool CallsAddPaginationResponse(MethodInfo method)
    {
        try
        {
            var body = method.GetMethodBody();
            if (body == null)
                return false;

            var il = body.GetILAsByteArray();
            if (il == null)
                return false;

            var module = method.Module;

            var i = 0;
            while (i < il.Length)
            {
                // call / callvirt opcodes: 0x28 / 0x6F — followed by 4-byte method token
                if ((il[i] == 0x28 || il[i] == 0x6F) && i + 4 < il.Length)
                {
                    var token = BitConverter.ToInt32(il, i + 1);
                    try
                    {
                        var calledMethod = module.ResolveMethod(token);
                        if (calledMethod?.Name == AddPaginationResponseMethod.Name &&
                            calledMethod.DeclaringType?.FullName == AddPaginationResponseMethod.DeclaringType?.FullName)
                            return true;
                    }
                    catch
                    {
                        // token not resolvable — skip
                    }

                    i += 5;
                    continue;
                }

                i++;
            }

            return false;
        }
        catch
        {
            return false;
        }
    }
}
