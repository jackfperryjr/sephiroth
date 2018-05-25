using Microsoft.EntityFrameworkCore;

namespace Api.Models
{
   public class CharacterContext : DbContext  
    {  
        public CharacterContext (DbContextOptions<CharacterContext> options)  
            : base(options)  
        {  
        }  
        public DbSet<Api.Models.Character> Character { get; set; }  
    }  
}