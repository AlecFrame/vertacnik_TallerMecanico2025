using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace vertacnik_TallerMecanico2025.Models
{
    public class RepositorioBase
    {
        protected readonly IConfiguration configuration;
        protected readonly string connectionString;

        public RepositorioBase(IConfiguration configuration)
        {
            this.configuration = configuration;
            connectionString = configuration["ConnectionStrings:DefaultConnection"];
        }
    }
}