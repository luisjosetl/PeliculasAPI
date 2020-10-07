
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeliculasAPI.DTOs
{
    public class PaginacionDTO
    {
        public int Pagina { get; set; } = 1;
        private int catidadRegistrosPorPagina = 10;
        private int catidadMaximaRegistrosPorPagina = 50;
        public int CatidadRegistrosPorPagina
        {
            get => catidadRegistrosPorPagina;
            set
            {
                catidadRegistrosPorPagina = (value > catidadMaximaRegistrosPorPagina) ? catidadMaximaRegistrosPorPagina : value;
            }
        }
    }
}
