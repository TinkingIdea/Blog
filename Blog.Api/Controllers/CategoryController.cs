using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Api.Dtos.Category;
using Blog.Core.Data;
using Blog.Core.Entities;
using Blog.Core.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Api.Controllers
{
    [Route("api/")]
    [ApiController]
    public class CategoryController : BaseController
    {
        private readonly IRepository<Category> _cateRepo;
        private readonly IUnitOfWork _uow;

        public CategoryController(IRepository<Category> cateRepo, IUnitOfWork uow)
        {
            _cateRepo = cateRepo;
            _uow = uow;
        }

        [HttpGet, Route("categories")]
        public IActionResult All([FromQuery] QueryInfoDto dto)
        {
            try
            {
                var categories = _cateRepo.All().AsNoTracking();
                if (!string.IsNullOrWhiteSpace(dto.Query))
                {
                    categories = categories.Where(c => c.Name.Contains(dto.Query));
                }

                var total = categories.Count();
                var data = categories.OrderByDescending(c => c.ModifiedAtUtc).Skip(dto.Skip).Take(dto.PageSize).Select(
                    c => new
                    {
                        id = c.Id, name = c.Name, state = c.Status == CustomerEnums.Status.Enabled
                    }).ToList();

                return Success(new {total, result = data});
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet, Route("categories/{id}/exist/{name}")]
        public IActionResult NameExist(long id, string name)
        {
            try
            {
                bool result;
                if (id == 0)
                {
                    result = _cateRepo.All().Any(c => c.Name == name);
                }
                else
                {
                    result = !_cateRepo.All().Any(c => c.Id == id && c.Name == name) &&
                             _cateRepo.All().Any(c => c.Name == name);
                }

                return Success(result);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet, Route("categories/{id}")]
        public IActionResult Edit(long id)
        {
            try
            {
                var category = _cateRepo.Get(id);

                var data = new
                {
                    id = category.Id,
                    Name = category.Name
                };

                return Success(data);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPost, Route("categories")]
        public IActionResult Edit([FromBody] EditDto dto)
        {
            try
            {
                bool isEdit = dto.Id > 0;

                var category = new Category
                {
                    Id = dto.Id,
                    Name = dto.Name,
                    Status = CustomerEnums.Status.Enabled
                };

                if (isEdit)
                {
                    _cateRepo.Update(category);
                }
                else
                {
                    _cateRepo.Add(category);
                }

                var success = _uow.Save();

                return success ? Success(null, "保存成功") : Error("保存失败");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet, Route("categories/{id}/state/{state}")]
        public IActionResult UpdateState(long id, bool state)
        {
            try
            {
                var cate = _cateRepo.Get(id);

                cate.Status = state ? CustomerEnums.Status.Enabled : CustomerEnums.Status.Disabled;

                _cateRepo.Update(cate);

                var success = _uow.Save();

                return success ? Success(null, "更新成功") : Error("更新失败");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpDelete, Route("categories/{id}")]
        public IActionResult Delete(long id)
        {
            try
            {
                var category = _cateRepo.Get(id);

                _cateRepo.Delete(category);

                var success = _uow.Save();

                return success ? Success(null, "删除成功") : Error("删除失败");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet, Route("categories/list")]
        public IActionResult List()
        {
            try
            {
                var categories = _cateRepo.All().Where(c => c.Status == CustomerEnums.Status.Enabled)
                    .OrderByDescending(c => c.ModifiedAtUtc).Select(c => new
                    {
                        label = c.Name, value = c.Id
                    }).ToList();

                return Success(categories);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
    }
}