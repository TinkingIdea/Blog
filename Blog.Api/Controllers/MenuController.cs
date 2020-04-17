using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Api.Dtos;
using Blog.Core.Data;
using Blog.Core.Entities;
using Blog.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Api.Controllers
{
    [Route("api/")]
    [ApiController]
    [Authorize]
    public class MenuController : BaseController
    {
        private readonly IRepository<Menu> _menuRepo;

        public MenuController(IRepository<Menu> menuRepo)
        {
            _menuRepo = menuRepo;
        }

        [HttpGet, Route("menus")]
        public IActionResult All([FromQuery] MenuQueryInfoDto dto)
        {
            try
            {
                var skip = (dto.PageNum - 1) * dto.PageSize;
                var menus = _menuRepo.All().AsNoTracking().Where(c=>c.Level == 0);

                if (!string.IsNullOrWhiteSpace(dto.Query))
                {
                    menus = menus.Where(c => c.Name.Contains(dto.Query));
                }

                var total = menus.Count();
                var data = menus.Skip(skip).Take(dto.PageSize).ToList();

                var list = new List<AsideMenuDto>();
                foreach (var item in data)
                {
                    var menu = new AsideMenuDto
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Icon = item.Icon,
                        State = item.Status == CustomerEnums.Status.Enabled,
                        Children = _menuRepo.All().Where(c => c.Level == item.Id).Select(c => new AsideMenuDto.Child
                        {
                            Id = c.Id,
                            Name = c.Name,
                            Path = c.Path,
                            State = c.Status == CustomerEnums.Status.Enabled
                        })
                    };

                    list.Add(menu);
                }
                var res = new
                {
                    total,
                    result = list
                };

                return Success(res);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet, Route("menus/list")]
        public IActionResult List()
        {
            try
            {
                var menus = _menuRepo.All().AsNoTracking().Where(c=>c.Status == CustomerEnums.Status.Enabled).ToList();

                var root = menus.Where(c => c.Level == 0).OrderBy(c=>c.Order);

                var list = new List<AsideMenuDto>();
                foreach (var item in root)
                {
                    var menu = new AsideMenuDto
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Icon = item.Icon,
                        Children = menus.Where(c => c.Level == item.Id).OrderBy(c=>c.Order).Select(c => new AsideMenuDto.Child
                        {
                            Id = c.Id,
                            Name = c.Name,
                            Path = c.Path
                        })
                    };
                    list.Add(menu);
                }

                return Success(list);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet, Route("menus/root")]
        public IActionResult Root()
        {
            try
            {
                var menus = _menuRepo.All().Where(c => c.Level == 0).Select(c => new
                {
                    value = c.Id, label = c.Name
                }).ToList();

                return Success(menus);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPut, Route("menus/{id}/state/{state}")]
        public IActionResult UpdateState(long id, bool state)
        {
            try
            {
                var menu = _menuRepo.Get(id);

                if (menu == null)
                    return Error("没有找到对应的菜单");

                menu.Status = state ? CustomerEnums.Status.Enabled : CustomerEnums.Status.Disabled;

                _menuRepo.Update(menu);

                var success = _menuRepo.Save();

                return success ? Success(null, "更新成功") : Error("更新失败");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet, Route("menus/{id}")]
        public IActionResult Get(long id)
        {
            try
            {
                var menu = _menuRepo.Get(id);

                if (menu == null)
                    return Error("没有找到对应的菜单");

                var data = new
                {
                    id = menu.Id,
                    name = menu.Name,
                    path = menu.Path,
                    order = menu.Order.ToString(),
                    icon = menu.Icon,
                    level = menu.Level == 0 ? "" : menu.Level.ToString()
                };

                return Success(data);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPost, Route("menus")]
        public IActionResult Post([FromBody] MenuDto dto)
        {
            try
            {
                bool isEdit = dto.Id > 0;

                var menu = new Menu
                {
                    Id = dto.Id,
                    Name = dto.Name,
                    Level = string.IsNullOrWhiteSpace(dto.Level) ? 0 : Convert.ToInt32(dto.Level),
                    Order = Convert.ToInt32(dto.Order),
                    Path = dto.Path,
                    Icon = dto.Icon,
                    Status = CustomerEnums.Status.Enabled
                };

                if (isEdit)
                {
                    _menuRepo.Update(menu);
                }
                else
                {
                    _menuRepo.Add(menu);
                }

                var success = _menuRepo.Save();

                return success ? Success(null, "保存成功") : Error("保存失败");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet, Route("menus/{id}/exist/{name}")]
        public IActionResult NameExist(long id, string name)
        {
            try
            {
                bool result;
                if (id == 0)
                {
                    result = _menuRepo.All().Any(c => c.Name == name);
                }
                else
                {
                    result = !_menuRepo.All().Any(c => c.Id == id && c.Name == name) &&
                             _menuRepo.All().Any(c => c.Name == name);
                }

                return Success(result);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
    }
}