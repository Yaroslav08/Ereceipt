﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ereceipt.Application.Interfaces;
using Ereceipt.Application.ViewModels.User;
using MediatR;
namespace Ereceipt.Application.MediatR.Queries
{
    public class SearchUsersQuery : IRequest<List<UserViewModel>>
    {
        public SearchUsersQuery(string name, int afterId)
        {
            Name = name;
            AfterId = afterId;
        }

        public string Name { get; set; }
        public int AfterId { get; set; }
    }

    public class SearchUsersHandler : IRequestHandler<SearchUsersQuery, List<UserViewModel>>
    {
        IUserService _userService;
        public SearchUsersHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<List<UserViewModel>> Handle(SearchUsersQuery request, CancellationToken cancellationToken)
        {
            return await _userService.SearchUsers(request.Name, request.AfterId);
        }
    }
}