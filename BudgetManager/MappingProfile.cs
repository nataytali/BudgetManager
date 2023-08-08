using AutoMapper;
using BudgetManager.Models;

namespace BudgetManager
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<RegistrationModel, UserModel>()
                .ForMember(u => u.UserName, opt => opt.MapFrom(x => x.Email));
        }
    }
}
