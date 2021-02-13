﻿using AutoMapper;
using Echack.Application.Interfaces;
using Echack.Application.ViewModels.User;
using Echack.Domain.Models;
using Echack.Infrastructure.Data;
using Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Echack.Application.Services
{
    public class UserService : IUserService
    {
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<UserViewModel> CreateUser(UserCreateViewModel model)
        {
            var user = new User
            {
                Name = model.Name,
                Login = model.Login,
                PasswordHash = PasswordManager.GeneratePasswordHash(model.Password),
                CreatedBy = "0",
                Role = "User"
            };
            return _mapper.Map<UserViewModel>(await _unitOfWork.UserRepository.CreateAsync(user));
        }

        public async Task<UserViewModel> EditUser(UserEditViewModel model)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsTrackingAsync(model.Id);
            if (user == null)
                return null;
            user.Name = model.Name;
            user.UpdatedAt = DateTime.Now;
            user.UpdatedBy = user.Id.ToString();
            return _mapper.Map<UserViewModel>(await _unitOfWork.UserRepository.UpdateAsync(user));
        }

        public async Task<List<UserViewModel>> GetAllUsers(int afterId)
        {
            return _mapper.Map<List<UserViewModel>>(await _unitOfWork.UserRepository.GetAllAsync(afterId));
        }

        public async Task<UserViewModel> GetUserById(int id)
        {
            return _mapper.Map<UserViewModel>(await _unitOfWork.UserRepository.FindAsync(d => d.Id == id));
        }

        public async Task<List<UserViewModel>> SearchUsers(string user, int afterId)
        {
            return _mapper.Map<List<UserViewModel>>(await _unitOfWork.UserRepository.SearchUsersAsync(user, afterId));
        }
    }
}