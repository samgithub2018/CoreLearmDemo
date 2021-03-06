﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.Interface;
using Core.Service;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Core.MainDemo
{
    /// <summary>
    /// 
    /// IIS部署需要注意在程序池里面选择托管代码
    /// 另外需要安装好core runtime
    /// 
    /// 【356】
    /// appsettings.json配置文件（和Framework下mvc中的web.config）
    /// IConfiguration 这个接口是用来管理appsettings.json配置文件的，读取，修改都是通过这个接口
    /// 这个看 Startup类 中的示例
    /// 
    /// 
    /// 【366】
    /// 服务注册
    /// ore程序的 session和cookie的那些需要手动注册，才能使用看
    /// 
    /// 【1111】
    /// IServiceCollection 
    /// 这个是容器，netcore内置的容器，core程序的注入都是通过这个来实现的
    /// 也可以自定义注册一些类型到这个容器中，只要是通过这个容器创建的实例都可以注入
    /// 请看当前类
    /// 
    /// 
    /// 使用log4net注入,对象创建成功了，不知道为何没有写日志
    /// 1.需要添加
    ///   log4net.dll
    ///   Microsoft.Extensions.Logging.Log4Net.AspNetCore.dll
    /// 2.通过构造函数注入，请看HomeController 示例
    /// 
    /// 
    /// 【333】
    /// autofac容器扩展
    /// 引入  
    /// Autofac
    /// Autofac.Extras.DynamicProxy
    /// Autofac.Extensions.DependencyInjec
    /// AOP实现请看：CustomAutofacAOP
    /// 
    /// 【123】
    /// Filters 扩展注册 
    /// ExceptionFilter没有成功
    /// 
    /// 
    /// 【115】
    /// 中间件(类似.Net Framework MVC中的Moudle)/middleware/管道处理模型
    /// 都是基于：IApplicationBuilder
    /// 每个中间件都是在处理HttpContext，然后返回HttpContext，给下个中间件处理
    /// 这些中间件都是围绕这 Action执行，一个俄罗斯套娃的方式，这些中间件执行顺序是根据注册顺序来执行的
    /// run 终结者
    /// use 中间件，可以做很多个这样的中间件
    /// map 判断请求路径，如果通过则执行分支中间件
    /// mapWhen 根据httpContext上下文内容做出判断，如果通过判断则执行分支中间内容
    /// 
    /// 
    /// .Net Framework MVC管道处理模型，Mudoule执行顺序都是固定的，一遍执行过去
    /// .Net Core MVC管道处理模型，中间件（Middleware）是分块的，由开发员自由拼装起来，组成执行流，顺序是由开发员控制的
    /// 中间件执行顺序还是“俄罗斯套娃”
    /// 
    /// 
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            #region  IServiceCollection容器的使用 【1111】

            IServiceCollection container = new ServiceCollection();
            container.AddSingleton<Interface1>(new Class1());
            container.AddTransient<Interface2, Class2>();
            container.AddScoped<Interface3, Class3>();//作用域           

            ServiceProvider serviceProvider1 = container.BuildServiceProvider();
            ServiceProvider serviceProvider2 = container.BuildServiceProvider();

            //单例比较
            Interface1 interface11 = serviceProvider1.GetService<Interface1>();
            Interface1 interface12 = serviceProvider1.GetService<Interface1>();
            bool b1 = object.ReferenceEquals(interface11, interface12);//true


            //瞬时对象
            Interface2 interface21 = serviceProvider1.GetService<Interface2>();
            Interface2 interface22 = serviceProvider1.GetService<Interface2>();
            bool b2 = object.ReferenceEquals(interface21, interface22);//false

            //作用域
            Interface3 interface31 = serviceProvider1.GetService<Interface3>();
            Interface3 interface32 = serviceProvider1.GetService<Interface3>();
            bool b3 = object.ReferenceEquals(interface31, interface32);//true
            //作用域对比
            Interface3 interface33 = serviceProvider2.GetService<Interface3>();
            Interface3 interface34 = serviceProvider2.GetService<Interface3>();
            bool b4 = object.ReferenceEquals(interface31, interface33);

            #endregion

            CreateWebHostBuilder(args).Build().Run();//创建一个服务器/可以理解为IIS服务器
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
