using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using MESystem.Service;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace MESystem.Data.TRACE;
public class ResourceAppointmentCollection
{
    private static readonly TraceDbContext _context;

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

    public class Data
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int GroupId { get; set; }
    }

    public static List<ResourceAppointment> GetAppointments(DateTime date, IEnumerable<EffPlan> EffPlan)
    {
        //DateTime date = DateTime.Now.Date;
        List<ResourceAppointment> Results = new List<ResourceAppointment>();

        List<EffPlan> effPlans = SortTitleAndId(EffPlan.ToList());
        foreach (EffPlan effPlan in effPlans)
        {
                if (!string.IsNullOrEmpty(effPlan.FromTime))
                {
                    var startDate = effPlan.FromTime.Split(":");
                    var endDate = effPlan.ToTime.Split(":");
                    ResourceAppointment temp = new();
                    string classColor = "";
                    if (effPlan.Percent > 90) classColor = "custom-green";
                    if (effPlan.Percent < 90 && effPlan.Percent > 80) classColor = "custom-orange"; 
                    if (effPlan.Percent < 80) classColor = "custom-red";

                if ((int.Parse( startDate[0] ) >= 6 && int.Parse( startDate[0] ) <= 23 ) && (int.Parse( endDate[0] ) < 6 ||  (int.Parse(endDate[0]) == 6 && int.Parse(endDate[1]) == 0))){
                        
                        temp = new ResourceAppointment
                        {
                            Caption = effPlan.SoBB + " - " + effPlan.PartNo + " - " + effPlan.Family,
                            Accepted = true,
                            StartDate = date + (new TimeSpan(0, int.Parse(startDate[0]), int.Parse(startDate[1]), int.Parse(startDate[2]))),
                            EndDate = date + (new TimeSpan(1, int.Parse(endDate[0]), int.Parse(endDate[1]), int.Parse(endDate[2]))),
                            Location = classColor,
                            ResourceId = effPlan.Id,
                            Description = string.Format("{0:F2}", effPlan.Percent)

                        };
                    } 
                    else if((int.Parse(startDate[0]) >=0) && (int.Parse(endDate[0]) < 6 || (int.Parse(endDate[0]) == 6 && int.Parse(endDate[1]) == 0)))
                    {
                         temp = new ResourceAppointment
                        {
                            Caption = effPlan.SoBB + " - " + effPlan.PartNo + " - " + effPlan.Family,
                            Accepted = true,
                            StartDate = date + (new TimeSpan(1, int.Parse(startDate[0]), int.Parse(startDate[1]), int.Parse(startDate[2]))),
                            EndDate = date + (new TimeSpan(1, int.Parse(endDate[0]), int.Parse(endDate[1]), int.Parse(endDate[2]))),
                             Location = classColor,
                            ResourceId = effPlan.Id,
                            Description = string.Format("{0:F2}", effPlan.Percent)
                         };
                  
                    } else
                    {
                        temp = new ResourceAppointment
                        {
                            Caption = effPlan.SoBB + " - " + effPlan.PartNo + " - " + effPlan.Family,
                            Accepted = true,
                            StartDate = date + (new TimeSpan(0, int.Parse(startDate[0]), int.Parse(startDate[1]), int.Parse(startDate[2]))),
                            EndDate = date + (new TimeSpan(0, int.Parse(endDate[0]), int.Parse(endDate[1]), int.Parse(endDate[2]))),
                            Location = classColor,
                            ResourceId = effPlan.Id,
                            Description = string.Format("{0:F2}", effPlan.Percent)
                        };
                     }

                    Results.Add(temp);
                }
        }
        return Results;
    }

    public static List<EffPlan> GetResourcesForGrouping(IEnumerable<EffPlan> effPlan)
    {
        return GetResources(effPlan).ToList();
    }

    public static List<EffPlan> GetResources(IEnumerable<EffPlan> EffPlan)
    {
        try
        {
            int id = 0;
            List<Data> flags = new();
            List<EffPlan> effPlans = EffPlan.ToList();
            List<EffPlan> Results = new();
            int groupId = 100;
            List<string> colors = new List<string>() {
                "dx-green-color",
                "dx-orange-color",
                "dx-purple-color",
                "dx-indigo-color",
                "dx-red-color",
                "dx-yellow-color",
                "dx-blue-color",
                "dx-green-color"
            };

            foreach (EffPlan plan in effPlans)
            {
                if (flags.Count() > 0)
                {
                    if (!string.IsNullOrEmpty(plan.RealLine))
                    {

                        if (!flags.Where(e => e.Title.Equals(plan.RealLine)).Any())
                        {
                            plan.Id = id;
                            plan.Name = plan.RealLine;
                            plan.BackgroundCss = colors[id];
                            plan.TextCss = "text-white";
                            plan.GroupId = groupId;
                            Data tempFlag = new();
                            tempFlag.Id = id;
                            tempFlag.Title = plan.RealLine;
                            tempFlag.GroupId = groupId;
                            flags.Add(tempFlag);
                            Results.Add(plan);
                            id++;
                            groupId++;
                        }
                    }

                }
                else
                {
                    plan.Id = id;
                    plan.Name = plan.RealLine;
                    plan.GroupId = groupId;
                    plan.BackgroundCss = colors[id];
                    plan.TextCss = "text-white";

                    Data tempFlag = new();
                    tempFlag.Id = id;
                    tempFlag.Title = plan.RealLine;
                    tempFlag.GroupId = groupId;
                    flags.Add(tempFlag);

                    Results.Add(plan);
                    id++;
                    groupId++;
                }

            }
            return Results;

        } catch(Exception ex)
        {
            return new();
        }
        
    }

    public static DateTime ChangeTime(DateTime dateTime, int hours, int minutes, int seconds, int milliseconds)
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

    public static List<EffPlan> SortTitleAndId(IEnumerable<EffPlan> effPlan)
    {
        int id = 0;
        int groupId = 100;
        List<Data> flags = new();
        List<EffPlan> effPlans = effPlan.ToList();


        foreach (EffPlan plan in effPlans)
        {
            if (flags.Count() > 0)
            {
                if (!string.IsNullOrEmpty(plan.RealLine))
                {

                    if (flags.Where(e => e.Title.Equals(plan.RealLine)).Any())
                    {
                        var temp = flags.Where(e => e.Title.Equals(plan.RealLine)).FirstOrDefault();
                        plan.Id = temp.Id;
                        plan.Name = temp.Title;
                        plan.GroupId = temp.GroupId;
                        plan.BackgroundCss = "dx-green-color";
                        plan.TextCss = "text-white";
                        
                    }
                    else
                    {
                        plan.Id = id;
                        plan.Name = plan.RealLine;
                        plan.BackgroundCss = "dx-green-color";
                        plan.TextCss = "text-white";
                        plan.GroupId = groupId;
                        Data tempFlag = new();
                        tempFlag.Id = id;
                        tempFlag.Title = plan.RealLine;
                        tempFlag.GroupId = groupId;
                        flags.Add(tempFlag);

                        id++; 
                        groupId++;
                    }
                }

            }
            else
            {
                plan.Id = id;
                plan.Name = plan.RealLine;
                plan.GroupId = groupId;
                plan.BackgroundCss = "dx-green-color";
                plan.TextCss = "text-white";

                Data tempFlag = new();
                tempFlag.Id = id;
                tempFlag.Title = plan.RealLine;
                tempFlag.GroupId = groupId;
                flags.Add(tempFlag);

                id++;
                groupId++;
            }

        }
        return effPlans;
    }

    public static List<EffPlan> GetResourceGroups()
    {
        return new List<EffPlan>() {
                new EffPlan() { Id=100, Name="SMD", IsGroup=true },
                new EffPlan() { Id=101, Name="MI", IsGroup=true },
                new EffPlan() { Id=101, Name="BB", IsGroup=true }
            };
    }
}