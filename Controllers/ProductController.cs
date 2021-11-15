using System.Collections.Generic;
using System.Threading.Tasks;
using Backoffice.Models;
using Microsoft.AspNetCore.Mvc;
using Backoffice.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace Shoping.Controllers
{
    [Route("v1/products")]
    public class ProductController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> Get([FromServices] DataContext context)
        {
            var products = await context.Products.Include(x => x.Category).AsNoTracking().ToListAsync();
            return Ok(products);
        }
        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> GetById([FromServices] DataContext context, int id)
        {
            var product = await context.Products.Include(x => x.Category).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return Ok(product);
        }
        [HttpGet]
        [Route("categories/{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> GetByCategory([FromServices] DataContext context, int id)
        {
            var products = await context.Products.Include(x => x.Category).AsNoTracking().Where(x => x.CategoryId == id).ToListAsync();
            return Ok(products);
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<Product>> Post(
        [FromBody] Product model,
        [FromServices] DataContext context
        )
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                context.Products.Add(model);
                await context.SaveChangesAsync();
                return Ok(model);

            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível criar o Produto" });
            }

        }
        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<Product>> Put(
        int id,
        [FromBody] Product model,
        [FromServices] DataContext context)
        {
            // Verifica se o ID informado é o mesmo do modelo
            if (id != model.Id)
            {
                return NotFound(new { message = "Produto não encontrada" });
            }
            // Verifica se os dados são válidos
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                context.Entry<Product>(model).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return Ok(model);
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new { message = "Esse registro já foi atualizado" });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível atualizar a categoria" });
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<Product>> Delete(
            int id,
            [FromServices] DataContext context)
        {
            var product = await context.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (product == null)
            {
                return NotFound(new { message = "Produto não encontrada" });
            }
            try
            {
                context.Products.Remove(product);
                await context.SaveChangesAsync();
                return Ok(new { message = "Produto removida" });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível remover o produto" });
            }
        }
    }
}