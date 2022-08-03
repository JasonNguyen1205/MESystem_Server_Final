using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using MESystem.Service;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace MESystem.Data.TRACE;
public partial class ResourceAppointmentCollection
{

    public TraceService _traceService;
    public ResourceAppointmentCollection(TraceService traceService)
    {
        _traceService =  traceService;
    }
    public List<ResourceAppointment> GetAppointments()
    {

        DateTime date = DateTime.Now.Date;
        var dataSource = new List<ResourceAppointment>() {
                new ResourceAppointment {
                    Accepted = true,
                    Caption = "Install New Router in Dev Room",
                    StartDate = date + (new TimeSpan(0, 10, 0, 0)),
                    EndDate = date + (new TimeSpan(0, 12, 0, 0)),
                    Status = 1,
                    ResourceId = 0
                },
                new ResourceAppointment {
                    Caption = "Upgrade Personal Computers",
                    Accepted = false,
                    StartDate = date + (new TimeSpan(0,  13, 0, 0)),
                    EndDate = date + (new TimeSpan(0, 14, 30, 0)),
                    Status = 1,
                    ResourceId = 0
                },
                new ResourceAppointment {
                    Caption = "Website Redesign Plan",
                    Accepted = false,
                    StartDate = date + (new TimeSpan(1, 9, 30, 0)),
                    EndDate = date + (new TimeSpan(1, 11, 30, 0)),
                    Status = 1,
                    ResourceId = 0
                },
                new ResourceAppointment {
                    Caption = "New Brochures",
                    Accepted = true,
                    StartDate = date + (new TimeSpan(1, 13, 30, 0)),
                    EndDate = date + (new TimeSpan(1, 15, 15, 0)),
                    Status = 1,
                    ResourceId = 0
                },
                new ResourceAppointment {
                    Caption = "Book Flights to San Fran for Sales Trip",
                    Accepted = false,
                    StartDate = date + (new TimeSpan(1, 12, 0, 0)),
                    EndDate = date + (new TimeSpan(1, 13, 0, 0)),
                    AllDay = true,
                    Status = 1,
                    ResourceId = 0
                },
                new ResourceAppointment {
                    Caption = "Approve Personal Computer Upgrade Plan",
                    Accepted = true,
                    StartDate = date + (new TimeSpan(0, 10, 0, 0)),
                    EndDate = date + (new TimeSpan(0, 12, 0, 0)),
                    Status = 1
                },
                new ResourceAppointment {
                    Caption = "Final Budget Review",
                    Accepted = true,
                    StartDate = date + (new TimeSpan(0, 13, 0, 0)),
                    EndDate = date + (new TimeSpan(0, 15, 0, 0)),
                    Status = 1,
                    ResourceId = 1
                },
                new ResourceAppointment {
                    Caption = "Install New Database",
                    Accepted = false,
                    StartDate = date + (new TimeSpan(0, 9, 45, 0)),
                    EndDate = date + (new TimeSpan(1, 11, 15, 0)),
                    Status = 1,
                    ResourceId = 1
                },
                new ResourceAppointment {
                    Accepted = true,
                    Caption = "Approve New Online Marketing Strategy",
                    StartDate = date + (new TimeSpan(1,  12, 0, 0)),
                    EndDate = date + (new TimeSpan(1, 14, 0, 0)),
                    Status = 1,
                    ResourceId = 1
                },
                new ResourceAppointment {
                    Accepted = true,
                    Caption = "Customer Workshop",
                    StartDate = date + (new TimeSpan(0,  11, 0, 0)),
                    EndDate = date + (new TimeSpan(0, 12, 0, 0)),
                    AllDay = true,
                    Status = 1,
                    ResourceId = 2
                },
                new ResourceAppointment {
                    Accepted = true,
                    Caption = "Prepare 2021 Marketing Plan",
                    StartDate = date + (new TimeSpan(0,  11, 0, 0)),
                    EndDate = date + (new TimeSpan(0, 13, 30, 0)),
                    Status = 1,
                    ResourceId = 2
                },
                new ResourceAppointment {
                    Accepted = false,
                    Caption = "Brochure Design Review",
                    StartDate = date + (new TimeSpan(0, 14, 0, 0)),
                    EndDate = date + (new TimeSpan(0, 15, 30, 0)),
                    Status = 1,
                    ResourceId = 2
                },
                new ResourceAppointment {
                    Accepted = true,
                    Caption = "Create Icons for Website",
                    StartDate = date + (new TimeSpan(1, 10, 0, 0)),
                    EndDate = date + (new TimeSpan(1, 11, 30, 0)),
                    Status = 1,
                    ResourceId = 1
                },
                new ResourceAppointment {
                    Accepted = true,
                    Caption = "Launch New Website",
                    StartDate = date + (new TimeSpan(1, 12, 20, 0)),
                    EndDate = date + (new TimeSpan(1, 14, 0, 0)),
                    Status = 1,
                    ResourceId = 2
                },
                new ResourceAppointment {
                    Accepted = false,
                    Caption = "Upgrade Server Hardware",
                    StartDate = date + (new TimeSpan(1, 9, 0, 0)),
                    EndDate = date + (new TimeSpan(1, 12, 0, 0)),
                    Status = 1,
                    ResourceId = 2
                },
                new ResourceAppointment {
                    Accepted = true,
                    Caption = "Book Flights to San Fran for Sales Trip",
                    StartDate = date + (new TimeSpan(0, 14, 0, 0)),
                    EndDate = date + (new TimeSpan(0, 17, 0, 0)),
                    Status = 1,
                    ResourceId = 3
                },
                new ResourceAppointment {
                    Accepted = true,
                    Caption = "Approve New Online Marketing Strategy",
                    StartDate = date + (new TimeSpan(0,  12, 0, 0)),
                    EndDate = date + (new TimeSpan(0, 15, 0, 0)),
                    Status = 1,
                    ResourceId = 4
                }
            };
        return dataSource;
    }

    public List<EffPlan> GetResourcesForGrouping(DateTime date)
    {
        return GetResources(date).Take(3).ToList();
    }

    public DateTime ChangeTime(DateTime dateTime, int hours, int minutes, int seconds, int milliseconds)
    {
        return new DateTime(
            dateTime.Year,
            dateTime.Month,
            dateTime.Day,
            hours,
            minutes,
            seconds,
            milliseconds,
            dateTime.Kind);
    }

    public class Data
    {
        public int Id { get; set; }
        public string StringData { get; set; }
    }

    public List<EffPlan> GetResources(DateTime date)
    {
        List<EffPlan> list = new List<EffPlan>();
        var dateTemp = ChangeTime(date, 06, 00, 00, 0);
        // Get Lines
        Task.Run(async () =>
        {
            list = await _traceService.LoadDataSearchByDate(dateTemp);
        });

        List<Data> Lines = new();
        int i = 100;
        int y = 0;
        foreach (EffPlan plan in list)
        {
            foreach (Data line in Lines)
            {
                if (!line.StringData.Equals(plan.RealLine))
                {
                    line.StringData = plan.RealLine;
                    plan.Id = y;
                    plan.Name = plan.RealLine;
                    plan.GroupId = i;
                    plan.BackgroundCss = "dx-green-color";
                    plan.TextCss = "text-white";
                    i++; y++;
                }
                else
                {
                    EffPlan temp = list.Where(e => e.RealLine.Equals(plan.RealLine)).FirstOrDefault();
                    plan.Id = temp.Id;
                    plan.GroupId = temp.GroupId;
                }
            }

        }
        return list;

        //return new List<EffPlan>() {
        //        new EffPlan() { Id=0 , Name="John Heart", GroupId=100, BackgroundCss="dx-green-color", TextCss="text-white" },
        //        new EffPlan() { Id=1 , Name="Samantha Bright", GroupId=101, BackgroundCss="dx-orange-color", TextCss="text-white" },
        //        new EffPlan() { Id=2 , Name="Arthur Miller", GroupId=100, BackgroundCss="dx-purple-color", TextCss="text-white" },
        //        new EffPlan() { Id=3 , Name="Robert Reagan", GroupId=101, BackgroundCss="dx-indigo-color", TextCss="text-white" },
        //        new EffPlan() { Id=4 , Name="Greta Sims", GroupId=100, BackgroundCss="dx-red-color", TextCss="text-white" }
        //    };
    }

    //public static List<Resource> GetResourceGroups()
    //{
    //    return new List<Resource>() {
    //            new Resource() { Id=100, Name="Sales and Marketing", IsGroup=true },
    //            new Resource() { Id=101, Name="Engineering", IsGroup=true }
    //        };
    //}
}