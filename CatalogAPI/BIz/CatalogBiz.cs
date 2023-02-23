using CatalogAPI.Dao;
using CatalogAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogAPI.BIz
{
    public class CatalogBiz
    {
        #region Enums
        /// <summary>
        /// Product order by Options
        /// </summary>
        public enum ProductOrderByOptions { 
            none = 0,
            ByNameAsc = 1,
            ByNameDesc = 2,
            ByCategoryAsc = 3,
            ByCategoryDesc = 4
        }
        #endregion

        #region Properties
        /// <summary>
        /// DB Context
        /// </summary>
        public CatalogContext context { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">DB Context</param>
        public CatalogBiz(CatalogContext context) => this.context = context;
        #endregion

        #region Public methods
        /// <summary>
        /// Search function
        /// </summary>
        /// <param name="searchText">String to search for</param>
        /// <param name="orderBy">Sort Type</param>
        /// <param name="pageNumber">Page Number for pagination</param>
        /// <param name="pageSize">Page Size for pagination</param>
        /// <returns>List of products</returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<Product>> GetProducts(string searchText, ProductOrderByOptions orderBy, int pageNumber, int pageSize) {
            var result = new List<Product>();
            try
            {
                var products = await context.Products.Include(b => b.ProductCategory).ToListAsync();
                //Order By
                switch (orderBy) {
                    case ProductOrderByOptions.ByNameAsc:
                        result =    (from p in products
                                    orderby p.ProductName ascending
                                    select p).ToList();
                        break;
                    case ProductOrderByOptions.ByNameDesc:
                        result = (from p in products
                                  orderby p.ProductName descending
                                  select p).ToList();
                        break;
                    case ProductOrderByOptions.ByCategoryAsc:
                        result = (from p in products
                                  orderby p.ProductCategory.ProductCategoryName ascending
                                  select p).ToList();
                        break;
                    case ProductOrderByOptions.ByCategoryDesc:
                        result = (from p in products
                                  orderby p.ProductCategory.ProductCategoryName descending
                                  select p).ToList();
                        break;
                    default:
                        result = (from p in products
                                  select p).ToList();
                        break;
                }
                
                //Search rows
                if (!string.IsNullOrWhiteSpace(searchText)) { 
                    result = result.Where(item => item.ProductName.ToLower().Contains(searchText.ToLower()) ||  item.ProductDescription.ToLower().Contains(searchText.ToLower()) || item.ProductCategory.ProductCategoryName.ToLower().Contains(searchText.ToLower())).ToList();
                }

                //Pagination
                pageNumber = (pageNumber <= 0) ? 1 : pageNumber;
                pageSize = (pageSize <= 0) ? 10 : pageSize;

                var totalRecords = result.Count;
                var totalPages = Math.Ceiling((double)totalRecords / pageSize);
                var skip = (pageNumber - 1) * pageSize;

                result = result.Skip(skip).Take(pageSize).ToList();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Find product by primary key
        /// </summary>
        /// <param name="id">Primary key</param>
        /// <returns>Product</returns>
        /// <exception cref="Exception"></exception>
        public async Task<Product> GetProduct(int id) {
            try
            {
                return await context.Products.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Update Product
        /// </summary>
        /// <param name="id">Primary Key</param>
        /// <param name="product">Product info</param>
        /// <returns>Updated product</returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> UpdateProduct(int id, Product product) {
            if (id != product.ProductId)
            {
                return false;
            }

            try
            {
                context.Entry(product).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!ProductExists(id))
                {
                    throw new Exception($"El producto {product.ProductId} - {product.ProductName} no existe!");
                }
                else
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Add product to catalog
        /// </summary>
        /// <param name="product">Product info</param>
        /// <returns>Succed</returns>
        public async Task<bool> CreateProduct(Product product)
        {
            try
            {
                if (product.ProductId != 0 && ProductExists(product.ProductId)) {
                    throw new Exception($"El producto {product.ProductId} - {product.ProductName} existe en el Catalogo!");
                }
                context.Products.Add(product);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Delete product from catalog
        /// </summary>
        /// <param name="id">Primary key</param>
        /// <returns>Succed</returns>
        public async Task<bool> DeleteProduct(int id)
        {
            try
            {
                var product = await context.Products.FindAsync(id);
                if (product == null)
                {
                    return false;
                }
                context.Products.Remove(product);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex) { 
                throw ex;
            }
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Check if exists product on catalog
        /// </summary>
        /// <param name="id">Primary key</param>
        /// <returns></returns>
        private bool ProductExists(int id)
        {
            return context.Products.Any(e => e.ProductId == id);
        }
        #endregion 
    }
}
