using Microsoft.EntityFrameworkCore;
using Npgsql;
using TestApi.Models;

namespace TestApi.Handlers
{
    public class DbUpdateExceptionHandler
    {
        public static ExceptionHandlerResult HandleDbUpdateException(DbUpdateException ex)
        {
            if (ex.InnerException is NpgsqlException npgsqlEx)
            {
                if (ex.InnerException is PostgresException postgresEx)
                {
                    /* DOCS
                    This exception only corresponds to a PostgreSQL-delivered error.
                    Other errors (e.g. network issues) will be raised via NpgsqlException,
                    and purely Npgsql-related issues which aren't related to the server will
                    be raised via the standard CLR exceptions (e.g. ).
                    */
                    return new ExceptionHandlerResult()
                    {
                        statusCode = StatusCodes.Status409Conflict,
                        message = postgresEx.Message
                    };
                }
                else
                {
                    return new ExceptionHandlerResult()
                    {
                        statusCode = StatusCodes.Status500InternalServerError,
                        message = npgsqlEx.Message
                    };
                }
            }

            // default
            return new ExceptionHandlerResult()
            {
                statusCode = StatusCodes.Status500InternalServerError,
                message = ex.Message
            };
        }
    }
}
