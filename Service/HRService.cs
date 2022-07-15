
using MESystem.Data.HR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json.Linq;

using RestSharp;

using System.Data;

using System.Linq;
using System.Net.Http;

using System.Net.Http.Headers;

using System.Text.Json;

namespace MESystem.Service;

public class HRService
{
    private readonly HRDbContext _context;

    public HRService(HRDbContext context)
    {
        _context=context;
    }

    public async Task<IEnumerable<CheckInOut>>
        LoadCheckInOutInformation()
    {

        return await _context.CheckInOuts
        .Where(_ => _.TimeStr>DateTime.Now.AddDays(-2))
                    .AsNoTracking()
                    .ToListAsync();

    }

    public async Task<IEnumerable<CheckInOut>>
        GetCheckInOut(int iDFilter)
    {

        return await _context.CheckInOuts
                         .Where(w => w.TimeStr>(DateTime.Now.AddDays(-2)))
                         .AsNoTracking()
                         .ToListAsync();

    }

    public async Task<IEnumerable<CheckInOut>>
       GetAllCheckInOut()
    {
        var td = from s in (from s in _context.CheckInOuts
                            join r in _context.UserInfos on s.UserEnrollNumber equals r.UserEnrollNumber
                            select new CheckInOut { TimeStr=s.TimeStr, UserEnrollNumber=s.UserEnrollNumber, TimeDate=s.TimeDate, MachineNo=s.MachineNo, UserFullName=r.UserFullName, UserIDTitle=r.UserIDTitle, UserIDD=r.UserIDD })
                 join r in _context.RelationDepts on s.UserIDD equals r.ID
                 select new CheckInOut { TimeStr=s.TimeStr, UserEnrollNumber=s.UserEnrollNumber, TimeDate=s.TimeDate, MachineNo=s.MachineNo, UserFullName=s.UserFullName, UserIDTitle=s.UserIDTitle, UserIDD=s.UserIDD, Desc=r.Description };

        return await td.ToListAsync();
    }

    public async Task<ActionResult<Employee>> PutEmployee(List<Employee> employees)
    {
        var emp = new Employee();

        foreach(var item in employees)
        {
            //var rs = await _httpClient.PutAsJsonAsync();

        }
        return null;
    }

    public async Task<string> GetAuthorizeToken()
    {
        // Initialization.
        string responseObj = string.Empty;

        // Posting.
        //using(var client = new HttpClient())
        {
            // Setting Base address.
            //client.BaseAddress=new Uri("https://auth.sandbox.vuiapp.vn/auth/realms/friwo/protocol/openid-connect/token");

            var client = new RestClient("https://auth.sandbox.vuiapp.vn/auth/realms/friwo/protocol/openid-connect/token");
            var request = new RestRequest {Method=Method.Post };
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("application/x-www-form-urlencoded", "grant_type=client_credentials&client_id=friwo-publicapi&client_secret=uoQociqjA56Tqtnzf1ryUlByPkMghJDS", ParameterType.RequestBody);
            var response = await client.ExecuteAsync(request);
            // Initialization.
            //HttpResponseMessage response = new HttpResponseMessage();
            //<KeyValuePair<string, string>> allIputParams = new List<KeyValuePair<string, string>>();

            // Convert Request Params to Key Value Pair.
            //allIputParams.Add(new KeyValuePair<string, string>("client_id", "friwo-publicapi"));
            //allIputParams.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
            //allIputParams.Add(new KeyValuePair<string, string>("client_secret", "uoQociqjA56Tqtnzf1ryUlByPkMghJDS"));

            //// URL Request parameters.
            //HttpContent requestParams = new FormUrlEncodedContent(allIputParams);

            //// HTTP POST
            //response=await client.PostAsync("Token", requestParams).ConfigureAwait(false);

            // Verification
            if(response.IsSuccessful)
            {
                // Reading Response.

                response.ContentType="application/json";
                responseObj=response.Content.ToString();
                var data = JObject.Parse(responseObj);

                responseObj=data.Property("access_token")?.Value?.ToString()??"";
            }
        }

        return responseObj.ToString();
    }


    public async Task<string> GetCompanyInfo(string authorizeToken)
    {
        // Initialization.
        string responseObj = string.Empty;

        // HTTP GET.
        using(var client = new HttpClient())
        {
            // Initialization
            string authorization = authorizeToken;

            // Setting Authorization.
            client.DefaultRequestHeaders.Authorization=new AuthenticationHeaderValue("Bearer", authorization);

            // Setting Base address.
            client.BaseAddress=new Uri("https://api.sandbox.vuiapp.vn/");

            // Setting content type.
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Initialization.
            HttpResponseMessage response = new HttpResponseMessage();

            // HTTP GET
            response=await client.GetAsync("v2/companies").ConfigureAwait(false);

            // Verification
            if(response.IsSuccessStatusCode)
            {
                // Reading Response.
                responseObj=await response.Content.ReadAsStringAsync();
            }
        }

        return responseObj;
    }
    
    public async Task<string> PutEmployeeInfo(List<Employee> employees, string authorizeToken)
    {
        string responseObj = "";
        var client = new RestClient("https://api.sandbox.vuiapp.vn/v2/employees");
        client.Options.MaxTimeout=-1;
        var request = new RestRequest();
        request.Method=Method.Put;
        request.AddHeader("Authorization", $"Bearer {authorizeToken}");
        request.AddHeader("Content-Type", "application/json");
        var body = JsonSerializer.Serialize(employees);
        Console.WriteLine(body);
        request.AddParameter("application/json", body, ParameterType.RequestBody);
        RestResponse response = await client.ExecuteAsync(request);
        Console.WriteLine(response.Content);

        responseObj=response.StatusCode.ToString(); 

        return responseObj;
    }

    public async Task<string> PutAttendanceInfo(List<Attendee> attendees, string authorizeToken)
    {
        string responseObj = "";
        var client = new RestClient("https://api.sandbox.vuiapp.vn/v2/attendances");
        client.Options.MaxTimeout=-1;
        var request = new RestRequest();
        request.Method=Method.Put;
        request.AddHeader("Authorization", $"Bearer {authorizeToken}");
        request.AddHeader("Content-Type", "application/json");
        var body = JsonSerializer.Serialize(attendees);
        Console.WriteLine(body);
        request.AddParameter("application/json", body, ParameterType.RequestBody);
        RestResponse response = await client.ExecuteAsync(request);
        Console.WriteLine(response.Content);

        responseObj=response.StatusCode.ToString();

        return responseObj+body;
    }


}
