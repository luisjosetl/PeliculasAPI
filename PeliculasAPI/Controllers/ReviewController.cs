﻿using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entidades;
using PeliculasAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace PeliculasAPI.Controllers
{
    [Route("api/peliculas/{peliculaId:int}/reviews")]
    [ServiceFilter(typeof(PeliculaExisteAttribute))]
    public class ReviewController : CustomBaseController
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public ReviewController(
            ApplicationDbContext context,
            IMapper mapper)
            :base(context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<ReviewDTO>>> Get(int peliculaId,
            [FromQuery] PaginacionDTO paginacionDTO)
        {
           

            var queryable = context.Reviews.Include(x => x.Usuario).AsQueryable();
            queryable = queryable.Where(x => x.PeliculaId == peliculaId);
            return await Get<Review, ReviewDTO>(paginacionDTO, queryable);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Post(int peliculaId, [FromBody] ReviewCreacionDTO reviewCreacionDTO)
        {
            

            var usuarioId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;

            var existeReview = await context.Reviews
                .AnyAsync(x => x.Id == peliculaId && x.UsuarioId == usuarioId);

            if (existeReview)
                return BadRequest("El usuario ya ha escrito un review de esta pelicula");

            var review = mapper.Map<Review>(reviewCreacionDTO);
            review.PeliculaId = peliculaId;
            review.UsuarioId = usuarioId;

            context.Add(review);
            await context.SaveChangesAsync();
            return NoContent();

        }

        [HttpPut("{reviewId:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Put(int peliculaId, int reviewId,
            [FromBody] ReviewCreacionDTO reviewCreacionDTO)
        {
            

            var reviewDB = await context.Reviews.FirstOrDefaultAsync(x => x.Id == reviewId);
            if (reviewDB == null)
                return NotFound();

            var usuarioId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;

            if (reviewDB.UsuarioId != usuarioId)
                return Forbid();

            reviewDB = mapper.Map(reviewCreacionDTO, reviewDB);

            await context.SaveChangesAsync();
            return NoContent();
        }


        [HttpDelete("{reviewId:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Delete(int reviewId)
        {
            var reviewDB = await context.Reviews.FirstOrDefaultAsync(x => x.Id == reviewId);
            if (reviewDB == null)
                return NotFound();

            var usuarioId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            if (reviewDB.UsuarioId != usuarioId)
                return Forbid();

            context.Remove(reviewDB);
            await context.SaveChangesAsync();
            return NoContent();

        }
    }
}
