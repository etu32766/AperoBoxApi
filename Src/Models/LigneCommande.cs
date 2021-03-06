﻿using System;
using System.Collections.Generic;

namespace AperoBoxApi.Models
{
    public partial class LigneCommande
    {
        public decimal Id { get; set; }
        public decimal Quantite { get; set; }
        public decimal Commande { get; set; }
        public decimal? Box { get; set; }
        public decimal? Produit { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual Box BoxNavigation { get; set; }
        public virtual Commande CommandeNavigation { get; set; }
        public virtual Produit ProduitNavigation { get; set; }
    }
}
