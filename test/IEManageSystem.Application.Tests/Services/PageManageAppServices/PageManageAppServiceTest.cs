﻿using IEManageSystem.EntityFrameworkCore;
using IEManageSystem.Services.ManageHome.CMS.Pages;
using IEManageSystem.Tests;
using IEManageSystem.Tests.TestDatas;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Linq;
using IEManageSystem.CMS.DomainModel.Pages;
using IEManageSystem.Dtos.CMS;
using IEManageSystem.CMS.DomainModel.PageDatas;
using IEManageSystem.CMS.DomainModel.ComponentDatas;
using Microsoft.EntityFrameworkCore;

namespace IEManageSystem.Application.Tests.Services.PageManageAppServices
{
    public class PageManageAppServiceTest : IEManageSystemTestBase
    {
        private IPageManageAppService _pageManageAppService { get; set; }

        public PageManageAppServiceTest() {
            _pageManageAppService = Resolve<IPageManageAppService>();
        }

        private void ReloadDB() {
            UsingDbContext(context => context.Database.EnsureDeleted());
            UsingDbContext(context => context.Database.EnsureCreated());
            UsingDbContext(context => new PermissionBuilder(context).Build());
            UsingDbContext(context => new PageBuilder(context).Build());
            UsingDbContext(context => new PageDataBuilder(context).Build());
        }

        [Fact]
        public void AddContentPage_BaseTest() {
            ReloadDB();

            _pageManageAppService.AddPage(new IEManageSystem.Services.ManageHome.CMS.Pages.Dto.AddPageInput() { 
                Name = "AddContentPage_BaseTest",
                Description = "Description1",
                DisplayName = "DisplayName1",
                PageType = "ContentPage"
            });

            var dbContext = LocalIocManager.Resolve<IEManageSystemDbContext>();
            dbContext.SaveChanges();

            Assert.True(dbContext.Pages.Any(e => e.Name == "AddContentPage_BaseTest"));
        }

        [Fact]
        public void AddStaticPage_BaseTest() {
            ReloadDB();

            _pageManageAppService.AddPage(new IEManageSystem.Services.ManageHome.CMS.Pages.Dto.AddPageInput() {
                Name = "AddStaticPage_BaseTest",
                Description = "Description1",
                DisplayName = "DisplayName1",
                PageType = "StaticPage"
            });

            var dbContext = LocalIocManager.Resolve<IEManageSystemDbContext>();
            dbContext.SaveChanges();

            Assert.True(dbContext.Pages.Any(e => e.Name == "AddStaticPage_BaseTest"));
        }

        [Fact]
        public void UpdatePage_BaseTest() {
            ReloadDB();

            _pageManageAppService.AddPage(new IEManageSystem.Services.ManageHome.CMS.Pages.Dto.AddPageInput()
            {
                Name = "UpdatePage_BaseTest",
                Description = "Description1",
                DisplayName = "DisplayName1",
                PageType = "StaticPage"
            });

            _pageManageAppService.UpdatePage(new IEManageSystem.Services.ManageHome.CMS.Pages.Dto.UpdatePageInput() {
                Name = "UpdatePage_BaseTest",
                Description = "Description2",
                DisplayName = "DisplayName2",
                PageType = "StaticPage"
            });


            var dbContext = LocalIocManager.Resolve<IEManageSystemDbContext>();
            dbContext.SaveChanges();

            Assert.True(dbContext.Pages.FirstOrDefault(e => e.Name == "UpdatePage_BaseTest").Description == "Description2");
            Assert.True(dbContext.Pages.FirstOrDefault(e => e.Name == "UpdatePage_BaseTest").DisplayName == "DisplayName2");
        }

        [Fact]
        public void UpdateContentPagePermission() {
            ReloadDB();

            _pageManageAppService.UpdateContentPagePermission(new IEManageSystem.Services.ManageHome.CMS.Pages.Dto.UpdateContentPagePermissionInput()
            {
                Name = "ContentPage1Name",
                ContentPagePeimissionCollection = new ContentPagePeimissionCollectionDto()
                {
                    IsEnableQueryPermission = false,
                    ManagePermissions = new List<ContentPagePermissionDto>(),
                    QueryPermissions = new List<ContentPagePermissionDto>()
                }
            });

            var dbContext = LocalIocManager.Resolve<IEManageSystemDbContext>();
            dbContext.SaveChanges();

            var contentPagePeimissionCollection = 
                dbContext.Set<ContentPage>()
                    .Include(e=>e.ContentPagePeimissionCollection).ThenInclude(e=>e.ManagePermissions)
                    .Include(e => e.ContentPagePeimissionCollection).ThenInclude(e => e.QueryPermissions)
                    .FirstOrDefault(e => e.Name == "ContentPage1Name").ContentPagePeimissionCollection;
            Assert.True(contentPagePeimissionCollection.ManagePermissions.Count == 0);
            Assert.True(contentPagePeimissionCollection.QueryPermissions.Count == 0);
            Assert.True(contentPagePeimissionCollection.IsEnableQueryPermission == false);
        }

        [Fact]
        public void DeletePage_BaseTest() {
            ReloadDB();

            _pageManageAppService.DeletePage(new IEManageSystem.Services.ManageHome.CMS.Pages.Dto.DeletePageInput() { 
                Name = "ContentPage1Name"
            });

            var dbContext = LocalIocManager.Resolve<IEManageSystemDbContext>();
            dbContext.SaveChanges();

            // 聚合根下的所有实体应该删除
            Assert.True(!dbContext.Set<PageComponentBase>().Any(e => e.Sign == "ContentPage1_Component1Sign"));
            Assert.True(!dbContext.Set<PageComponentSetting>().Any(e => e.Name == "ContentPage1_Component1_PageComponentSetting1Name"));
            Assert.True(!dbContext.Set<SingleSettingData>().Any(e => e.Name == "ContentPage1_Component1_PageComponentSetting1_SingleSettingData1Name"));

            // 删除的默认数据
            Assert.True(!dbContext.Set<DefaultComponentData>().Any(e => e.Sign == "ContentPage1_Component1Sign"));
            Assert.True(!dbContext.Set<SingleComponentData>().Any(e => e.Name == "PageData1_DefaultComponentData1_SingleComponentData1Name"));
        }

        [Fact]
        public void UpdatePageComponent_BaseTest() {
            ReloadDB();

            _pageManageAppService.UpdatePageComponent(new IEManageSystem.Services.ManageHome.CMS.Pages.Dto.UpdatePageComponentInput() { 
                Name = "ContentPage1Name",
                PageComponents = new List<Dtos.CMS.PageComponentDto>() { 
                    new Dtos.CMS.PageComponentDto(){
                        Name = "UpdatePageComponent_BaseTest_ComponentName",
                        Sign = "UpdatePageComponent_BaseTest_PageComponentSign",
                        PageComponentBaseSetting = new PageComponentBaseSettingDto(){ 
                            Width = "12",
                        },
                        PageComponentSettings = new List<PageComponentSettingDto>(){
                            new PageComponentSettingDto(){
                                Name = "UpdatePageComponent_BaseTest_PageComponentSettingName",
                                SingleDatas = new List<SingleSettingDataDto>(){
                                    new SingleSettingDataDto(){ Name = "UpdatePageComponent_BaseTest_PageComponentSetting_SingleSettingDataName" }
                                }
                            }
                        }
                    }
                },
                DefaultComponentDatas = new List<ComponentDataDto>() {
                    new ComponentDataDto(){
                    Sign = "UpdatePageComponent_BaseTest_DefaultComponentDatas1",
                        SingleDatas = new List<SingleComponentDataDto>(){
                            new SingleComponentDataDto(){ Name = "UpdatePageComponent_BaseTest_SingleComponentData1" },
                            new SingleComponentDataDto(){ Name = "UpdatePageComponent_BaseTest_SingleComponentData2" }
                        }
                    },
                    new ComponentDataDto(){ Sign = "UpdatePageComponent_BaseTest_DefaultComponentDatas2" }
                }
            });

            var dbContext = LocalIocManager.Resolve<IEManageSystemDbContext>();
            dbContext.SaveChanges();

            // 删除原先的组件
            Assert.True(!dbContext.Set<PageComponentBase>().Any(e => e.Sign == "ContentPage1_Component1Sign"));
            Assert.True(!dbContext.Set<PageComponentSetting>().Any(e => e.Name == "ContentPage1_Component1_PageComponentSetting1Name"));
            Assert.True(!dbContext.Set<SingleSettingData>().Any(e => e.Name == "ContentPage1_Component1_PageComponentSetting1_SingleSettingData1Name"));

            // 使用新的组件
            Assert.True(dbContext.Set<PageComponentBase>().Any(e => e.Sign == "UpdatePageComponent_BaseTest_PageComponentSign"));
            Assert.True(dbContext.Set<PageComponentSetting>().Any(e => e.Name == "UpdatePageComponent_BaseTest_PageComponentSettingName"));
            Assert.True(dbContext.Set<SingleSettingData>().Any(e => e.Name == "UpdatePageComponent_BaseTest_PageComponentSetting_SingleSettingDataName"));

            // 删除原先的默认数据
            Assert.True(!dbContext.Set<DefaultComponentData>().Any(e => e.Sign == "ContentPage1_Component1Sign"));
            Assert.True(!dbContext.Set<SingleComponentData>().Any(e => e.Name == "PageData1_DefaultComponentData1_SingleComponentData1Name"));

            // 使用新的默认数据
            Assert.True(dbContext.Set<DefaultComponentData>().Any(e => e.Sign == "UpdatePageComponent_BaseTest_DefaultComponentDatas1"));
            Assert.True(dbContext.Set<SingleComponentData>().Any(e => e.Name == "UpdatePageComponent_BaseTest_SingleComponentData1"));
        }

        /// <summary>
        /// 添加文章，期望只添加聚合根
        /// </summary>
        [Fact]
        public void AddPageData_BaseTest() {
            ReloadDB();

            _pageManageAppService.AddPageData(new IEManageSystem.Services.ManageHome.CMS.Pages.Dto.AddPageDataInput() {
                PageName= "ContentPage1Name",
                Name= "AddPageData_BaseTest_Name",
                Title= "AddPageData_BaseTest_Title"
            });

            var dbContext = LocalIocManager.Resolve<IEManageSystemDbContext>();
            dbContext.SaveChanges();

            Assert.True(dbContext.PageDatas.Any(e => e.Name == "AddPageData_BaseTest_Name"));
        }

        /// <summary>
        /// 更新文章，期望只更新聚合根
        /// </summary>
        [Fact]
        public void UpdatePageData_BaseTest() 
        {
            ReloadDB();

            _pageManageAppService.AddPageData(new IEManageSystem.Services.ManageHome.CMS.Pages.Dto.AddPageDataInput()
            {
                PageName = "ContentPage1Name",
                Name = "UpdatePageData_BaseTest_Name",
                Title = "UpdatePageData_BaseTest_Title"
            });

            var dbContext = LocalIocManager.Resolve<IEManageSystemDbContext>();
            dbContext.SaveChanges();
            var post = dbContext.PageDatas.FirstOrDefault(e=>e.Name == "UpdatePageData_BaseTest_Name");

            _pageManageAppService.UpdatePageData(new IEManageSystem.Services.ManageHome.CMS.Pages.Dto.UpdatePageDataInput()
            {
                PageName = "ContentPage1Name",
                Id = post.Id,
                Name = "UpdatePageData_BaseTest_Name1",
                Title = "UpdatePageData_BaseTest_Title1"
            });

            dbContext.SaveChanges();

            Assert.True(dbContext.PageDatas.Any(e => e.Name == "UpdatePageData_BaseTest_Name1"));
            Assert.True(!dbContext.PageDatas.Any(e => e.Name == "UpdatePageData_BaseTest_Name"));
        }

        /// <summary>
        /// 删除文章，期望文章下所有东西都删除
        /// </summary>
        [Fact]
        public void DeletePageData_BaseTest() {
            ReloadDB();

            _pageManageAppService.DeletePageData(new IEManageSystem.Services.ManageHome.CMS.Pages.Dto.DeletePageDataInput() { 
                PageName = "ContentPage1Name",
                Name = "PageData1Name"
            });

            var dbContext = LocalIocManager.Resolve<IEManageSystemDbContext>();
            dbContext.SaveChanges();

            Assert.True(!dbContext.Set<PageData>().Any(e => e.Name == "PageData1Name"));
            Assert.True(!dbContext.Set<ContentComponentData>().Any(e => e.Sign == "ContentPage1_Component1Sign"));
            Assert.True(!dbContext.Set<SingleComponentData>().Any(e => e.Name == "PageData1_ContentComponentData1_SingleComponentData2Name"));
        }

        /// <summary>
        /// 更新组件数据
        /// </summary>
        [Fact]
        public void UpdateComponentData_BaseTest() {
            ReloadDB();

            _pageManageAppService.UpdateComponentData(new IEManageSystem.Services.ManageHome.CMS.Pages.Dto.UpdateComponentDataInput() {
                PageName = "ContentPage1Name",
                PageDataName = "PageData1Name",
                ComponentDatas = new List<ComponentDataDto>() {
                    new ComponentDataDto(){
                        Sign = "UpdateComponentData_BaseTest_ContentComponentDataSign",
                        SingleDatas = new List<SingleComponentDataDto>(){
                            new SingleComponentDataDto(){ Name = "UpdateComponentData_BaseTest_SingleComponentDataName" }
                        }
                    }
                }
            });

            var dbContext = LocalIocManager.Resolve<IEManageSystemDbContext>();
            dbContext.SaveChanges();

            // 移除所有旧的数据
            Assert.True(!dbContext.Set<ContentComponentData>().Any(e => e.Sign == "ContentPage1_Component1Sign"));
            Assert.True(!dbContext.Set<SingleComponentData>().Any(e => e.Name == "PageData1_ContentComponentData1_SingleComponentData2Name"));

            // 添加新的数据
            Assert.True(dbContext.Set<ContentComponentData>().Any(e => e.Sign == "UpdateComponentData_BaseTest_ContentComponentDataSign"));
            Assert.True(dbContext.Set<SingleComponentData>().Any(e => e.Name == "UpdateComponentData_BaseTest_SingleComponentDataName"));
        }
    }
}
