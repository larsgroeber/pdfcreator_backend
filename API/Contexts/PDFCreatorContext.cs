using Microsoft.EntityFrameworkCore;

namespace API.Contexts
{
    public class PDFCreatorContext: DbContext
    {
        public PDFCreatorContext(DbContextOptions<PDFCreatorContext> options)
            : base(options)
        {
        }

    }
}