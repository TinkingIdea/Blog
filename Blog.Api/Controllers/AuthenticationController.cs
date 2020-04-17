using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Blog.Api.Dtos;
using Blog.Api.Models;
using Blog.Core.Data;
using Blog.Core.Entities;
using Blog.Core.Enums;
using Blog.Core.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;

namespace Blog.Api.Controllers
{
    [Route("api/")]
    [ApiController]
    public class AuthenticationController : BaseController
    {
        private readonly IRepository<AdminUser> _userRepo;
        private readonly Token _token;

        public AuthenticationController(IRepository<AdminUser> userRepo, IOptions<Token> token)
        {
            _userRepo = userRepo;
            _token = token.Value;
        }

        [HttpPost, Route("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Error("数据格式不正确");
                }

                var exist = _userRepo.All().Any(c => c.UserName == dto.UserName);
                if (!exist)
                {
                    var adminUser = new AdminUser
                    {
                        UserName = "admin",
                        DisplayName = "admin",
                        Status = CustomerEnums.Status.Enabled,
                        HashedPassword = Encrypt.DesEncrypt("123456")
                    };

                    _userRepo.Add(adminUser);

                    var success = _userRepo.Save();

                    return Error("该账号不存在, 已创建默认admin账号, 请重新登录");
                }

                var hashedPassword = Encrypt.DesEncrypt(dto.Password);
                var user = _userRepo.All()
                    .FirstOrDefault(c => c.UserName == dto.UserName && c.HashedPassword == hashedPassword);

                if (user == null)
                {
                    return Error("账号密码错误");
                }

                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, dto.UserName)
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_token.Secret));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var jwtToken = new JwtSecurityToken(_token.Issuer, _token.Audience, claims, expires: DateTime.Now.AddMinutes(_token.AccessExpiration), signingCredentials: credentials);

                var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);

                var res = new
                {
                    token,
                    result = user
                };

                return Success(res);

            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
    }
}