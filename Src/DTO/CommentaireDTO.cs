using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AperoBoxApi.DTO
{
    public class CommentaireDTO
    {
        public decimal Id { get; set; }
        [Required]
        [StringLength(1000)]
        public string Texte { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime DateCreation { get; set; }
        public decimal Utilisateur { get; set; }
        public decimal Box { get; set; }
        public byte[] RowVersion { get; set; }
    }
}