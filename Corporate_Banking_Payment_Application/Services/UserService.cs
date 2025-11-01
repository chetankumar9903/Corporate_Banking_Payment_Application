using AutoMapper;
using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Models;
using Corporate_Banking_Payment_Application.Repository.IRepository;
using Corporate_Banking_Payment_Application.Services.IService;

namespace Corporate_Banking_Payment_Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepo, IMapper mapper)
        {
            _userRepo = userRepo;
            _mapper = mapper;
        }

        //public async Task<IEnumerable<UserDto>> GetAllUsers()
        //{
        //    var users = await _userRepo.GetAllUsers();
        //    return _mapper.Map<IEnumerable<UserDto>>(users);
        //}

        public async Task<PagedResult<UserDto>> GetAllUsers(string? searchTerm, string? sortColumn, SortOrder? sortOrder, int pageNumber, int pageSize)
        {
            //var pagedResult = await _userRepo.GetAllUsers(searchTerm, sortColumn, sortOrder, pageNumber, pageSize);
            var pagedResult = await _userRepo.GetAllUsers(searchTerm, sortColumn, sortOrder, pageNumber, pageSize);
            // Map the items on the current page to DTOs
            var itemsDto = _mapper.Map<IEnumerable<UserDto>>(pagedResult.Items);

            return new PagedResult<UserDto>
            {
                Items = itemsDto.ToList(),
                TotalCount = pagedResult.TotalCount
            };
        }

        public async Task<UserDto?> GetUserById(int id)
        {
            var user = await _userRepo.GetUserById(id);
            return user == null ? null : _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> CreateUser(CreateUserDto dto)
        {
            // Optional: check duplicate username
            var exists = await _userRepo.GetUserByUserName(dto.UserName);
            if (exists != null)
                throw new Exception($"User with username '{dto.UserName}' already exists.");

            var user = _mapper.Map<User>(dto);
            var created = await _userRepo.AddUser(user);
            return _mapper.Map<UserDto>(created);
        }

        public async Task<UserDto?> UpdateUser(int id, UpdateUserDto dto)
        {
            var existing = await _userRepo.GetUserById(id);
            if (existing == null) return null;

            _mapper.Map(dto, existing);
            await _userRepo.UpdateUser(existing);
            return _mapper.Map<UserDto>(existing);
        }

        public async Task<bool> DeleteUser(int id)
        {
            var user = await _userRepo.GetUserById(id);
            if (user == null) return false;

            await _userRepo.DeleteUser(user);
            return true;
        }
    }
}
