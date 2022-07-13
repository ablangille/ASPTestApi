using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using TestApi.Models;

namespace TestApi.Handlers
{
    public class DbUpdateExceptionHandler
    {
        public static ExceptionHandlerResult HandleDbUpdateException(DbUpdateException ex)
        {
            if (ex.InnerException is SqlException sqlEx)
            {
                switch (sqlEx.Number)
                {
                    case 2627: // Unique constraint error
                    case 547: // Constraint check violation
                    case 2601: // Duplicated key row error
                        return new ExceptionHandlerResult()
                        {
                            statusCode = StatusCodes.Status409Conflict,
                            message = sqlEx.Message
                        };
                    default:
                        return new ExceptionHandlerResult()
                        {
                            statusCode = StatusCodes.Status500InternalServerError,
                            message = sqlEx.Message
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
