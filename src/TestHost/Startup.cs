using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using Medidata.ZipkinTracer.Core;
using Medidata.ZipkinTracer.Core.Middlewares;
using Owin;

namespace TestHost
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseZipkin(GetZipkinConfig());

            var httpConfiguration = new HttpConfiguration();
            WebApiConfig.Register(httpConfiguration);
            app.UseWebApi(httpConfiguration);
        }

        internal static ZipkinConfig GetZipkinConfig()
        {
            return new ZipkinConfig
            {
                Domain = r => new Uri("http://ScottL-SuperFly-ZipkinTestHost.idm.local"),
                ZipkinBaseUri = new Uri("http://10.111.69.31:9411"),
                SpanProcessorBatchSize = 10,
                SampleRate = 0.5,
                Create128BitTraceId = true,
                Bypass = r => false,
                ExcludedPathList = new[] { "/elmah", "/elmah.axd", "/startup", "/swagger" }
            };
        }
    }
}