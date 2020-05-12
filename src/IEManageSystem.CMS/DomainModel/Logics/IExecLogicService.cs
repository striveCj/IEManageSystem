﻿using Abp.Domain.Services;
using IEManageSystem.CMS.DomainModel.PageDatas;
using IEManageSystem.CMS.DomainModel.Pages;
using System;
using System.Collections.Generic;
using System.Text;

namespace IEManageSystem.CMS.DomainModel.Logics
{
    public interface IExecLogicService : IDomainService
    {
        void Exec(
            Logic logic, 
            PageBase pageBase, 
            string pageComponentBaseSign, 
            PageData pageData, 
            string contentComponentDataSign,
            string request);
    }
}
