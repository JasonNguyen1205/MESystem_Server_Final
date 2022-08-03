namespace MESystem.Data.TRACE
{
    public interface IResourceAppointmentCollection
    {
        public List<ResourceAppointmentCollection.ResourceAppointment> GetAppointments();
        public List<EffPlan> GetResourcesForGrouping(DateTime date);
    }
}