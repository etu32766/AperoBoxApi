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
    public class CommentaireController : ControllerBase
    {
        private readonly AperoBoxApi_dbContext context;
        private readonly CommentaireDAO commentaireDAO;
        private readonly UtilisateurDAO utilisateurDAO;
        private readonly IMapper mapper;
        public CommentaireController(AperoBoxApi_dbContext context, IMapper mapper)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.commentaireDAO = new CommentaireDAO(context);
            this.utilisateurDAO = new UtilisateurDAO(context);
            this.mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = Constants.Roles.Admin)] 
        [ProducesResponseType(200, Type = typeof(IEnumerable<CommentaireDTO>))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<IEnumerable<Commentaire>>> GetAllCommentaires()
        {
            List<Commentaire> commentaires = await commentaireDAO.GetAllCommentaires();
            if (commentaires == null)
                return NotFound();

            return Ok(mapper.Map<List<CommentaireDTO>>(commentaires));
        }

        [HttpGet("Box/{idBox}")]
        [AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CommentaireDTO>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<IEnumerable<Commentaire>>> GetAllCommentairesByIdBox(int idBox)
        {
            List<Commentaire> commentaires = await commentaireDAO.GetAllCommentairesByIdBox(idBox);
            if (commentaires == null)
                return NotFound();

            return Ok(mapper.Map<List<CommentaireDTO>>(commentaires));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = Constants.Roles.Admin)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CommentaireDTO>))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<Commentaire>> GetCommentaireById(int id)
        {
            Commentaire commentaire = await commentaireDAO.GetCommentaireById(id);
            if (commentaire == null)
                return NotFound();

            return Ok(mapper.Map<CommentaireDTO>(commentaire));
        }

        [HttpPost]
        [Authorize(Roles = Constants.Roles.Utilisateur)]
        [ProducesResponseType(201, Type = typeof(CommentaireDTO))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        public async Task<ActionResult> AjouterCommentaire([FromBody]CommentaireDTO commentaireDTO)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            int userId = int.Parse(User.Claims.First(c => c.Type == PrivateClaims.UserId).Value);
            Utilisateur utilisateur = await utilisateurDAO.GetUtilisateurById(userId);
            if (utilisateur == null && !User.IsInRole(Constants.Roles.Utilisateur))
                return Forbid();

            commentaireDTO.Utilisateur = userId;

            Commentaire commentaire = mapper.Map<Commentaire>(commentaireDTO);
            commentaire = await commentaireDAO.AjouterCommentaire(commentaire);
            return Created($"api/Commentaire/{commentaire.Id}", mapper.Map<CommentaireDTO>(commentaire));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Constants.Roles.Admin)]
        [ProducesResponseType(200, Type = typeof(CommentaireDTO))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> SuppressionCommentaire(int id) 
        {
            Commentaire commentaire = await commentaireDAO.GetCommentaireById(id);
            if(commentaire == null)
                return NotFound();
            
            await commentaireDAO.SuppressionCommentaire(commentaire);
            return Ok();
        }
    }
}
