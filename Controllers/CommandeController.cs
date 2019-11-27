using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AperoBoxApi.Models;
using AperoBoxApi.Context;

namespace AperoBoxApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CommandeController : ControllerBase
    {
        public CommandeController()
        {
            ;
        }

        [HttpGet]
        public IEnumerable<Commande> Get()
        {
            using (AperoBoxApi_dbContext context = new AperoBoxApi_dbContext())
            {
                var commandes = context.Commande
                    .ToList();
                    
                return commandes;
            }
        }


        [HttpPost]
        public void Post()
        {
            
        }
    }
}
