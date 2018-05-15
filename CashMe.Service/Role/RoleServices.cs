using CashMe.Data.DAL;
using CashMe.Shared.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Microsoft.AspNet.Identity;

namespace CashMe.Service.Role
{
    public interface IRoleServices
    {
        IEnumerable<IdentityRole> GetAllRoles();
        IdentityRole GetRoleById(string id);
        bool AddOrUpdateRole(IdentityRole role);
    }
    public class RoleServices : IRoleServices
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        private readonly CashMeContext _context = new CashMeContext();     

        public IEnumerable<IdentityRole> GetAllRoles()
        {
            return _unitOfWork.IdentityRoleRepository.GetAll();
        }
        public IdentityRole GetRoleById(string id)
        {
            return _unitOfWork.IdentityRoleRepository.GetById(id);
        }
        public bool AddOrUpdateRole(IdentityRole role)
        {
            var result = false;
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(_context));
            var existRole = _unitOfWork.IdentityRoleRepository.GetById(role.Id);
            if (existRole != null)
            {
                existRole.Name = role.Name;
                roleManager.Update(existRole);
                //result = _unitOfWork.IdentityRoleRepository.Update(role);
            }
            else
            {                
                var tmp = roleManager.Create(role);
                result = tmp.Succeeded;
            }

            return result;
        }        
    }
}
