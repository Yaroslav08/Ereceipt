﻿using Ereceipt.Application.ViewModels.Group;
using Ereceipt.Application.ViewModels.Receipt;
using Ereceipt.Application.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Ereceipt.Application.ViewModels.GroupMember
{
    public class GroupMemberViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public GroupViewModel Group { get; set; }
        public UserReceiptViewModel User { get; set; }
    }
}