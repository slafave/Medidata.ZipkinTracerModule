using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Medidata.ZipkinTracer.Core;
using TestHost.Model;

namespace TestHost.Controllers
{
    [RoutePrefix("api/customers")]
    public class CustomersController : ApiController
    {
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
        {
            IList<Customer> customers = new List<Customer>();
            customers.Add(new Customer() { Name = "Nice customer", Address = "USA", Telephone = "123345456" });
            customers.Add(new Customer() { Name = "Good customer", Address = "UK", Telephone = "9878757654" });
            customers.Add(new Customer() { Name = "Awesome customer", Address = "France", Telephone = "34546456" });
            return Ok<IList<Customer>>(customers);
        }

        [HttpGet]
        [Route("ExternalCall")]
        public async Task<IHttpActionResult> CallAnotherService()
        {
            var context = Request.GetOwinContext();
            var config = Startup.GetZipkinConfig();

            var zipkinClient = new ZipkinClient(config, context);

            var url = "http://authenticationsvc.idm.local:80";
            var requestUri = "/api/credentials/4552";
            HttpResponseMessage result;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);

                // start client trace
                var span = zipkinClient.StartClientTrace(new Uri(client.BaseAddress, requestUri), "GET", zipkinClient.TraceProvider.GetNext());
                
                zipkinClient.Record(span, "This is a standard Annotation.");

                result = await client.GetAsync(requestUri).ConfigureAwait(false);

                //zipkinClient.RecordLocalComponent(span, "This is a Local Component Annotation");

                // Record the total memory used after the call
                zipkinClient.RecordBinary(span, "client.memory", GC.GetTotalMemory(false));

                // end client trace
                zipkinClient.EndClientTrace(span, (int)result.StatusCode);
            }
            return Ok(await result.Content.ReadAsStringAsync());
        }
    }
}
