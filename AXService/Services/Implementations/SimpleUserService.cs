using AXService.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AXService.Services.Implementations
{
    public class SimpleUserService : ISimpleUserService
    {
        #region Prop
        private readonly IConfiguration _configuration;
        public SimpleUserService(IConfiguration configuration )
        {
            _configuration = configuration;
        }
        #endregion

        public bool ValidUserPass(string user, string pass)
        {

            var lstAuthen = _configuration.GetSection("Authentication").GetChildren()
                    .ToList()
                    .Select(x => new {
                        UserName = x.GetValue<string>("UserName"),
                        Password = x.GetValue<string>("Password")
                    }); ;
            return lstAuthen.Any(x => x.UserName == user && x.Password == pass);
        }
    }
}
