using SellersZone.Core.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SellersZone.Core.Interfaces
{
    public interface ITokenService
    {
        (string Token, long Expires) CreateToken(AppUser user, dynamic role);


    }
}
