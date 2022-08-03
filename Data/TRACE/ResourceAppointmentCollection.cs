using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using MESystem.Service;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace MESystem.Data.TRACE;
public static partial class ResourceAppointmentCollection
{

    public class ResourceAppointment
    {
        public ResourceAppointment() { }
        public bool Accepted { get; set; }
        public int AppointmentType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Caption { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public int? Label { get; set; }
        public int Status { get; set; }
        public bool AllDay { get; set; }
        public string Recurrence { get; set; }
        public int? ResourceId { get; set; }
    }

    public static List<ResourceAppointment> GetAppointments()
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

    public static List<EffPlan> GetResourcesForGrouping()
    {
        return GetResources().ToList();
    }

    public static List<EffPlan> GetResources()
    {
        return new List<EffPlan>() {
                new EffPlan() { Id=0 , Name="John Heart", GroupId=100, BackgroundCss="dx-green-color", TextCss="text-white" },
                new EffPlan() { Id=1 , Name="Samantha Bright", GroupId=101, BackgroundCss="dx-orange-color", TextCss="text-white" },
                new EffPlan() { Id=2 , Name="Arthur Miller", GroupId=100, BackgroundCss="dx-purple-color", TextCss="text-white" },
                new EffPlan() { Id=3 , Name="Robert Reagan", GroupId=101, BackgroundCss="dx-indigo-color", TextCss="text-white" },
                new EffPlan() { Id=4 , Name="Greta Sims", GroupId=100, BackgroundCss="dx-red-color", TextCss="text-white" }
            };
    }

    //public static List<EffPlan> GetResourceGroups()
    //{
    //    return new List<EffPlan>() {
    //            new EffPlan() { Id=100, Name="Sales and Marketing", IsGroup=true },
    //            new EffPlan() { Id=101, Name="Engineering", IsGroup=true }
    //        };
    //}
}