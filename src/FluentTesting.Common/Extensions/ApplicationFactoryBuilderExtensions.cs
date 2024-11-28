using FluentTesting.Common.Images;
using FluentTesting.Common.Interfaces;
using System.Diagnostics;

namespace FluentTesting.Common.Extensions
{
    public static class ApplicationFactoryBuilderExtensions
    {
        /// <summary>
        /// Not use proxied images - use with caution on local machine in case of lack of VPN
        /// </summary>
        public static IApplicationFactoryBuilder UseProxiedImages(this IApplicationFactoryBuilder builder, string proxy)
        {
            Trace.WriteLine("DO NOT FORGET TO REMOVE THIS BEFORE PUSHING TO AzDo!!");

            ImagesSetting.ImageProxyPath = proxy;

            builder.UseProxiedImages = true;
            return builder;
        }
    }
}
