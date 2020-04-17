using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Api.Dtos.User;
using Blog.Core.Data;
using Blog.Core.Entities;
using Blog.Core.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop.Infrastructure;

namespace Blog.Api.Controllers
{
    [Route("api/")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly IRepository<AdminUser> _userRepo;

        public UserController(IRepository<AdminUser> userRepo)
        {
            _userRepo = userRepo;
        }

        [HttpGet, Route("users")]
        public IActionResult All([FromQuery] QueryInfoDto dto)
        {
            try
            {
                var users = _userRepo.All().AsNoTracking();
                if (!string.IsNullOrWhiteSpace(dto.Query))
                {
                    users = users.Where(c => c.UserName.Contains(dto.Query) || c.DisplayName.Contains(dto.Query));
                }

                var total = users.Count();
                var data = users.OrderByDescending(c => c.ModifiedAtUtc).Skip(dto.Skip).Take(dto.PageSize).Select(c =>
                    new
                    {
                        id = c.Id, userName = c.UserName, displayName = c.DisplayName,
                        state = c.Status == CustomerEnums.Status.Enabled
                    }).ToList();

                return Success(new {total, result = data});
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
    }
}