using System;
using System.Collections.Generic;
using GraphQL;
using GraphQL.Types;

namespace API.Services
{
    public static class GraphQlErrorService
    {
        public static dynamic Context { get; set; }
        public static void Add(Exception e)
        {
            Context.Errors.Add(new ExecutionError(e.Message));
            if (e.InnerException != null)
            {
                Context.Errors.Add(new ExecutionError(e.InnerException.Message));
            }
        }

        public static void AttachError(dynamic context, Exception e)
        {
            context.Errors.Add(new ExecutionError(e.Message));
            if (e.InnerException != null)
            {
                context.Errors.Add(new ExecutionError(e.InnerException.Message));
            }
        }
    }
}