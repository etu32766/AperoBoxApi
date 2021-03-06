using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AperoBoxApi.Models;
using AperoBoxApi.Context;
using AperoBoxApi.DAO;
using AperoBoxApi.DTO;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace AperoBoxApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/[controller]")]
    public class CommandeController : ControllerBase
    {
        private readonly AperoBoxApi_dbContext context;
        private readonly CommandeDAO commandeDAO;
        private readonly UtilisateurDAO utilisateurDAO;
        private readonly IMapper mapper;
        public CommandeController(AperoBoxApi_dbContext context, IMapper mapper)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.commandeDAO = new CommandeDAO(context);
            this.utilisateurDAO = new UtilisateurDAO(context);
            this.mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = Constants.Roles.Admin)] 
        [ProducesResponseType(200, Type = typeof(IEnumerable<CommandeDTO>))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<IEnumerable<Commande>>> GetAllCommandes()
        {
            List<Commande> commandes = await commandeDAO.GetAllCommandes();
            if(commandes == null)
                return NotFound();

            return Ok(mapper.Map<List<CommandeDTO>>(commandes));
        }

        [HttpPost]
        [Authorize(Roles = Constants.Roles.Utilisateur)]
        [ProducesResponseType(201, Type = typeof(CommandeDTO))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        public async Task<ActionResult> AjouterCommande([FromBody] CommandeDTO commandeDTO)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            int userId = int.Parse(User.Claims.First(c => c.Type == PrivateClaims.UserId).Value);
            Utilisateur utilisateur = await utilisateurDAO.GetUtilisateurById(userId);
            if (utilisateur == null && !User.IsInRole(Constants.Roles.Utilisateur))
                return Forbid();

            commandeDTO.Utilisateur = userId;
            commandeDTO.Adresse = utilisateur.Adresse;
            if (commandeDTO.Promotion == 0)
                commandeDTO.Promotion = null;

            Commande commande = mapper.Map<Commande>(commandeDTO);
            commande = await commandeDAO.AjouterCommande(commande);
            return Created($"api/Commande/{commande.Id}", mapper.Map<CommandeDTO>(commande));
        }
    }
}
