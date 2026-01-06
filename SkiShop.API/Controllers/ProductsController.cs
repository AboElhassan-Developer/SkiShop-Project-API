using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkiShop.API.RequestHelpers;

namespace SkiShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IUnitOfWork unit) : BaseApiController
    {
        [Cache(600)]
        [HttpGet]
        public async Task<ActionResult<IReadOnlySet<Product>>> GetProducts(
            [FromQuery]ProductSpecParams specParams)
        {
           var spec = new ProductSpecification(specParams);

            return await CreatePageResult (unit.Repository<Product>(),spec, specParams.PageIndex, specParams.PageSize);

        }
        [Cache(600)]
        [HttpGet ("{id:int}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await unit.Repository<Product>().GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return product;
        }
        [InvalidateCache("/api/products|")]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            unit.Repository<Product>().Add(product);
            if (await unit.Complete())
            {
                return CreatedAtAction("GetProduct",new {id=product.Id}, product);
            }
            return BadRequest("Problem creating product");
        }
        [InvalidateCache("/api/products|")]
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult>UpdateProduct(int id, Product product)
        {
            if (product.Id !=id || !ProductExists(id))
            
                return BadRequest("Cannot Update this Product");

            unit.Repository<Product>().Update(product);

           if(await unit.Complete())
            {
                return NoContent();
            }

            return BadRequest("Problem updateing the product");
        }
        [InvalidateCache("/api/products|")]
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var product =await unit.Repository<Product>().GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            unit.Repository<Product>().Remove(product);
            if (await unit.Complete())
            {
                return NoContent();
            }

            return BadRequest("Problem deleting the product");

            
        }
        [Cache(10000)]
        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlySet<string>>> GetBrands()
        {
           var spec = new BrandListSpecification();
            var brands = await unit.Repository<Product>().ListAsync(spec);
            return Ok(brands);
        }
        [Cache(10000)]
        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlySet<string>>> GetTypes()
        {
           var spec = new TypeListSpecification();
            var types = await unit.Repository<Product>().ListAsync(spec);
            return Ok(types);
        }
        private bool ProductExists(int id)
        {
            return unit.Repository<Product>().Exists(id);
        }
    }
}
