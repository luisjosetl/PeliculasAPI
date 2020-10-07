using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PeliculasAPI.DTOs
{
    public class SalaDeCineCercanoFiltroDTO
    {
        [Range(-90, 90)]
        public double Latitud { get; set; }
        [Range(-180, 180)]
        public double Longitud { get; set; }
        private int distanciaEnMinKms = 10;
        private int distanciaMaxEnKms = 50;
        public int DistanciaEnKms {
            get 
            {
                return distanciaEnMinKms;
            } 
            set 
            {
                distanciaEnMinKms = (value > distanciaMaxEnKms) ? distanciaMaxEnKms : value;
            } 
        }
    }
}
