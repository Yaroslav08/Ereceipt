﻿using Ereceipt.Application.Interfaces;
using Ereceipt.Application.ViewModels.GroupMember;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace Ereceipt.Application.MediatR.Queries
{
    public class GetGroupMembersQuery : IRequest<List<GroupMemberViewModel>>
    {
        public Guid Id { get; }

        public GetGroupMembersQuery(Guid id)
        {
            Id = id;
        }
    }


    public class GetGroupMembersQueryHandler : IRequestHandler<GetGroupMembersQuery, List<GroupMemberViewModel>>
    {
        IGroupService _groupService;
        public GetGroupMembersQueryHandler(IGroupService groupService)
        {
            _groupService = groupService;
        }


        public async Task<List<GroupMemberViewModel>> Handle(GetGroupMembersQuery request, CancellationToken cancellationToken)
        {
            return await _groupService.GetGroupMembers(request.Id);
        }
    }
}