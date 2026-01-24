using Application.DTOs.Commons;
using Application.DTOs.Identity;
using Application.Interfaces.Commons;
using Application.Interfaces.Queries;
using Application.Interfaces.Services.Caching;
using Application.Services.Identity;
using AutoMapper;
using Domain.Entities.Identity;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using Shared.Results;
using System.Linq.Expressions;

namespace Application.UnitTest.Services
{
    public class AccountServiceTests
    {
        private readonly Mock<IGenericRepository<ApplicationUser, string>> _applicationUserRepository;
        private readonly Mock<IGenericRepository<ApplicationRole, string>> _applicationRoleRepository;
        private readonly Mock<IGenericRepository<UserRole, string>> _userRoleRepository;
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IUserQuery> _userQuery;
        private readonly Mock<UserManager<ApplicationUser>> _userManager;
        private readonly Mock<ICacheService> _cacheService;
        private readonly AccountService _service;

        public AccountServiceTests()
        {
            _applicationUserRepository = new Mock<IGenericRepository<ApplicationUser, string>>();
            _applicationRoleRepository = new Mock<IGenericRepository<ApplicationRole, string>>();
            _userRoleRepository = new Mock<IGenericRepository<UserRole, string>>();
            _mapper = new Mock<IMapper>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _userQuery = new Mock<IUserQuery>();
            _userManager = MockUserManager<ApplicationUser>();
            _cacheService = new Mock<ICacheService>();
            _service = new AccountService(
                _applicationUserRepository.Object,
                _applicationRoleRepository.Object,
                _userRoleRepository.Object,
                _mapper.Object,
                _unitOfWork.Object,
                _userManager.Object,
                _cacheService.Object,
                _userQuery.Object
            );
        }

        private static Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            return new Mock<UserManager<TUser>>(
                store.Object,
                null, null, null, null, null, null, null, null
            );
        }

        [Fact]
        public async Task CreateAsync_Should_Return_Error_When_UserManager_Fails()
        {
            // Arrange
            var dto = new CreateAccountDto
            {
                UserName = "test",
                Email = "test@test.com",
                Password = "123",
                RoleIds = new List<string>()
            };

            _userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), dto.Password))
                .ReturnsAsync(IdentityResult.Failed(
                    new IdentityError { Description = "Password invalid" }
                ));

            // Act
            var result = await _service.CreateAsync(dto);

            // Assert
            result.ResultType.Should().Be(ServiceResultType.Error);
            result.Message.Should().Contain("Password invalid");

            _unitOfWork.Verify(
                x => x.SaveChangesAsync(CancellationToken.None),
                Times.Never
            );

        }

        [Fact]
        public async Task CreateAsync_Should_Create_User_Without_Roles()
        {
            // Arrange
            var dto = new CreateAccountDto
            {
                UserName = "test",
                Email = "test@test.com",
                Password = "Password@123",
                RoleIds = new List<string>()
            };

            _userManager
                .Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), dto.Password))
                .ReturnsAsync(IdentityResult.Success);

            _applicationRoleRepository
                .Setup(x => x.GetMulti(It.IsAny<Expression<Func<ApplicationRole, bool>>>(),
                    It.IsAny<string[]>()))
                .Returns(Enumerable.Empty<ApplicationRole>().AsQueryable());

            _mapper
                .Setup(x => x.Map<AccountDto>(It.IsAny<ApplicationUser>()))
                .Returns(new AccountDto());

            // Act
            var result = await _service.CreateAsync(dto);

            // Assert
            result.ResultType.Should().Be(ServiceResultType.Created);

            _userRoleRepository.Verify(x => x.Add(It.IsAny<UserRole>()), Times.Never);
            _unitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Never);
        }
        
        [Fact]
        public async Task CreateAsync_Should_Assign_Roles_When_Roles_Exist()
        {
            // Arrange
            var roleId = Guid.NewGuid().ToString();

            var dto = new CreateAccountDto
            {
                UserName = "test",
                Email = "test@test.com",
                Password = "Password@123",
                RoleIds = new List<string> { roleId }
            };

            _userManager
                .Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), dto.Password))
                .ReturnsAsync(IdentityResult.Success);

            var roles = new List<ApplicationRole>
            {
                new ApplicationRole { Id = roleId }
            }.AsQueryable();

            _applicationRoleRepository
                .Setup(x => x.GetMulti(It.IsAny<Expression<Func<ApplicationRole, bool>>>(),
                    It.IsAny<string[]>()))
                .Returns(roles);

            _mapper
                .Setup(x => x.Map<AccountDto>(It.IsAny<ApplicationUser>()))
                .Returns(new AccountDto());

            _mapper
                .Setup(x => x.Map<List<RoleDto>>(roles))
                .Returns(new List<RoleDto> { new RoleDto() });

            // Act
            var result = await _service.CreateAsync(dto);

            // Assert
            result.ResultType.Should().Be(ServiceResultType.Created);

            _userRoleRepository.Verify(
                x => x.Add(It.IsAny<UserRole>()),
                Times.Exactly(roles.Count()));

            _unitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
        }


        [Fact]
        public async Task GetListAsync_Should_ReturnsSuccess()
        {
            // Arrange
            var parameters = new CommonQueryParameters();

            var pagedResult = new PagedResult<AccountDto>
            {
                Items = new List<AccountDto>(),
                TotalCount = 0
            };

            _userQuery
                .Setup(x => x.GetList(
                    It.IsAny<CommonQueryParameters>(),
                    It.IsAny<string[]>()))
                .Returns(pagedResult);

            // Act
            var result = await _service.GetListAsync(parameters);

            // Assert
            result.ResultType.Should().Be(ServiceResultType.Success);
            result.Data.Should().Be(pagedResult);

            _userQuery.Verify(
                x => x.GetList(It.IsAny<CommonQueryParameters>(), It.IsAny<string[]>()),
                Times.Once);
        }

    }

}
