using GESAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GESAL.ViewModels
{
    public class BandejaEntradaViewModel
    {
        private Usuario _usuario;

        public BandejaEntradaViewModel(Usuario usuario)
        {
            _usuario = usuario;
        }
    }
}
