using System;
using GraphQL;
using GraphQL.Types;

namespace API.Services
{
    public static class GraphQlErrorService
    {
        public static void AttachError(ResolveFieldContext<object> context, Exception e)
        {
            context.Errors.Add(new ExecutionError(e.Message));
            if (e.InnerException != null)
            {
                context.Errors.Add(new ExecutionError(e.InnerException.Message));
            }
        }
    }
}